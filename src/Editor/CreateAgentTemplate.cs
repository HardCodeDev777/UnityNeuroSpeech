#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityNeuroSpeech.Runtime;
using UnityNeuroSpeech.Utils;
using Whisper;
using Whisper.Utils;
using LogUtils = UnityNeuroSpeech.Utils.LogUtils;

namespace UnityNeuroSpeech.Editor
{
    /// <summary>
    /// Template class for future CreateAgent.cs. Some parts'll be replaced in CreateSettings.cs
    /// </summary>
    internal sealed class CreateAgentTemplate : EditorWindow
    {
        private string _modelName = "deepseek-r1:7b", _agentName = "Alex", _systemPrompt = "Your answer must be fewer than 50 words";
        private int _agentIndex = 0;

        private Button _enableMicButton;
        private Sprite _enableMicSprite, _disableMicSprite;
        private AudioSource _responseAudioSource;

        private ReorderableList _emotionsReorderableList, _actionsReorderableList;
        private List<string> _emotions = new(), _actions = new();

        private bool _wasAgentGenerated;

        // [MenuItem("UnityNeuroSpeech/Create Agent")]
        // public static void ShowWindow() => GetWindow<CreateAgent>("CreateAgent");

        /// <summary>
        /// Just to make cool lists in Editor
        /// </summary>
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

            _actionsReorderableList = new(_actions, typeof(string), true, true, true, true);

            _actionsReorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Actions");
            };

            _actionsReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                _actions[index] = EditorGUI.TextField(
                    new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight),
                    _actions[index]
                );
            };

            _actionsReorderableList.onAddCallback = (ReorderableList list) =>
            {
                _actions.Add(string.Empty);
            };

            _actionsReorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                _actions.RemoveAt(list.index);
            };
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Agent parameters", EditorStyles.boldLabel);

            _agentIndex = EditorGUILayout.IntField(new GUIContent("Agent index", "This index will be using for selecting voice for agent. E.g. if your voice file name is \"en_voice2\" and you want to use it for this agent, you need to write \"2\""), _agentIndex);

            if (string.IsNullOrEmpty(_modelName)) GUI.backgroundColor = Color.red;

            _modelName = EditorGUILayout.TextField("LLM Model name", _modelName);

            GUI.backgroundColor = Color.white;

            if (string.IsNullOrEmpty(_agentName)) GUI.backgroundColor = Color.red;

            _agentName = EditorGUILayout.TextField("Agent name", _agentName);

            GUI.backgroundColor = Color.white;

            EditorGUILayout.LabelField("System prompt");

            var style = new GUIStyle(EditorStyles.textArea) { wordWrap = true };
            _systemPrompt = EditorGUILayout.TextArea(_systemPrompt, style, GUILayout.Height(150));

            EditorGUILayout.Space(10);

            if (_emotions.Count == 0) GUI.backgroundColor = Color.red;

            _emotionsReorderableList.DoLayoutList();

            GUI.backgroundColor = Color.white;

            _actionsReorderableList.DoLayoutList();

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Agent component values", EditorStyles.boldLabel);

            if (_enableMicButton == null) GUI.backgroundColor = Color.red;

            _enableMicButton = (Button)EditorGUILayout.ObjectField(new GUIContent("Microphone enable/disable button"), _enableMicButton, typeof(Button), true);

            GUI.backgroundColor = Color.white;

            if (_enableMicSprite == null) GUI.backgroundColor = Color.red;

            _enableMicSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Enabled microphone sprite"), _enableMicSprite, typeof(Sprite), true);

            GUI.backgroundColor = Color.white;

            if (_disableMicSprite == null) GUI.backgroundColor = Color.red;

            _disableMicSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Disabled microphone sprite"), _disableMicSprite, typeof(Sprite), true);

            GUI.backgroundColor = Color.white;

            if (_responseAudioSource == null) GUI.backgroundColor = Color.red;

            _responseAudioSource = (AudioSource)EditorGUILayout.ObjectField(new GUIContent("Response audiosource"), _responseAudioSource, typeof(AudioSource), true);

            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Generate agent"))
            {
                if (_emotions.Count == 0 || _emotions.Contains(string.Empty))
                {
                    LogUtils.LogError("[UnityNeuroSpeech] You need to add at least one emotion!");
                    return;
                }

                if (string.IsNullOrEmpty(_modelName) || string.IsNullOrEmpty(_agentName))
                {
                    LogUtils.LogError("[UnityNeuroSpeech] \"Model name\" and \"Agent name\" can't be empty!");
                    return;
                }

                if (_enableMicButton == null || _disableMicSprite == null || _enableMicSprite == null || _responseAudioSource == null)
                {
                    LogUtils.LogError("[UnityNeuroSpeech] All values in \"Agent component values\" can't be empty!");
                    return;
                }

                // Logs here are kinda broken
                SafeExecutionUtils.SafeExecute("GenerateAgent", GenerateAgent);
            }

            if (_wasAgentGenerated) if (GUILayout.Button("Create agent in scene")) SafeExecutionUtils.SafeExecute("CreateAgentInScene", CreateAgentInScene);
        }

        private void GenerateAgent()
        {
            SafeExecutionUtils.SafeExecute("CreateAgentSettings", CreateAgentSettings);
            SafeExecutionUtils.SafeExecute("CreateAgentController", CreateAgentController);
            _wasAgentGenerated = true;
        }

        private void CreateAgentSettings()
        {
            // Flatten all emotions into a single comma-separated string.
            var emotionsString = "";
            foreach (var em in _emotions) emotionsString += $"<{em}>, ";

            // Same with actions
            var actionsString = "";
            if (_actions.Count != 0 && !_actions.Contains(string.Empty))
                foreach (var act in _actions) actionsString += $"<{act}>, ";

            // If already exists AgentSettings with this agentName, delete it
            if (File.Exists(Application.dataPath + $"/UnityNeuroSpeech/Runtime/GeneratedAgents/Agent_{_agentName}.asset")) 
                File.Delete(Application.dataPath + $"/UnityNeuroSpeech/Runtime/GeneratedAgents/Agent_{_agentName}.asset");

            // Create the ScriptableObject with all the selected parameters
            var so = CreateInstance<AgentSettings>();
            so.agentIndex = _agentIndex;
            so.agentName = _agentName;
            so.systemPrompt = _actions.Count == 0 || _actions.Contains(string.Empty) ? $"You MUST response with emotion tag and your response MUST starts with emotion tag.You can only use these emotions: {emotionsString}. WRITE THEM ONLY LIKE I SAID. Also NEVER USE EMOJI" : $"You MUST response with emotion tag and then with action tag, and your response MUST starts with emotion tag, ONLY then action tag.You can only use these emotions: {emotionsString} and these tags: {actionsString}. WRITE THEM ONLY LIKE I SAID. Also NEVER USE EMOJI";
            so.modelName = _modelName;

            var path = AssetDatabase.GenerateUniqueAssetPath($"Assets/UnityNeuroSpeech/Runtime/GeneratedAgents/Agent_{_agentName}.asset");
            AssetDatabase.CreateAsset(so, path);
        }

        private void CreateAgentController()
        {
            // We copy the base agent controller and create a new script with the agent's name
            AssetDatabase.CopyAsset("Assets/UnityNeuroSpeech/Editor/BaseAgentController.cs", $"Assets/UnityNeuroSpeech/Runtime/GeneratedAgents/{_agentName}Controller.cs");

            var path = Path.Combine(Application.dataPath, $"UnityNeuroSpeech/Runtime/GeneratedAgents/{_agentName}Controller.cs");

            var content = File.ReadAllText(path);

            // Turn it into a functional runtime script
            content = content.Replace("BaseAgentController", $"{_agentName}Controller");
            content = content.Replace("using UnityNeuroSpeech.Runtime;", "");
            content = content.Replace("UnityNeuroSpeech.Editor", "UnityNeuroSpeech.Runtime");
            content = content.Replace("internal", "public");
            content = content.Replace("#if UNITY_EDITOR", "");
            content = content.Replace("#endif", "");
            content = content.Replace("/// Base agent controller. This script gets duplicated and modified by CreateAgent.cs,", $"/// {_agentName} controller");
            content = content.Replace("/// but the core functionality stays unchanged", "");

            File.WriteAllText(path, content);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CreateAgentInScene()
        {
            var unsManager = GameObject.Find("UnityNeuroSpeechManager");

            if (unsManager == null)
            {
                LogUtils.LogError("[UnityNeuroSpeech] Create UnityNeuroSpeechManager in your scene!");
                return;
            }

            var agentObj = new GameObject($"{_agentName}Agent");

            // Average Linq moment
            var agentControllerType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes()).FirstOrDefault(t => t.Name == $"{_agentName}Controller");

            if (agentControllerType == null)
            {
                LogUtils.LogError($"[UnityNeuroSpeech] No {_agentName}Controlller was found!");
                return;
            }

            var agentController = agentObj.AddComponent(agentControllerType);

            var settingsPath = $"Assets/UnityNeuroSpeech/Runtime/GeneratedAgents/Agent_{_agentName}.asset";
            var generatedAgentSettings = AssetDatabase.LoadAssetAtPath<AgentSettings>(settingsPath);

            agentControllerType.GetField("agentSettings")?.SetValue(agentController, generatedAgentSettings);
            agentController.ChangePrivateField("_whisper", unsManager.GetComponent<WhisperManager>());
            agentController.ChangePrivateField("_microphoneRecord", unsManager.GetComponent<MicrophoneRecord>());
            agentController.ChangePrivateField("_enableMicButton", _enableMicButton);
            agentController.ChangePrivateField("_enableMicSprite", _enableMicSprite);
            agentController.ChangePrivateField("_disableMicSprite", _disableMicSprite);
            agentController.ChangePrivateField("_responseAudioSource", _responseAudioSource);
        }
    }
}
#endif