#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityNeuroSpeech.Runtime;
using UnityNeuroSpeech.Utils;
using Whisper;
using Whisper.Utils;
using LogUtils = UnityNeuroSpeech.Utils.LogUtils;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class CreateAgentTemplate : EditorWindow
    {
        private string _modelName = "qwen3:1.7b", _agentName = "Alex", _systemPrompt = "Your answer must be fewer than 50 words";

        private Button _enableMicButton;
        private Sprite _enableMicSprite, _disableMicSprite;
        private AudioSource _responseAudioSource;

        // Commented out to avoid conflicts with the generated CreateAgent.cs file.
        // [MenuItem("UnityNeuroSpeech/Create Agent")]
        // public static void ShowWindow() => GetWindow<CreateAgent>("CreateAgent");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Agent parameters", EditorStyles.boldLabel);

            if (string.IsNullOrEmpty(_modelName)) GUI.backgroundColor = Color.red;

            _modelName = EditorGUILayout.TextField("Model name", _modelName);

            GUI.backgroundColor = Color.white;

            if (string.IsNullOrEmpty(_agentName)) GUI.backgroundColor = Color.red;

            _agentName = EditorGUILayout.TextField("Agent name", _agentName);

            GUI.backgroundColor = Color.white;

            _systemPrompt = EditorGUILayout.TextField("System prompt", _systemPrompt);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Agent component values", EditorStyles.boldLabel);

            if (_enableMicButton == null) GUI.backgroundColor = Color.red;

            _enableMicButton = (Button)EditorGUILayout.ObjectField(new GUIContent("Microphone enable Button"), _enableMicButton, typeof(Button), true);

            GUI.backgroundColor = Color.white;

            if (_enableMicSprite == null) GUI.backgroundColor = Color.red;

            _enableMicSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Enabled microphone Sprite"), _enableMicSprite, typeof(Sprite), true);

            GUI.backgroundColor = Color.white;

            if (_disableMicSprite == null) GUI.backgroundColor = Color.red;

            _disableMicSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Disabled microphone Sprite"), _disableMicSprite, typeof(Sprite), true);

            GUI.backgroundColor = Color.white;

            if (_responseAudioSource == null) GUI.backgroundColor = Color.red;

            _responseAudioSource = (AudioSource)EditorGUILayout.ObjectField(new GUIContent("Response AudioSource"), _responseAudioSource, typeof(AudioSource), true);

            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Generate Agent"))
            {
                if (string.IsNullOrEmpty(_modelName) || string.IsNullOrEmpty(_agentName))
                {
                    LogUtils.LogError("[UnityNeuroSpeech] \"Model name\" and \"Agent name\" can't be empty!");
                    return;
                }

                if(_enableMicButton == null || _disableMicSprite == null || _enableMicSprite == null || _responseAudioSource == null)
                {
                    LogUtils.LogError("[UnityNeuroSpeech] All values in \"Agent component values\" can't be empty!");
                    return;
                }

                    SafeExecutionUtils.SafeExecute("CreateAgentSettings", CreateAgentSettings);
                SafeExecutionUtils.SafeExecute("CreateAgentController", CreateAgentController);
            }

            if (GUILayout.Button("Create agent in scene")) SafeExecutionUtils.SafeExecute("CreateAgentInScene", CreateAgentInScene);
        }

        private void CreateAgentSettings()
        {
            // Create the ScriptableObject with all the selected parameters.
            var so = CreateInstance<AgentSettings>();
            so.agentName = _agentName;

            // The part “For example: <angry>, <happy>, <sad>, etc.” will be replaced in CreateSettings.cs
            // using the custom emotion list defined earlier.
            // This is a "hidden" prefix system prompt (you’ll see it when selecting ScriptableObjects),
            // and it’s prepended to the prompt entered when creating the agent.
            var finalSystemPrompt = $"You MUST response with emotion tag and your response MUST starts with emotion tag. For example: <angry>, <happy>, <sad>, etc. Only then write your response. Also NEVER use emoji. NEVER IGNORE THIS RULES! \n {_systemPrompt}";
            so.systemPrompt = finalSystemPrompt;

            so.modelName = _modelName;
            // Create the asset file on disk.
            var path = AssetDatabase.GenerateUniqueAssetPath($"Assets/UnityNeuroSpeech/Runtime/GeneratedAgents/Agent_{_agentName}.asset");
            AssetDatabase.CreateAsset(so, path);
        }

        private void CreateAgentController()
        {
            // Here’s where the magic starts.

            // We copy the base agent controller (used only for copying — it’s even marked internal),
            // and create a new script with the agent's name.
            AssetDatabase.CopyAsset("Assets/UnityNeuroSpeech/Editor/BaseAgentController.cs", $"Assets/UnityNeuroSpeech/Runtime/GeneratedAgents/{_agentName}Controller.cs");

            var path = Path.Combine(Application.dataPath, $"UnityNeuroSpeech/Runtime/GeneratedAgents/{_agentName}Controller.cs");

            // Read the newly created script so we can modify its content.
            var content = File.ReadAllText(path);

            // Perform string replacements to turn it into a functional runtime script.
            content = content.Replace("BaseAgentController", $"{_agentName}Controller");
            content = content.Replace("using UnityNeuroSpeech.Runtime;", "");
            content = content.Replace("UnityNeuroSpeech.Editor", "UnityNeuroSpeech.Runtime");
            content = content.Replace("internal", "public");
            content = content.Replace("#if UNITY_EDITOR", "");
            content = content.Replace("#endif", "");
            content = content.Replace("/// Base agent controller. This script gets duplicated and modified by the editor window,", $"/// {_agentName} controller");
            content = content.Replace("/// but the core functionality stays unchanged.", "");

            // Save it back to disk.
            File.WriteAllText(path, content);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CreateAgentInScene()
        {
            // Find GameObject in scene
            var unsManager = GameObject.Find("UnityNeuroSpeechManager");

            if (unsManager == null)
            {
                LogUtils.LogError("[UnityNeuroSpeech] Create UnityNeuroSpeechManager in your scene!");
                return;
            }

            // Create new GameObject in scene
            var agentObj = new GameObject($"{_agentName}Agent");

            // Find generated agent controller
            var agentControllerType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes()).FirstOrDefault(t => t.Name == $"{_agentName}Controller");

            if (agentControllerType == null)
            {
                LogUtils.LogError($"[UnityNeuroSpeech] No {_agentName}Controlller was found!");
                return;
            }

            // Add component with this type
            var agentController = agentObj.AddComponent(agentControllerType);

            // Find generated agent settings
            var settingsPath = $"Assets/UnityNeuroSpeech/Runtime/GeneratedAgents/Agent_{_agentName}.asset";
            var generatedAgentSettings = AssetDatabase.LoadAssetAtPath<AgentSettings>(settingsPath);

            // Change values in component
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