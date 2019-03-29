#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

#include <string>

__declspec(dllexport) extern int64_t c_sum(const char* s)
{
	int64_t retval = 0;
	for (size_t i = 0; i < strlen(s); i++)
		retval += static_cast<unsigned char>(s[i]);
	return retval;
}

__declspec(dllexport) extern int64_t wc_sum(const wchar_t* s)
{
	int64_t retval = 0;
	for (size_t i = 0; i < wcslen(s); i++)
		retval += static_cast<int64_t>(s[i]);
	return retval;
}

