using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityNeuroSpeech.Utils
{
    internal enum LogLevel
    {
        None,
        Error,
        All
    }

    /// <summary>
    /// Class for logging
    /// </summary>
    internal static class LogUtils
    {
        public static LogLevel logLevel = LogLevel.All;

        public static void LogMessage(string msg)
        {
            if (logLevel == LogLevel.All) Debug.Log(msg);
        }

        public static void LogError(string msg)
        {
            if (logLevel == LogLevel.Error || logLevel == LogLevel.All) Debug.LogError(msg);
        }
    }
}
