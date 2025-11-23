// This is literally Ollama inside Editor

#if UNITY_EDITOR
#pragma warning disable 0168

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityNeuroSpeech.Shared;
using UnityNeuroSpeech.Utils;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class PromptTesting : EditorWindow
    {
        private AgentSettings _generatedSettings;
        private string _prompt, _llmResponse;
        private Stopwatch _sw = new();

        [MenuItem("UnityNeuroSpeech/Tools/Prompt Testing")]
        public static void ShowWindow() => GetWindow<PromptTesting>("PromptTesting");

        private async void OnGUI()
        {
            EditorGUILayout.LabelField("Values for chatting", EditorStyles.boldLabel);

            // Unity sometimes makes goofy errors without reason, so you will find some weird try-catch'es to fix them
            try
            {
                if (!_generatedSettings) GUI.backgroundColor = Color.red;

                _generatedSettings = (AgentSettings)EditorGUILayout.ObjectField(new GUIContent("Generated agent settings", "Select ScriptableObject you generated for agent"), _generatedSettings, typeof(AgentSettings), false);

                GUI.backgroundColor = Color.white;
            }
            catch (ExitGUIException _) {}

            var style = new GUIStyle(EditorStyles.textArea) { wordWrap = true };

            if (string.IsNullOrEmpty(_prompt)) GUI.backgroundColor = Color.red;

            _prompt = EditorGUILayout.TextArea(_prompt, style, GUILayout.Height(200));

            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Send to LLM")) 
            {
                if(!_generatedSettings || string.IsNullOrEmpty(_prompt))
                {
                    LogUtils.LogError("Fill all required fields!");
                    return;
                }

                _sw.Start();
                LogUtils.LogError("Sending prompt to Ollama...");

                _llmResponse = await SendPromptToOllama(_generatedSettings.modelName, _generatedSettings.systemPrompt, _prompt);
                _sw.Stop();
            }
  
            EditorGUILayout.LabelField("LLM", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"LLM time: {_sw?.ElapsedMilliseconds} ms");
            
            GUI.enabled = false;

            try
            {
                _llmResponse = EditorGUILayout.TextArea(_llmResponse, style, GUILayout.Height(600));
            }
            catch (ArgumentException _) {}

            GUI.enabled = true;
        }

        private async UniTask<string> SendPromptToOllama(string modelName, string systemPrompt, string prompt)
        {
            var json = Resources.Load<TextAsset>("UnityNeuroSpeech/UnityNeuroSpeechSharedSettings").text;
            var data = JsonUtility.FromJson<SharedJsonData>(json);

            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            { DisableDefaults = true });

            builder.Services.AddChatClient(new OllamaChatClient(data.ollamaURI, modelName));

            var chatClient = builder.Build().Services.GetRequiredService<IChatClient>();

            var chatHistory = new List<ChatMessage>();
            chatHistory.Add(new(ChatRole.System, systemPrompt));

            chatHistory.Add(new(ChatRole.User, _prompt));

            var chatResponse = "";
            await foreach (var item in chatClient.GetStreamingResponseAsync(chatHistory)) chatResponse += item.Text;

            chatHistory.Add(new(ChatRole.Assistant, chatResponse));

            return chatResponse;
        }
    }
}
#endif