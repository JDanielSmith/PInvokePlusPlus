using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
	public interface C
	{
		[DllImport("", EntryPoint = ".", ExactSpelling = true, PreserveSig = true)]
		int f_int_int(int i);

        [DllImport("", EntryPoint = ".", ExactSpelling = true, PreserveSig = true)]
        int g_int_int(IntPtr @this, int i); // ?g_int_int@C@@QEAAHH@Z

        [DllImport("", EntryPoint = ".", ExactSpelling = true, PreserveSig = true)]
        int g_int_int__const(IntPtr @this, int i); // ?g_int_int@C@@QEAAHH@Z
    }

    namespace _.my.ns
    {
        public interface C
        {
            [DllImport("", EntryPoint = ".", ExactSpelling = true, PreserveSig = true)]
            int f_int_int(int i);

            [DllImport("", EntryPoint = ".", ExactSpelling = true, PreserveSig = true)]
            int g_int_int(IntPtr @this, int i);
        }
    }

    [TestClass]
	public class call_C_methods
	{
		[TestMethod]
		public void call_static()
		{
			var c = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<C>("UnitTestCpp");
			int actual = c.f_int_int(314);
			Assert.AreEqual(316, actual);
		}

        [TestMethod]
        public void call_ns_static()
        {
            var c = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<_.my.ns.C>("UnitTestCpp");
            int actual = c.f_int_int(314);
            Assert.AreEqual(318, actual);
        }

        [TestMethod]
        public void call_instance()
        {
            var c = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<C>("UnitTestCpp");
            int actual = c.g_int_int(IntPtr.Zero, 314);
            Assert.AreEqual(414, actual);
        }

        [TestMethod]
        public void call_const_instance()
        {
            var c = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<C>("UnitTestCpp");
            int actual = c.g_int_int__const(IntPtr.Zero, 314);
            Assert.AreEqual(415, actual);
        }

        [TestMethod]
        public void call_ns_instance()
        {
            var c = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<_.my.ns.C>("UnitTestCpp");
            int actual = c.g_int_int(IntPtr.Zero, 314);
            Assert.AreEqual(514, actual);
        }
    }
}
