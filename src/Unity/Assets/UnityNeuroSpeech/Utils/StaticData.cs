using UnityEngine;
using System.IO;

namespace UnityNeuroSpeech.Utils
{
    internal static class StaticData
    {
        #region STT
        public static string WHISPER_MODELS_LOCATION_FULL_PATH => Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Whisper");

        #endregion

        #region TTS
        public static string BASE_TTS_PATH => Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "TTSModel");
        public static string CONFIG_TTS_PATH => Path.Combine(BASE_TTS_PATH, "config.json");
        public static string BASE_VOICE_PATH => Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "Voices");
        public static string GENERATED_VOICES_PATH => Path.Combine(BASE_VOICE_PATH, "Generated");
        #endregion

        #region Agent
        public static string AGENT_HISTORY_PATH => Path.Combine(Application.streamingAssetsPath, "UnityNeuroSpeech", "History");
        public static string GENERATED_AGENTS_PATH => Path.Combine(Application.dataPath, "UnityNeuroSpeech", "Runtime", "GeneratedAgents");
        public static string GENERATED_AGENTS_PATH_IN_ASSETS => Path.Combine("Assets", "UnityNeuroSpeech", "Runtime", "GeneratedAgents");
        public static string BASE_AGENT_CONT_PATH_IN_ASSETS => Path.Combine("Assets", "UnityNeuroSpeech", "Editor", "BaseAgentController.cs");
        #endregion

        #region Framework settings
        public static string FRAMEWORK_SETTINGS_FILE_PATH_IN_RESOURCES => Path.Combine("UnityNeuroSpeech", "UnityNeuroSpeechSharedSettings");
        public static string FRAMEWORK_SETTINGS_DIR_PATH => Path.Combine(Application.dataPath, "Resources", "UnityNeuroSpeech");
        public static string FRAMEWORK_SETTINGS_FILE_FULL_PATH => Path.Combine(FRAMEWORK_SETTINGS_DIR_PATH, "UnityNeuroSpeechSharedSettings.json");
        #endregion
    }
}