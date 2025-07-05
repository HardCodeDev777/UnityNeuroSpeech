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
        public Action<AgentState> BeforeTTS { get; set; }
        public Action AfterTTS { get; set; }
    }

    /// <summary>
    /// Base class to define agent behavior
    /// </summary>
    // For now it only supports pre/post-TTS hooks,
    // since I don't see much use for anything else (yet).
    // But future expansion is possible.
    public abstract class AgentBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Use this to bind the behaviour script to an agent.
        /// Recommended: <see cref="AgentManager.SetBehaviourToAgent{T}(T, AgentBehaviour)"/>
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Called before sending input to the Text-To-Speech model
        /// </summary>
        /// <param name="state">Current agent state</param>
        public abstract void BeforeTTS(AgentState state);

        /// <summary>
        /// Called after receiving and playing the TTS response
        /// </summary>
        public abstract void AfterTTS();
    }

    /// <summary>
    /// Lightweight structure representing the agent's internal state
    /// </summary>
    public readonly struct AgentState
    {
        /// <summary>
        /// Total number of responses generated so far
        /// </summary>
        public readonly int responseCount;
        /// <summary>
        /// Agent's message (currently just the response from Ollama)
        /// </summary>
        public readonly string agentMessage;
        /// <summary>
        /// Emotion tag parsed from the response. It's a string like "happy", "sad", etc.
        /// </summary>
        public readonly string emotion;

        public AgentState(int responseCount, string agentMessage, string emotion)
        {
            this.responseCount = responseCount;
            this.agentMessage = agentMessage;
            this.emotion = emotion;
        }
    }
}