using System.Reflection;
using System.Runtime.InteropServices;

namespace JDanielSmith.Runtime.InteropServices
{
	public interface IEntrypointMangler
	{
		string Mangle(MethodInfo method, CharSet charSet);
	}
}
