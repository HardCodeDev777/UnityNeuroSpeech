#if UNITY_EDITOR
using UnityNeuroSpeech.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityNeuroSpeech.Shared;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class CreateSettings : EditorWindow
    {
        private int _selectedLogIndex;
        private string[] _logOptions = new[] { "None", "Error", "All" };

        // I really wouldn't want someone to do this, but I personally like to throw all tools and assets into Assets/Imports.
        // In this case, path logic breaks, which obviously shouldn't happen.
        private bool _isFrameworkInAnotherFolder;
        private string _anotherFolderName;

        private ReorderableList _emotionsReorderableList;

        private List<string> _emotions = new();

        [MenuItem("UnityNeuroSpeech/Create Settings")]
        public static void ShowWindow() => GetWindow<CreateSettings>("CreateSettings");

        // OnEnable is purely for initializing a nice and user-friendly list.
        private void OnEnable()
        {
            _emotionsReorderableList = new(_emotions, typeof(string), true, true, true, true);

            _emotionsReorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Emotions");
            };

            _emotionsReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                _emotions[index] = EditorGUI.TextField(
                    new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight),
                    _emotions[index]
                );
            };

            _emotionsReorderableList.onAddCallback = (ReorderableList list) =>
            {
                _emotions.Add(string.Empty);
            };

            _emotionsReorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                _emotions.RemoveAt(list.index);
            };
        }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            _selectedLogIndex = EditorGUILayout.Popup("Logging type", _selectedLogIndex, _logOptions);

            _emotionsReorderableList.DoLayoutList();

            _isFrameworkInAnotherFolder = EditorGUILayout.Toggle(new GUIContent("Not in Assets folder", "If framework isn't in Assets directory, turn it on"), _isFrameworkInAnotherFolder);

            if (_isFrameworkInAnotherFolder)
            {
                _anotherFolderName = EditorGUILayout.TextField(new GUIContent("Directory name", "For example, if you throw this framework in Assets\\MyImports\\Frameworks, then write \"MyImports/Frameworks\""), _anotherFolderName);
            }

            if (GUILayout.Button("Save"))
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

                    // First, remove the old CreateAgent.cs and copy a template version.
                    // Later, we’ll replace parts of it to fit the custom folder structure.
                    AssetDatabase.DeleteAsset($"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs");
                    AssetDatabase.CopyAsset($"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgentTemplate.cs", $"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs");

                    // Since we'll use System.IO, we must use absolute paths.
                    createAgentScriptPath = Application.dataPath + $"/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs";

                    // Get the generated script content from disk.
                    createAgentScriptContent = File.ReadAllText(Application.dataPath + $"/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs");

                    // Replace asset paths inside the code to match the custom folder.
                    createAgentScriptContent = createAgentScriptContent.Replace("UnityNeuroSpeech/Runtime", $"{_anotherFolderName}/UnityNeuroSpeech/Runtime");
                    createAgentScriptContent = createAgentScriptContent.Replace("UnityNeuroSpeech/Editor", $"{_anotherFolderName}/UnityNeuroSpeech/Editor");
                }
                else
                {
                    // If the framework is in the default location, we just use standard paths and follow the same flow.

                    AssetDatabase.DeleteAsset($"Assets/UnityNeuroSpeech/Editor/CreateAgent.cs");
                    AssetDatabase.CopyAsset($"Assets/UnityNeuroSpeech/Editor/CreateAgentTemplate.cs", $"Assets/UnityNeuroSpeech/Editor/CreateAgent.cs");

                    createAgentScriptPath = Application.dataPath + $"/UnityNeuroSpeech/Editor/CreateAgent.cs";

                    createAgentScriptContent = File.ReadAllText(Application.dataPath + $"/UnityNeuroSpeech/Editor/CreateAgent.cs");
                }

                if (_emotions.Count == 0)
                {
                    LogUtils.LogError("[UnityNeuroSpeech] You need to add at least one emotion!");
                    return;
                }

                // Flatten all emotions into a single comma-separated string.
                var emotionsString = "";
                foreach (var em in _emotions) emotionsString += $"<{em}>, ";

                // Replace the system prompt to explicitly instruct the model to use only these emotions.
                // (Note: some smaller models might still mess up, even with strict prompts.)
                createAgentScriptContent = createAgentScriptContent.Replace("For example: <angry>, <happy>, <sad>, etc.", $"You can only use this emotions: {emotionsString}. WRITE THEM ONLY LIKE I SAID.");

                // Turn the template into a real editor window script.
                createAgentScriptContent = createAgentScriptContent.Replace("CreateAgentTemplate", "CreateAgent");
                createAgentScriptContent = createAgentScriptContent.Replace("// [MenuItem(\"UnityNeuroSpeech/Create Agent\")]", "[MenuItem(\"UnityNeuroSpeech/Create Agent\")]");
                createAgentScriptContent = createAgentScriptContent.Replace("// public static void ShowWindow() => GetWindow<CreateAgent>(\"CreateAgent\");", "public static void ShowWindow() => GetWindow<CreateAgent>(\"CreateAgent\");");
                createAgentScriptContent = createAgentScriptContent.Replace("// Commented out to avoid conflicts with the generated CreateAgent.cs file.", "");

                File.WriteAllText(createAgentScriptPath, createAgentScriptContent);

                // Save the settings into a JSON file.   
                string finalSettingsPath;
                if (_isFrameworkInAnotherFolder) finalSettingsPath = $"Assets/{_anotherFolderName}/UnityNeuroSpeech/Resources/Settings/UnityNeuroSpeechSettings.json";
                else finalSettingsPath = $"Assets/UnityNeuroSpeech/Resources/Settings/UnityNeuroSpeechSettings.json";    
         
                var data = new JsonData(LogUtils.logLevel);
                var json = JsonUtility.ToJson(data, true);
                File.WriteAllText(finalSettingsPath, json);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif