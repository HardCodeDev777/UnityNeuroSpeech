@echo off
cd /d %~dp0\Server

echo *************************************************
echo UnityNeuroSpeech Text-To-Speech server is running!
echo UnityNeuroSpeech official GitHub repository: https://github.com/HardCodeDev777/UnityNeuroSpeech
echo *************************************************

call .venv\Scripts\activate
python main.py

pause