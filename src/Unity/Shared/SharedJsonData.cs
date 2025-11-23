using UnityNeuroSpeech.Utils;

namespace UnityNeuroSpeech.Shared
{
    /// <summary>
    /// Stores framework settings
    /// </summary>
    [System.Serializable]
    internal struct SharedJsonData
    {
        public LogLevel logLevel;
        public string ollamaURI, anotherFolderName;

        public SharedJsonData(LogLevel logLevel, string ollamaURI, string anotherFolderName)
        {
            this.logLevel = logLevel;
            this.ollamaURI = ollamaURI;
            this.anotherFolderName = anotherFolderName;
        }

        public static implicit operator bool(SharedJsonData? data)
        {
            return data.HasValue && !string.IsNullOrEmpty(data.Value.ollamaURI);
        }
    }
}