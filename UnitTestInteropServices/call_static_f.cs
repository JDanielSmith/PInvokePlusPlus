using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
	public interface C
	{
		[DllImport("", EntryPoint = ".", ExactSpelling = true, PreserveSig = true)]
		int f_int_int(int i);
	}

    namespace _.my.ns
    {
        public interface C
        {
            [DllImport("", EntryPoint = ".", ExactSpelling = true, PreserveSig = true)]
            int f_int_int(int i);
        }
    }

    [TestClass]
	public class call_static_f
	{
		[TestMethod]
		public void call_int_int()
		{
			var c = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<C>("UnitTestCpp");
			int actual = c.f_int_int(314);
			Assert.AreEqual(316, actual);
		}

        [TestMethod]
        public void call_ns_int_int()
        {
            var c = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<_.my.ns.C>("UnitTestCpp");
            int actual = c.f_int_int(314);
            Assert.AreEqual(318, actual);
        }
    }
}
