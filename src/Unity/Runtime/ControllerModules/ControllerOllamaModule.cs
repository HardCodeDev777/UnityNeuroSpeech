using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityNeuroSpeech.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace UnityNeuroSpeech.Runtime.Ollama
{
    internal class ControllerOllamaModule
    {
        private int _responseCount;
        private IChatClient _chatClient;
        public List<ChatMessage> ChatHistory { get; private set; } = new();

        public void InitOllamaModular(string systemPrompt, string modelName, string ollamaURI)
        {
            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
            { DisableDefaults = true });

            builder.Services.AddChatClient(new OllamaChatClient(ollamaURI, modelName));

            _chatClient = builder.Build().Services.GetRequiredService<IChatClient>();

            ChatHistory.Add(new(ChatRole.System, systemPrompt));
        }

        public async UniTask<AgentState> SendMessageModular(string userPrompt, string lang, CancellationToken token)
        {
            LogUtils.LogMessage("Sending message to Ollama...");

            ChatHistory.Add(new(ChatRole.User, userPrompt));

            var chatResponse = "";
            await foreach (var item in _chatClient.GetStreamingResponseAsync(ChatHistory).WithCancellation(token)) chatResponse += item.Text;
            
            LogUtils.LogMessage($"Ollama response: {chatResponse}");

            ChatHistory.Add(new(ChatRole.Assistant, chatResponse));
            _responseCount++;

            var responseWithoutThinking = CleanThinking(chatResponse);

            LogUtils.LogMessage($"Ollama response without thinking: {responseWithoutThinking}");

            // Usually parsing works, but it fully depends on LLM response
            var emotion = ParseTag(responseWithoutThinking);
            var cleanedResponse = responseWithoutThinking.Replace($"<{emotion}>", "");

            var action = ParseTag(cleanedResponse);
            LogUtils.LogMessage($"Parsed emotion: {emotion}");

            if (!string.IsNullOrEmpty(action))
            {
                LogUtils.LogMessage($"Parsed action: {action}");
                cleanedResponse = cleanedResponse.Replace($"<{action}>", "");
            }

            return new(_responseCount, cleanedResponse, userPrompt, emotion, action);
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
    }
}