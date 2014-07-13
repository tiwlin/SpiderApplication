using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpiderLib;
using Common;
using System.Threading;
using System.Net;

namespace SpiderApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        public void Init()
        {
            Dictionary<string, string[]> dicAirline = new Dictionary<string, string[]>();
            //using (CollectServiceClient client = new CollectServiceClient())
            //{
            //    dicAirline = client.GetListDic(tbSystemCode.Text);
            //}
            string[] str = { "CANPEK01", "CANSHA01", "SHAPEK01", "CANPEK02", "CANSHA02", "SHAPEK02", "SZXPEK01", "SZXSHA01", "SZXPEK01" };
            //dicAirline.Add("taobao", str);
            //dicAirline.Add("qunar", str);
            //dicAirline.Add("elong", str);
            //ThreadManager.UrlList.Add(new TaobaoSourceModel(wbTaobao) { MaxAccessCount = 100, Name = "taobao", UrlDic = new Dictionary<string, string>() });
            //ThreadManager.UrlList.Add(new QunarSourceModel(wbQunar) { MaxAccessCount = 5, Name = "qunar", UrlDic = new Dictionary<string, string>() });

            MeituanSourceModel meituanModel = new MeituanSourceModel() { Name = "meituan", UrlDic = new Dictionary<string, string>() };
            meituanModel.UrlDic.Add("sh", "http://sh.meituan.com/category");
            meituanModel.UrlDic.Add("bj", "http://bj.meituan.com/category");
            //WebProxy proxy = new WebProxy(new Uri("http://58.67.143.165:63000"));
            //meituanModel.Proxy = proxy;
            meituanModel.Proxy = null;
            ThreadManager.UrlList.Add(meituanModel);


            int count = dicAirline.Count;
            new ThreadManager(count + 1, 10);
            foreach (string key in dicAirline.Keys)
            {
                Dictionary<string, string> urlDic = new Dictionary<string, string>();
                string[] collectFlags = dicAirline[key];
                int c = 0;
                foreach (string collectFlag in collectFlags)
                {
                    string codeS = "";
                    string codeE = "";
                    string date = "";
                    string codeS_ZH = "";//起飞城市中文
                    string codeE_ZH = "";//到达城市中文
                    string codeS_ZHPY = "";//起飞城市拼音
                    string codeE_ZHPY = "";//到达城市拼音
                    GetInfo(collectFlag, ref codeS, ref codeE, ref date);
                    if (!AirlineHelper.GetCityNameZHAndPY(codeS, codeE, ref codeS_ZH, ref codeE_ZH, ref codeS_ZHPY, ref codeE_ZHPY))
                        continue;
                    string url = "";
                    switch (key)
                    {
                        case "taobao":
                            if (codeS == "PEK")
                                codeS = "BJS";
                            if (codeE == "PEK")
                                codeE = "BJS";
                            string zhS_gbk = Base.ZHToGBK(codeS_ZH).ToUpper();
                            string zhE_gbk = Base.ZHToGBK(codeE_ZH).ToUpper();
                            string searchBy = "1280";
                            url = string.Format("http://s.jipiao.trip.taobao.com/flight_search_result.htm?tripType=0&depCityName={0}&depCity={1}&arrCityName={2}&arrCity={3}&tripType=0&depDate={4}&arrDate=yyyy-mm-dd&searchBy={5}", zhS_gbk, codeS, zhE_gbk, codeE, date, searchBy);
                            //ThreadManager.UrlList.Add(new TaobaoSourceModel() { Name = key, UrlDic = urlDic, MaxAccessCount = 3 });
                            break;
                        case "qunar":
                            string dateArr = Convert.ToDateTime(date).AddDays(2).ToString("yyyy-MM-dd");
                            string zhS_utf = Base.ZHToUTF8(codeS_ZH);
                            string zhE_utf = Base.ZHToUTF8(codeE_ZH);
                            url = string.Format("http://flight.qunar.com/site/oneway_list.htm?searchDepartureAirport={0}&searchArrivalAirport={1}&searchDepartureTime={2}&searchArrivalTime={3}&nextNDays=0&startSearch=true&from=qunarindex", zhS_utf, zhE_utf, date, dateArr);
                            //ThreadManager.UrlList.Add(new QunarSpiderModel() { Name = key, UrlDic = urlDic, MaxAccessCount = 3 });
                            break;
                        case "elong":
                            string strDay = collectFlag.Substring(6, 2);
                            int day = 0;
                            if (strDay.Substring(0, 1) == "0")
                                day = int.Parse(strDay.Substring(1, 1));
                            else
                                day = int.Parse(strDay);
                            url = string.Format("http://flight.elong.com/{0}-{1}/cn_day{2}.html", codeS_ZHPY, codeE_ZHPY, day);
                           //ThreadManager.UrlList.Add(new ElongSpiderModel() { Name = key, UrlDic = urlDic, MaxAccessCount = 3 });
                            break;
                        case "ctrip":
                            if (codeS == "PEK")
                                codeS = "BJS";
                            if (codeE == "PEK")
                                codeE = "BJS";
                            url = "http://flights.ctrip.com/domesticsearch/search/SearchFirstRouteFlights?DCity1=SHA&ACity1=BJS&SearchType=S&DDate1=2014-07-01&r=0.16250303971213142";
                            //ThreadManager.UrlList.Add(new CtripSpiderModel() { Name = key, UrlDic = urlDic, MaxAccessCount = 3 });
                            break;
                        case "ly":
                            //ThreadManager.UrlList.Add(new LYSpiderModel() { Name = key, UrlDic = urlDic, MaxAccessCount = 3 });
                            break;
                        default:
                            break;
                    }
                    c++;
                    ThreadManager.UrlList.First(b => b.Name == key).UrlDic.Add(codeS + codeE + date + "|" + c, url);
                }


            }
            //ThreadManager.UrlList.Add(new ItourSpiderModel() { Name = "Itour", MaxAccessCount = 2 });
            //ThreadManager.UrlList.Add(new ElongSpiderModel() { Name = "Elong", MaxAccessCount = 5 });

            Start();
        }

        private void Start()
        {
            for (int i = 0; i < ThreadManager.ThreadSize; i++)
            {
                //TManager.UrlList.Add("the url should be spide" + i);

                ThreadPool.QueueUserWorkItem(new WaitCallback((b) =>
                {
                    SourceModelBase spider = (SourceModelBase)b;

                    ThreadManager.AddThread(Thread.CurrentThread, spider);
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                    Console.WriteLine(spider.Name);

                    //for (int j = 0; j <spider.UrlList.Count; j++)
                    //{
                    //    Thread.Sleep(5000);
                    //    spider.Spider();
                    //}
                    spider.Spider();
                    //Thread.Sleep(10000); // 操作
                    //Console.WriteLine(b.ToString());
                    Console.WriteLine("Second:" + Thread.CurrentThread.ManagedThreadId);
                }),
                    ThreadManager.UrlList[i]);
            }
        }


        private void GetInfo(string collectFlag, ref string codeS, ref string codeE, ref string date)
        {
            codeS = collectFlag.Substring(0, 3);
            codeE = collectFlag.Substring(3, 3);
            string strDay = collectFlag.Substring(6, 2);
            int day = 0;
            if (strDay.Substring(0, 1) == "0")
                day = Convert.ToInt32(strDay.Substring(1, 1));
            else
                day = Convert.ToInt32(strDay);
            date = DateTime.Now.AddDays(day).ToString("yyyy-MM-dd");
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            Init();
            timerBrowser.Enabled = true;
        }

        private void timerBrowser_Tick(object sender, EventArgs e)
        {
            TaobaoSourceModel.CheckDocumentCompleted();
            QunarSourceModel.CheckDocumentCompleted();
        }
    }
}
