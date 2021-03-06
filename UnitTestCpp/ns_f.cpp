#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

// f()s in namespace

namespace my
{
	__declspec(dllexport) extern int f()
	{
		return 100;
	}

	namespace ns
	{
		__declspec(dllexport) extern int f()
		{
			return 101;
		}
	}
}

namespace ns
{
	__declspec(dllexport) extern int f()
	{
		return 102;
	}
}