// UnitTestsCpp.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

__declspec(dllexport) extern int f_int_int(int i)
{
	return i + 1;
}
