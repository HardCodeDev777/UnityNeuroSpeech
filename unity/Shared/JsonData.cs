using UnityNeuroSpeech.Utils;

namespace UnityNeuroSpeech.Shared
{

    [System.Serializable]
    internal struct JsonData
    {
        public LogLevel logLevel;
        public string ollamaURI;

        public JsonData(LogLevel logLevel, string ollamaURI) 
        {
            this.logLevel = logLevel;
            this.ollamaURI = ollamaURI;
        }  
    }
}