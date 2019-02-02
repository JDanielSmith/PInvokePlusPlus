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

		string typeToString(string functionName, Type type, CharSet charSet = CharSet.Unicode)
		{
			switch (charSet)
			{
				case CharSet.Unicode:
					{
						return typeToString_[type];
					}
				case CharSet.Ansi:
					{
						if (type == typeof(Char))
							return "D"; // char
						else
							return typeToString_[type];
					}
				case CharSet.Auto:
					{
						if (functionName.EndsWith("W", StringComparison.Ordinal))
							return typeToString(functionName, type, CharSet.Unicode);
						else if (functionName.EndsWith("A", StringComparison.Ordinal))
							return typeToString(functionName, type, CharSet.Ansi);

						goto default;
					}
				default:
					throw new InvalidOperationException("Unknown 'CharSet' value: " + charSet.ToString());
			}
		}

		string getReturn(string functionName, ParameterInfo returnParameter, CharSet charSet = CharSet.Unicode)
		{
			// https://en.wikiversity.org/wiki/Visual_C%2B%2B_name_mangling
			// A - no CV modifier
			string ret = "A";
			string value = typeToString(functionName, returnParameter.ParameterType, charSet);
			return ret + value;
		}

		string getParameter(string functionName, ParameterInfo parameter, CharSet charSet = CharSet.Unicode)
		{
			// Type modifier
			// A - reference
			// P - pointer

			// CV prefix
			// E __ptr64

			// CV modifier
			// A - none
			// B - const


			return typeToString(functionName, parameter.ParameterType, charSet);
		}

		public string Mangle(MethodInfo method)
		{
			// foo - name
			var name = method.Name;

			string access = "Y"; // "none" (not public/private/protected static/virtual/thunk)

			string parameters = String.Empty;
			foreach (var parameter in method.GetParameters())
			{
				parameters += getParameter(name, parameter);
			}
			if (!String.IsNullOrWhiteSpace(parameters))
				parameters += "@"; // end of parameter list
			else
				parameters = typeToString_[typeof(void)];

			var returnType = getReturn(name, method.ReturnParameter);

			// ? - decorated name
			// name@ - name fragment
			// @Z - end
			return "?" + name + "@@" + access + returnType + parameters + "Z";
		}
	}
}
