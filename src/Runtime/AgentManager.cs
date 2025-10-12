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
        public static void SetBehaviourToAgent<T>(T agent, AgentBehaviour beh) where T: MonoBehaviour, IAgent
        {
            agent.BeforeTTS += beh.BeforeTTS;
            agent.AfterTTS += beh.AfterTTS;
            agent.AfterSTT += beh.AfterSTT;
        }

        /// <summary>
        /// Binds json dialog history file name to agent. Works without encryption
        /// </summary>
        /// <typeparam name="T">Generated agent controller</typeparam>
        /// <param name="jsonFileName">Json dialog history file name</param>
        public static void SetJsonDialogHistory<T>(T agent, string jsonFileName) where T : MonoBehaviour, IAgent => agent.JsonDialogHistoryFileName = jsonFileName;

        /// <summary>
        /// Binds json dialog history file name to agent. Works with AES encryption
        /// </summary>
        /// <typeparam name="T">Generated agent controller</typeparam>
        /// <param name="jsonFileName">Json dialog history file name</param>
        /// <param name="encryptionKey">16 characters key</param>
        public static void SetJsonDialogHistory<T>(T agent, string jsonFileName, string encryptionKey) where T : MonoBehaviour, IAgent 
        { 
            agent.JsonDialogHistoryFileName = jsonFileName; 
            agent.EncryptionHistoryKey = encryptionKey;
        }
    }
}
