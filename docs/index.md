# 🚀 Quick Start

---

## 🛠 Installing Requirements

---

UnityNeuroSpeech requires several programs to be installed.  
You can simply run `setup.bat` — it will download everything automatically.  
Then just import the `.unitypackage` into your project.

---

## 🎙️ Voice Files

---

Don’t forget that you need voice files for TTS speech.  
Make sure your files meet the following requirements:

- Format: `.wav`  
- Duration: 5–15 seconds (longer files work, but TTS will load them more slowly)  
- Contain only one voice and one language, without background noise

Since UnityNeuroSpeech supports multiple voices for multiple agents, files must be named correctly:  
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