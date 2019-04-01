using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
    [TestClass]
    public class StructParameter
    {
        public interface UnitTestCpp_f
        {
            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int xyz([Ptr,Const] ref UnitTestCpp._.ns.S s);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");

            UnitTestCpp._.ns.S ns_s = null;
            var actual = unitTestCpp_f.xyz(ref ns_s);
            Assert.AreEqual(7, actual);
        }
    }
}
