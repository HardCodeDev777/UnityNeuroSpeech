# 🏗️ Buidling Game

---
You can’t just hit `Build` and expect UnityNeuroSpeech to work out of the box in your final game build.
But don’t worry — **it takes less than a minute to set it up properly.**
---

## 📁 Adding Whisper files in build folder

After building your game, go to the `Managed` folder.

There, create folders that match the `Model Path` value you set in the `SetupWhisperPath` script.

### 🗒️ Example 1

If you wrote:
`UnityNeuroSpeech/Whisper/ggml-medium.bin`
Then create a `UnityNeuroSpeech` folder → inside it, a `Whisper` folder → then put your model `.bin` file in there.

### 🗒️ Example 2

If you wrote:
`Imports/Frameworks/UnityNeuroSpeech/Whisper/ggml-medium.bin`
Then replicate that entire folder structure.

---

## 📁 Adding `Server` folder in build folder

Just drag the entire `Server` folder (from the UnityNeuroSpeech zip) into your game’s build folder.

Also, don’t forget to include the `run_server.bat` file.
You’ll need to run this `.bat` file **every time you launch the game**.

---

### 💡 Tip: Make a Startup Script

You can make a simple `.bat` file that launches both the TTS server and the game:


```batchfile
@echo off
start run_server.bat
start YouGameName.exe
```

--- 

✅ That’s it! Now your AI agents will talk even in the final build.

**Have fun! :)**