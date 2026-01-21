#pragma once

#include <Windows.h>
#include <thread>

#define EXPORT extern "C" __declspec(dllexport)

EXPORT void RunProcessNative(const wchar_t* command);