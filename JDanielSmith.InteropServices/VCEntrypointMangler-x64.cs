using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace JDanielSmith.Runtime.InteropServices
{
	public class VCEntrypointMangler : IEntrypointMangler
	{
		internal class TypeToString
		{
			readonly Dictionary<Type, string> typeToString_ = new Dictionary<Type, string>
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
			private string AsString(Type type, CharSet charSet)
			{
				if ((charSet == CharSet.Ansi) && (type == typeof(Char)))
				{
					return "D"; // char
				}

				return typeToString_[type];
			}

			readonly Dictionary<string, string> typeFullNameToString_ = new Dictionary<string, string>
			{
				// https://en.wikiversity.org/wiki/Visual_C%2B%2B_name_mangling
				{ typeof(SByte).FullName, "C" }, // int8_t
				// "D" // char
				{ typeof(Byte).FullName, "E" }, // uint8_t, unsigned char

				{ typeof(Int16).FullName, "F" },
				{ typeof(UInt16).FullName, "G" },

				{ typeof(Int32).FullName, "H" },
				{ typeof(UInt32).FullName, "I" },

				{ typeof(Int64).FullName, "_J" },
				{ typeof(UInt64).FullName, "_K" },

				{ typeof(float).FullName, "M" },
				{ typeof(double).FullName, "N" },

				{ typeof(Char).FullName, "_W" }, // wchar_t

				{ typeof(void).FullName, "X" },
			};
			private string AsString(string typeFullName, CharSet charSet)
			{
				if ((charSet == CharSet.Ansi) && (typeFullName == typeof(Char).FullName))
				{
					return "D"; // char
				}

				if (!typeFullNameToString_.ContainsKey(typeFullName))
				{
					return String.Empty;
				}
				return typeFullNameToString_[typeFullName];
			}

			static string RemoveRef(Type type)
			{
				if (!type.IsByRef)
				{
					return type.FullName;
				}

				string fullName = type.FullName;
				var fullNameNoRef = fullName.Substring(0, fullName.Length - 1); // remove trailing "&"
				return fullNameNoRef;
			}

			private string AsString(Type type, CharSet charSet, bool isPtr, bool isConst)
			{
				string retval = "";
				if (type == typeof(String)) // -> "const wchar_t*"
				{
					retval = "PEB"; // const __int64 pointer
					type = typeof(Char);
				}

				string modifier = "";
				if (type.IsByRef)
				{
					// Type modifier
					// A - reference
					// P - pointer
					modifier = isPtr ? "P" : "A";

					// CV prefix
					// E __ptr64
					modifier += "E"; // pointer __ptr64

					// CV modifier
					// A - none
					// B - const
					modifier += isConst ? "B" : "A";
				}

				var fullName = RemoveRef(type);
				return retval + modifier + AsString(fullName, charSet);
			}
			public string AsString(Type type)
			{
				return AsString(type, CharSet.Unicode, isPtr: false, isConst: false);
			}


			public string AsString(ParameterInfo parameter, CharSet charSet)
			{
				var ptrAttribute = parameter.GetCustomAttribute<JDanielSmith.Runtime.InteropServices.PtrAttribute>();
				var constAttribute = parameter.GetCustomAttribute<JDanielSmith.Runtime.InteropServices.ConstAttribute>();
				return AsString(parameter.ParameterType, charSet, ptrAttribute != null, constAttribute != null);
			}
		}

		static readonly TypeToString typeToString = new TypeToString();

		string GetReturn(ParameterInfo returnParameter, CharSet charSet)
		{
			// https://en.wikiversity.org/wiki/Visual_C%2B%2B_name_mangling
			// A - no CV modifier
			string ret = "A";
			string value = typeToString.AsString(returnParameter, charSet);
			return ret + value;
		}

		string GetParameter(ParameterInfo parameter, CharSet charSet)
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

			return typeToString.AsString(parameter, charSet);
		}

		private static string GetCppNamespace(MethodInfo method)
		{
			var ns = method.DeclaringType.Namespace;

			// We'll look for the start of a C++ namespace in one of three places: "global::DllImport" (sure, why not?),
			// "... .DllImport. ...", and "... ._. ..." (because "DllImport" is a lot of typing).
			int skip = 1;
			int cppNsStart = ns.IndexOf("DllImport.", StringComparison.InvariantCulture);
			if (cppNsStart < 0)
			{
				skip = 2;
				cppNsStart = ns.IndexOf(".DllImport.", StringComparison.InvariantCulture);
				if (cppNsStart < 0)
				{
					cppNsStart = ns.IndexOf("._.", StringComparison.InvariantCulture);
				}
			}

			string retval = "";
			if (cppNsStart >= 0)
			{
				var cppNs = ns.Substring(cppNsStart);
				var names = cppNs.Split('.');
				var cppNames = names.Skip(skip).Reverse(); // ignore the marker and reverse the order
				foreach (var cppName in cppNames)
				{
					retval += "@" + cppName;
				}
			}

			return retval;
		}

		const string @const = "__const";
		private static bool IsConst(MethodInfo method)
		{
			return method.Name.EndsWith(@const, StringComparison.Ordinal) ||
				method.GetCustomAttribute<JDanielSmith.Runtime.InteropServices.ConstAttribute>() != null;
		}

		private static string GetName(MethodInfo method, System.Runtime.InteropServices.ComTypes.FUNCKIND funcKind)
		{
			var cppNs = GetCppNamespace(method);

			var methodName = method.Name;

			// C++ has a lot more overloads than C#, "void f(int*)" and "void f(const int*)".  We need
			// a way to specify that "void g(ref int)" should really call f().
			var dllImportAttribute = method.GetCustomAttribute<JDanielSmith.Runtime.InteropServices.DllImportAttribute>();
			string entryPoint = dllImportAttribute.EntryPoint;
			if (!String.IsNullOrWhiteSpace(entryPoint))
			{
				methodName = entryPoint;
			}

			if (funcKind == System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_NONVIRTUAL) // i.e., "static" method
			{
				cppNs = "@" + method.DeclaringType.Name + cppNs;
			}
			else if (funcKind == System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_VIRTUAL) // i.e., instance method
			{
				cppNs = "@" + method.DeclaringType.Name + cppNs;
				if (IsConst(method))
				{
					int lastIndex_const = methodName.LastIndexOf(@const, StringComparison.Ordinal);
					bool isConstMethodName = lastIndex_const == (methodName.Length - @const.Length);
					methodName = isConstMethodName ? methodName.Remove(lastIndex_const, @const.Length) : methodName;
				}
			}

			// foo - name
			return methodName + cppNs;
		}

		public string Mangle(MethodInfo method, System.Runtime.InteropServices.ComTypes.FUNCKIND funcKind, CharSet charSet = CharSet.Unicode)
		{
			string access = "Y"; // "none" (not public/private/protected static/virtual/thunk)
			if (funcKind == System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_NONVIRTUAL)
			{
				access = "S"; // "static"
			}
			else if (funcKind == System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_VIRTUAL) // i.e., instance method
			{
				access = "QE"; // member function, __thiscall,
				access += IsConst(method) ? "B" : "A";
			}

			var methodParameters_ = method.GetParameters();
			// first parameter is "this", don't use it for mangling
			int skip = funcKind == System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_VIRTUAL ? 1 : 0; // i.e., instance method
			var methodParameters = methodParameters_.Skip(skip);

			string parameters = String.Empty;
			foreach (var parameter in methodParameters)
			{
				parameters += GetParameter(parameter, charSet);
			}
			if (!String.IsNullOrWhiteSpace(parameters))
				parameters += "@"; // end of parameter list
			else
				parameters = typeToString.AsString(typeof(void));

			var returnType = GetReturn(method.ReturnParameter, charSet);

			// ? - decorated name
			// name@ - name fragment
			// @Z - end
			return "?" + GetName(method, funcKind) + "@@" + access + returnType + parameters + "Z";
		}
	}
}
