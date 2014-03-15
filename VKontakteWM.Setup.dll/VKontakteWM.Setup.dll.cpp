// VKontakteWM.Setup.dll.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "ce_setup.h"
#include "process.h"

codeINSTALL_INIT Install_Init(
    HWND        hwndParent,
    BOOL        fFirstCall,     // is this the first time this function is being called?
    BOOL        fPreviouslyInstalled,
    LPCTSTR     pszInstallDir)
{
	// TODO: Add custom installation code here

	// To continue installation, return codeINSTALL_INIT_CONTINUE
	// If you want to cancel installation, 
	// return codeINSTALL_EXIT_UNINSTALL
	return codeINSTALL_INIT_CONTINUE;

}

codeINSTALL_EXIT Install_Exit(
    HWND    hwndParent,
    LPCTSTR pszInstallDir,
    WORD    cFailedDirs,
    WORD    cFailedFiles,
    WORD    cFailedRegKeys,
    WORD    cFailedRegVals,
    WORD    cFailedShortcuts)
{
	// TODO: Add custom installation code here

	// To exit the installation DLL normally, 
	// return codeINSTALL_EXIT_DONE
	// To unistall the application after the function exits,
	// return codeINSTALL_EXIT_UNINSTALL   Ошибка при установке на wm 6.5 с разноязычной прошивкой 

	TCHAR szShortcutPath[MAX_PATH];
	TCHAR szOutFile[MAX_PATH];
	TCHAR szOutFileLink[MAX_PATH];
	HKEY hKey, hSubKey;
	HRESULT hr = S_OK;

	SHGetSpecialFolderPath(0, szShortcutPath, CSIDL_PROGRAMS, false); 
	wsprintf(szOutFile,_T("\"%s\\%s\""), pszInstallDir, _T("VKontakteWM.exe"));
	wsprintf(szOutFileLink,_T("%s\\%s"), szShortcutPath, _T("ВКонтакте.lnk"));
	SHCreateShortcut(szOutFileLink, szOutFile);

	hr = RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("Security\\Shell\\StartInfo\\Start\\ВКонтакте.lnk"), 0, 0, &hKey);
	if(hr != ERROR_SUCCESS)
	{
		RegCloseKey(hKey);
		hr = RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("Security\\Shell\\StartInfo\\Start"), 0, 0, &hKey);
		hr = RegCreateKeyEx(hKey,
			TEXT("ВКонтакте.lnk"), 0, NULL, REG_OPTION_NON_VOLATILE, NULL, NULL, &hSubKey, NULL);
		RegCloseKey(hKey);
		hKey = hSubKey;
	}

	wsprintf(szOutFile,_T("%s\\%s"), pszInstallDir, _T("vkontakte.png"));
	hr = RegSetValueEx(hKey, TEXT("Icon"), 0, REG_SZ, (LPBYTE)szOutFile, 200);
	RegCloseKey(hKey);
      
	return codeINSTALL_EXIT_DONE;
}

codeUNINSTALL_INIT Uninstall_Init(
    HWND        hwndParent,
    LPCTSTR     pszInstallDir)
{
	// TODO: Add custom uninstallation code here

	// To continue uninstallation, return codeUNINSTALL_INIT_CONTINUE
	// If you want to cancel installation,
	// return codeUNINSTALL_INIT_CANCEL

	TCHAR szShortcutPath[MAX_PATH];
	TCHAR szOutFileLink[MAX_PATH];
	HKEY hKey;
	HRESULT hr = S_OK;

	KillProcess(_T("VKontakteWM.Notification.exe"));

	SHGetSpecialFolderPath(0, szShortcutPath, CSIDL_PROGRAMS, false); 
	wsprintf(szOutFileLink,_T("%s\\%s"), szShortcutPath, _T("ВКонтакте.lnk"));
	DeleteFile(szOutFileLink);

	hr = RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("Security\\Shell\\StartInfo\\Start"), 0, 0, &hKey);
	if(hr == ERROR_SUCCESS)
	{
		RegDeleteKey(hKey, TEXT("ВКонтакте.lnk"));
		RegCloseKey(hKey);
	}

	return codeUNINSTALL_INIT_CONTINUE;
}

codeUNINSTALL_EXIT Uninstall_Exit(
    HWND    hwndParent)
{
	// TODO: Add custom uninstallation code here

	return codeUNINSTALL_EXIT_DONE;

}