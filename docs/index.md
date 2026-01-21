# 🚀 Getting Started

---

## 🛠 Installing Requirements

UnityNeuroSpeech requires several things to be installed before using it in Unity. Here what you need to install:

---

### 1. Ollama

**Ollama** is a platform for running large language models (LLMs) locally. You can use models like DeepSeek, Gemma, Qwen, etc. 
Note that small models might affect accuracy and context understanding, but big models response can take a long time.

Install it from the [official website](https://ollama.com/download).

Then you need to download LLM model with this command:
```console
ollama pull modelname
```
> For quick test I recommend to download **qwen2.5:3b** - it responds very fast.

---

### 2. STT

**Whisper** — Speech-To-Text model that transcribes and translates audio with high accuracy.

You need to download Whisper model from [here](https://huggingface.co/ggerganov/whisper.cpp/tree/main).
> For quick test I recommend to download `ggml-base.bin`.

---

### 3. TTS

**UV** — a modern, ultra-fast Python package and environment manager. It replaces traditional tools like `pip`. 
**Coqui TTS**(runs XTTS) uses **UV** to simplify installation and allows running the TTS command directly, without manual Python setup.

**Coqui XTTS** — a Text-To-Speech model that can generate speech in any custom voice you want: Chester Bennington, Vito Corleone (The Godfather), Cyn (Murder Drones) or any other.

&nbsp;

Install **UV** with this command:

```console
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
```

Then install **Coqui TTS**:

```console
uv tool install --python 3.11 "coqui-tts==0.27.2"
```
> You can try to install latest **Coqui TTS** version, but I can't guarantee that it will work.

---

### 4. Unity deps

UnityNeuroSpeech uses **UniTask** for... I think everyone knows what **UniTask** is. 
You can install it from the [official repository](https://github.com/Cysharp/UniTask/releases).

Or via **UPM**:

```console
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

---

### 5. UnityNeuroSpeech

Now you can finally import **UnityNeuroSpeech** to your Unity project. 

Download `UnityNeuroSpeech.X.X.X.unitypackage` and `UNS_StreamingAssets.unitypackage` from the [official repository](https://github.com/HardCodeDev777/UnityNeuroSpeech/releases). Then you can import them in your project.

They are splitted to avoid importing almost 2GB XTTS model in your project if you only need code/fixes.

**Don't forget to put your downloaded Whisper model(`.bin`) in `Assets/StreamingAssets/UnityNeuroSpeech/Whisper/` - yes, it's important.**

---

## 🎙️ Voice Files

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

You’ll need two sprites for the microphone state (enabled/disabled).
But for quick test you can use random default sprites from Unity.