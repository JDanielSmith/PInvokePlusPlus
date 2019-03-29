using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JDanielSmith.Runtime.InteropServices;

namespace UnitTestInteropServices
{
    [TestClass]
    public class test_chars
    {
        public interface UnitTestCpp_chars
        {
            [DllImport("", ExactSpelling = true, PreserveSig = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            [Extern]
            long wc_sum(string s); // wc_sum(const wchar_t*)

            [DllImport("", ExactSpelling = true, PreserveSig = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
            [Extern]
            long c_sum(string s); // c_sum(const char*)
        }

        static long wc_sum(string s)
        {
            long retval = 0;
            foreach (var ch in s)
            {
                retval += (int)ch;
            }
            return retval;
        }

        static long c_sum(string s)
        {
            long retval = 0;
            foreach (var b in System.Text.Encoding.Default.GetBytes(s))
            {
                retval += (int)b;
            }
            return retval;
        }

        [TestMethod]
        public void test_chars_ascii()
        {
            var unitTestCpp_chars = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_chars>("UnitTestCpp");

            string s = "abc";

            var actual = unitTestCpp_chars.wc_sum(s);
            Assert.AreEqual(wc_sum(s), actual);

            actual = unitTestCpp_chars.c_sum(s);
            Assert.AreEqual(c_sum(s), actual);
            Assert.AreEqual(wc_sum(s), actual);
        }

        [TestMethod]
        public void test_chars_unicode()
        {
            var unitTestCpp_chars = JDanielSmith.NativeLibraryBuilder.Default.ActivateInterface<UnitTestCpp_chars>("UnitTestCpp");

            string s = "αβγ";

            var actual = unitTestCpp_chars.wc_sum(s);
            Assert.AreEqual(wc_sum(s), actual);

            actual = unitTestCpp_chars.c_sum(s);
            Assert.AreEqual(c_sum(s), actual);
        }
    }
}
