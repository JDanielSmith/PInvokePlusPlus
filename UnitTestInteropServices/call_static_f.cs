using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
	public interface C
	{
		[DllImport("", EntryPoint = "=", ExactSpelling = true, PreserveSig = true)]
		int f_int_int(int i);
	}

	[TestClass]
	public class call_static_f
	{
		[TestMethod]
		public void call_ns_f_int_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<C>("UnitTestCpp");
			int actual = unitTestCpp_f.f_int_int(314);
			Assert.AreEqual(324, actual);
		}
	}
}
