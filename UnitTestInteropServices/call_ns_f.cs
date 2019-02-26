using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
	namespace _.my.ns
	{
		public interface UnitTestCpp_ns_f
		{
			[DllImport("", EntryPoint = "*", ExactSpelling = true, PreserveSig = true)]
			int f_int_int(int i);
		}
	}

	[TestClass]
	public class call_ns_f
	{
		[TestMethod]
		public void call_ns_f_int_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<_.my.ns.UnitTestCpp_ns_f>("UnitTestCpp");
			int actual = unitTestCpp_f.f_int_int(314);
			Assert.AreEqual(324, actual);
		}
	}
}
