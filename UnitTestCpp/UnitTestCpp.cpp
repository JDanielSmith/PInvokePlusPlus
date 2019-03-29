// UnitTestsCpp.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

#include <string>

extern "C"
{
	__declspec(dllexport) extern int f()
	{
		return 0;
	}
}

