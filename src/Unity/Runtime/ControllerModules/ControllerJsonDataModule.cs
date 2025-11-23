using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityNeuroSpeech.Shared;
using UnityNeuroSpeech.Utils;

namespace UnityNeuroSpeech.Runtime.JsonData
{
    internal class ControllerJsonDataModule
    {
        private string _jsonDialogHistoryFileName, _encryptionHistoryKey;

        public ControllerJsonDataModule(string jsonDialogHistoryFileName, string encryptionHistoryKey)
        {
            _jsonDialogHistoryFileName = jsonDialogHistoryFileName;
            _encryptionHistoryKey = encryptionHistoryKey;
        }

        public SharedJsonData? LoadSharedSettingsModular()
        {
            var jsonFile = Resources.Load<TextAsset>("UnityNeuroSpeech/UnityNeuroSpeechSharedSettings");

            if (!jsonFile)
            {
                LogUtils.LogError("You need to create settings in \"UnityNeuroSpeech/Create Settings\"!");
                return null;
            }
            
            return JsonUtility.FromJson<SharedJsonData>(jsonFile.text);
        }

        public void LoadOrCreateJsonDialogHistoryModular(List<ChatMessage> chatHistory)
        {
            // If user don't want to save dialog history to json
            if (string.IsNullOrEmpty(_jsonDialogHistoryFileName)) return;

            // If some history already exists and saved
            if (File.Exists(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{_jsonDialogHistoryFileName}.json")))
            {
                var json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{_jsonDialogHistoryFileName}.json"));

                RuntimeJsonData runtimeData;

                // Encryption?
                if (!string.IsNullOrEmpty(_encryptionHistoryKey))
                {
                    try
                    {
                        var encryptedData = EncryptionUtils.Decrypt(json, _encryptionHistoryKey);
                        runtimeData = JsonUtility.FromJson<RuntimeJsonData>(encryptedData);
                    }
                    catch (Exception e)
                    {
                        LogUtils.LogError($"Error decoding encrypted data with this key! If you don't use any encryption, delete key parameter in your AgentBehaviour script. Full error message: {e}");
                        return;
                    }
                }
                else runtimeData = JsonUtility.FromJson<RuntimeJsonData>(json);

                // Add it to current _chatHistory
                foreach (var message in runtimeData.dialogHistory)
                {
                    chatHistory.Add(new(ChatRole.User, message.userMessage));
                    chatHistory.Add(new(ChatRole.Assistant, message.llmResponse));
                }

                LogUtils.LogMessage("Dialog history restored");
            }
            else
            {
                LogUtils.LogMessage("No dialog history was found");

                var emptyList = new List<DialogData>();
                // If don't add anything and just creating empty json file, JsonUtility won't be able to deserialize RuntimeJsonData
                emptyList.Add(new(string.Empty, string.Empty));

                var json = JsonUtility.ToJson(new RuntimeJsonData(emptyList), false);

                if (!string.IsNullOrEmpty(_encryptionHistoryKey))
                {
                    File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{_jsonDialogHistoryFileName}.json"), EncryptionUtils.Encrypt(json, _encryptionHistoryKey));
                }
                else File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{_jsonDialogHistoryFileName}.json"), json);
            }
        }

        /// <summary>
        /// For Ollama
        /// </summary>
        public void UpdateJsonDialogHistoryModular(DialogData lastDialog)
        {
            // If user doesn't want to save history
            if (string.IsNullOrEmpty(_jsonDialogHistoryFileName)) return;

            var jsonLoaded = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{_jsonDialogHistoryFileName}.json"));

            RuntimeJsonData runtimeData;
            if (!string.IsNullOrEmpty(_encryptionHistoryKey))
            {
                try
                {
                    var encryptedData = EncryptionUtils.Decrypt(jsonLoaded, _encryptionHistoryKey);
                    runtimeData = JsonUtility.FromJson<RuntimeJsonData>(encryptedData);
                }
                catch
                {
                    LogUtils.LogError($"Error decoding encrypted data with this key! If you don't use any encryption, delete key parameter in your AgentBehaviour script.");
                    return;
                }
            }
            else runtimeData = JsonUtility.FromJson<RuntimeJsonData>(jsonLoaded);

            runtimeData.dialogHistory.Add(lastDialog);

            var jsonToSave = JsonUtility.ToJson(runtimeData, true);

            if (!string.IsNullOrEmpty(_encryptionHistoryKey))
            {
                File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{_jsonDialogHistoryFileName}.json"), EncryptionUtils.Encrypt(jsonToSave, _encryptionHistoryKey));
            }
            else File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History", $"{_jsonDialogHistoryFileName}.json"), jsonToSave);

            LogUtils.LogMessage("Dialog history updated");
        }
    }
}