# 📝 Agent API

---

Fully monitor your agents with a clean and lightweight API.

---

## 🕹️ Handle Agent State

The Agent API is simple and elegant — **just 2 classes, 1 struct and 6 methods**.

To use the `UnityNeuroSpeech Agent API`, you need to:

1. Create a new script.  
2. Add `using UnityNeuroSpeech.Runtime;` at the top.  
3. Derive your class from `AgentBehaviour`.

Once you do that, you must implement four abstract methods:

- `Awake`
- `BeforeTTS`
- `AfterTTS`
- `AfterSTT`

You’ll also need a reference to your `YourAgentNameController` type (in this example, `AlexController`).  
Your script should look like this:

```csharp
using UnityEngine;
using UnityNeuroSpeech.Runtime;

public class AlexBehaviour : AgentBehaviour
{
    [SerializeField] private AlexController _alex;

    public override void Awake() {}
    
    public override void AfterTTS() {}

    public override void BeforeTTS(AgentState state) {}
    
    public override void AfterSTT(string playerMessage) {}
}
```

---

### 🔍 Methods Overview

---

- **AfterTTS** — Called after the audio playback finishes.  
- **BeforeTTS** — Called before sending text to the TTS model.  
- **AfterSTT** — Called after the STT model finishes transcribing microphone input.  
- **Awake** — Works like `MonoBehaviour.Awake()`, but required.  
  Use it to link your behaviour to an agent:

```csharp
public override void Awake() 
{
    AgentManager.SetBehaviourToAgent(_alex, this);
    AgentManager.SetJsonDialogHistory(_alex, "AlexDialogHistory"); 
    // or, if you use encryption:
    AgentManager.SetJsonDialogHistory(_alex, "AlexDialogHistory", "yBIWJczdP7aSbSxB");
}
```

---

### 💡 What Is `SetBehaviourToAgent()`?

---

The `SetBehaviourToAgent()` method connects your `AgentBehaviour` to the agent’s internal event hooks:

```csharp
[HideInInspector] public Action<int, string, string, string> BeforeTTS { get; set; }
[HideInInspector] public Action AfterTTS { get; set; }
[HideInInspector] public Action AfterSTT { get; set; }
```

This ensures UnityNeuroSpeech calls your methods at the correct moments.

---

### 💡 What Is `SetJsonDialogHistory()`?

If you use `SetJsonDialogHistory()`, all dialog data between the player and the LLM will be stored in a `.json` file inside `StreamingAssets/`, one file per agent.  
The **second parameter** is the file name (without the `.json` extension).  
The optional **third parameter** is a 16-character AES encryption key.

If encryption is enabled (highly recommended), the player won’t be able to view dialog history in builds since it’s encrypted.  
**To decrypt it in the Editor, see the “Useful Tools” section in the docs.**

---

### 🟢 About AgentState struct:
- responseCount: Number of total replies by the agent.
- emotion: Emotion tag parsed from the LLM response (e.g. "happy", "sad").
- action: Action tag parsed from the LLM response (e.g. "open_door", "play_cutscene_123").
- agentMessage: Raw response from the LLM.
- userPrompt: Player's voice input.

---

✅ Don’t forget to attach your behaviour script to a `GameObject` in the scene.
