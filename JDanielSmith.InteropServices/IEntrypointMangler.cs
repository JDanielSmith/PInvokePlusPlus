using System.Reflection;

namespace JDanielSmith.Runtime.InteropServices
{
	public interface IEntrypointMangler
	{
		string Mangle(MethodInfo method);
	}
}
