
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

int WINAPI WinMain(HINSTANCE hInstance, 
  HINSTANCE hPrevInstance, 
  LPWSTR lpCmdLine, 
  int nShowCmd ) {

	DWORD dwReturnValue = 0;
	return (SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, 0,
		(LPARAM) "Environment", SMTO_ABORTIFHUNG,
		5000, &dwReturnValue) > 0) ? 0 : 1;
}
