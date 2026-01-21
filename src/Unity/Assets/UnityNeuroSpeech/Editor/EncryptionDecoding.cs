// To check if encrypted data correct

#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityNeuroSpeech.Utils;
using Object = UnityEngine.Object;

namespace UnityNeuroSpeech.Editor
{
    internal sealed class EncryptionDecoding : EditorWindow
    {
        private Object _jsonToDecrypt;
        private string _key, _decryptedResult;

        [MenuItem("UnityNeuroSpeech/Tools/Encryption Decoding")]
        public static void ShowWindow() => GetWindow<EncryptionDecoding>("EncryptionDecoding");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Data to decrypt", EditorStyles.boldLabel);

            if (!_jsonToDecrypt) GUI.backgroundColor = Color.red;

            // TODO: Why DefaultAsset?
            _jsonToDecrypt = EditorGUILayout.ObjectField(
                new GUIContent("Json data", "Select json data you want to decrypt"), _jsonToDecrypt, typeof(DefaultAsset), false);

            GUI.backgroundColor = Color.white;

            if (string.IsNullOrEmpty(_key)) GUI.backgroundColor = Color.red;

            _key = EditorGUILayout.TextField(new GUIContent("Key to encrypt", "16 characters key you used in your AgentBehaviour script"), _key);

            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Result data", EditorStyles.boldLabel);
            _decryptedResult = EditorGUILayout.TextArea(_decryptedResult, GUILayout.Height(300));

            if (GUILayout.Button("Decrypt"))
            {
                if (string.IsNullOrEmpty(_key) || !_jsonToDecrypt)
                {
                    LogUtils.LogError("Fill all required fields!");
                    return;
                }

                var path = AssetDatabase.GetAssetPath(_jsonToDecrypt);
                var content = File.ReadAllText(path);  
                
                _decryptedResult = EncryptionUtils.Decrypt(content, _key);  
            }
        }
    }
}
#endif