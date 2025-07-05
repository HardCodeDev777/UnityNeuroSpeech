# 🧠 Creating an Agent

---

An agent in UnityNeuroSpeech is a Unity object that can listen, talk, and respond using LLMs.  
**Once you create your first agent, you’ll be able to speak with your AI!**

---

## 🛠 How to Create an Agent

Go to **UnityNeuroSpeech → Create Agent**.  
You will see the window with these settings:


#### 👤 Agent Parameters

| Setting         | Description                                                                 |
|-----------------|-----------------------------------------------------------------------------|
| **Model name**  | Name of the LLM you downloaded via Ollama.                                 |
| **Agent name**  | Internal name for your agent. Avoid spaces or dashes.                      |
| **System prompt** | Base prompt used to control the AI’s behavior and tone.                  |

---

### ✅ Final Setup in Scene

1. Create a `Canvas` in your scene.
2. Add a `Dropdown` with at least three options and a `Button`.
3. Add an `AudioSource` to your scene.
4. Create an empty `GameObject` and attach the following scripts:
   - `WhisperManager`
   - `MicrophoneRecord`
   - `YourAgentNameController`
   - `SetupWhisperPath`
5. Configure the scripts:

#### 🔧 `WhisperManager`
- Leave `Model Path` empty.
- Turn off:
  - `Is Model Path In StreamingAssets`
  - `Init On Awake`
  - `Use VAD`
- Set `Language` to `auto`.

#### 🎙 `MicrophoneRecord`
- Turn off `Use VAD`
- Assign the `Dropdown` to `Microphone Dropdown`.

#### 🤖 `YourAgentNameController`
- `Agent Settings`: assign the `Agent_YourAgentName` ScriptableObject.
- `WhisperManager`, `MicrophoneRecord`: assign the same GameObject.
- Assign your `Button`, `Enable Mic Sprite`, `Disable Mic Sprite`, and `AudioSource`.

#### 📁 `SetupWhisperPath`
- Drag the GameObject into `Whisper`.
- For `Model Path`, use:  
  `UnityNeuroSpeech/Whisper/ggml-medium.bin`  
  (replace with your actual file name and folder if moved).

---

### 🔄 Start the TTS Server

Run the `run_server.bat` file.  
⚠️ It **must be in the same directory** as the `Server` folder (i.e., not inside it).

---

🎉 That’s it! When you run the game:

- Select a microphone in the dropdown  
- Click the button to start recording  
- Speak → click again  
- **AI responds with voice**

---

### ❕ Note

If you want to add **more than one agent** in one scene, after creating new empty `GameObject`, add only `YourAgentNameController` script.

---

### 🎧 Tip

The response delay depends on:

- LLM model size
- Whisper model speed
- TTS processing time

