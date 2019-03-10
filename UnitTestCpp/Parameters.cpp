#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

#include <string>

__declspec(dllexport) extern int ggg(int* pI)
{
	(*pI)++;
	return (*pI) + 1;
}
