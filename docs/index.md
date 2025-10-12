# 🚀 Quick Start

---

## 🛠 Installing Requirements

---

UnityNeuroSpeech requires several programs to be installed.  
You can simply run `setup.bat` — it will download everything automatically.  
Then just import the `.unitypackage` into your project.

---

## 💡 What Are These Requirements?

---

- **Ollama** — a platform for running large language models (LLMs) locally. You can use models like **DeepSeek**, **Gemma**, **Qwen**, etc. Avoid small models — they might reduce accuracy and context understanding.
- **UV** — a modern, ultra-fast Python package and environment manager. It replaces traditional tools like `pip` and `venv`. **Coqui XTTS** uses **UV** to simplify installation and allows running the TTS command directly, without manual Python setup.
- **Coqui XTTS** — a Text-To-Speech model that can generate speech in any custom voice you want: Chester Bennington, Chris Tucker, Vito Corleone (*The Godfather*), Cyn (*Murder Drones*), or any other.
- **Whisper** — a Speech-To-Text model. You can use lightweight versions like `ggml-tiny.bin` for speed, or heavier ones like `ggml-medium.bin` for better accuracy.

---

## 🎙️ Voice Files

---

Don’t forget that you need voice files for AI speech.  
Make sure your files meet the following requirements:

- Format: `.wav`  
- Duration: 5–15 seconds (longer files work, but TTS will load them more slowly)  
- Contain only one voice and one language, without background noise

Since UnityNeuroSpeech supports multiple voices for multiple agents simultaneously, files must be named correctly:  
`<language>_voice<index>.wav`

**Examples:**

1. English voice, agent index `0` → `en_voice0.wav`  
2. Russian voice, agent index `3` → `ru_voice3.wav`

All voices must be placed in:  
`Assets/StreamingAssets/UnityNeuroSpeech/Voices/`

---

## 🖼️ Microphone Sprites

---

You’ll need two sprites for the microphone state (enabled/disabled).  
Yes — without them, it won’t work 🤠