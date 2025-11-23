// WinApi sucks

#pragma warning (disable: 6387)

#include "pch.h"
#include <Windows.h>
#include <iostream>
#include <objbase.h>

#define EXPORT extern "C" __declspec(dllexport)

EXPORT wchar_t* RunProcessAndReadLogsNative(const wchar_t* command)
{
    SECURITY_ATTRIBUTES sa;
    sa.nLength = sizeof(SECURITY_ATTRIBUTES);
    sa.bInheritHandle = TRUE;
    sa.lpSecurityDescriptor = NULL;

    HANDLE readPipe, writePipe;
    if (!CreatePipe(&readPipe, &writePipe, &sa, 0))
        return nullptr;

    STARTUPINFOW si;
    ZeroMemory(&si, sizeof(si));
    si.cb = sizeof(si);
    si.hStdOutput = writePipe;
    si.hStdError = writePipe;
    si.dwFlags |= STARTF_USESTDHANDLES;

    PROCESS_INFORMATION pi;
    ZeroMemory(&pi, sizeof(pi));

    BOOL success = CreateProcessW(
        NULL,
        (LPWSTR)command,
        NULL,
        NULL,
        TRUE,
        CREATE_NO_WINDOW,
        NULL,
        NULL,
        &si,
        &pi
    );

    CloseHandle(writePipe);

    if (!success)
        return nullptr;

    char buffer[1024];
    DWORD bytesRead;
    std::string result;

    while (ReadFile(readPipe, buffer, sizeof(buffer) - 1, &bytesRead, NULL) && bytesRead > 0)
    {
        buffer[bytesRead] = 0;
        result += buffer;
    }

    CloseHandle(readPipe);

    WaitForSingleObject(pi.hProcess, INFINITE);
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    int size_needed = MultiByteToWideChar(CP_UTF8, 0, result.c_str(), (int)result.size(), NULL, 0);
    std::wstring wresult(size_needed, 0);
    MultiByteToWideChar(CP_UTF8, 0, result.c_str(), (int)result.size(), &wresult[0], size_needed);

    size_t size = (wresult.size() + 1) * sizeof(wchar_t);
    wchar_t* output = (wchar_t*)CoTaskMemAlloc(size);
    memcpy(output, wresult.c_str(), size);

    return output;
}