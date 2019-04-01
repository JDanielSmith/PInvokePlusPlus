using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
    [TestClass]
    public class test_f_overloads
    {
        public interface UnitTestCpp_f
        {
            [DllImport("UnitTestCpp")]
            int f();

            [DllImport("", EntryPoint = "f", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f_1();

            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f(int i);

            [DllImport("", EntryPoint = "f", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f_2([Const] ref int i); // f(const int&)

            [DllImport("", EntryPoint = "f", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f_3([Const, Ptr] ref int i); // f(const int*)

            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f(ref int i); // f(int&)

            [DllImport("", EntryPoint = "f", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f_4([Ptr] ref int i); // f(int*)

            [DllImport("", ExactSpelling = true, PreserveSig = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            [Extern]
            ulong f(string s); // f(const wchar_t*)

            [DllImport("", EntryPoint = "f", ExactSpelling = true, PreserveSig = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
            [Extern]
            ulong f_5(string s); // f(const char*)

            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f([Ptr] ref UnitTestCpp._.ns.S s);
        }

        [TestMethod]
        public void invoke_f_overloads_C()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");

            var actual = unitTestCpp_f.f();
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void invoke_f_overloads_cpp()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");

            var actual = unitTestCpp_f.f(1);
            Assert.AreEqual(2, actual);

            int i = 1;
            actual = unitTestCpp_f.f(ref i);
            Assert.AreEqual(2, i);
            Assert.AreEqual(6, actual);

            string s = "abc";
            actual = (int) unitTestCpp_f.f(s);
            Assert.AreEqual(s.Length, actual);

            s = "ΑΒ";
            actual = (int) unitTestCpp_f.f(s);
            Assert.AreEqual(s.Length, actual);

            //UnitTestCpp._.ns.S ns_s = null;
            //actual = unitTestCpp_f.f(ref ns_s);
            //Assert.AreEqual(7, actual);
        }

        [TestMethod]
        public void invoke_renamed_const_f_overloads_cpp()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");

            int i = 1;
            var actual = unitTestCpp_f.f_2(ref i);
            Assert.AreEqual(1, i); // actually, "const int&"
            Assert.AreEqual(3, actual);

            actual = unitTestCpp_f.f_3(ref i);
            Assert.AreEqual(1, i); // actually, "const int*"
            Assert.AreEqual(4, actual);

            string s = "abc";
            actual = (int) unitTestCpp_f.f_5(s);
            Assert.AreEqual(s.Length, actual);
            
            s = "ΑΒ";
            actual = (int) unitTestCpp_f.f_5(s);
            Assert.AreEqual(s.Length, actual);
        }

        [TestMethod]
        public void invoke_renamed_f_overloads_cpp()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");

            int i = 1;
            var actual = unitTestCpp_f.f_4(ref i);
            Assert.AreEqual(2, i); // "f(int&)
            Assert.AreEqual(7, actual);
        }
    }
}