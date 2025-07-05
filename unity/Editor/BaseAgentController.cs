#pragma warning disable CS0108

#if UNITY_EDITOR
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Whisper.Utils;
using UnityNeuroSpeech.Runtime;
using LogUtils = UnityNeuroSpeech.Utils.LogUtils;
using UnityNeuroSpeech.Utils;
using UnityNeuroSpeech.Shared;

namespace UnityNeuroSpeech.Editor
{
    /// <summary>
    /// Base agent controller. This script gets duplicated and modified by the editor window,
    /// but the core functionality stays unchanged.
    /// </summary>
    internal sealed class BaseAgentController : MonoBehaviour, IAgent
    {
        // General
        /// <summary>
        /// The generated ScriptableObject
        /// </summary>
        [Header("General")]      
        public AgentSettings agentSettings;

        /// <summary>
        /// Action invoked before sending text to the TTS model
        /// </summary>
        [HideInInspector] public Action<AgentState> BeforeTTS { get; set; }
        /// <summary>
        /// Action invoked after TTS playback finishes
        /// </summary>
        [HideInInspector] public Action AfterTTS { get; set; }
        private int _responseCount;

        // STT
        [Header("Speech-To-Text")]
        [SerializeField] private Whisper.WhisperManager _whisper;
        [SerializeField] private MicrophoneRecord _microphoneRecord;
        [SerializeField] private UnityEngine.UI.Button _enableMicButton;
        [SerializeField] private Sprite _enableMicSprite, _disableMicSprite;
        /// <summary>
        /// Output text from Whisper
        /// </summary>
        private string _output;

        [Header("TTS")]
        [SerializeField] private AudioSource _responseAudioSource;

        // Ollama       
        /// <summary>
        /// Used to prevent unnecessary API requests when this script is disabled
        /// </summary>
        private bool _stopRequesting;
        private List<ChatMessage> _chatHistory = new();
        private IChatClient _chatClient;

        private void Awake()
        {
            // At the moment, it's only used for proper logging. In future updates, it might become much more useful.
            string json;
            try
            {
                json = Resources.Load<TextAsset>("Settings/UnityNeuroSpeechSettings").text;
            }
            catch
            {
                LogUtils.LogError("[UnityNeuroSpeech] You must create settings in \"HardCodeDev/Create Settings\"!");
                return;
            }
            var data = JsonUtility.FromJson<JsonData>(json);
            LogUtils.logLevel = data.logLevel;


            SafeExecutionUtils.SafeExecute("InitOllama", InitOllama, agentSettings.systemPrompt, agentSettings.modelName);

            _microphoneRecord.OnRecordStop += OnRecordStop;
            _enableMicButton.onClick.AddListener(OnButtonPressed);
            _enableMicButton.image.sprite = _disableMicSprite;
        }

        private void OnDisable() => _stopRequesting = true;

        #region Ollama
        private void InitOllama(string systemPrompt, string modelName)
        {
            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            { DisableDefaults = true });

            builder.Services.AddChatClient(new OllamaChatClient("http://localhost:11434", modelName));

            _chatClient = builder.Build().Services.GetRequiredService<IChatClient>();
            _chatHistory.Add(new(ChatRole.System, systemPrompt));
        }

        private async Task SendMessage(string userPrompt)
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
            // Increments the response counter
            _responseCount++;

            // Removes thinking tags (helps with reasoning models)
            var cleanedResponse = SafeExecutionUtils.SafeExecute("CleanThinking", CleanThinking, chatResponse);

            // Extracts emotion from tags. Result format: "happy", "sad", etc.
            var emotion = SafeExecutionUtils.SafeExecute("ParseEmotion", ParseEmotion, cleanedResponse);

            LogUtils.LogMessage($"[UnityNeuroSpeech] Parsed emotion: {emotion}");

            LogUtils.LogMessage($"[UnityNeuroSpeech] Invoking BeforeTTS() for agent");
            // Call the subscribed BeforeTTS event
            BeforeTTS?.Invoke(new(_responseCount, chatResponse, emotion));

            LogUtils.LogMessage($"[UnityNeuroSpeech] Sending Ollama reponse to TTS model in server");
            // Send response to the TTS model on the local server
            StartCoroutine(PostText(cleanedResponse));

            LogUtils.LogMessage($"[UnityNeuroSpeech] Invoking AfterTTS() for agent");
            // Call the subscribed AfterTTS event
            AfterTTS?.Invoke();
        }

        private string ParseEmotion(string response)
        {
            var start = response.IndexOf("<") + 1;
            var end = response.IndexOf(">");
            var rawEmotion = response.Substring(start, end - start);
            return rawEmotion.Trim();
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
            if (!_microphoneRecord.IsRecording)
            {
                _microphoneRecord.StartRecord();
                _enableMicButton.image.sprite = _enableMicSprite;
            }

            else
            {
                _microphoneRecord.StopRecord();
                _enableMicButton.image.sprite = _disableMicSprite;
            }
        }

        /// <summary>
        /// Once the mic stops, send the transcribed text to Ollama
        /// </summary>
        /// <param name="recordedAudio"></param>
        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            var res = await _whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null) return;

            _output = res.Result;
            await SafeExecutionUtils.SafeExecute("SendMessage", SendMessage, _output);
        }
        #endregion

        #region TTS
        /// <summary>
        /// Sends the final response from Ollama to Coqui XTTS (running on the local server)
        /// </summary>
        private IEnumerator PostText(string text)
        {
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(text);
            using (var request = new UnityWebRequest("http://localhost:7777/tts", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerAudioClip("http://localhost:7777/tts", AudioType.WAV);
                request.SetRequestHeader("Content-Type", "text/plain");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success) LogUtils.LogError($"[UnityNeuroSpeech] TTS server probably is not running! Full error message: {request.error}");

                else
                {
                    _responseAudioSource.clip = DownloadHandlerAudioClip.GetContent(request);
                    _responseAudioSource.Play();
                }
            }
        }
        #endregion
    }
}
#endif