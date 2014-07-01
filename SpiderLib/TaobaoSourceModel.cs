using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using System.Threading;

namespace SpiderLib
{
    public class TaobaoSourceModel : SourceModelBase
    {
        #region Fileds
        /// <summary>
        /// 是否加载完成
        /// </summary>
        private static bool _isLoadCompleted = true;

        /// <summary>
        /// 访问的浏览器控件
        /// </summary>
        private static WebBrowser _webBrowser = null;

        /// <summary>
        /// 当前访问的网页的Key
        /// </summary>
        private static string _currentKey = string.Empty;

        /// <summary>
        /// 当前访问的网页的内容
        /// </summary>
        private static HtmlDocument _currentHtmlDocument = null;

        private List<tnx.collectservice.AirTicketCollectData> _airTickets = null;

        /// <summary>
        /// 页面加载完成检查的次数
        /// </summary>
        private static int _checkCount = 0;
        #endregion

        #region Properties
        /// <summary>
        /// 航班的信息
        /// </summary>
        public List<tnx.collectservice.AirTicketCollectData> AirTickets
        {
            get
            {
                return _airTickets;
            }
        }
        #endregion

        #region Methods
        public TaobaoSourceModel(WebBrowser webBrowser)
        {
            _webBrowser = webBrowser;
        }

        public static void CheckDocumentCompleted()
        {
            if (!_isLoadCompleted && _webBrowser != null && _webBrowser.ReadyState == WebBrowserReadyState.Complete && _webBrowser.IsBusy == false)
            {

                HtmlDocument doc = _webBrowser.Document;

                int flag = CheckCompleted(doc);
                if (flag == 100 || flag == 110)
                {
                    _currentHtmlDocument = doc;
                    _isLoadCompleted = true;
                }
            }
        }

        public override void Spider()
        {
            foreach (var key in UrlDic.Keys)
            {
                _currentKey = key;
                string url = UrlDic[key];

                _isLoadCompleted = false;
                _webBrowser.Navigate(UrlDic[key]);

                while (!_isLoadCompleted)
                {
                    Thread.Sleep(1000);
                }

                string codeS = key.Substring(0, 3);
                string codeE = key.Substring(3, 3);
                string date = key.Substring(6).Split('|')[0];

                GetAirTickets(_currentHtmlDocument, codeS, codeE, date);

                base.Spider();
            }
        }

        private static int CheckCompleted(HtmlDocument doc)
        {
            int flat = 101;

            try
            {
                List<tnx.collectservice.AirTicketCollectData> list = new List<tnx.collectservice.AirTicketCollectData>();

                foreach (HtmlElement div in doc.Body.GetElementsByTagName("div"))
                {
                    if (div.GetAttribute("className").Equals("inner"))
                    {
                        if (div.InnerHtml != null && div.InnerHtml.Length > 100)
                        {
                            flat = 100;
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "页面解析出错：" + _currentKey + "\r\n" + ex.ToString();
            }

            return flat;
        }

        private void GetAirTickets(HtmlDocument doc, string codeS, string codeE, string date)
        {
            try
            {
                foreach (HtmlElement div in doc.Body.GetElementsByTagName("div"))
                {
                    if (div.GetAttribute("className").Equals("inner"))
                    {
                        if (div.InnerHtml != null && div.InnerHtml.Length > 100)
                        {
                            _airTickets = new List<tnx.collectservice.AirTicketCollectData>();

                            foreach (HtmlElement divPlane in div.Children)
                            {
                                if (divPlane.GetAttribute("className").Equals("item clearfix J_onePlane"))
                                {
                                    string flightNO = RegexHelper.GetValue(divPlane.InnerHtml, "<STRONG>", "</STRONG>").Trim();
                                    string carrier = flightNO.Substring(0, 2);
                                    decimal price = Convert.ToDecimal(RegexHelper.GetValue(divPlane.InnerHtml, "J_cPrice>", "</EM>").Trim());
                                    decimal agio = Convert.ToDecimal(RegexHelper.GetValue(divPlane.InnerHtml, "discount-num\">", "</SPAN>").Replace("折", "").Trim());
                                    int nums = 10;
                                    if (divPlane.InnerHtml.IndexOf("剩") > -1)
                                        nums = int.Parse(RegexHelper.GetValue(divPlane.InnerHtml, "剩", "张").Trim());

                                    _airTickets.Add(new tnx.collectservice.AirTicketCollectData
                                    {
                                        Discount = agio,
                                        DepartAirport = codeS,
                                        ArriveAirport = codeE,
                                        Carrier = carrier,
                                        DepartDate = Convert.ToDateTime(date),
                                        FlightNO = flightNO,
                                        Seatings = nums,
                                        SettlementPrice = price
                                    });
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string airline = codeS + codeE;
                string msg = "页面解析出错：" + airline + "_" + date + "\r\n" + ex.ToString();
                Base.WriteLog(ex.ToString() + "\r\n" + doc.Body.InnerHtml, Base.Net.TaoBao.ToString(), airline + "_" + date + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"), true, false, Base.Net.TaoBao + "_GetAirPriceInfoTaoBao");
            }
        }
        #endregion
    }
}
