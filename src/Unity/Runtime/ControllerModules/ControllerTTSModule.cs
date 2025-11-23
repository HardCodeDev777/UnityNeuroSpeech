using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityNeuroSpeech.Utils;

namespace UnityNeuroSpeech.Runtime.TTS
{
    internal class ControllerTTSModule
    {
        private AudioSource _ttsAudioSource;
        private int _agentIndex;

        public ControllerTTSModule(int agentIndex, AudioSource ttsAudioSource)
        {
            _agentIndex = agentIndex;
            _ttsAudioSource = ttsAudioSource;
        }

#if ENABLE_MONO
        /// <summary>
        /// Starts TTS process in Mono
        /// </summary>
        public Process StartTTSProcessMonoModular(string llmResponse, string currentLang)
        {
            llmResponse = llmResponse.Replace("\r", "").Replace("\n", " ").Trim();

            var ttsProcess = new Process();
            ttsProcess.StartInfo.FileName = "cmd.exe";

            // The most interesting TTS part
            ttsProcess.StartInfo.Arguments = $"/C tts " +
                $"--text \"{llmResponse}\" " +

                $"--model_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "TTSModel")}\" " +

                $"--config_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "TTSModel", "config.json")}\" " +

                $"--speaker_wav \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", $"{currentLang}_voice{_agentIndex}.wav")}\" " +

                $"--out_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", "Generated", $"{currentLang}_voice{_agentIndex}_result.wav")}\" " +

                $"--language_idx {currentLang}";
            ttsProcess.StartInfo.RedirectStandardOutput = true;
            ttsProcess.StartInfo.RedirectStandardError = true;
            ttsProcess.StartInfo.UseShellExecute = false;
            ttsProcess.StartInfo.CreateNoWindow = true;

            LogUtils.LogMessage($"Final command: {ttsProcess.StartInfo.Arguments}");

            ttsProcess.OutputDataReceived += (sender, o) =>
            {
                if (!string.IsNullOrEmpty(o.Data)) LogUtils.LogMessage($"Agent {_agentIndex} TTS output: {o.Data}");
            };

            // It can be error or warning. Note: you'll definetily have two "errors"(actually warnings)
            ttsProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) LogUtils.LogError($"Agent {_agentIndex} TTS error(or warning): {e.Data}");
            };

            ttsProcess.Start();
            ttsProcess.BeginOutputReadLine();
            ttsProcess.BeginErrorReadLine();

            return ttsProcess;
        }

        public async UniTask CheckTTSProcessMonoModular(string currentLang, Process currentProcess)
        {
            if (currentProcess != null && currentProcess.HasExited) await CheckIfResultFileExits(currentLang);
        }

#else
        /// <summary>
        /// Starts and monitors TTS process in IL2CPP
        /// </summary>
        public async UniTask RunTTSProcessIL2CPPModular(string llmResponse, string currentLang)
        {
            LogUtils.LogMessage("In IL2CPP process handling is harder and logs are limited. For better debug use Mono");

            llmResponse = llmResponse.Replace("\r", "").Replace("\n", " ").Trim();

            var command = $"cmd.exe /C tts " +
                $"--text \"{llmResponse}\" " +

                $"--model_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "TTSModel")}\" " +

                $"--config_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "TTSModel", "config.json")}\" " +

                $"--speaker_wav \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", $"{currentLang}_voice{_agentIndex}.wav")}\" " +

                $"--out_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", "Generated", $"{currentLang}_voice{_agentIndex}_result.wav")}\" " +

                $"--language_idx {currentLang}";

            LogUtils.LogMessage($"Final command: {command}");

            var logs = NativeUtils.RunTTSProcessInI2CPP(command);
            LogUtils.LogMessage($"TTS process logs: {logs}");

            await CheckIfResultFileExits(currentLang);
        }
#endif

        private async UniTask CheckIfResultFileExits(string currentLang)
        {
            if (File.Exists(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", "Generated", $"{currentLang}_voice{_agentIndex}_result.wav")))
            {
                LogUtils.LogMessage("Generated wav result exists!");

                await PlayGeneratedAudioModular(absPath: Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", "Generated", $"{currentLang}_voice{_agentIndex}_result.wav"));
            }
        }

        private async UniTask PlayGeneratedAudioModular(string absPath)
        {
            var url = "file:///" + absPath.Replace("\\", "/");
            using (var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
            {
                await request.SendWebRequest().ToUniTask();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var clip = DownloadHandlerAudioClip.GetContent(request);
                    _ttsAudioSource.clip = clip;
                    _ttsAudioSource.Play();

                    await UniTask.Delay(TimeSpan.FromSeconds(clip.length + 2));
                    if (File.Exists(absPath))
                    {
                        File.Delete(absPath);       
                        LogUtils.LogMessage($"File at path {absPath} deleted succesfully!");
                    }
                }
                else LogUtils.LogError($"Error loading generated audio! Full error message: {request.error}");
            }
        }
    }
}