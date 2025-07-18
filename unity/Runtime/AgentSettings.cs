using UnityEngine;

namespace UnityNeuroSpeech.Runtime
{
    public class AgentSettings : ScriptableObject
    {
        public string modelName, agentName;
        [TextArea(50, 50)] public string systemPrompt;
    }
}
