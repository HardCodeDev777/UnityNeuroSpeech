// Handy utility script for keeping small agent-related features together instead of splitting them across many files

using System;
using UnityEngine;

namespace UnityNeuroSpeech.Runtime
{
    /// <summary>
    /// Interface used to identify agents and allow <see cref="AgentBehaviour"/> to subscribe to agent Actions
    /// </summary>
    public interface IAgent
    {
        public Action<int, string, string, string> BeforeTTS { get; set; }
        public Action AfterTTS { get; set; }
        public Action<string> AfterSTT { get; set; }
        public string JsonDialogHistoryFileName { get; set; }
        public string EncryptionHistoryKey { get; set; }
    }

    /// <summary>
    /// Base class to define agent behavior
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
        public abstract void BeforeTTS(int responseCount, string agentMessage, string emotion, string action);

        /// <summary>
        /// Called after receiving and playing the Text-To-Speech response
        /// </summary>
        public abstract void AfterTTS();

        /// <summary>
        /// Called after Speech-To-Text transcription
        /// </summary>
        public abstract void AfterSTT(string playerMessage);
    }
}