using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace JDanielSmith.Runtime.InteropServices
{
	public class VCEntrypointMangler : IEntrypointMangler
	{
		static readonly Dictionary<Type, string> typeToString_ = new Dictionary<Type, string>
		{
			// https://en.wikiversity.org/wiki/Visual_C%2B%2B_name_mangling
			{ typeof(SByte), "C" }, // int8_t
			// "D" // char
			{ typeof(Byte), "E" }, // uint8_t, unsigned char

			{ typeof(Int16), "F" },
			{ typeof(UInt16), "G" },

			{ typeof(Int32), "H" },
			{ typeof(UInt32), "I" },

			{ typeof(Int64), "_J" },
			{ typeof(UInt64), "_K" },

			{ typeof(float), "M" },
			{ typeof(double), "N" },

			{ typeof(Char), "_W" }, // wchar_t

			{ typeof(void), "X" },
		};
		static string typeToString(Type type, CharSet charSet)
		{
			if ((charSet == CharSet.Ansi) && (type == typeof(Char)))
			{
				return "D"; // char
			}

			return typeToString_[type];
		}

		string getStringForType(Type type, CharSet charSet)
		{
			string retval = "";
			if (type == typeof(String))
			{
				retval = "PEB"; // const __int64 pointer
				type = typeof(Char);
			}

			return retval + typeToString(type, charSet);
		}

		string getReturn(string functionName, ParameterInfo returnParameter, CharSet charSet)
		{
			// https://en.wikiversity.org/wiki/Visual_C%2B%2B_name_mangling
			// A - no CV modifier
			string ret = "A";
			string value = getStringForType(returnParameter.ParameterType, charSet);
			return ret + value;
		}

		string getParameter(string functionName, ParameterInfo parameter, CharSet charSet)
		{
			// Type modifier
			// A - reference
			// P - pointer

			// CV prefix
			// E __ptr64

			// CV modifier
			// A - none
			// B - const

			// "const wchar_t*" -> PEB_W


			return getStringForType(parameter.ParameterType, charSet);
		}

		public string Mangle(MethodInfo method, CharSet charSet = CharSet.Unicode)
		{
			// foo - name
			var name = method.Name;

			string access = "Y"; // "none" (not public/private/protected static/virtual/thunk)

			string parameters = String.Empty;
			foreach (var parameter in method.GetParameters())
			{
				parameters += getParameter(name, parameter, charSet);
			}
			if (!String.IsNullOrWhiteSpace(parameters))
				parameters += "@"; // end of parameter list
			else
				parameters = typeToString_[typeof(void)];

			var returnType = getReturn(name, method.ReturnParameter, charSet);

			// ? - decorated name
			// name@ - name fragment
			// @Z - end
			return "?" + name + "@@" + access + returnType + parameters + "Z";
		}
	}
}
