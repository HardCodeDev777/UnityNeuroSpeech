# ⚙️ Steps To Make It Work

---

This guide explains only the essential settings.  
You can find tooltips for each field directly in the Unity Editor.

---

## Step 1. 🧪 Settings

---

Go to **UnityNeuroSpeech → Create Settings** in the Unity toolbar.  
Default settings are recommended.  
Don’t forget to click the button (same applies for every step)!

---

## Step 2. 👀 UNS Manager

---

**UnityNeuroSpeech Manager** is a GameObject in your scene that controls all non-agent scripts.  
Without it, no agent (talkable AI) will work.

---

Create a `Dropdown` in your scene.  
Then go to **UnityNeuroSpeech → Create UNS Manager**.  
The important setting there is:

- **Whisper model path in StreamingAssets** — path to your downloaded Whisper model (`.bin`) inside the `StreamingAssets` folder (without the `Assets` directory).  
  Example:  
  If the full path is  
  `Assets/StreamingAssets/UnityNeuroSpeech/Whisper/ggml-medium.bin`  
  then you should enter  
  `UnityNeuroSpeech/Whisper/ggml-medium.bin`

---

## Step 3. 🧠 Agent

---

An **Agent** in UnityNeuroSpeech is a GameObject that can listen, respond, and talk using LLMs.  
**Once you create your first agent, you’ll be able to talk with your AI!**

---

Add a `Button` and an `AudioSource` to your scene.  
Then go to **UnityNeuroSpeech → Create Agent**.  
Here are some important settings:

- **Agent index** — the index mentioned in the Quick Start.  
  It links an agent to its voice file.  
  ⚠️ Each agent must have a unique index!

- **Emotions** — the AI can respond with *emotion tags*.  
  Example:  
  `– How are you, DeepSeek?`  
  `– <happy> I’m feeling grateful. What about you?`  
  The word inside `< >` is the emotion chosen by the AI.  
  Emotions are used for monitoring via the Agent API.  
  The system prompt (generated automatically by UNS) defines how emotions are used.

- **Actions** — optional behavior tags like  
  `"turn_off_lights"`, `"enable_cutscene_123"`, `"play_horror_sound"`, etc.

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
> Emotions and actions cannot be modified yet — stay tuned for updates 😁

---

Agent performance (“speed”) depends on:

- LLM model size  
- Whisper model size  
- Voice file length  
- AI response size  

Small models like **deepseek-r1:7b** or **ggml-tiny.bin** run fast but may ignore system prompts (emotions, actions, etc.).  
Large models like **ggml-large.bin** usually work perfectly — but will be slow as hell 😐

Choose models depending on your goals.  
Is it a problem? Maybe. But it only takes some time of testing to find the perfect setup and build something amazing with this tech.

> On first load, TTS may respond slowly — it’s normal. It will work faster next time.