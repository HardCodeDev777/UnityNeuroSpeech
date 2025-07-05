using System;
using System.Threading.Tasks;

namespace UnityNeuroSpeech.Utils
{
    /// <summary>
    /// Super handy class to avoid wrapping everything in try-catch manually
    /// </summary>
    internal static class SafeExecutionUtils
    {
        /// <summary>
        /// Safe wrapper for functions that return a result and take one parameter
        /// </summary>
        public static TResult SafeExecute<T1, TResult>(string methodName, Func<T1, TResult> func, T1 param1)
        {
            try
            {
                var result = func(param1);
                LogUtils.LogMessage($"[UnityNeuroSpeech] {methodName} completed successfully!");
                return result;
            }
            catch (Exception ex)
            {
                LogUtils.LogError($"[UnityNeuroSpeech] Unexpected error in {methodName}! Full error message: {ex}");
                return default;
            }
        }

        /// <summary>
        /// Safe wrapper for async Task-returning functions with one parameter
        /// </summary>
        public static async Task SafeExecute<T1>(string methodName, Func<T1, Task> func, T1 param1)
        {
            try
            {
                await func(param1);
                LogUtils.LogMessage($"[UnityNeuroSpeech] {methodName} completed successfully!");
            }
            catch (Exception ex)
            {
                LogUtils.LogError($"[UnityNeuroSpeech] Unexpected error in {methodName}! Full error message: {ex}");
            }
        }

        /// <summary>
        /// Safe wrapper for void-returning functions with two parameters
        /// </summary>
        public static void SafeExecute<T1, T2>(string methodName, Action<T1, T2> func, T1 param1, T2 param2)
        {
            try
            {
                func.Invoke(param1, param2);
                LogUtils.LogMessage($"[UnityNeuroSpeech] {methodName} completed successfully!");
            }
            catch (Exception ex)
            {
                LogUtils.LogError($"[UnityNeuroSpeech] Unexpected error in {methodName}! Full error message: {ex}");
            }
        }

        /// <summary>
        /// Safe wrapper for parameterless void functions
        /// </summary>
        public static void SafeExecute(string methodName, Action func)
        {
            try
            {
                func.Invoke();
                LogUtils.LogMessage($"[UnityNeuroSpeech]  {methodName} completed successfully!");
            }
            catch (Exception ex)
            {
                LogUtils.LogError($"[UnityNeuroSpeech] Unexpected error in {methodName}! Full error message: {ex}");
            }
        }
    }
}