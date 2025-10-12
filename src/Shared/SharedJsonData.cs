using UnityNeuroSpeech.Utils;

namespace UnityNeuroSpeech.Shared
{
    /// <summary>
    /// Stores data for both Runtime and Editor
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
    }
}