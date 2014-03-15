#include <stdafx.h>
#include <windows.h>
#include <tlhelp32.h>
#pragma comment (lib,"toolhelp.lib")
#include <tchar.h>
#include <stdio.h>
 
#define TH32CS_SNAPNOHEAPS   0x40000000   // optimization for text shell to not snapshot heaps

DWORD FindProcess(TCHAR *szName) 
{ 
  HINSTANCE         hProcessSnap   = NULL; 
  PROCESSENTRY32   pe32           = {0}; 
  DWORD            dwTaskCount    = 0; 
  
  hProcessSnap = (HINSTANCE)CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS | TH32CS_SNAPNOHEAPS, 0); 
  if (hProcessSnap == (HANDLE)-1) return 0; 

  dwTaskCount = 0; 
  pe32.dwSize = sizeof(PROCESSENTRY32);   // must be filled out before use 
  if (Process32First(hProcessSnap, &pe32)) { 
    do { 
      if (_wcsicmp(pe32.szExeFile,szName)==0) 
     {
        CloseToolhelp32Snapshot(hProcessSnap);
        return pe32.th32ProcessID; 
     }
    } 
    while (Process32Next(hProcessSnap, &pe32)); 
  }
  CloseToolhelp32Snapshot(hProcessSnap);
  return 0; 
} 


void KillProcess(TCHAR *szName)
{
   DWORD dwPID = FindProcess(szName);
   HANDLE hProcess;
   
   if (dwPID)
   {
      hProcess = OpenProcess(PROCESS_ALL_ACCESS,false,dwPID);
      TerminateProcess(hProcess,0);
      CloseHandle(hProcess);
   }
}