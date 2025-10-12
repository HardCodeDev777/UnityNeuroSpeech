using System.Collections.Generic;

namespace UnityNeuroSpeech.Runtime
{
    [System.Serializable]
    internal struct DialogData
    {
        public string userMessage, llmResponse;

        public DialogData(string userMessage, string llmResponse)
        {
            this.userMessage = userMessage;
            this.llmResponse = llmResponse;
        }
    }

    /// <summary>
    /// Stores dialog history
    /// </summary>
    [System.Serializable]
    internal struct RuntimeJsonData
    {
        public List<DialogData> dialogHistory;

        public RuntimeJsonData(List<DialogData> dialogHistory) => this.dialogHistory = dialogHistory;
    }
}