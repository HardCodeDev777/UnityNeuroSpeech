#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Whisper;
using Whisper.Utils;
using LogUtils = UnityNeuroSpeech.Utils.LogUtils;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class CreateUNSManager : EditorWindow
    {
        // I'd use TMP_Dropdown, but Whisper supports only legacy UI(bruh)
        private Dropdown _microphoneDropdown;
        private string _whisperModelPath;

        [MenuItem("UnityNeuroSpeech/Main/Create UNS Manager")]
        public static void ShowWindow() => GetWindow<CreateUNSManager>("Create UNS Manager");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("UnityNeuroSpeech manager settings", EditorStyles.boldLabel);

            if (!_microphoneDropdown) GUI.backgroundColor = Color.red;

            _microphoneDropdown = (Dropdown)EditorGUILayout.ObjectField(new GUIContent("Dropdown with Microphone choice"), _microphoneDropdown, typeof(Dropdown), true);

            GUI.backgroundColor = Color.white;

            if (string.IsNullOrEmpty(_whisperModelPath)) GUI.backgroundColor = Color.red;

            _whisperModelPath = EditorGUILayout.TextField(new GUIContent("Whisper model path in StreamingAssets", "Example: \"UnityNeuroSpeech/Whisper/ggml-medium.bin\""), _whisperModelPath);

            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Create UnityNeuroSpeech manager in scene"))
            {
                if (string.IsNullOrEmpty(_whisperModelPath) || !_microphoneDropdown)
                {
                    LogUtils.LogError("Fill all required fields!");
                    return;
                }

                var managerObj = new GameObject("UnityNeuroSpeechManager");


               
                var wm = managerObj.AddComponent<WhisperManager>();
                wm.useVad = false;
                wm.language = "auto";
                wm.IsModelPathInStreamingAssets = true;
                wm.ModelPath = _whisperModelPath;
                wm.useGpu = true;
                wm.flashAttention = true;

                var mr = managerObj.AddComponent<MicrophoneRecord>();
                mr.useVad = false;
                mr.echo = false;
                mr.microphoneDropdown = _microphoneDropdown;
            }
        }
    }
}
#endif