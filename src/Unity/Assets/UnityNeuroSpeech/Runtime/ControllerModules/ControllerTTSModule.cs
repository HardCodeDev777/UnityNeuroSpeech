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
            ttsProcess.StartInfo.Arguments = GenerateProcessCommand(llmResponse, currentLang);
            ttsProcess.StartInfo.RedirectStandardOutput = true;
            ttsProcess.StartInfo.RedirectStandardError = true;
            ttsProcess.StartInfo.UseShellExecute = false;
            ttsProcess.StartInfo.CreateNoWindow = true;

            LogUtils.LogMessage($"Final command: {ttsProcess.StartInfo.Arguments}");

            ttsProcess.OutputDataReceived += (sender, o) =>
            {
                if (!string.IsNullOrEmpty(o.Data)) LogUtils.LogMessage($"Agent {_agentIndex} TTS output: {o.Data}");
            };

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
        /// Controls TTS process in IL2CPP. You don't need to monitor it like in Mono since it handled in Native
        /// </summary>
        public async UniTask HandleTTSProcessAndOutputIL2CPP(string llmResponse, string currentLang)
        {
            LogUtils.LogMessage("In IL2CPP process handling is harder and there are no logs. For better debug use Mono");

            llmResponse = llmResponse.Replace("\r", "").Replace("\n", " ").Trim();

            var command = GenerateProcessCommand(llmResponse, currentLang);
            LogUtils.LogMessage($"Final command: {command}");

            NativeUtils.RunProcessNative(command);

            var filePath = Path.Combine(StaticData.GENERATED_VOICES_PATH, $"{currentLang}_voice{_agentIndex}_result.wav");

            // 5 mins should be enough
            var timeoutMs = 300000;
            
            var sw = Stopwatch.StartNew();

            while (!File.Exists(filePath) && sw.ElapsedMilliseconds < timeoutMs) await UniTask.Delay(50); 
            
            if (!File.Exists(filePath))
            {
                LogUtils.LogError($"Error TTS result file not created after {timeoutMs} ms: {filePath}");
                return;
            }

            LogUtils.LogMessage("TTS result file found, playing audio");
            await PlayGeneratedAudioModular(filePath);
        }

#endif

        private string GenerateProcessCommand(string llmResponse, string currentLang)
        {
            // The most interesting TTS part
            var command = $"/C tts " +
                $"--text \"{llmResponse}\" " +

                $"--model_path \"{StaticData.BASE_TTS_PATH}\" " +

                $"--config_path \"{StaticData.CONFIG_TTS_PATH}\" " +

                $"--speaker_wav \"{Path.Combine(StaticData.BASE_VOICE_PATH, $"{currentLang}_voice{_agentIndex}.wav")}\" " +

                $"--out_path \"{Path.Combine(StaticData.GENERATED_VOICES_PATH, $"{currentLang}_voice{_agentIndex}_result.wav")}\" " +

                $"--language_idx {currentLang}";
#if !ENABLE_MONO
            command = command.Replace("/C tts", "cmd.exe /C tts");
#endif
            return command;
        }

        private async UniTask CheckIfResultFileExits(string currentLang) 
        { 
            if (File.Exists(Path.Combine(StaticData.GENERATED_VOICES_PATH, $"{currentLang}_voice{_agentIndex}_result.wav"))) 
            { 
                LogUtils.LogMessage("Generated wav result exists!"); 
                await PlayGeneratedAudioModular(absPath: Path.Combine(StaticData.GENERATED_VOICES_PATH, $"{currentLang}_voice{_agentIndex}_result.wav")); } 
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