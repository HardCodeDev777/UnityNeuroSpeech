using UnityEngine;

namespace Whisper.Utils
{
    public enum LogLevel
    {
        Verbose,
        Log,
        Warning,
        Error,
    }

    public static class LogUtils
    {
        public static LogLevel Level = LogLevel.Verbose;
        
        public static void Error(string msg) => Debug.LogError($"[UnityNeuroSpeech] {msg}");
        
        public static void Warning(string msg)
        {
            if (Level > LogLevel.Warning) return;
            Debug.LogWarning($"[UnityNeuroSpeech] {msg}");
        }

        public static void Log(string msg)
        {
            if (Level > LogLevel.Log) return;
            Debug.Log($"[UnityNeuroSpeech] {msg}");
        }
        
        public static void Verbose(string msg)
        {
            if (Level > LogLevel.Verbose) return;
            Debug.Log($"[UnityNeuroSpeech] {msg}");
        }      
    }
}