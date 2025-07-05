# 🐍 Python Server

---


The Text-to-Speech model used in `UnityNeuroSpeech` is **Coqui XTTS**.
To balance performance and usability, the framework runs it through a small Python script that launches a local server.

> Yes — this means you must start the Python server before running game.

You can fully rewrite the TTS or server logic if you prefer. Check out `main.py`.