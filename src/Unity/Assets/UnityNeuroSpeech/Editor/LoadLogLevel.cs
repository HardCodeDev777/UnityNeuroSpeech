#if UNITY_EDITOR
using UnityNeuroSpeech.Utils;
using UnityEditor;
using UnityEngine;
using UnityNeuroSpeech.Shared;
using System.IO;

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
            if (File.Exists(StaticData.FRAMEWORK_SETTINGS_FILE_FULL_PATH))
            {
                _sharedDataText = Resources.Load<TextAsset>(StaticData.FRAMEWORK_SETTINGS_FILE_PATH_IN_RESOURCES).text;

                var settings = JsonUtility.FromJson<FrameworkSettings>(_sharedDataText);
                if (!settings)
                {
                    LogUtils.LogError($"No framework settings was found in framework settings file!");
                    return;
                }

                LogUtils.logLevel = settings.logLevel;
            }
        }
    }
}
#endif
