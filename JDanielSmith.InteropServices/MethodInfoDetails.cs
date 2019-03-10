using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.CodeAnalysis;


namespace JDanielSmith.Runtime.InteropServices
{
	/// <summary>
	/// Process MethodInfo in ways useful to NativeLibraryBuilder
	/// </summary>
	class MethodInfoDetails
	{
		readonly Dictionary<string, MetadataReference> references_ = new Dictionary<string, MetadataReference>(); // use a dictionary to avoid duplicates
		public IEnumerable<MetadataReference> References => references_.Values;

		readonly string Dll;
		readonly IEntrypointMangler EntrypointMangler;

		public MethodInfoDetails(string dll, Type interfaceType, IEntrypointMangler entrypointMangler)
		{
			Dll = dll;

			// the real [DllImport] attribute, not our's
			AddReference(typeof(System.Runtime.InteropServices.DllImportAttribute));
			AddReference(interfaceType);

			EntrypointMangler = entrypointMangler;
		}

		void AddReference(Type type)
		{
			string location = type.Assembly.Location;
			if (!references_.ContainsKey(location))
			{
				var metadataReference = MetadataReference.CreateFromFile(location);
				references_.Add(location, metadataReference);
			}
		}
		void AddReferences(MethodInfo method)
		{
			AddReference(method.ReturnType);
			foreach (var parameter in method.GetParameters())
			{
				AddReference(parameter.ParameterType);
			}
		}

		static string GetParameterTypeName(Type parameterType)
		{
			string fullName = parameterType.FullName;
			if (parameterType.IsByRef && fullName.EndsWith("&", StringComparison.Ordinal))
			{
				fullName = "ref " + fullName.Substring(0, fullName.Length - 1);
			}

			return fullName;
		}

		static string GetDeclParams(IEnumerable<ParameterInfo> parameters)
		{
			var declParams = new StringBuilder("(");
			string prefix = "";
			foreach (var parameter in parameters)
			{
				string paramName = "p" + parameter.Position.ToString();
				declParams.Append(prefix + GetParameterTypeName(parameter.ParameterType) + " " + paramName);
				prefix = ", ";
			}
			declParams.Append(")");

			return declParams.ToString();
		}

		public string GetInterfaceSignature(MethodInfo method)
		{
			AddReferences(method);

			var parameters = method.GetParameters();

			var bodyArgs = new StringBuilder("NativeMethods." + method.Name + "(");
			string prefix = "";
			foreach (var parameter in parameters)
			{
				string paramName = "p" + parameter.Position.ToString();
				if (parameter.ParameterType.IsByRef)
				{
					paramName = "ref " + paramName;
				}

				bodyArgs.Append(prefix + paramName);

				prefix = ", ";
			}
			bodyArgs.Append(");");

			var text = "public " + method.ReturnType.FullName + " " + method.Name + GetDeclParams(parameters);
			text += "{ return " + bodyArgs.ToString() + " }";
			return text;
		}
        static System.Runtime.InteropServices.ComTypes.FUNCKIND GetFUNCKIND(MethodInfo method)
        {
			// There are three types of mangled names names:
			// * global functions, perhaps inside of a namespace
			// * "static" methods, inside a class
			// * "instance" method, inside a class using the "this" pointer
			//
			// There isn't any way with just an interface method to distinguish between
			// these.  So, we'll overload the "EntryPoint" property and parameter list a
			// little bit (to avoid creating a new [Attribute]).
			// * "::" means a global function, ::foo::bar::baz()
			// * "." means a class method
			// * if first parameter is named "this", it is an instance method; otherwise "static"

			var dllImportAttribute = method.GetCustomAttribute<JDanielSmith.Runtime.InteropServices.DllImportAttribute>();
			var entryPoint = dllImportAttribute.EntryPoint;
            if (entryPoint.StartsWith("::", StringComparison.Ordinal))
                return System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_STATIC;

			if (!entryPoint.StartsWith(".", StringComparison.Ordinal))
			{
				return System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_DISPATCH; // i.e., don't know anything; try elsewhere
			}

			var firstParameter = method.GetParameters().FirstOrDefault();
			if (firstParameter == null)
			{
				return System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_NONVIRTUAL; // "static" method with no arguments
			}

			string firstParameterName = firstParameter.Name;
			if (firstParameterName == "this")
			{
				return System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_VIRTUAL;
			}

            return System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_NONVIRTUAL;
        }

        static (bool, System.Runtime.InteropServices.ComTypes.FUNCKIND) shouldMangleName(MethodInfo method)
		{
			var dllImportAttribute = method.GetCustomAttribute<JDanielSmith.Runtime.InteropServices.DllImportAttribute>();

			// By default, using [JDanielSmith.Runtime.InteropServices.DllImportAttribute] and NativeLibraryBuilder.ActivateInterface()
			// should work exactly the same as directly using P/Invoke; this makes it easier to integrate with existing code.
			//
			// To enable C++ name-manging, the [DllImport] attribute must be include: (ExactSpelling=true, PreserveSig=true, EntryPoint="*")
			// 	[DllImport("", EntryPoint ="*", ExactSpelling = true, PreserveSig = true)]

			var dontMangle = (false, System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_DISPATCH);
            // ExactSpelling=true means don't try to mangle "foo()" as "fooA" (ANSI) or "fooW" ("wide"/Unicode)
            if (!dllImportAttribute.ExactSpelling) return dontMangle;

			// PreserveSig=true means don't try to turn a return value into an Exception
			if (!dllImportAttribute.PreserveSig) return dontMangle;

			// For now (at least), the attribute's Value (DLL name) should be empty; this is because
			// the DLL name is specified as an argument to ActivateInterface()
			if (!String.IsNullOrWhiteSpace(dllImportAttribute.Value)) return dontMangle;

            var funcKind = GetFUNCKIND(method);
            return ((funcKind != System.Runtime.InteropServices.ComTypes.FUNCKIND.FUNC_DISPATCH), funcKind);
		}

		static readonly System.Runtime.InteropServices.DllImportAttribute DefaultDllImportAttribute = new System.Runtime.InteropServices.DllImportAttribute("a dummy name:|/\\"); // include characters that can't be part of any filename

		static void MakeDllImportArgument(StringBuilder retval, string name, bool arg, bool defArg)
		{
			if (arg != defArg)
			{
				retval.Append(", " + name + "=" + (arg ? "true" : "false"));
			}
		}

		static string MakeDllImportArguments(string entryPoint, System.Runtime.InteropServices.CharSet charSet,
			bool setLastError, bool exactSpelling, bool preserveSig,
			System.Runtime.InteropServices.CallingConvention callingConvention,
			bool bestFitMapping, bool throwOnUnmappableChar)
		{
			var retval = new StringBuilder();
			if (!String.IsNullOrWhiteSpace(entryPoint))
			{
				retval.Append(", EntryPoint=\"" + entryPoint + '"');
			}
			if (charSet != DefaultDllImportAttribute.CharSet)
			{
				retval.Append(", CharSet = System.Runtime.InteropServices.CharSet." + charSet.ToString());
			}

			MakeDllImportArgument(retval, "SetLastError", setLastError, DefaultDllImportAttribute.SetLastError);
			MakeDllImportArgument(retval, "ExactSpelling", exactSpelling, DefaultDllImportAttribute.ExactSpelling);
			MakeDllImportArgument(retval, "PreserveSig", preserveSig, DefaultDllImportAttribute.PreserveSig);

			if (callingConvention != DefaultDllImportAttribute.CallingConvention)
			{
				retval.Append(", CallingConvention=" + callingConvention.ToString());
			}

			MakeDllImportArgument(retval, "BestFitMapping", bestFitMapping, DefaultDllImportAttribute.BestFitMapping);
			MakeDllImportArgument(retval, "ThrowOnUnmappableChar", throwOnUnmappableChar, DefaultDllImportAttribute.ThrowOnUnmappableChar);

			return retval.ToString();
		}

		string GetDllImportAttribute(MethodInfo method)
		{
			var dllImportAttribute = method.GetCustomAttribute<JDanielSmith.Runtime.InteropServices.DllImportAttribute>();

			string dllImport = @"[System.Runtime.InteropServices.DllImport(""";

            var (mangleName, funcKind) = shouldMangleName(method);
			if (!mangleName)
			{
				var dll = String.IsNullOrWhiteSpace(dllImportAttribute.Value) ? Dll : dllImportAttribute.Value;
				dllImport += dll + @"""";
				// EntryPoint, CharSet, SetLastError, ExactSpelling, PreserveSig, CallingConvention, BestFitMapping, ThrowOnUnmappableChar
				dllImport += MakeDllImportArguments(dllImportAttribute.EntryPoint, dllImportAttribute.CharSet,
					dllImportAttribute.SetLastError, dllImportAttribute.ExactSpelling, dllImportAttribute.PreserveSig,
					dllImportAttribute.CallingConvention, dllImportAttribute.BestFitMapping, dllImportAttribute.ThrowOnUnmappableChar);
			}
			else
			{
                dllImport += Dll + @"""";
				string entryPoint = EntrypointMangler.Mangle(method, funcKind, dllImportAttribute.CharSet);
				// EntryPoint, CharSet, SetLastError, ExactSpelling, PreserveSig, CallingConvention, BestFitMapping, ThrowOnUnmappableChar
				dllImport += MakeDllImportArguments(entryPoint, dllImportAttribute.CharSet,
					dllImportAttribute.SetLastError, dllImportAttribute.ExactSpelling, dllImportAttribute.PreserveSig,
					dllImportAttribute.CallingConvention, dllImportAttribute.BestFitMapping, dllImportAttribute.ThrowOnUnmappableChar);
			}

			dllImport += ")]";
			return dllImport;
		}

		public string GetDllImportSignature(MethodInfo method)
		{
			string dllImport = GetDllImportAttribute(method);
			var parameters = GetDeclParams(method.GetParameters());
			return dllImport + "\n" + @"internal static extern " + method.ReturnType.FullName + " " + method.Name + parameters + ";\n";
		}
	}
}