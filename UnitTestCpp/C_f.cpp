#include "pch.h"
#include "framework.h"
#include "UnitTestCpp.h"

#include <string>

struct __declspec(dllexport) C final
{
	static int f() {
		return 1000;
	}

	int f(int i) {
		return i + 1001;
	}
	int f(int i) const {
		return i + 1002;
	}
};

namespace ns
{
	struct __declspec(dllexport) S final
	{
		static int f() {
			return 1003;
		}

		int f(int i) {
			return i + 1004;
		}
		int f(int i) const {
			return i + 1005;
		}
	};
}