#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

// f()s in namespace

namespace my
{
	__declspec(dllexport) extern int f()
	{
		return 1;
	}

	namespace ns
	{
		__declspec(dllexport) extern int f()
		{
			return 2;
		}
	}
}

namespace ns
{
	__declspec(dllexport) extern int f()
	{
		return 3;
	}
}