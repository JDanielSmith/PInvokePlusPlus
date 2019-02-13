using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
	[TestClass]
	public class call_f
	{
		public interface UnitTestCpp_f
		{
			[DllImport("", EntryPoint = "*", ExactSpelling = true, PreserveSig = true)]
			int f_int_int(int i);
		}

		[TestMethod]
		public void call_f_int_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");
			int actual = unitTestCpp_f.f_int_int(314);
			Assert.AreEqual(315, actual);
		}
	}
}
