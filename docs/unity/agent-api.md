# 📝 Agent API

---

Easily trigger events based on your agent’s emotion, response count, or message using the `UnityNeuroSpeech Agent API`.

---

## 🕹️ Handle Agent State

The Agent API is simple and elegant — **just 4 methods and 2 classes**.

To use `UnityNeuroSpeech Agent API` you need to:
1. Create a new MonoBehaviour script.
2. Add `using UnityNeuroSpeech.Runtime;` on top of your code.
3. Derive from `AgentBehaviour` class.

Once you do that, you will need to implement three abstract methods:
  - Start
  - BeforeTTS
  - AfterTTS

Also, you need to create a field with your `YourAgentNameController` type (in this example, `AlexController`). Your code will look like this:

```csharp
using UnityEngine;
using UnityNeuroSpeech.Runtime;

public class AlexBehaviour : AgentBehaviour
{
    [SerializeField] private AlexController _alexAgentController;

    public override void AfterTTS() {}

    public override void BeforeTTS(AgentState state) {}

    public override void Start() {}
}
```

---

### 🔍 Methods Overview

- **AfterTTS** - Called after the audio is played.
- **BeforeTTS** - Called before sending text to the TTS model.
- **Start** - Same as MonoBehaviour’s Start(), but required. Use it to bind your behaviour to an agent:

```csharp
public override void Start() => AgentManager.SetBehaviourToAgent(_alexAgentController, this);
```

---

### 🟢 About AgentState struct:
- responseCount: Number of total replies by the agent.
- emotion: Emotion tag parsed from the LLM response (e.g. "happy", "sad").
- agentMessage: Raw response from the LLM.

---

### 💡 What is `SetBehaviourToAgent()`?

`SetBehaviourToAgent` function connects your AgentBehaviour to the agent's internal hooks:

```csharp
[HideInInspector] public Action<AgentState> BeforeTTS { get; set; }
[HideInInspector] public Action AfterTTS { get; set; }
```

This lets UnityNeuroSpeech know when to call your methods at the right moments.

---

### 🤔 `IAgent` interface

Also, you might notice `IAgent` interface. You don’t need to use this interface yourself, because it powers the core subscription logic.

---

### 🗒️ Example

```csharp
using UnityEngine;
using UnityNeuroSpeech.Runtime;

public class AlexBehaviour : AgentBehaviour
{
    [SerializeField] private AlexController _alexAgentController;

    public override void AfterTTS() {}

    public override void BeforeTTS(AgentState state)
    {
        if(state.responseCount == 5)
        {
            if (state.emotion == "happy") Debug.Log("AI is happy");
            else if (state.emotion == "sad") Debug.Log("AI is not happy...");
        }
    }

    public override void Start() => AgentManager.SetBehaviourToAgent(_alexAgentController, this);
}
```

**Don't forget to attach your behaviour script to any `GameObject` in your scene.**

---

### 👀 Full API Reference


`AgentUtils.cs`:

```csharp
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
```

`AgentManager.cs`:

```csharp
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
```

---

#### 😎 You now have full control over your agents!

Design smart behaviours, react to emotions, and go full sentient AI 🤖

UnityNeuroSpeech puts the power in your hands.