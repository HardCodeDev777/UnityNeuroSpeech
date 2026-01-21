# ⚙️ Steps To Make It Work

---

This guide explains only the essential settings.  
You can find tooltips for each field directly in the Unity Editor.

---

## Step 1. 🧪 Settings

Go to **UnityNeuroSpeech → Main → Create Settings** in the Unity toolbar.  
Default settings are recommended.

---

## Step 2. 👀 UNS Manager

**UnityNeuroSpeech Manager** is a GameObject in your scene that controls all non-agent scripts.  
Without it, no agent (talkable AI) will work.


Create a `Dropdown` in your scene.  
Then go to **UnityNeuroSpeech → Main → Create UNS Manager**.

---

## Step 3. 🧠 Agent

An **Agent** in UnityNeuroSpeech is a GameObject that can listen, respond, and talk using LLM.  
**Once you create your first agent, you’ll be able to talk with your AI!**

---

Add a `Button` and an `AudioSource` to your scene.  
Then go to **UnityNeuroSpeech → Main → Create Agent**.  
Here are some important settings:

- **Agent index** — the index mentioned in the Getting Started.  
  It links an agent to its voice file.  
  ⚠️ Each agent must have a unique index!

- **Emotions** — AI can respond with *emotion tags*.  
  Example:  
  `– How are you, DeepSeek?`  
  `– <happy> I’m feeling grateful. What about you?`  
  The word inside `< >` is the emotion chosen by AI.  
  Emotions are used for monitoring via the Agent API.  
  The system prompt (generated automatically by UNS) defines how emotions are used.

- **Actions** — optional behavior tags like  
  `"turn_off_lights"`, `"enable_cutscene_123"`, `"play_horror_sound"`, etc. Works as emotions

Click **Generate Agent**, then **Create Agent In Scene** — *only in that order!*

---

🎉 That’s it! When you run the game:

1. Select a microphone in the dropdown  
2. Click the button to start recording  
3. Speak → click again  
4. **AI responds with voice**

---

After you click **Generate Agent**, two files will be created:  
- `AgentNameController.cs` — your agent controller (you don’t need to modify it)  
- `AgentNameSettings.asset` — ScriptableObject with agent settings (system prompt, model name, index, etc.)  

You can edit the settings as you wish.

---

Agent performance (“speed”) depends on:

- LLM model size  
- Whisper model size  
- Voice files length  
- AI response size  

Small models like **qwen2.5:3b** or **ggml-base.bin** run fast, but may ignore system prompts (emotions, actions, etc.).  
Large models like **ggml-large.bin** usually work perfectly — but will be very slow 😐

> On first load, TTS may respond slowly — it’s ok. It'll work faster next time.