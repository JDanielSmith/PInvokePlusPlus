using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
    [TestClass]
    public class StructParameter
    {
		public interface S
		{
		}

		public interface UnitTestCpp_f
        {
            [DllImport("", ExactSpelling = true, PreserveSig = true)]
            [Extern]
            int abc([Ptr,Const] ref S s);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var unitTestCpp_f = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_f>("UnitTestCpp");

			S s = null;
            var actual = unitTestCpp_f.abc(ref s);
            Assert.AreEqual(7, actual);
        }
    }
}
