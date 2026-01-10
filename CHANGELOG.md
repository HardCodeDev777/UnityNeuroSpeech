> [!IMPORTANT]
> Only the latest version is officially supported.


# v2.1.0 [23.11.2025]

## Unity

- **Added full support for IL2CPP**, by writing native C++ plugin.
- Replaced all Coroutines and Tasks with **UniTask**.
- Ollama, TTS and Json logics from one large `BaseAgentController.cs` was splitted into their own modules. Now `BaseAgentController.cs` is much cleaner.
- Whisper now **fully** integrated in UnityNeuroSpeech package. That means that now you don't need to install it manually and that I can easily modify it. Already was deleted a lot of unnecessary code.
- Added `AgentState` struct with more fields.
- Removed `SafeExecutionUtils.cs`, `ReflectionUtils.cs`, `AgentManager.cs`.
- `AgentManager` was moved to `AgentUtils.cs`.
- Improved code comments, logs, and internal validation.

---

## Setup

- Added dependencies validation.

## Post-release fixes (framework wasn't changed)

- Fixed error with latest **Coqui TTS** broken version `0.27.3` in Setup. Now it's pinned to latest working `0.27.2` version

---

&nbsp;

# v2.0.0 [12.10.2025]

## ~TTS~

- **No more Python! No more local servers! No more pain!** UnityNeuroSpeech now uses the amazing and actively maintained [Coqui XTTS fork by Idiap](https://github.com/idiap/coqui-ai-TTS) — and everything runs directly through the CLI. Yes, you’ll need to install [UV](https://github.com/astral-sh/uv), but it’s so much better than before.

## Unity

- Added full support for **multiple voices, languages, and agents**
- Added **dialog history saving** (with or without AES encryption) between player and LLM
- Whisper models now work from `StreamingAssets/`, meaning **full Mono support**
- Removed `AgentState` struct 
- Added `SetJsonDialogHistory` method
- Added two new powerful Editor tools: **Prompts Test** and **Decode Encoded**
- Improved code comments, logs, and internal validation

## Docs

- Simplified and cleaned up documentation — removed duplicate info already shown in Unity tooltips

## Setup

- One amazing `setup.bat` file handles everything — it installs all required dependencies automatically 😎 Just a `.unitypackage` and one script to run

## Post-release fixes (framework wasn't changed)

- Fixed error with `llvmlite` while installing **Coqui TTS** in Setup

---

&nbsp;

# Older versions [06.07.2025 - 11.10.2025]

Older releases were removed as they were based on an early experimental architecture
and a deprecated Python-based TTS setup (local Flask server).

Starting from **v2.0.0**, UnityNeuroSpeech was significantly reworked and can be considered
the first stable and production-ready release of the framework.
