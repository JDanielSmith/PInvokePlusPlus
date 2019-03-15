#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

#include <string>

__declspec(dllexport) extern int ggg(int* pI)
{
	(*pI)++;
	return (*pI) + 1;
}

__declspec(dllexport) extern int ggg(const int* pI)
{
	return (*pI) + 1;
}

__declspec(dllexport) extern int hhh(int& i)
{
	i += 10;
	return i + 100;
}