using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
    namespace UnitTestCpp._.my
    {
        public interface UnitTestCpp_f
        {
            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f();
        }

        namespace ns
        {
            public interface UnitTestCpp_f
            {
                [DllImport("", ExactSpelling = true, PreserveSig = true)]
                [Extern]
                int f();
            }
        }
    }

    namespace UnitTestCpp._.ns
    {
        public interface UnitTestCpp_f
        {
            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int f();
        }
    }

    [TestClass]
    public class test_f_in_namespaces
    {
        [TestMethod]
        public void invoke_my_f()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp._.my.UnitTestCpp_f>("UnitTestCpp");

            var actual = unitTestCpp_f.f();
            Assert.AreEqual(100, actual);
        }

        [TestMethod]
        public void invoke_my_ns_f()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp._.my.ns.UnitTestCpp_f>("UnitTestCpp");

            var actual = unitTestCpp_f.f();
            Assert.AreEqual(101, actual);
        }

        [TestMethod]
        public void invoke_ns_f()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp._.ns.UnitTestCpp_f>("UnitTestCpp");

            var actual = unitTestCpp_f.f();
            Assert.AreEqual(102, actual);
        }
    }
}
