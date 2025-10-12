#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Whisper;
using Whisper.Utils;
using UnityEngine.UI;
using LogUtils = UnityNeuroSpeech.Utils.LogUtils;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class SceneManagerCreation : EditorWindow
    {
        // I'd use TMP_Dropdown, but Whisper supports only legacy UI(bruh)
        private Dropdown _microphoneDropdown;
        private string _whisperModelPath;

        [MenuItem("UnityNeuroSpeech/Create UNS Manager")]
        public static void ShowWindow() => GetWindow<SceneManagerCreation>("CreateUNSManager");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("UnityNeuroSpeech manager settings", EditorStyles.boldLabel);

            if (_microphoneDropdown == null) GUI.backgroundColor = Color.red;

            _microphoneDropdown = (Dropdown)EditorGUILayout.ObjectField(new GUIContent("Microphone dropdown"), _microphoneDropdown, typeof(Dropdown), true);

            GUI.backgroundColor = Color.white;

            if (string.IsNullOrEmpty(_whisperModelPath)) GUI.backgroundColor = Color.red;

            _whisperModelPath = EditorGUILayout.TextField(new GUIContent("Whisper model path in StreamingAssets", "For example: \"UnityNeuroSpeech/Whisper/ggml-medium.bin\""), _whisperModelPath);

            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Create UnityNeuroSpeech manager in scene"))
            {
                if (string.IsNullOrEmpty(_whisperModelPath) || _microphoneDropdown == null)
                {
                    LogUtils.LogError("[UnityNeuroSpeech] All fields in \"SceneManagerCreation\" can't be empty!");
                    return;
                }

                var managerObj = new GameObject("UnityNeuroSpeechManager");

                var wm = managerObj.AddComponent<WhisperManager>();
                wm.useVad = false;
                wm.language = "auto";
                wm.IsModelPathInStreamingAssets = true;
                wm.ModelPath = _whisperModelPath;

                var mr = managerObj.AddComponent<MicrophoneRecord>();
                mr.useVad = false;
                mr.echo = false;
                mr.microphoneDropdown = _microphoneDropdown;
            }
        }
    }
}
#endif