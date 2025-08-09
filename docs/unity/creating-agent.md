# 🧠 Creating Agent

---

Agent in UnityNeuroSpeech is a GameObject that can listen, talk, and respond using LLMs.  
**Once you create your first agent, you’ll be able to speak with your AI!**

---

> Before creating agent make sure you have any two sprites. They're used for Microphone state(enabled/disabled).

---

## ✅ Scene Setup

1. Add a `Button` to your scene.
2. Add an `AudioSource` to your scene.

---

## ️⚙️ Opening Agent settings

Go to **UnityNeuroSpeech → Create Agent**.  
You will see the window with these settings:

### 👤 Agent Parameters

| Setting         | Description                                                              |
|-----------------|--------------------------------------------------------------------------|
| **Model name**  | Name of the LLM you downloaded via Ollama                               |
| **Agent name**  | Internal name for your agent. Avoid spaces or dashes                    |
| **System prompt** | Base prompt used to control the AI’s behavior and tone                  |

### 👤 Agent Component Values

| Setting                        | Description                    |
|--------------------------------|--------------------------------|
| **Microphone enable button**   | Button you created before      |
| **Enabled microphone sprite**  | Sprite for enabled microphone  |
| **Disabled microphone sprite** | Sprite for disabled microphone |
| **Response audiosource**       | AudioSource you created before |


📝 When you're done, click **Generate agent**.
After scripts compilation click **Create agent in scene**.

---

## 🔄 Start the TTS Server

Run the `run_server.bat` file.  
⚠️ It **must be in the same directory** as the `Server` folder (i.e., not inside it).

---

🎉 That’s it! When you run the game:

- Select a microphone in the dropdown  
- Click the button to start recording  
- Speak → click again  
- **AI responds with voice**

---

## 🎧 Tip

The response delay depends on:

- LLM model size
- Whisper model speed
- TTS processing time

