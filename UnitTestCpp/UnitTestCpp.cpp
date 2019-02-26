// UnitTestsCpp.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

#include <string>

__declspec(dllexport) extern int f_int_int(int i)
{
	return i + 1;
}

extern "C"
{
	__declspec(dllexport) int f_wcslen_C(const wchar_t* s)
	{
		return wcslen(s);
	}
}
__declspec(dllexport) extern int f_wcslen(const wchar_t* s)
{
	return wcslen(s); // ?f_wcslen@@YAHPEB_W@Z
}

extern "C"
{
	__declspec(dllexport) int f_strlen_C(const char* s)
	{
		return strlen(s);
	}
}
__declspec(dllexport) int f_strlen(const char* s)
{
	return strlen(s); // ?f_strlen@@YAHPEBD@Z
}

struct __declspec(dllexport) C
{
	static int f_int_int(int i)
	{
		return i + 1;
	}
};

namespace my
{
	namespace ns
	{
		__declspec(dllexport) extern int f_int_int(int i)
		{
			return i + 10;
		}

		struct __declspec(dllexport) C
		{
			static int f_int_int(int i)
			{
				return i + 100;
			}
		};
	}
}

