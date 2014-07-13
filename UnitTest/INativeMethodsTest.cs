using CommonHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace UnitTest
{
    
    
    /// <summary>
    ///This is a test class for INativeMethodsTest and is intended
    ///to contain all INativeMethodsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class INativeMethodsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for InternetGetCookieEx
        ///</summary>
        [TestMethod()]
        public void InternetGetCookieExTest()
        {
            string Url = "http://gz.meituan.com"; ; // TODO: Initialize to an appropriate value
            string cookieName = string.Empty; // TODO: Initialize to an appropriate value
            StringBuilder cookieData = new StringBuilder(); // TODO: Initialize to an appropriate value
            StringBuilder cookieDataExpected = null; // TODO: Initialize to an appropriate value
            uint pchCookieData = 0; // TODO: Initialize to an appropriate value
            uint pchCookieDataExpected = 0; // TODO: Initialize to an appropriate value
            uint flags = (uint)INativeMethods.InternetFlags.INTERNET_COOKIE_HTTPONLY; ; // TODO: Initialize to an appropriate value
            IntPtr reserved =IntPtr.Zero; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = INativeMethods.InternetGetCookieEx(Url, null, null, ref pchCookieData, flags, reserved);

            pchCookieData++;

            cookieData = new StringBuilder((int)pchCookieData);

            actual = INativeMethods.InternetGetCookieEx(Url, null, cookieData, ref pchCookieData, flags, IntPtr.Zero);

            Assert.AreEqual(cookieDataExpected, cookieData);
            Assert.AreEqual(pchCookieDataExpected, pchCookieData);
            Assert.AreEqual(expected, actual);
        }
    }
}
