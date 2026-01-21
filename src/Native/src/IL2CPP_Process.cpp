#include "IL2CPP_Process.hpp"

EXPORT void RunProcessNative(const wchar_t* command)
{
    std::thread([command]()
    {
        STARTUPINFOW si;
        ZeroMemory(&si, sizeof(si));
        si.cb = sizeof(si);

        PROCESS_INFORMATION pi;
        ZeroMemory(&pi, sizeof(pi));

        BOOL success = CreateProcessW(
            NULL,
            (LPWSTR)command,
            NULL,
            NULL,
            FALSE,            
            CREATE_NO_WINDOW, 
            NULL,
            NULL,
            &si,
            &pi
        );

        if (!success) return;

        WaitForSingleObject(pi.hProcess, INFINITE);

        CloseHandle(pi.hProcess);
        CloseHandle(pi.hThread);

    }).detach(); 
}
