using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
	[TestClass]
	public class TestParameters
	{
		public interface Parameters
		{
			[DllImport("", EntryPoint = "::", ExactSpelling = true, PreserveSig = true)]
			int ggg(ref int i); // ?ggg@@YAHPEAH@Z
		}

		[TestMethod]
		public void ggg_ref_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<Parameters>("UnitTestCpp");
			int i = 314;
			int actual = unitTestCpp_f.ggg(ref i);
			Assert.AreEqual(315, i);
			Assert.AreEqual(316, actual);
		}
	}
}
