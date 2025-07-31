using UnityEditor;
using UnityEngine;
using UnityNeuroSpeech.Runtime;
using UnityNeuroSpeech.Utils;
using Whisper;
using Whisper.Utils;
using UnityEngine.UI;
using LogUtils = UnityNeuroSpeech.Utils.LogUtils;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class SceneManagerCreation : EditorWindow
    {
        // I'd use TMP_Dropdown, but whisper.unity supports only legacy UI(bruh)
        private Dropdown _microphoneDropdown;

        private string _whisperModelPath;

        [MenuItem("UnityNeuroSpeech/Scene Manager Creation")]
        public static void ShowWindow() => GetWindow<SceneManagerCreation>("SceneManagerCreation");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("UnityNeuroSpeech manager settings", EditorStyles.boldLabel);

            if (_microphoneDropdown == null) GUI.backgroundColor = Color.red;

            _microphoneDropdown = (Dropdown)EditorGUILayout.ObjectField(new GUIContent("Microphone Dropdown"), _microphoneDropdown, typeof(Dropdown), true);

            GUI.backgroundColor = Color.white;

            if (string.IsNullOrEmpty(_whisperModelPath)) GUI.backgroundColor = Color.red;

            _whisperModelPath = EditorGUILayout.TextField(new GUIContent("Whisper Model Path", "\"Without Assets directory. For example: UnityNeuroSpeech/Whisper/ggml-medium.bin\""), _whisperModelPath);

            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Create UnityNeuroSpeech manager in scene"))
            {
                if (string.IsNullOrEmpty(_whisperModelPath) || _microphoneDropdown == null)
                {
                    LogUtils.LogError("[UnityNeuroSpeech] All fields in \"SceneManagerCreation\" can't be empty!");
                    return;
                }

                // Create new GameObject in scene
                var managerObj = new GameObject("UnityNeuroSpeechManager");

                // Add and configure WhisperManager.cs
                var wm = managerObj.AddComponent<WhisperManager>();
                wm.useVad = false;
                wm.language = "auto";
                wm.IsModelPathInStreamingAssets = false;
                wm.ModelPath = "";
                // Idk why, but it's private
                wm.ChangePrivateField("initOnAwake", false);

                // Add and configure MicrophoneRecord.cs
                var mr = managerObj.AddComponent<MicrophoneRecord>();
                mr.useVad = false;
                mr.echo = false;
                mr.microphoneDropdown = _microphoneDropdown;

                // Add and configure SetupWhisperPath.cs
                var swp = managerObj.AddComponent<SetupWhisperPath>();
                swp.ChangePrivateField("_whisper", wm);
                swp.ChangePrivateField("_modelPath", _whisperModelPath);
            }
        }
    }

}