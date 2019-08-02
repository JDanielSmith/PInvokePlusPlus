#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

#include <string>

// a whole bunch of f() overloads

__declspec(dllexport) extern int f()
{
	return 1;
}

__declspec(dllexport) extern int f(int i)
{
	return i + 1;
}

__declspec(dllexport) extern int f(const int& i)
{
	return i + 2;
}

__declspec(dllexport) extern int f(const int* pI)
{
	return (*pI) + 3;
}

__declspec(dllexport) extern int f(int& i)
{
	i++;
	return i + 4;
}

__declspec(dllexport) extern int f(int* pI)
{
	(*pI)++;
	return (*pI) + 5;
}

__declspec(dllexport) extern size_t f(const char* s)
{
	return strlen(s);
}

__declspec(dllexport) extern size_t f(const wchar_t* s)
{
	return wcslen(s); // ?f_wcslen@@YAHPEB_W@Z
}

//__declspec(dllexport) extern size_t f(const std::string& s)
//{
//	return s.size();
//}

struct S; // forward
__declspec(dllexport) extern int abc(const S*)
{
	return 6;
}

//namespace ns
//{
//	struct S; // forward
//}
//__declspec(dllexport) extern int xyz(const ns::S*)
//{
//	return 7;
//}