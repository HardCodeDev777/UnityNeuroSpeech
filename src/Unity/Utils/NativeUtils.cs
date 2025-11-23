#if !ENABLE_MONO
// Unity doesn't provide System.Diagnostics.Process analog for IL2CPP, so I had to do it myself.

using System;
using System.Runtime.InteropServices;

namespace UnityNeuroSpeech.Utils
{
    internal static class NativeUtils
    {
        [DllImport("TTSProcessStartIL2CPP", CharSet=CharSet.Unicode)]
        private static extern IntPtr RunProcessAndReadLogsNative(string command);

        /// <summary>
        /// Runs TTS process in IL2CPP via native method
        /// </summary>
        /// <returns>Logs from exited Process</returns>
        public static string RunTTSProcessInI2CPP(string command)
        {
            var ptrRes = RunProcessAndReadLogsNative(command);
            var logs = Marshal.PtrToStringUni(ptrRes);
            Marshal.FreeCoTaskMem(ptrRes);
            return logs;
        }
    }
}
#endif