// Handy script for keeping small agent-related features together instead of splitting them across many files

using System;
using UnityEngine;

namespace UnityNeuroSpeech.Runtime
{
    #region IAgent
    /// <summary>
    /// Interface used to identify agents and allow <see cref="AgentBehaviour"/> to subscribe to agent Actions
    /// </summary>
    public interface IAgent
    {
        public Action<AgentState> BeforeTTS { get; set; }
        public Action AfterTTS { get; set; }
        public Action<string> AfterSTT { get; set; }
        public string JsonDialogHistoryFileName { get; set; }
        public string EncryptionHistoryKey { get; set; }
    }
    #endregion

    public readonly struct AgentState
    {
        /// <summary>
        /// Total number of responses generated so far
        /// </summary>
        public readonly int responseCount;
        /// <summary>
        /// Response from LLM
        /// </summary>
        public readonly string agentMessage;
        /// <summary>
        /// Emotion tag parsed from the response. It's a string like "happy", "sad", etc.
        /// </summary>
        public readonly string emotion;
        /// <summary>
        /// Action tag parsed from the response. It's a string like "open_door", "turn_light", etc.
        /// </summary>
        public readonly string action;
        /// <summary>
        /// Prompt from user
        /// </summary>
        public readonly string userPrompt;

        public AgentState(int responseCount, string agentMessage, string userPrompt, string emotion, string action)
        {
            this.responseCount = responseCount;
            this.agentMessage = agentMessage;
            this.userPrompt = userPrompt;
            this.emotion = emotion;
            this.action = action;
        }
    }

    #region AgentBehaviour
    /// <summary>
    /// Base class to define agent monitoring
    /// </summary>
    public abstract class AgentBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Use this to bind some important things to agent: <see cref="AgentManager.SetBehaviourToAgent{T}(T, AgentBehaviour)"/>, <see cref="AgentManager.SetJsonDialogHistory{T}(T, string)"/> 
        /// </summary>
        public abstract void Awake();

        /// <summary>
        /// Called before sending input to the Text-To-Speech model
        /// </summary>
        public abstract void BeforeTTS(AgentState agentState);

        /// <summary>
        /// Called after receiving and playing the Text-To-Speech response
        /// </summary>
        public abstract void AfterTTS();

        /// <summary>
        /// Called after Speech-To-Text transcription
        /// </summary>
        public abstract void AfterSTT(string playerMessage);
    }
    #endregion

    #region AgentManager
    /// <summary>
    /// Centralized manager for agents
    /// </summary>
    public static class AgentManager
    {
        /// <summary>
        /// Binds a behaviour script to an agent
        /// </summary>
        /// <typeparam name="T">Generated agent controller</typeparam>
        public static void SetBehaviourToAgent<T>(T agent, AgentBehaviour beh) where T : MonoBehaviour, IAgent
        {
            agent.BeforeTTS += beh.BeforeTTS;
            agent.AfterTTS += beh.AfterTTS;
            agent.AfterSTT += beh.AfterSTT;
        }

        /// <summary>
        /// Binds json dialog history file name to agent. Works without any encryption
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
    #endregion

}