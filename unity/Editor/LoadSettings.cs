#if UNITY_EDITOR
using UnityNeuroSpeech.Utils;
using UnityEditor;
using UnityEngine;
using UnityNeuroSpeech.Shared;

namespace UnityNeuroSpeech.Editor
{
    /// <summary>
    /// Loads framework settings in the Editor only
    /// (in runtime this is handled inside each agent's Awake method)
    /// </summary>
    internal static class LoadSettings
    {
        [InitializeOnLoadMethod]
        private static void LoadFrameworkSettings()
        {
            string dataText;
            // Attempts to access the JSON settings file
            try
            {
                dataText = Resources.Load<TextAsset>("Settings/UnityNeuroSpeechSettings").text;   
            }
            catch{
                LogUtils.LogMessage("[UnityNeuroSpeech] No settings file was found.");
                return;
            }

            // If the file exists, apply the configuration
            try
            {
                var data = JsonUtility.FromJson<JsonData>(dataText);
                LogUtils.logLevel = data.logLevel;
            }
            catch (System.Exception ex)
            {
                LogUtils.LogError($"[UnityNeuroSpeech] Unexpected error happened while loading settings file! Full error message: {ex}");
            }
        }

    }
}
#endif