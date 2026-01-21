@echo off

IF EXIST build\ rmdir /s /q build
IF EXIST result\ rmdir /s /q result

mkdir build
mkdir result

cmake -S . -B build
cmake --build build --config Release

copy ".\build\Release\UnityNeuroSpeechNative.dll" ".\result\"

pause