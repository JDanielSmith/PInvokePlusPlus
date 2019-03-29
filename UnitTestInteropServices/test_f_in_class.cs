using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
    namespace UnitTestCpp._.ns
    {
        public interface S
        {
            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Static]
            int f();

            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            int f(IntPtr @this, int i);

            [DllImport("", EntryPoint="f", ExactSpelling = true, PreserveSig = true)]
            [Const]
            int f_1(IntPtr @this, int i);
        }
    }

    [TestClass]
    public class test_f_in_class
    {
        [TestMethod]
        public void invoke_ns_S_f()
        {
            var s = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp._.ns.S>("UnitTestCpp");

            var actual = s.f();
            Assert.AreEqual(1003, actual);

            actual = s.f(IntPtr.Zero, 1);
            Assert.AreEqual(1005, actual);

            actual = s.f_1(IntPtr.Zero, 1);
            Assert.AreEqual(1006, actual);
        }

        public interface C
        {
            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Static]
            int f();

            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            int f(IntPtr @this, int i);

            [DllImport("", EntryPoint = "f", ExactSpelling = true, PreserveSig = true)]
            [Const]
            int f_1(IntPtr @this, int i);
        }

        [TestMethod]
        public void invoke_C_f()
        {
            var c = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<C>("UnitTestCpp");

            var actual = c.f();
            Assert.AreEqual(1000, actual);

            actual = c.f(IntPtr.Zero, 1);
            Assert.AreEqual(1002, actual);

            actual = c.f_1(IntPtr.Zero, 1);
            Assert.AreEqual(1003, actual);
        }

    }
}
