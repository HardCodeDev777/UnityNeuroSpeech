# 🚀 Quick Start

---

## ✅ Unity Requirements

1. **com.whisper.unity** must be installed. You can get it from the [official GitHub repository](https://github.com/Macoron/whisper.unity).
2. Add two sprites to your project (these are used for the "microphone enabled" and "microphone disabled" states).
3. UnityNeuroSpeech is developed for **Unity 6 (6000.0.47f1)**. Other versions might work, but Unity 6 is highly recommended.

---

## 🛠 Other Requirements

1. [Ollama](https://ollama.com) installed (along with any LLM you want to use).
2. [Python 3.11](https://www.python.org/downloads/release/python-31113/) installed.
3. A downloaded [Whisper model](https://huggingface.co/ggerganov/whisper.cpp/tree/main).
4. At least one `.wav` file for voice cloning.

### 💡 What are these tools?

- **Ollama** is a platform for running large language models (LLMs) locally. You can use models like DeepSeek, Gemma, Qwen, etc. Avoid small models—they might affect accuracy and context understanding.
- **Python 3.11** — use this specific version because it's stable and tested with this framework.
- **Whisper** is a speech-to-text model. You can use lightweight versions like `ggml-tiny.bin` for speed, or heavier ones like `ggml-medium.bin` for better accuracy.

---

## ⚙️ Base Setup

1. Go to the [UnityNeuroSpeech GitHub repository](https://github.com/HardCodeDev777/UnityNeuroSpeech) and download the following **three files** from the latest Release:

    - `UnityNeuroSpeech.X.X.X.zip` – main framework files
    - `TTSModel.zip` – pretrained XTTS model
    - `Setup.zip` - files for quick automatic setup

2. Create a new empty folder anywhere on your computer (name it however you like).
3. Unpack `Setup.zip`
3. Drag all the following into that folder:

    - Contents from `Setup.zip`
    - Two other `.zip` archives mentioned above

4. Run `RunPowershell.bat`

5. After setup finishes, you'll see a new folder `UnityNeuroSpeech X.X.X`. Open it and drag the `.unitypackage` into your Unity project. **Do not move or import the other files. Keep them outside the Unity project folder.**
6. Place your `.wav` voice files into `Server/Voices`.  
   Each file must follow the naming pattern: `en_voice.wav`, `ru_voice.wav`, etc.

7. In the `UnityNeuroSpeech` folder, you’ll see an empty `Whisper/` folder. Drop your Whisper `.bin` model file into it.


> Some folders (like `Whisper/`) may contain `.txt` placeholder files.  
> These are only used to ensure Unity exports the folder. You can safely delete them after setup.

---


✅ **Done! You’re ready to build your first talking AI agent.**
