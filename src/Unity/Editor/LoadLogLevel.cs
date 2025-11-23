#if UNITY_EDITOR
using UnityNeuroSpeech.Utils;
using UnityEditor;
using UnityEngine;
using UnityNeuroSpeech.Shared;

namespace UnityNeuroSpeech.Editor
{
    /// <summary>
    /// Loads framework log settings in the Editor only (in runtime this is handled inside each agent's Awake method)
    /// </summary>
    internal static class LoadLogLevel
    {
        private static string _sharedDataText;

        [InitializeOnLoadMethod]
        private static void LoadLogSettings()
        {
            try
            {
                _sharedDataText = Resources.Load<TextAsset>("UnityNeuroSpeech/UnityNeuroSpeechSharedSettings").text;
            }
            // If error, then no settings file exists
            catch { return; }

            var sharedData = JsonUtility.FromJson<SharedJsonData>(_sharedDataText);
            if (!sharedData) 
            { 
                LogUtils.LogError($"No shared data was found in shared settings file!");
                return;
            }

            LogUtils.logLevel = sharedData.logLevel;   
        }
    }
}
#endif
