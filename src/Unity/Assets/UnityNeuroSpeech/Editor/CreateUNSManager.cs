#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Whisper;
using Whisper.Utils;
using UnityNeuroSpeech.Utils;
using LogUtils = UnityNeuroSpeech.Utils.LogUtils;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class CreateUNSManager : EditorWindow
    {
        private Dropdown _microphoneDropdown;
        private string _whisperModelName;

        [MenuItem("UnityNeuroSpeech/Main/Create UNS Manager")]
        public static void ShowWindow() => GetWindow<CreateUNSManager>("Create UNS Manager");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("UnityNeuroSpeech manager settings", EditorStyles.boldLabel);

            if (!_microphoneDropdown) GUI.backgroundColor = Color.red;

            _microphoneDropdown = (Dropdown)EditorGUILayout.ObjectField(new GUIContent("Microphone dropdown", "Just create a dropdown in your scene and select it here. In runtime here will be displayed all availabe microphones"), _microphoneDropdown, typeof(Dropdown), true);

            GUI.backgroundColor = Color.white;

            if (string.IsNullOrEmpty(_whisperModelName)) GUI.backgroundColor = Color.red;

            _whisperModelName = EditorGUILayout.TextField(new GUIContent("Whisper model name", "Example: \"ggml-medium.bin\""), _whisperModelName);

            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Create UnityNeuroSpeech manager in scene"))
            {
                if (string.IsNullOrEmpty(_whisperModelName) || !_microphoneDropdown)
                {
                    LogUtils.LogError("Fill all required fields!");
                    return;
                }

                var managerObj = new GameObject("UnityNeuroSpeechManager");
               
                var wm = managerObj.AddComponent<WhisperManager>();
                wm.useVad = false;
                wm.language = "auto";
                wm.IsModelPathInStreamingAssets = true;
                wm.ModelPath = Path.Combine(StaticData.WHISPER_MODELS_LOCATION_FULL_PATH, _whisperModelName);
                wm.useGpu = true;

                var mr = managerObj.AddComponent<MicrophoneRecord>();
                mr.useVad = false;
                mr.echo = false;
                mr.microphoneDropdown = _microphoneDropdown;
            }
        }
    }
}
#endif