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
echo [32m    ╠══1) UV and Coqui TTS [0m
echo [32m    ║[0m
echo [32m    ╠══2) Ollama [0m
echo [32m    ║[0m
set /p typeOfInstallation=[32m    ╠══:[0m
echo.

if "%typeOfInstallation%"=="1" goto uv_and_coqui_tts
if "%typeOfInstallation%"=="2" goto ollama_ask
goto menu

@REM--------------------- UV and Coqui TTS---------------------
:uv_and_coqui_tts
echo [32mInstalling UV...[0m
powershell -ExecutionPolicy ByPass -c "$ProgressPreference='SilentlyContinue'; irm https://astral.sh/uv/install.ps1 | iex" >> setup.log 2>&1

where uv >> setup.log 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Failed to install UV! You can open 'setup.log' to see errors
    pause
    exit
)

echo [32mUV installed![0m
echo.

echo [32mInstalling Coqui-TTS via UV...[0m
uv tool install --python 3.11 "coqui-tts==0.27.2" >> setup.log 2>&1

where tts >> setup.log 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Failed to install Coqui TTS! You can open 'setup.log' to see errors
    pause
    exit
)

echo [32mCoqui TTS installed![0m
echo.

:uv_continue
set /p uvCnt=[32mType Y to continue:[0m
if /I "%uvCnt%"=="Y" goto menu
goto uv_continue

@REM--------------------- Ollama ---------------------
:ollama_ask

echo [32mAre you sure you want to download Ollama via this tool? It'll download large(1GB+) setup file.[0m
echo [32mRecommended way is to download it manually from https://ollama.com/download[0m

set /p uvCnt=[32mType Y if you want to continue:[0m
if /I "%uvCnt%"=="Y" goto ollama
goto menu

:ollama
echo [32mInstalling Ollama (it may take some time)...[0m
powershell -NoLogo -NoProfile -ExecutionPolicy Bypass -Command ^
  "$ProgressPreference='SilentlyContinue'; Invoke-WebRequest 'https://ollama.com/download/OllamaSetup.exe' -OutFile 'OllamaSetup.exe' -UseBasicParsing" >> setup.log 2>&1

start /wait "" "OllamaSetup.exe"

where ollama >> setup.log 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Ollama is not installed! You can open 'setup.log' to see errors
    pause
    exit
)

echo [32mOllama installed![0m
echo.

echo [32mYou can download LLM models using 'ollama pull modelname'[0m
echo [32mYou can find models here - https://ollama.com/search[0m
echo.

:ollama_continue
set /p ollamaCnt=[32mType Y to continue:[0m
if /I "%ollamaCnt%"=="Y" goto menu
goto ollama_continue