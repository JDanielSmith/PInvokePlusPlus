using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

using JDanielSmith.Runtime.InteropServices;

namespace JDanielSmith
{
	// https://github.com/Firwood-Software/AdvanceDLSupport/blob/master/docs/quickstart.md
	public class NativeLibraryBuilder
	{
		static IEntrypointMangler GetDefaultMangler()
		{
			// TODO: logic to determine default manger for different platforms/compilers
			return new JDanielSmith.Runtime.InteropServices.VCEntrypointMangler();
		}
		readonly IEntrypointMangler DefaultEntrypointMangler = GetDefaultMangler();

		static readonly string myNamespace = typeof(NativeLibraryBuilder).Namespace;

		private NativeLibraryBuilder()
		{
		}
		public static readonly NativeLibraryBuilder Default = new NativeLibraryBuilder();

		public TInterface ActivateInterface<TInterface>(string dll, IEntrypointMangler entrypointMangler = null) where TInterface : class
		{
			if (entrypointMangler == null)
				entrypointMangler = DefaultEntrypointMangler;
			return ActivateInterface_<TInterface>(dll, entrypointMangler);
		}

		static IEnumerable<MethodInfo> GetInterfaceMethods(Type type)
		{
			// We might have been given something other than an actual "interface": the generic
			// constraint is "class".  Also, this could make it easier to do P/Invoke from existing infrastructure.
			var interfaceMethods = from method in type.GetMethods()
								   where !(method.IsStatic || !method.IsPublic || !method.IsVirtual || !method.IsAbstract) // should have "public" non-static, virtual+abstract methods
								   where method.GetMethodBody() == null // interface methods don't have a body
								   where method.GetCustomAttribute<JDanielSmith.Runtime.InteropServices.DllImportAttribute>() != null // must have our custom attribute
								   select method;
			return interfaceMethods;
		}

		static (string, string) MakeSyntax(Type interfaceType, string nativeMethods, string interfaceImplementation)
		{
			var strGuid = System.Guid.NewGuid().ToString().Replace('-', '_');
			var namespaceName = myNamespace + ".Namespace_" + strGuid; // a unique namespace; e.g., "JDanielSmith.NativeLibraryBuilder.Namespace_<guid>"
			var interfaceFullName = interfaceType.FullName.Replace('+', '.'); // '+' in nested classes
			// e.g., for an interface of "My.Namespace.INativeMethods", our implmentation class is "My_Namespace_INativeMethods_Implementation"
			var className = interfaceFullName.Replace('.', '_') + "_Implementation";
			string text = @"namespace " + namespaceName + @"
			{
				public sealed class " + className + " : " + interfaceFullName + @"
				{
					private static class NativeMethods
					{
					" + nativeMethods + @"
					}
				" + interfaceImplementation + @"
				}
			}";

			var implementationTypeName = namespaceName + "." + className; // e.g., "JDanielSmith.NativeLibraryBuilder.Namespace_<guid>.My_Namespace_INativeMethods_Implementation"
			return (text, implementationTypeName);
		}

		static TInterface EmitCSharpCompilation<TInterface>(SyntaxTree syntaxTree, IEnumerable<MetadataReference> references, string implementationTypeName) where TInterface : class
		{
			var compilation = CSharpCompilation.Create(
							Path.GetRandomFileName(),
							syntaxTrees: new[] { syntaxTree },
							references: references,
							options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			using (var ms = new MemoryStream())
			{
				var result = compilation.Emit(ms);

				if (!result.Success)
				{
					var failures = result.Diagnostics.Where(diagnostic =>
						diagnostic.IsWarningAsError ||
						diagnostic.Severity == DiagnosticSeverity.Error);

					var diagnosticExceptions = from diagnostic in failures
											   let message = $"{diagnostic.Id}: {diagnostic.GetMessage()}"
											   select new InvalidOperationException(message);
					throw new AggregateException(diagnosticExceptions);
				}

				ms.Seek(0, SeekOrigin.Begin);
				var assembly = Assembly.Load(ms.ToArray());
				var type = assembly.GetType(implementationTypeName);
				return Activator.CreateInstance(type) as TInterface;
			}
		}

		TInterface ActivateInterface_<TInterface>(string dll, IEntrypointMangler entrypointMangler) where TInterface : class
		{
			var interfaceType = typeof(TInterface);

            var methods = GetInterfaceMethods(interfaceType);
			var methodInfoDetails = new MethodInfoDetails(dll, interfaceType, entrypointMangler);

			// https://github.com/dotnet/roslyn-sdk/blob/master/samples/CSharp/APISamples/FAQ.cs
			var nativeMethods = new StringBuilder();
			var interfaceImplementation = new StringBuilder();
			foreach (var method in methods)
			{
				nativeMethods.AppendLine(methodInfoDetails.GetDllImportSignature(method));
				interfaceImplementation.AppendLine(methodInfoDetails.GetInterfaceSignature(method));
			}

			var (syntax, implementationTypeName) = MakeSyntax(interfaceType, nativeMethods.ToString(), interfaceImplementation.ToString());
			var syntaxTree = SyntaxFactory.ParseSyntaxTree(syntax);

			var references = methodInfoDetails.References;
			return EmitCSharpCompilation<TInterface>(syntaxTree, references, implementationTypeName);
		}
	}
}
