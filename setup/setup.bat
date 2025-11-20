:: Batch is amazing

@echo off
chcp 65001 > nul
title UnityNeuroSpeech Setup

goto menu

:menu
cls
echo.

echo                             [38;2;60;220;180m██╗   ██╗███╗   ██╗███████╗    ███████╗███████╗████████╗██╗   ██╗██████╗ [0m
echo                             [38;2;30;210;150m██║   ██║████╗  ██║██╔════╝    ██╔════╝██╔════╝╚══██╔══╝██║   ██║██╔══██╗[0m
echo                             [38;2;0;200;120m██║   ██║██╔██╗ ██║███████╗    ███████╗█████╗     ██║   ██║   ██║██████╔╝[0m 
echo                             [38;2;0;190;90m██║   ██║██║╚██╗██║╚════██║    ╚════██║██╔══╝     ██║   ██║   ██║██╔═══╝ [0m  
echo                             [38;2;0;170;70m╚██████╔╝██║ ╚████║███████║    ███████║███████╗   ██║   ╚██████╔╝██║     [0m  
echo                           [38;2;0;150;50m   ╚═════╝ ╚═╝  ╚═══╝╚══════╝    ╚══════╝╚══════╝   ╚═╝    ╚═════╝ ╚═╝     [0m  

echo.
echo [32m    ╔═Choose what you want to install: [0m
echo [32m    ║[0m
echo [32m    ╠══1) UV and Coqui-tts [0m
echo [32m    ║[0m
echo [32m    ╠══2) Ollama [0m
echo [32m    ║[0m
echo [32m    ╠══3) whisper.unity [0m
echo [32m    ║[0m
set /p typeOfInstallation=[32m    ╠══:[0m
echo.

if "%typeOfInstallation%"=="1" goto uv
if "%typeOfInstallation%"=="2" goto ollama
if "%typeOfInstallation%"=="3" goto whisper
goto menu

::--------------------- UV ---------------------
:uv
echo [32mInstalling UV...[0m
powershell -ExecutionPolicy ByPass -c "$ProgressPreference='SilentlyContinue'; irm https://astral.sh/uv/install.ps1 | iex" >> setup.log 2>&1
echo [32mUV installed![0m
echo.
echo [32mInstalling coqui-tts via UV...[0m
uv tool install --python 3.11 coqui-tts >> setup.log 2>&1
echo [32mCoqui-tts installed![0m
echo.

:uv_continue
set /p uvCnt=[32mType Y to continue:[0m
if /I "%uvCnt%"=="Y" goto menu
goto uv_continue

::--------------------- OLLAMA ---------------------
:ollama
echo [32mInstalling Ollama (it may take some time)...[0m
powershell -NoLogo -NoProfile -ExecutionPolicy Bypass -Command ^
  "$ProgressPreference='SilentlyContinue'; Invoke-WebRequest 'https://ollama.com/download/OllamaSetup.exe' -OutFile 'OllamaSetup.exe' -UseBasicParsing" >> setup.log 2>&1
start /wait "" "OllamaSetup.exe"

echo [32mOllama installed![0m
echo.
echo [32mYou can download LLM models using 'ollama pull modelname'[0m
echo [32mFind here - https://ollama.com/search[0m
echo.

:ollama_continue
set /p ollamaCnt=[32mType Y to continue:[0m
if /I "%ollamaCnt%"=="Y" goto menu
goto ollama_continue

::--------------------- WHISPER ---------------------
:whisper
echo [32mUnfortunately, there's no way to install package to Unity via CLI (by git link).[0m
echo [32mHere you can find git link and install it via Package Manager: https://github.com/Macoron/whisper.unity?tab=readme-ov-file#getting-started[0m
echo.
echo [32mAlso you need to download Whisper model from here: https://huggingface.co/ggerganov/whisper.cpp/tree/main[0m
echo.

:whisper_continue
set /p whisperCnt=[32mType Y to continue:[0m
if /I "%whisperCnt%"=="Y" goto menu
goto whisper_continue
