#if !ENABLE_MONO

using System.Runtime.InteropServices;

namespace UnityNeuroSpeech.Utils
{
    internal static class NativeUtils
    {
        private const string DLL_NAME = "UnityNeuroSpeechNative";

        [DllImport(DLL_NAME)]
        public static extern void RunProcessNative([MarshalAs(UnmanagedType.LPWStr)] string command);
    }
}
#endif