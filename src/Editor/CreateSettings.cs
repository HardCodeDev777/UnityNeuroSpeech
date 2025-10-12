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
        private bool _isFrameworkInAnotherFolder;
        private string _anotherFolderName, _customOllamaURI;

        private int _selectedLogIndex = 2;
        private string[] _logOptions = new[] { "None", "Error", "All" };

        [MenuItem("UnityNeuroSpeech/Create Settings")]
        public static void ShowWindow() => GetWindow<CreateSettings>("CreateSettings");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

            _selectedLogIndex = EditorGUILayout.Popup("Logging type", _selectedLogIndex, _logOptions);

            _isFrameworkInAnotherFolder = EditorGUILayout.Toggle(new GUIContent("Not in Assets folder", "If framework isn't in Assets directory, turn it on"), _isFrameworkInAnotherFolder);

            if (!_isFrameworkInAnotherFolder)
            {
                GUI.enabled = false;
                GUI.backgroundColor = Color.white;
            }

            if (_isFrameworkInAnotherFolder && string.IsNullOrEmpty(_anotherFolderName)) GUI.backgroundColor = Color.red;

            _anotherFolderName = EditorGUILayout.TextField(new GUIContent("Directory name", "For example, if you throw this framework in Assets\\MyImports\\Frameworks, then write \"MyImports/Frameworks\""), _anotherFolderName);

            GUI.backgroundColor = Color.white;

            if (!_isFrameworkInAnotherFolder) GUI.enabled = true;

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);

            _customOllamaURI = EditorGUILayout.TextField(new GUIContent("Custom Ollama URI", "If empty, Ollama URI will be default \"localhost:11434\""), _customOllamaURI);

            if (GUILayout.Button("Create settings file"))
            {
                LogUtils.logLevel = _logOptions[_selectedLogIndex] switch
                {
                    "None" => LogLevel.None,
                    "Error" => LogLevel.Error,
                    "All" => LogLevel.All,
                    _ => LogLevel.All
                };

                string createAgentScriptContent, createAgentScriptPath;

                if (_isFrameworkInAnotherFolder)
                {
                    // If the framework is placed in another folder, we'll have to change paths in multiple places.

                    if (string.IsNullOrEmpty(_anotherFolderName))
                    {
                        LogUtils.LogError("[UnityNeuroSpeech] If you enabled \"Not in Assets folder\", you need to write \"Directory name\"?");
                        return;
                    }

                    // Remove the old CreateAgent.cs and copy a template version.
                    AssetDatabase.DeleteAsset($"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs");
                    AssetDatabase.CopyAsset($"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgentTemplate.cs", $"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs");

                    createAgentScriptPath = Application.dataPath + $"/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs";
                    createAgentScriptContent = File.ReadAllText(createAgentScriptPath);

                    // Replace asset paths inside the code to match the custom folder.
                    createAgentScriptContent = createAgentScriptContent.Replace("UnityNeuroSpeech/Runtime", $"{_anotherFolderName}/UnityNeuroSpeech/Runtime");
                    createAgentScriptContent = createAgentScriptContent.Replace("UnityNeuroSpeech/Editor", $"{_anotherFolderName}/UnityNeuroSpeech/Editor");
                }
                else
                {
                    // If the framework is in the default location, we just use standard paths

                    AssetDatabase.DeleteAsset($"Assets/UnityNeuroSpeech/Editor/CreateAgent.cs");
                    AssetDatabase.CopyAsset($"Assets/UnityNeuroSpeech/Editor/CreateAgentTemplate.cs", $"Assets/UnityNeuroSpeech/Editor/CreateAgent.cs");

                    createAgentScriptPath = Application.dataPath + $"/UnityNeuroSpeech/Editor/CreateAgent.cs";

                    createAgentScriptContent = File.ReadAllText(createAgentScriptPath);
                }

                // Turn the template into a real editor window script.
                createAgentScriptContent = createAgentScriptContent.Replace("CreateAgentTemplate", "CreateAgent");
                createAgentScriptContent = createAgentScriptContent.Replace("// [MenuItem(\"UnityNeuroSpeech/Create Agent\")]", "[MenuItem(\"UnityNeuroSpeech/Create Agent\")]");
                createAgentScriptContent = createAgentScriptContent.Replace("// public static void ShowWindow() => GetWindow<CreateAgent>(\"CreateAgent\");", "public static void ShowWindow() => GetWindow<CreateAgent>(\"CreateAgent\");");
                createAgentScriptContent = createAgentScriptContent.Replace("/// Template class for future CreateAgent.cs. Some parts'll be replaced in CreateSettings.cs", "/// Generated from CreateSettings.cs");

                File.WriteAllText(createAgentScriptPath, createAgentScriptContent);
  
                var sharedData = new SharedJsonData(LogUtils.logLevel, string.IsNullOrEmpty(_customOllamaURI) ? "http://localhost:11434" : _customOllamaURI, _isFrameworkInAnotherFolder? $"/{_anotherFolderName}" :  string.Empty);

                var sharedJson = JsonUtility.ToJson(sharedData, true);

                var sharedDir = Application.dataPath + "/Resources/UnityNeuroSpeech";

                if (!Directory.Exists(sharedDir)) Directory.CreateDirectory(sharedDir);
                File.WriteAllText(Path.Combine(sharedDir, "UnityNeuroSpeechSharedSettings.json"), sharedJson);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif
