using System;
using System.Collections.Generic;

namespace UnityNeuroSpeech.Runtime
{
    [Serializable]
    internal struct DialogData
    {
        public string userMessage, llmResponse;

        public DialogData(string userMessage, string llmResponse)
        {
            this.userMessage = userMessage;
            this.llmResponse = llmResponse;
        }
    }

    [Serializable]
    internal struct DialogHistoryData
    {
        public List<DialogData> dialogHistory;

        public DialogHistoryData(List<DialogData> dialogHistory) => this.dialogHistory = dialogHistory;
    }
}