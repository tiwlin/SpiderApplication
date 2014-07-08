using SpiderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MeituanSourceModelTest and is intended
    ///to contain all MeituanSourceModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MeituanSourceModelTest
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
        ///A test for Init
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SpiderLib.dll")]
        public void InitTest()
        {
            MeituanSourceModel_Accessor target = new MeituanSourceModel_Accessor(); // TODO: Initialize to an appropriate value
            target.UrlDic = new System.Collections.Generic.Dictionary<string, string>();
            target.UrlDic.Add("gz", "http://gz.meituan.com/category");
            WebProxy proxy = new WebProxy(new Uri("http://62.113.208.89:7808"));
            target.Proxy = proxy;
            //target.Init();
            target.Spider();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod]
        public void TestDncode()
        {
            string str = System.Web.HttpUtility.UrlDecode("params=%7B%22mteventParams%22%3A%7B%22la%22%3A%22deal%2Fsee%22%2C%22tf%22%3A%22xiuxianyule%22%2C%22geo%22%3A%22subway%22%2C%22st%22%3A%22default%22%2C%22pg%22%3A1%2C%22rs%22%3A%22default%22%7D%7D&geotype=4&areaid=subway&offset=12&dealids=4847288%2C25260687%2C9297462%2C2825973%2C25244537%2C22265039%2C15488636%2C25254982%2C15701539%2C3948914%2C25159531%2C5432523%2C9932937%2C25091002%2C12113936%2C7429149%2C25209712%2C25099641%2C2122388%2C17784911%2C4554248%2C1386764%2C5689934%2C6255452%2C1245820%2C2640637%2C25001086%2C7619386%2C3133808%2C25389029%2C7665409%2C25412190%2C25133865%2C16552281%2C6205137%2C25124765", System.Text.Encoding.UTF8);
            Console.WriteLine(str);
        }

        [TestMethod]
        public void TestEncode()
        {
            string str = System.Web.HttpUtility.UrlEncode("params={\"mteventParams\":{\"la\":\"deal/see\",\"tf\":\"zizhucan\",\"geo\":\"beijinglushangyequ\",\"st\":\"default\",\"pg\":1,\"rs\":\"default\"}}&geotype=1&areaid=719&offset=0&dealids1975818,3323923,25184429,24987962,25183386,25140679,25140678,25184426,24934947,21262908,2015437,24987959,11863950,25062330,2180859,24934928,25064137,11864011,21262804,25114282,24987955,24934936,21262805,7071019,8841456,3323691", System.Text.Encoding.UTF8);
            Console.WriteLine(str);
        }

        [TestMethod]
        public void UnicodeToChs()
        {
            string str = "\u30103\u5e97\u901a\u7528\u3011\u4e0a\u97e9\u9986\u97e9\u5f0f\u81ea\u52a93D\u4e3b\u9898\u9910\u5385";

            //string result = UniconToString(str);
        }

        public string UniconToString(string str)
        {
            string outStr = "";
            if (!string.IsNullOrEmpty(str))
            {
                string[] strlist = str.Replace("\\", "").Split('u');
                try
                {
                    for (int i = 1; i < strlist.Length; i++)
                    {
                        //将unicode字符转为10进制整数，然后转为char中文字符
                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch (FormatException ex)
                {
                    outStr = ex.Message;
                }
            }
            return outStr;
        }

        [TestMethod]
        public void InitProxyUri()
        {
            MeituanSourceModel_Accessor target = new MeituanSourceModel_Accessor();
            target.Proxy = null;
            target.InitProxyUris();
        }
    }
}
