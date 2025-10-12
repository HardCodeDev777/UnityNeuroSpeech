# 🛠 Useful Editor Tools

---

UnityNeuroSpeech provides several Editor tools to make development more convenient.

---

## 🗒️ Prompts Test

---

Let’s say you want to check how a selected LLM model responds to a specific prompt.  
Normally, you would have to run the game, wait for Whisper to load, say something into the microphone (and risk transcription errors), then wait for the LLM and TTS to finish — quite the hardcore workflow, right?

This tool allows you to test prompts instantly.  
You only wait for the **LLM** (as usual) to generate a response — and you can even see the **generation time in milliseconds**!

---

To access it, go to **UnityNeuroSpeech → Prompts Test**.

---

## 🕵️‍♂️ Decode Encoded

---

If you use AES encryption, your `.json` dialog history files will be encrypted.  
But what if you want to view their contents?  
This tool lets you decrypt and read them easily.

---

To access it, go to **UnityNeuroSpeech → Decode Encoded**.

Note about the **Key to encrypt** field:  
You must use the same key you specified in your `AgentBehaviour` script.
