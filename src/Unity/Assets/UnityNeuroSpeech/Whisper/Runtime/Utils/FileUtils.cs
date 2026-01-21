using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Whisper.Utils
{
    public static class FileUtils
    {
        /// <summary>
        /// Read file on any platform.
        /// </summary>
        public static byte[] ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                LogUtils.Error($"Path {path} doesn't exist!");
                return null;
            }
            return File.ReadAllBytes(path);
        }
        
        /// <summary>
        /// Async read file on any platform.
        /// </summary>
        public static async Task<byte[]> ReadFileAsync(string path)
        {
            if (!File.Exists(path))
            {
                LogUtils.Error($"File: '{path}' doesn't exist!");
                return null;
            }
            return await ReadAllBytesAsync(path);
        }

        /// <summary>
        /// Read Android file using "web-request".
        /// </summary>
        public static byte[] ReadFileWebRequest(string path)
        {
            var request = UnityWebRequest.Get(path);
            request.SendWebRequest();
            
            while (!request.isDone) {}
            
            if (HasError(request))
            {
                Debug.LogError($"Error while opening weights at {path}!");
                if (!string.IsNullOrEmpty(request.error))
                    Debug.LogError(request.error);
                return null;
            }

            return request.downloadHandler.data;
        }
        
        /// <summary>
        /// Async read Android file using "web-request".
        /// </summary>
        public static async Task<byte[]> ReadFileWebRequestAsync(string path)
        {
            var request = UnityWebRequest.Get(path);
            request.SendWebRequest();

            while (!request.isDone)
                await Task.Yield();

            if (HasError(request))
            {
                Debug.LogError($"Error while opening weights at {path}!");
                if (!string.IsNullOrEmpty(request.error))
                    Debug.LogError(request.error);
                return null;
            }

            return request.downloadHandler.data;
        }

        // to suppress obsolete warning
        private static bool HasError(UnityWebRequest request)
        {
            return request.result != UnityWebRequest.Result.Success;
        }
        
        // to support .NET Standard 2.0
        private static async Task<byte[]> ReadAllBytesAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }
    }
}