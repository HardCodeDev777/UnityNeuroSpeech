#if UNITY_EDITOR
using UnityNeuroSpeech.Runtime;
using UnityNeuroSpeech.Utils;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class CreateAgentTemplate : EditorWindow
    {
        private string _modelName = "qwen3:1.7b", _agentName = "Alex", _systemPrompt = "Your answer must be fewer than 50 words";

        // Commented out to avoid conflicts with the generated CreateAgent.cs file.
        // [MenuItem("UnityNeuroSpeech/Create Agent")]
        // public static void ShowWindow() => GetWindow<CreateAgent>("CreateAgent");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Agent parametrs", EditorStyles.boldLabel);

            _modelName = EditorGUILayout.TextField("Model name", _modelName);
            _agentName = EditorGUILayout.TextField("Agent name", _agentName);

            _systemPrompt = EditorGUILayout.TextField("System prompt", _systemPrompt);

            if (GUILayout.Button("Generate Agent"))
            {
                // The foundation. Without this, nothing works.
                if (string.IsNullOrEmpty(_modelName) || string.IsNullOrEmpty(_modelName))
                {
                    LogUtils.LogError("[UnityNeuroSpeech] \"Model name\" and \"Agent name\" must not be empty!");
                    return;
                }

                SafeExecutionUtils.SafeExecute("CreateAgentSettings", CreateAgentSettings);
                SafeExecutionUtils.SafeExecute("CreateAgentController", CreateAgentController);
            }
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
    }
}
#endif