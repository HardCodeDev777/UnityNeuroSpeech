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
        // I really wouldn't want someone to do this, but I personally like to throw all tools and assets into Assets/Imports.
        // In this case, path logic breaks, which obviously shouldn't happen.
        private bool _isFrameworkInAnotherFolder;
        private string _anotherFolderName;

        // Custom URIs
        private string _customOllamaURI, _customTTSURI;

        // Log level
        private int _selectedLogIndex;
        private string[] _logOptions = new[] { "None", "Error", "All" };

        // Request timeout
        private int _requestTimeout = 30;

        // Python debug
        private bool _enablePythonDebug = true;
        private string _absolutePathToMainPy;

        // Emotions
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
            // General
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

            // Agents
            EditorGUILayout.LabelField("Agents", EditorStyles.boldLabel);

            if (_emotions.Count == 0) GUI.backgroundColor = Color.red;

            _emotionsReorderableList.DoLayoutList();

            GUI.backgroundColor = Color.white;

            if (_requestTimeout == 0) GUI.backgroundColor = Color.red;

            _requestTimeout = EditorGUILayout.IntField(new GUIContent("Request timeout(secs)", "Timeout for requests to local TTS Python sever"), _requestTimeout);

            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            // Python
            EditorGUILayout.LabelField("Python", EditorStyles.boldLabel);

            _enablePythonDebug = EditorGUILayout.Toggle(new GUIContent("Enable Python debug", "If framework isn't in Assets directory, turn it on"), _enablePythonDebug);

            if (string.IsNullOrEmpty(_absolutePathToMainPy)) GUI.backgroundColor = Color.red;

            _absolutePathToMainPy = EditorGUILayout.TextField(new GUIContent("Absolute path to main.py"), _absolutePathToMainPy);

            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            // Advanced
            EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);

            _customOllamaURI = EditorGUILayout.TextField(new GUIContent("Custom Ollama URI", "If empty, Ollama URI will be default \"localhost:11434\""), _customOllamaURI);

            _customTTSURI = EditorGUILayout.TextField(new GUIContent("Custom TTS URI", "If empty, TTS URI will be default \"localhost:7777\""), _customTTSURI);
            //

            if (GUILayout.Button("Save"))
            {
                LogUtils.logLevel = _logOptions[_selectedLogIndex] switch
                {
                    "None" => LogLevel.None,
                    "Error" => LogLevel.Error,
                    "All" => LogLevel.All,
                    _ => LogLevel.All
                };

                if (_emotions.Count == 0 || _emotions.Contains(string.Empty))
                {
                    LogUtils.LogError("[UnityNeuroSpeech] You need to add at least one emotion!");
                    return;
                }

                if (_requestTimeout == 0)
                {
                    LogUtils.LogError("[UnityNeuroSpeech] \"Request timeout\" field can't be empty!");
                    return;
                }

                if (string.IsNullOrEmpty(_absolutePathToMainPy))
                {
                    LogUtils.LogError("[UnityNeuroSpeech] You need to write absolute path to main.py!");
                    return;
                }

                if (!File.Exists(_absolutePathToMainPy))
                {
                    LogUtils.LogError("[UnityNeuroSpeech] main.py doesn't exist in this path!");
                    return;
                }

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
                    // Later, we�ll replace parts of it to fit the custom folder structure.
                    AssetDatabase.DeleteAsset($"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs");
                    AssetDatabase.CopyAsset($"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgentTemplate.cs", $"Assets/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs");

                    // Since we'll use System.IO, we must use absolute paths.
                    createAgentScriptPath = Application.dataPath + $"/{_anotherFolderName}/UnityNeuroSpeech/Editor/CreateAgent.cs";

                    // Get the generated script content from disk.
                    createAgentScriptContent = File.ReadAllText(createAgentScriptPath);

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

                    createAgentScriptContent = File.ReadAllText(createAgentScriptPath);
                }

                // Flatten all emotions into a single comma-separated string.
                var emotionsString = "";
                foreach (var em in _emotions) emotionsString += $"<{em}>, ";

                // Replace the system prompt to explicitly instruct the model to use only these emotions.
                // (Note: some smaller models might still mess up, even with strict prompts)
                createAgentScriptContent = createAgentScriptContent.Replace("For example: <angry>, <happy>, <sad>, etc.", $"You can only use these emotions: {emotionsString}. WRITE THEM ONLY LIKE I SAID.");

                // Turn the template into a real editor window script.
                createAgentScriptContent = createAgentScriptContent.Replace("CreateAgentTemplate", "CreateAgent");
                createAgentScriptContent = createAgentScriptContent.Replace("// [MenuItem(\"UnityNeuroSpeech/Create Agent\")]", "[MenuItem(\"UnityNeuroSpeech/Create Agent\")]");
                createAgentScriptContent = createAgentScriptContent.Replace("// public static void ShowWindow() => GetWindow<CreateAgent>(\"CreateAgent\");", "public static void ShowWindow() => GetWindow<CreateAgent>(\"CreateAgent\");");
                createAgentScriptContent = createAgentScriptContent.Replace("// Commented out to avoid conflicts with the generated CreateAgent.cs file.", "");

                File.WriteAllText(createAgentScriptPath, createAgentScriptContent);

                // Changing debug in Python
                string mainPyContent;

                mainPyContent = File.ReadAllText(_absolutePathToMainPy);

                if (_enablePythonDebug)
                {
                    // If debug already enabled
                    if (!mainPyContent.Contains("# warnings.simplefilter(action='ignore', category=FutureWarning)"))
                    {
                        mainPyContent = mainPyContent.Replace("warnings.simplefilter(action='ignore', category=FutureWarning", "# warnings.simplefilter(action='ignore', category=FutureWarning)");
                        mainPyContent = mainPyContent.Replace("sys.stdout = open(os.devnull, 'w')", "# sys.stdout = open(os.devnull, 'w')");
                        mainPyContent = mainPyContent.Replace("logging.disable(logging.CRITICAL)", "# logging.disable(logging.CRITICAL)");
                        mainPyContent = mainPyContent.Replace("# print(f\"Python executable(for gebug): {sys.executable}\")", "print(f\"Python executable(for gebug): {sys.executable}\")");
                    }
                }
                else
                {
                    mainPyContent = mainPyContent.Replace("# warnings.simplefilter(action='ignore', category=FutureWarning)", "warnings.simplefilter(action='ignore', category=FutureWarning");
                    mainPyContent = mainPyContent.Replace("# sys.stdout = open(os.devnull, 'w')", "sys.stdout = open(os.devnull, 'w')");
                    mainPyContent = mainPyContent.Replace("# logging.disable(logging.CRITICAL)", "logging.disable(logging.CRITICAL)");
                    mainPyContent = mainPyContent.Replace("print(f\"Python executable(for gebug): {sys.executable}\")", "# print(f\"Python executable(for gebug): {sys.executable}\")");
                }

                File.WriteAllText(_absolutePathToMainPy, mainPyContent);

                // Save the settings into a JSON file(unreadable code moment)   
                var data = new JsonData(LogUtils.logLevel, string.IsNullOrEmpty(_customOllamaURI) ? "http://localhost:11434" : _customOllamaURI, string.IsNullOrEmpty(_customTTSURI) ? "http://localhost:7777" : _customTTSURI, _requestTimeout);

                var json = JsonUtility.ToJson(data, true);

                var dir = Application.dataPath + "/Resources/Settings";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "UnityNeuroSpeechSettings.json"), json);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif