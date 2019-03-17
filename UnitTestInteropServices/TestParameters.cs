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
			int ggg([Ptr] ref int i); // ?ggg@@YAHPEAH@Z

			[DllImport("", EntryPoint = "ggg", ExactSpelling = true, PreserveSig = true)]
			[Extern]
			int ggg_const([Const, Ptr] ref int i); // ?ggg@@YAHPEBH@Z

			[DllImport("", ExactSpelling = true, PreserveSig = true)]
			[Extern]
			int hhh(ref int i); // ?hhh@@YAHAEAH@Z

			[DllImport("", EntryPoint = "hhh", ExactSpelling = true, PreserveSig = true)]
			[Extern]
			int hhh_const([Const] ref int i); // ?hhh@@YAHAEAH@Z
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
		public void ggg_Const_ref_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<Parameters>("UnitTestCpp");
			int i = 314;
			int actual = unitTestCpp_f.ggg_const(ref i);
			Assert.AreEqual(314, i);
			Assert.AreEqual(315, actual);
		}

		[TestMethod]
		public void hhh_ref_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<Parameters>("UnitTestCpp");
			int i = 314;
			int actual = unitTestCpp_f.hhh(ref i);
			Assert.AreEqual(324, i);
			Assert.AreEqual(424, actual);
		}

		[TestMethod]
		public void hhh_const_ref_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<Parameters>("UnitTestCpp");
			int i = 314;
			int actual = unitTestCpp_f.hhh_const(ref i);
			Assert.AreEqual(314, i);
			Assert.AreEqual(415, actual);
		}
	}
}
