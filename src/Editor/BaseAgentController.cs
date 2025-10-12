#if UNITY_EDITOR

using UnityNeuroSpeech.Runtime;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityNeuroSpeech.Shared;
using UnityNeuroSpeech.Utils;
using Whisper.Utils;
using LogUtils = UnityNeuroSpeech.Utils.LogUtils;

namespace UnityNeuroSpeech.Editor
{
    /// <summary>
    /// Base agent controller. This script gets duplicated and modified by CreateAgent.cs,
    /// but the core functionality stays unchanged
    /// </summary>
    internal sealed class BaseAgentController : MonoBehaviour, IAgent
    {
        #region Variables
        // General
        /// <summary>
        /// Generated ScriptableObject
        /// </summary>
        [Header("General")]
        public AgentSettings agentSettings;
        [HideInInspector] public Action<int, string, string, string> BeforeTTS { get; set; }
        [HideInInspector] public Action AfterTTS { get; set; }
        [HideInInspector] public Action<string> AfterSTT { get; set; }
        public string JsonDialogHistoryFileName { get; set; } = string.Empty;
        public string EncryptionHistoryKey { get; set; } = string.Empty;
        private int _responseCount;

        // STT
        [Header("Speech-To-Text")]
        [SerializeField] private Whisper.WhisperManager _whisper;
        [SerializeField] private MicrophoneRecord _microphoneRecord;
        [SerializeField] private UnityEngine.UI.Button _enableMicButton;
        [SerializeField] private Sprite _enableMicSprite, _disableMicSprite;
        private bool _processingOtherActions;

        // TTS
        [Header("TTS")]
        [SerializeField] private AudioSource _responseAudioSource;
        private Process _currentProcess;
        private string _currentLang;
        private bool _wasTTSResponsePlayed;

        // Ollama       
        /// <summary>
        /// Used to prevent unnecessary API requests when this script is disabled
        /// </summary>
        private bool _stopRequesting;
        private string _ollamaURI;
        private List<ChatMessage> _chatHistory = new();
        private IChatClient _chatClient;

        #endregion

        #region Unity methods

        private void Awake()
        {
            // So we don't need to check it in random moment in Update and lose a lot of performance
            StartCoroutine(CheckTTSProcessCoroutine());

            string json;
            try
            {
                json = Resources.Load<TextAsset>("UnityNeuroSpeech/UnityNeuroSpeechSharedSettings").text;
            }
            catch
            {
                LogUtils.LogError("[UnityNeuroSpeech] You must create settings in \"UnityNeuroSpeech/Create Settings\"!");
                return;
            }
            var data = JsonUtility.FromJson<SharedJsonData>(json);
            LogUtils.logLevel = data.logLevel;
            _ollamaURI = data.ollamaURI;

            SafeExecutionUtils.SafeExecute("InitOllama", InitOllama, agentSettings.systemPrompt, agentSettings.modelName);

            // Setting Whisper and UI
            _microphoneRecord.OnRecordStop += OnRecordStop;
            _enableMicButton.onClick.AddListener(OnButtonPressed);
            _enableMicButton.image.sprite = _disableMicSprite;
        }

        /// <summary>
        /// Start only for managing json dialog history file creation/loading
        /// </summary>
        private void Start()
        {
            // If user don't want to save dialog history to json
            if (string.IsNullOrEmpty(JsonDialogHistoryFileName)) return;

            // If some history already exists and saved
            if (File.Exists(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{JsonDialogHistoryFileName}.json")))
            {
                var json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{JsonDialogHistoryFileName}.json"));

                RuntimeJsonData runtimeData;

                // Encryption?
                if (!string.IsNullOrEmpty(EncryptionHistoryKey))
                {
                    try
                    {
                        var encryptedData = EncryptionUtils.Decrypt(json, EncryptionHistoryKey);
                        runtimeData = JsonUtility.FromJson<RuntimeJsonData>(encryptedData);
                    }
                    catch (Exception e)
                    {
                        LogUtils.LogError($"[UnityNeuroSpeech] Error decoding encrypted data with this key! If you don't use any encryption, delete key parameter in your AgentBehaviour script. Full error message: {e}");
                        return;
                    }
                }
                else runtimeData = JsonUtility.FromJson<RuntimeJsonData>(json);

                // Add it to current _chatHistory
                foreach (var message in runtimeData.dialogHistory)
                {
                    _chatHistory.Add(new(ChatRole.User, message.userMessage));
                    _chatHistory.Add(new(ChatRole.Assistant, message.llmResponse));
                }

                LogUtils.LogMessage("[UnityNeuroSpeec] Dialog history restored");
            }
            else
            {
                LogUtils.LogMessage("[UnityNeuroSpeec] No dialog history was found");

                var emptyList = new List<DialogData>();
                // If don't add anything and just creating empty json file, JsonUtility won't be able to deserialize RuntimeJsonData
                emptyList.Add(new(string.Empty, string.Empty));

                var json = JsonUtility.ToJson(new RuntimeJsonData(emptyList), true);

                if(!string.IsNullOrEmpty(EncryptionHistoryKey))
                {
                    File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{JsonDialogHistoryFileName}.json"), EncryptionUtils.Encrypt(json, EncryptionHistoryKey));
                }
                else File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{JsonDialogHistoryFileName}.json"), json);
                AssetDatabase.Refresh();
            }
        }

        private void OnDisable()
        {
            _stopRequesting = true;
            _currentProcess?.Kill();
        }

        #endregion

        #region Ollama
        private void InitOllama(string systemPrompt, string modelName)
        {
            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            { DisableDefaults = true });

            builder.Services.AddChatClient(new OllamaChatClient(_ollamaURI, modelName));

            _chatClient = builder.Build().Services.GetRequiredService<IChatClient>();
            _chatHistory.Add(new(ChatRole.System, systemPrompt));
        }
        private async Task SendMessage(string userPrompt, string lang)
        {
            LogUtils.LogMessage("[UnityNeuroSpeech] Sending message to Ollama");

            _chatHistory.Add(new(ChatRole.User, userPrompt));

            var chatResponse = "";
            await foreach (var item in _chatClient.GetStreamingResponseAsync(_chatHistory))
            {
                if (_stopRequesting == true) break;
                chatResponse += item.Text;
            }

            LogUtils.LogMessage($"[UnityNeuroSpeech] Ollama response: {chatResponse}");

            _chatHistory.Add(new(ChatRole.Assistant, chatResponse));
            _responseCount++;

            var responseWithoutThinking = SafeExecutionUtils.SafeExecute("CleanThinking", CleanThinking, chatResponse);

            LogUtils.LogMessage($"[UnityNeuroSpeech] Ollama response without thinking: {responseWithoutThinking}");

            // Usually parsing works, but it fully depends on LLM response
            var emotion = SafeExecutionUtils.SafeExecute("ParseTag", ParseTag, responseWithoutThinking);
            var cleanedResponse = responseWithoutThinking.Replace($"<{emotion}>", "");
            var action = SafeExecutionUtils.SafeExecute("ParseTag", ParseTag, cleanedResponse);
            LogUtils.LogMessage($"[UnityNeuroSpeech] Parsed emotion: {emotion}");

            if (!string.IsNullOrEmpty(action))
            {
                LogUtils.LogMessage($"[UnityNeuroSpeech] Parsed action: {action}");
                cleanedResponse = cleanedResponse.Replace($"<{action}>", "");
            }

            LogUtils.LogMessage($"[UnityNeuroSpeech] Invoking BeforeTTS() for agent");
            BeforeTTS?.Invoke(_responseCount, cleanedResponse, emotion, action);

            LogUtils.LogMessage($"[UnityNeuroSpeech] Sending Ollama reponse to TTS");
            SafeExecutionUtils.SafeExecute("StartTTSProcess", StartTTSProcess, cleanedResponse, lang);

            LogUtils.LogMessage($"[UnityNeuroSpeech] Invoking AfterTTS() for agent");
            AfterTTS?.Invoke();

            // If user doesn't want to save history
            if (string.IsNullOrEmpty(JsonDialogHistoryFileName)) return;

            var jsonLoaded = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{JsonDialogHistoryFileName}.json"));
            RuntimeJsonData runtimeData;
            if (!string.IsNullOrEmpty(EncryptionHistoryKey))
            {
                try
                {
                    var encryptedData = EncryptionUtils.Decrypt(jsonLoaded, EncryptionHistoryKey);
                    runtimeData = JsonUtility.FromJson<RuntimeJsonData>(encryptedData);
                }
                catch
                {
                    LogUtils.LogError($"[UnityNeuroSpeech] Error decoding encrypted data with this key! If you don't use any encryption, delete key parameter in your AgentBehaviour script.");
                    return;
                }
            }
            else runtimeData = JsonUtility.FromJson<RuntimeJsonData>(jsonLoaded);

            runtimeData.dialogHistory.Add(new(userPrompt, cleanedResponse));

            var jsonToSave = JsonUtility.ToJson(runtimeData, true);
            if (!string.IsNullOrEmpty(EncryptionHistoryKey))
            {
                File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{JsonDialogHistoryFileName}.json"), EncryptionUtils.Encrypt(jsonToSave, EncryptionHistoryKey));
            }
            else File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{JsonDialogHistoryFileName}.json"), jsonToSave);

            LogUtils.LogMessage("[UnityNeuroSpeec] Dialog history updated");
        }


        /// <returns>Tag in message. If there's no tag, returns empty string</returns>
        private string ParseTag(string message)
        {
            var start = message.IndexOf("<") + 1;
            var end = message.IndexOf(">");

            if (end == -1) return string.Empty;

            var rawMessage = message.Substring(start, end - start);
            return rawMessage.Trim();
        }

        private string CleanThinking(string input)
        {
            var start = input.IndexOf("<think>");
            var end = input.IndexOf("</think>");
            if (start != -1 && end != -1 && end > start) return input.Remove(start, (end + "</think>".Length) - start);
            return input;
        }
        #endregion


        #region STT
        private void OnButtonPressed()
        {
            if (_processingOtherActions) return;

            if (!_microphoneRecord.IsRecording)
            {
                _microphoneRecord.StartRecord();
                _enableMicButton.image.sprite = _enableMicSprite;
            }

            else
            {
                _microphoneRecord.StopRecord();
                _enableMicButton.image.sprite = _disableMicSprite;
                _processingOtherActions = true;
            }
        }

        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            var res = await _whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null) return;

            LogUtils.LogMessage($"[UnityNeuroSpeech] Invoking AfterSTT() for agent");
            AfterSTT?.Invoke(res.Result);

            await SafeExecutionUtils.SafeExecute("SendMessage", SendMessage, res.Result, res.Language);
        }
        #endregion

        #region TTS

        /// <summary>
        /// To not play audio million times
        /// </summary>
        private IEnumerator CheckTTSProcessCoroutine()
        {
            while (true)
            {
                if (_currentProcess != null && _currentProcess.HasExited && !_wasTTSResponsePlayed)
                {
                    if (File.Exists(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", "Generated", $"{_currentLang}_voice{agentSettings.agentIndex}_result.wav")))
                    {
                        LogUtils.LogMessage("[UnityNeuroSpeech] Result wav exists");
                        StartCoroutine(PlayGeneratedAudio(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", "Generated", $"{_currentLang}_voice{agentSettings.agentIndex}_result.wav")));
                        _wasTTSResponsePlayed = true;
                    }
                }

                yield return new WaitForSeconds(5f);
            }
        }

        private void StartTTSProcess(string text, string lang)
        {
            text = text.Replace("\r", "").Replace("\n", " ").Trim();
            var ttsProcess = new Process();
            ttsProcess.StartInfo.FileName = "cmd.exe";
            ttsProcess.StartInfo.Arguments = $"/C tts " +
                $"--text \"{text}\" " +

                $"--model_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "TTSModel")}\" " +

                $"--config_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "TTSModel", "config.json")}\" " +

                $"--speaker_wav \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", $"{lang}_voice{agentSettings.agentIndex}.wav")}\" " +

                $"--out_path \"{Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices", "Generated", $"{lang}_voice{agentSettings.agentIndex}_result.wav")}\" " +

                $"--language_idx {lang}";
            ttsProcess.StartInfo.RedirectStandardOutput = true;
            ttsProcess.StartInfo.RedirectStandardError = true;
            ttsProcess.StartInfo.UseShellExecute = false;
            ttsProcess.StartInfo.CreateNoWindow = true;

            LogUtils.LogMessage($"[UnityNeuroSpeech] Final command: {ttsProcess.StartInfo.Arguments}");

            ttsProcess.OutputDataReceived += (sender, o) =>
            {
                if (!string.IsNullOrEmpty(o.Data)) LogUtils.LogMessage($"[UnityNeuroSpeech] Agent {agentSettings.agentIndex} TTS output: {o.Data}");
            };

            // It can be error or warning. Btw you will definetily have two "errors"(actually warnings)
            ttsProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) LogUtils.LogError($"[UnityNeuroSpeech] Agent {agentSettings.agentIndex} TTS error(or warning): {e.Data}");
            };

            ttsProcess.Start();
            ttsProcess.BeginOutputReadLine();
            ttsProcess.BeginErrorReadLine();
            _wasTTSResponsePlayed = false;

            _currentLang = lang;
            _currentProcess = ttsProcess;
        }

        private IEnumerator PlayGeneratedAudio(string absPath)
        {
            var url = "file:///" + absPath.Replace("\\", "/");
            using (var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var clip = DownloadHandlerAudioClip.GetContent(request);
                    _responseAudioSource.clip = clip;
                    _responseAudioSource.Play();
                    LogUtils.LogMessage($"[UnityNeuroSpeech] Invoking AfterTTS() for agent {agentSettings.agentIndex}");
                    StartCoroutine(WaitToDeleteFile(clip.length, absPath));
                    AfterTTS?.Invoke();
                }
                else LogUtils.LogError($"[UnityNeuroSpeech] Error loading generated audio! Full error message: {request.error}");

                _processingOtherActions = false;
            }
        }

        private IEnumerator WaitToDeleteFile(float secs, string absPath)
        {
            yield return new WaitForSeconds(secs + 10);
            if (File.Exists(absPath))
            {
                try
                {
                    File.Delete(absPath);
                }
                catch (Exception e)
                {
                    LogUtils.LogError($"[UnityNeuroSpeech] Error deleting file at path {absPath}! Full error message: {e}");
                }
                LogUtils.LogMessage($"[UnityNeuroSpeech] File at path {absPath} deleted succesfully!");
            }
        }
        #endregion
    }
}
#endif