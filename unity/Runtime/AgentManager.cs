using UnityEngine;

namespace UnityNeuroSpeech.Runtime
{
    /// <summary>
    /// Centralized manager for agents
    /// </summary>
    public static class AgentManager
    {
        /// <summary>
        /// Binds a behaviour script to an agent
        /// </summary>
        /// <typeparam name="T">Generated agent controller</typeparam>
        /// <param name="agent">Agent instance</param>
        /// <param name="beh">Behaviour to attach</param>
        public static void SetBehaviourToAgent<T>(T agent, AgentBehaviour beh) where T: MonoBehaviour, IAgent
        {
            agent.AfterTTS += beh.AfterTTS;
            agent.BeforeTTS += beh.BeforeTTS;
        }
    }
}
