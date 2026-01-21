#if UNITY_EDITOR

using UnityNeuroSpeech.Utils;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityNeuroSpeech.Shared;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class CreateSettings : EditorWindow
    {
        private string _customOllamaURI;

        private int _selectedLogIndex = 2;
        private string[] _logOptions = new[] { "None", "Error", "All" };

        [MenuItem("UnityNeuroSpeech/Main/Create Settings")]
        public static void ShowWindow() => GetWindow<CreateSettings>("CreateSettings");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

            _selectedLogIndex = EditorGUILayout.Popup("Logging type", _selectedLogIndex, _logOptions);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);

            _customOllamaURI = EditorGUILayout.TextField(new GUIContent("Custom Ollama URI", "If empty, Ollama URI will be default \"localhost:11434\""), _customOllamaURI);

            if (GUILayout.Button("Save"))
            {
                LogUtils.logLevel = _logOptions[_selectedLogIndex] switch
                {
                    "None" => LogLevel.None,
                    "Error" => LogLevel.Error,
                    "All" => LogLevel.All,
                    _ => LogLevel.All
                };
  
                var frameworkSettings = new FrameworkSettings(LogUtils.logLevel, string.IsNullOrEmpty(_customOllamaURI) ? "http://localhost:11434" : _customOllamaURI);

                var settingsJson = JsonUtility.ToJson(frameworkSettings, true);

                var settingsDir = StaticData.FRAMEWORK_SETTINGS_DIR_PATH;
                if (!Directory.Exists(settingsDir)) Directory.CreateDirectory(settingsDir);
                File.WriteAllText(StaticData.FRAMEWORK_SETTINGS_FILE_FULL_PATH, settingsJson);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif
