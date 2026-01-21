using UnityEngine;

[CreateAssetMenu(fileName = "AgentSettings", menuName = "UnityNeuroSpeech/AgentSettings")]
public sealed class AgentSettings : ScriptableObject
{
    public int agentIndex;
    public string modelName, agentName;
    [TextArea(50, 50)] public string systemPrompt;
}