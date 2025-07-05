using UnityNeuroSpeech.Utils;

namespace UnityNeuroSpeech.Shared
{

    // Sure, right now it only stores one logLevel field, so it might seem overkill(YAGNI moment.)
    // But when more settings come, this structure will be very useful
    [System.Serializable]
    internal struct JsonData
    {
        public LogLevel logLevel;
        public JsonData(LogLevel logLevel) => this.logLevel = logLevel;      
    }
}