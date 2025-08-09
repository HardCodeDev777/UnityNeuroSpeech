# ⚙️ Configure Settings

---

To use UnityNeuroSpeech, you will need to configure some initial settings. Don’t worry — it only takes a few minutes.

---

## 🛠 Opening the Settings

Go to **UnityNeuroSpeech → Create Settings** in the Unity toolbar.  
You’ll see the window with these settings:


### 🔥 General

| Setting                  | Description                                                                                  |
|--------------------------|----------------------------------------------------------------------------------------------|
| **Logging type**         | Controls how much info you want to see in the Unity console                                  |
| **Not in Assets folder** | Check this if you moved the framework folder outside the default location                    |
| **Directory name**       | For example: if your folder path is `Assets/MyImports/Frameworks`, enter `MyImports/Frameworks` |

### 🤖 Agents

| Setting                   | Description                                                                                     |
|---------------------------|-------------------------------------------------------------------------------------------------|
| **Emotions**              | Add at least one emotion. These are passed to the LLM                                           |
| **Request timeout(secs)** | Timeout for requests to local TTS Python server                                                 |

### 🐍 Python

| Setting                      | Description                                |
|------------------------------|--------------------------------------------|
| **Enable Python debug**      | Enables debug for local TTS Python server  |
| **Absolute path to main.py** | Full path to `main.py` in `Server/` folder |

### 😎 Advanced

| Setting               | Description                                            |
|-----------------------|--------------------------------------------------------|
| **Custom Ollama URI** | If empty, Ollama URI will be default "localhost:11434" |
| **Custom TTS URI**    | If empty, TTS URI will be default "localhost:7777"     |


📝 When you're done, click **Save**.  
Now you can work with UnityNeuroSpeech and start setting up your scene!
