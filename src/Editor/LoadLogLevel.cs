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

            try
            {
                var sharedData = JsonUtility.FromJson<SharedJsonData>(_sharedDataText);
                LogUtils.logLevel = sharedData.logLevel;
            }
            catch (System.Exception ex)
            {
                LogUtils.LogError($"[UnityNeuroSpeech] Unexpected error happened while loading shared settings file! Full error message: {ex}");
            }
        }
    }
}
#endif
