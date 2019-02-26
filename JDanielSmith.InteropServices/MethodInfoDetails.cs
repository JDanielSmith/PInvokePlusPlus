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

		static string GetDeclParams(IEnumerable<ParameterInfo> parameters)
		{
			var declParams = new StringBuilder("(");
			string prefix = "";
			foreach (var parameter in parameters)
			{
				string paramName = "p" + parameter.Position.ToString();
				declParams.Append(prefix + parameter.ParameterType.FullName + " " + paramName);
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
				bodyArgs.Append(prefix + paramName);

				prefix = ", ";
			}
			bodyArgs.Append(");");

			var text = "public " + method.ReturnType.FullName + " " + method.Name + GetDeclParams(parameters);
			text += "{ return " + bodyArgs.ToString() + " }";
			return text;
		}

		static bool shouldMangleName(JDanielSmith.Runtime.InteropServices.DllImportAttribute dllImportAttribute)
		{
			// By default, using [JDanielSmith.Runtime.InteropServices.DllImportAttribute] and NativeLibraryBuilder.ActivateInterface()
			// should work exactly the same as directly using P/Invoke; this makes it easier to integrate with existing code.
			//
			// To enable C++ name-manging, the [DllImport] attribute must be include: (ExactSpelling=true, PreserveSig=true, EntryPoint="*")
			// 	[DllImport("", EntryPoint ="*", ExactSpelling = true, PreserveSig = true)]

			// ExactSpelling=true means don't try to mangle "foo()" as "fooA" (ANSI) or "fooW" ("wide"/Unicode)
			if (!dllImportAttribute.ExactSpelling) return false;

			// PreserveSig=true means don't try to turn a return value into an Exception
			if (!dllImportAttribute.PreserveSig) return false;

			// For now (at least), the attribute's Value (DLL name) should be empty; this is because
			// the DLL name is specified as an argument to ActivateInterface()
			if (!String.IsNullOrWhiteSpace(dllImportAttribute.Value)) return false;

			// '*' is a common "wildcard" which seems like a decent EntryPoint name to indicate
			// name-mangling is desired. '=' is also used to indicate name-mangling, in this case,
			// the interface name is the class name for 'static' methods
			return (dllImportAttribute.EntryPoint == "*") || (dllImportAttribute.EntryPoint == "="); ; // explicilty turn on name-mangling
		}

		static readonly System.Runtime.InteropServices.DllImportAttribute DefaultDllImportAttribute = new System.Runtime.InteropServices.DllImportAttribute("a dummy name:|/\\"); // include characters that can't be part of any filename

		static void MakeDllImportArgument(StringBuilder retval, string name, bool arg, bool defArg)
		{
			if (arg != defArg)
			{
				retval.Append(", " + name + "=" + (arg ? "true" : "false"));
			}
		}

		string MakeDllImportArguments(string entryPoint, System.Runtime.InteropServices.CharSet charSet,
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
			if (!shouldMangleName(dllImportAttribute))
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
				bool staticMethodInClass = dllImportAttribute.EntryPoint == "=";

				dllImport += Dll + @"""";
				string entryPoint = EntrypointMangler.Mangle(method, staticMethodInClass, dllImportAttribute.CharSet);
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