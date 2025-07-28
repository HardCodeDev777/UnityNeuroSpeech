using UnityNeuroSpeech.Utils;

namespace UnityNeuroSpeech.Shared
{

    [System.Serializable]
    internal struct JsonData
    {
        public LogLevel logLevel;
        public string ollamaURI, TTSURI;
        public int requestTimeout;

        public JsonData(LogLevel logLevel, string ollamaURI, string TTSURI, int requestTimeout)
        {
            this.logLevel = logLevel;
            this.ollamaURI = ollamaURI;
            this.TTSURI = TTSURI;
            this.requestTimeout = requestTimeout;
        }
    }
}