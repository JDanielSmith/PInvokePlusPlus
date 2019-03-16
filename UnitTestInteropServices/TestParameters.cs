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
			[DllImport("", ExactSpelling = true, PreserveSig = true)]
			[Extern]
			int ggg(ref int i); // ?ggg@@YAHPEAH@Z

			[DllImport("", EntryPoint = "ggg", ExactSpelling = true, PreserveSig = true)]
			[Extern]
			int ggg_const([Const] ref int i); // ?ggg@@YAHPEBH@Z

			[DllImport("", ExactSpelling = true, PreserveSig = true)]
			[Extern]
			int hhh(ref int i); // ?ggg@@YAHPEBH@Z
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

		[TestMethod]
		public void ggg_In_ref_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<Parameters>("UnitTestCpp");
			int i = 314;
			int actual = unitTestCpp_f.ggg_const(ref i);
			Assert.AreEqual(315, actual);
		}
	}
}
