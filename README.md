![Unity](https://img.shields.io/badge/Unity-unity?logo=Unity&color=%23000000)
&nbsp;

![License](https://img.shields.io/github/license/HardCodeDev777/UnityNeuroSpeech?color=%2305991d)
![Last commit](https://img.shields.io/github/last-commit/HardCodeDev777/UnityNeuroSpeech?color=%2305991d)
![Tag](https://img.shields.io/github/v/tag/HardCodeDev777/UnityNeuroSpeech)
![Top lang](https://img.shields.io/github/languages/top/HardCodeDev777/UnityNeuroSpeech)
![quality](https://img.shields.io/badge/quality-best-brightgreen)
![code_style](https://img.shields.io/badge/code_style-goat-lightgrey)

<div align="center">
  <img src="docs/media/logo.png">
</div>

#

> **Make your Unity characters hear, think, and talk — using real voice AI. Locally. No cloud.**

---

UnityNeuroSpeech is an open-source framework for creating **fully voice-interactive AI agents** inside Unity.  
It connects:

- 🧠 **Whisper** (STT) – converts your speech into text  
- 💬 **Ollama** (LLM) – generates smart responses  
- 🗣️ **XTTS** (TTS) – speaks back with *custom voice + emotions*

All locally. All offline.  
No subscriptions, no accounts, no OpenAI API keys.

---

## 🚀 What can you build with UnityNeuroSpeech?

- 🎮 AI characters that understand your voice and reply in real time  
- 🗿 NPCs with personality and memory  
- 🧪 Experiments in AI conversation and narrative design  
- 🕹️ Voice-driven gameplay mechanics  
- 🤖 Interactive bots with humanlike voice responses

---

## ✨ Core Features

| Feature | Description                                                                                |
|--------|--------------------------------------------------------------------------------------------|
| 🎙️ **Voice Input** | Uses [whisper.unity](https://github.com/Macoron/whisper.unity) for accurate speech-to-text |
| 🧠 **AI Brain (LLM)** | Easily connect to any local model via [Ollama](https://ollama.com)                         |
| 🗣️ **Custom TTS** | Supports any voice with [Coqui XTTS](https://github.com/idiap/coqui-ai-TTS)                      |
| 😄 **Emotions** | Emotion tags (`<happy>`, `<sad>`, etc.) parsed automatically from LLM                      |
| 🎬 **Actions** | Action tags (`<turn_off_lights>`, `<play_cutscene_123>`, etc.) also parsed automatically from LLM                      |
| 🎛️ **Agent API** | Subscribe to events like `BeforeTTS()` to monitor your agents                  |
| 📝 **History Saving in JSON** | Save dialog history between player and LLM in JSON with/without AES encryption                 |
| 🛠️ **Editor Tools** | Create, manage and customize agents inside Unity Editor easily                                  |
| 🧱 **No Cloud** | All models and voice run locally on your machine                                           |
| 🌐 **Multilingual** | Works with over **15+ languages**, including English, Russian, Chinese, etc.               |
| 🔊 **Multiple Voices and Languages for Multiple Agents** | Each Agent can have each voice file for any available laguage              |
| ⚡ **High Performance** | Uses **UniTask** instead of Coroutines and Tasks for optimal performance.             |
| 🔧 **Full Build Support** | Full compatibility with both **Mono** and **IL2CPP** scripting backends.           |


---

## 🧪 Built with:

- 🧠 [OllamaSharp](https://github.com/awaescher/OllamaSharp) 
- 🎤 [whisper.unity](https://github.com/Macoron/whisper.unity)
- ⚡ [UniTask](https://github.com/Cysharp/UniTask)
- 🧊 [Coqui XTTS](https://github.com/idiap/coqui-ai-TTS)
- 🖥️ [UV](https://github.com/astral-sh/uv)
- 🤖 Unity 6

---

## ⚙️ Compatibility

| Scripting backend | Windows |  Other platforms    |
|-------------------|---------|--------------------|
| Mono              | ✅       | ❌ (not planned)    |
| IL2CPP            | ✅       | ❌ (not planned)    |

---

## 📚 Getting Started

See [UnityNeuroSpeech official documentation](https://hardcodedev777.github.io/UnityNeuroSpeech/).

---

## 😎 Who made this?

UnityNeuroSpeech was created by [HardCodeDev](https://github.com/HardCodeDev777) —  solo dev from Russia.

---

## 🗒️ License

UnityNeuroSpeech is licensed under the **MIT License**.
For other Licenses, see [Licenses](LICENSES.md).
