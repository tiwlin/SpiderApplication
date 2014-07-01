using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Windows.Forms;
using System.Threading;

namespace SpiderLib
{
    public class QunarSourceModel : SourceModelBase
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
        public QunarSourceModel(WebBrowser webBrowser)
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
                HtmlElement eltDivResultPanel = doc.Body.Document.GetElementById("hdivResultPanel");
                if (eltDivResultPanel.InnerHtml.IndexOf("请稍等") <= -1)
                {
                    foreach (HtmlElement div in eltDivResultPanel.Children)
                    {
                        if (div.GetAttribute("className").Equals("avt_column"))
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
                List<tnx.collectservice.AirTicketCollectData> list = new List<tnx.collectservice.AirTicketCollectData>();
                HtmlElement eltDivResultPanel = doc.Body.Document.GetElementById("hdivResultPanel");
                if (eltDivResultPanel.InnerHtml.IndexOf("请稍等") <= -1)
                {
                    foreach (HtmlElement div in eltDivResultPanel.Children)
                    {
                        if (div.GetAttribute("className").Equals("avt_column"))
                        {
                            string html = div.InnerHtml;
                            string flightNO = RegexHelper.GetValue(html, "<STRONG>", "</STRONG>").Trim();
                            string carrier = flightNO.Substring(0, 2);
                            decimal agio = Convert.ToDecimal(RegexHelper.GetValue(html, "class=f_tha>", "</SPAN>").Trim());
                            /*
                             <EM class=prc><B style="WIDTH: 33px; LEFT: -33px">339</B><B style="LEFT: -11px">5</B><B style="LEFT: -33px">4</B></EM>
                             */
                            string flightNOShare = "";
                            string prc = "";
                            foreach (HtmlElement em in div.GetElementsByTagName("em"))
                            {
                                if (em.GetAttribute("className").Equals("prc"))
                                {
                                    int left = 0;
                                    foreach (HtmlElement eltB in em.Children)
                                    {
                                        string htmlB = eltB.OuterHtml.ToUpper();
                                        string prcB = eltB.InnerHtml;
                                        int leftB = int.Parse(RegexHelper.GetValue(htmlB, "-", "PX").Trim().Substring(0, 1));
                                        if (htmlB.IndexOf("WIDTH") > -1)//假价格
                                        {
                                            prc = prcB;
                                            left = leftB;
                                        }
                                        else//真实价格单位数
                                        {
                                            if (left == leftB)//如果是第一位
                                                prc = prcB + "" + prc.Substring(1);
                                            else
                                            {//4321
                                                //left:5 leftB:3    54321
                                                leftB = int.Parse("-" + leftB) + left;
                                                prc = prc.Substring(0, leftB) + "" + prcB + "" + prc.Substring(leftB + 1);
                                            }
                                        }
                                    }
                                }
                                else if (em.GetAttribute("className").Equals("lnk_a"))
                                    flightNOShare = RegexHelper.GetValue(em.OuterHtml, "航空", " ").Trim();
                            }
                            decimal price = Convert.ToDecimal(prc);

                            _airTickets.Add(new tnx.collectservice.AirTicketCollectData
                            {
                                Discount = agio,
                                DepartAirport = codeS,
                                ArriveAirport = codeE,
                                Carrier = carrier,
                                DepartDate = Convert.ToDateTime(date),
                                FlightNO = flightNO,
                                // Seatings = nums,
                                SettlementPrice = price
                            });

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string airline = codeS + codeE;
                string msg = "页面解析出错：" + airline + "_" + date + "\r\n" + ex.ToString();

                Base.WriteLog(ex.ToString() + "\r\n" + doc.Body.InnerHtml, Base.Net.Qunar.ToString(), airline + "_" + date + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"), true, false, Base.Net.Qunar + "_GetAirPriceInfoQunar");
            }
        }
        #endregion
    }
}
