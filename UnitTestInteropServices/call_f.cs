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

			[DllImport("UnitTestCpp", ExactSpelling = true, PreserveSig = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
			int f_wcslen_C(String s);

			[DllImport("", EntryPoint = "*", ExactSpelling = true, PreserveSig = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
			int f_wcslen(String s);

			[DllImport("", EntryPoint = "*", ExactSpelling = true, PreserveSig = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
			int f_strlen(String s);
		}

		[TestMethod]
		public void call_f_int_int()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");
			int actual = unitTestCpp_f.f_int_int(314);
			Assert.AreEqual(315, actual);
		}

		internal sealed class NativeMethods 
		{
			[System.Runtime.InteropServices.DllImport("UnitTestCpp", ExactSpelling = true, PreserveSig = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
			public extern static int f_wcslen_C(String s);
		}
		[TestMethod]
		public void call_f_wcslen_C_NativeMethods()
		{
			int actual = NativeMethods.f_wcslen_C("abc");
			Assert.AreEqual(3, actual);
		}

		[TestMethod]
		public void call_f_wcslen_C()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");
			int actual = unitTestCpp_f.f_wcslen_C("abc");
			Assert.AreEqual(3, actual);
		}

		[TestMethod]
		public void call_f_wcslen()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");
			int actual = unitTestCpp_f.f_wcslen("abc");
			Assert.AreEqual(3, actual);
		}

		[TestMethod]
		public void call_f_strlen()
		{
			var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");
			int actual = unitTestCpp_f.f_strlen("abc");
			Assert.AreEqual(3, actual);
		}
	}
}
