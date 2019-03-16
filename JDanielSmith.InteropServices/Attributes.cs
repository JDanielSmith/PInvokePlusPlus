using System;

namespace JDanielSmith.Runtime.InteropServices
{
	/// <summary>
	/// Indicates that a method is a "static" member function
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class StaticAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that a method is global (extern) function, not a member of a class
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class ExternAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that a member function is "const." (can also be done with a "__const" suffix at the end of the method name)
	/// Indicates that a parameter is "const."
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	public sealed class ConstAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that a parameter is a "pointer" parameter, "int*"
	/// (c.f., shared_ptr or unique_ptr)
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	public sealed class PtrAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that a parameter is a "reference" parameter, "int&"
	/// (c.f., "ptr")
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	public sealed class RefAttribute : Attribute
	{
	}
}

