using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PageLoader;
using System.Text.RegularExpressions;
using System.Net;
using SpiderLib.Meituan;
using Newtonsoft.Json;
using BLL;
using Model;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SpiderLib
{
    public class MeituanSourceModel : SourceModelBase
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(string lpszUrlName, string lbszCookieName, StringBuilder lpszCookieData, ref int lpdwSize);

        [DllImport("kernel32.dll")]
        public static extern Int32 GetLastError();

        private static readonly bool _isStore = true;

        private string[] _categories = { "自助餐", "美食", "电影", "休闲娱乐", "丽人", "生活服务", "酒店", "旅游", "购物", "抽奖" };

        private string[] _regions = { "越秀区", "天河区", "番禺区", "海珠区", "白云区", "荔湾区", "黄埔区", "萝岗区", "增城市", "花都区", "从化市", "南沙区" };

        private string[] _railways = { "1号线", "2号线", "3号线", "3号线支线", "4号线", "5号线", "6号线", "8号线", "APM线", "广佛线" };

        private static readonly string _domainFormat = "http://{0}.meituan.com";

        private static readonly string _deallistLinkFormat = "http://{0}.meituan.com/index/deallist";

        private static readonly string _poilistLinkFromat = "http://www.meituan.com/deal/poilist/{0}";
        private static readonly string _urlTemplate = "http://{0}.meituan.com/category/{1}/{2}";

        private static readonly string[] _arrCookieKey = new string[4] { "uuid", "SID", "ci", "abt" };

        /// <summary>
        /// 所有筛选条件的正则表达式
        /// </summary>
        private static string _regexFilter = @"(?is)<a[^>]*?href=(['""]?)(?<url>[^'""\s>]+)\1[^>]*>(?<text>(\w+))<span>(?<count>(\d+))</span></a>";

        /// <summary>
        /// 所有市区县的正则表达式
        /// </summary>
        private static string _regexRegion = @"(?is)<a[^>]*?href=(['""]?)(?<url>[^'""\s>]+/category/all/(?<key>\w+))\1[^>]*>(?<text>([\w]+[市区县沟]))<span>(?<count>(\d+))</span></a>";

        /// <summary>
        /// 所有分类的正则表达式
        /// </summary>
        private static string _regexCategoty = @"(?is)<a[^>]*?href=(['""]?)(?<url>[^'""\s>]+/category/(?<key>\w+))\1[^>]*>(?<text>([\w]+))<span>(?<count>(\d+))</span></a>";

        /// <summary>
        /// 商圈的正则表达式
        /// </summary>
        private static string _regexBusinessArea = @"(?is)<a[^>]*?href=(['""]?)[^'""\s>]+/category/all/(?<key>\w+)\1[^>]*>(?<text>([\w/]+))<span>(?<count>(\d+))</span></a>";

        /// <summary>
        /// 分类的子类的正则表达式
        /// </summary>
        private static string _regexSubCategory = @"(?is)<a[^>]*?href=(['""]?)[^'""\s>]+/category/(?<key>\w+)\1[^>]*>(?<text>([\w]+))<span>(?<count>(\d+))</span></a>";

        /// <summary>
        /// 懒加载数据（通过模拟加载数据）正则表达式
        /// </summary>
        private static string _regexAsynLoadParams = @"(?is)(['""]?)data\1:{\1params\1:(?<mteventParams>{\1mteventParams\1:{[^}]+}}),\s*\1geotype\1:(?<geotype>\d+),\s*\1areaid\1:(?<areaid>\1*\w+\1*).*\1asyncPageviewData\1:{\1acms\1:\[(?<acms>[\w"",]+)\],\s*\1deals\1:\1(?<dealids>[\d,]+)\1";

        /// <summary>
        /// 返回的列表的产品数据的正则表达式
        /// </summary>
        private static string _regexDeal = "(?is)<div class=\\\\\"deal-tile.*?<a href=\\\\\"(?<url>[^'\"\\s>]+deal\\\\/(?<dealid>\\d+).html?[^'\"\\s>]+)\\\\\".*?class=\\\\\"xtitle\\\\\">(?<xtitle>[\\s\\\\/\\w+]+).*?<\\\\/div><\\\\/div>";
        //private static string _regexProduct = "(?is)<div class=\\\\\"deal-tile.*?class=\\\\\"xtitle\\\\\">(?<xtitle>[\\s\\\\/\\w+]+).*?<\\\\/div><\\\\/div>";

        public string[] Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
            }
        }

        public string[] Regions
        {
            get
            {
                return _regions;
            }
            set
            {
                _regions = value;
            }
        }

        private IDictionary<string, IList<TuangouUrlModel>> _dctUrls;
        /// <summary>
        /// 请求的地址
        /// </summary>
        public IDictionary<string, IList<TuangouUrlModel>> DctUrls
        {
            get
            {
                return _dctUrls;
            }
            set
            {
                _dctUrls = value;
            }
        }


        private IList<TuangouUrlModel> _urls;
        /// <summary>
        /// 进行爬的地址
        /// </summary>
        public IList<TuangouUrlModel> Urls
        {
            get
            {
                return _urls;
            }
            set
            {
                _urls = value;
            }
        }

        private Dictionary<string, List<TuangouUrlModel>> _dctDealUrls;
        /// <summary>
        /// 交易的产品地址
        /// </summary>
        public Dictionary<string, List<TuangouUrlModel>> DctDealUrls
        {
            get
            {
                return _dctDealUrls;
            }
            set
            {
                _dctDealUrls = value;
            }
        }

        private IList<TuangouBasicDataModel> _basicDatas;
        /// <summary>
        /// 基础信息
        /// </summary>
        public IList<TuangouBasicDataModel> BasicDatas
        {
            get
            {
                return _basicDatas;
            }
            set
            {
                _basicDatas = value;
            }
        }

        private CookieContainer _cookieContainer;
        public CookieContainer CookieContainer
        {
            get
            {
                return _cookieContainer;
            }
            set
            {
                _cookieContainer = value;
            }
        }

        //private string _cookieString = "uuid=0545b76a232bb03d916e.1401257199.2.0.1,ci=20;abt=1404870606.1405526400%7CBCE;rvct=20%2C10,__mta=219569062.1403575087122.1404908982309.1404921856363.235,__utma=211559370.2068030844.1403575087.1404905929.1404921856.32,__utmz=211559370.1403575087.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none),__utmv=211559370.|1=city=gz=1^2=usertype=3=1^3=dealtype=319=1^5=cate=meishi=1,rvd=25413813%2C25408685%2C8173880%2C25274919%2C11598389,rus=1,lun=13570244952;lsu=13570244952;ufbanner=4;ttgr=288958;SID=6o7808unkmlfutqgntrfi5p4n5,__utmb=211559370.2.9.1404921862236,__utmc=211559370";
        private string _cookieString = "uuid=9f28e7cc52db1a855f5c.1404925723.0.0.0,ci=20,abt=1404922692.1405526400%7CBDE,rvct=20%2C10,__mta=219569062.1403575087122.1404923085988.1404923323282.239,__utma=211559370.2068030844.1403575087.1404905929.1404921856.32,__utmz=211559370.1403575087.1.1.utmcsr,__utmv=211559370.|1,rvd=25413813%2C25408685%2C8173880%2C25274919%2C11598389,rus=1,lun=13570244952,lsu=13570244952,ufbanner=4";
        //private string _cookieString = "uuid=9f28e7cc52db1a855f5c.1404925723.0.0.0, SID=2lhvt8o0egjvfqnlpafprm43o6, ci=20, abt=1404925723.1405526400%7CADE";
        public string CookieString
        {
            get
            {
                if (string.IsNullOrEmpty(_cookieString))
                {
                    CookieCollection cookies = _cookieContainer.GetCookies(new Uri("http://www.meituan.com"));
                    foreach (Cookie item in cookies)
                    {
                        _cookieString += item.ToString() + ";";
                    }

                    _cookieString = _cookieString.Substring(0, _cookieString.Length - 1);
                }

                return _cookieString;
            }
            set
            {
                _cookieString = value;
            }
        }

        private List<string> ShopIds = new List<string>();

        public override void Spider()
        {
            Init();

            _dctDealUrls = new Dictionary<string, List<TuangouUrlModel>>();

            foreach (string key in _dctUrls.Keys)
            {
                foreach (TuangouUrlModel urlModel in _dctUrls[key])
                {
                    AnalyzeShopInformation(key, urlModel);
                }
            }

            base.Spider();
        }
        
        private void AnalyzeShopInformation(string city, TuangouUrlModel indexUrlModel)
        {
            bool isFinish = false;

            List<TuangouUrlModel> lstDeal = new List<TuangouUrlModel>();

            Loader loader = new Loader();
            Parser parser = new Parser();

            IDictionary<string, string> dctXRequestWith = new Dictionary<string, string>();
            dctXRequestWith.Add("X-Requested-With", "XMLHttpRequest");

            int index = 0;
            while (!isFinish)
            {
                index++;
                TuangouUrlModel urlModel = new TuangouUrlModel() 
                { 
                    Region = indexUrlModel.Region, 
                    Category = indexUrlModel.Category,
                    Url = new PageUrl(string.Format(indexUrlModel.Url.Url + "/page{0}", index))
                };

                ResultStatus status = loader.ReadStream(urlModel.Url, string.Empty, _proxy, _cookieContainer);

                //_cookieContainer.GetCookies(

                if (!status.Success)
                {
                    //IterateProxy(urlModel.Url);
                    ChangeCookie(urlModel.Url);
                }

                PostParams postParams = parser.ParseContent(urlModel.Url, _regexAsynLoadParams, GetPostParams);

                //如果为空，则直接读取页面的数据
                if (postParams == null)
                {
                    isFinish = true;
                }
                else
                {
                    string pageContent = CommonHelper.HttpHelper.GetResponse(string.Format(_deallistLinkFormat, city), postParams.ToString(), CookieString, string.Empty, null, null, _proxy, dctXRequestWith);

                    // 交易的产品信息
                    IList<TuangouUrlModel> deals = parser.ParseContent(pageContent, _regexDeal, GetDealUrl);

                    if (deals != null && deals.Count > 0)
                    {
                        ((List<TuangouUrlModel>)deals).ForEach(b => { b.Category = urlModel.Category; b.Region = urlModel.Region; });
                        lstDeal.AddRange(deals);
                    }

                    // 获取商家的信息
                    foreach (TuangouUrlModel item in deals)
                    {
                        InsertShopInformation(item);
                    }

                }
            }

            if (_dctDealUrls.ContainsKey(city))
            {
                _dctDealUrls[city].AddRange(lstDeal);
            }
            else
            {
                _dctDealUrls.Add(city, lstDeal);
            }
        }
        /// <summary>
        /// 初始化所有基础数据
        /// </summary>
        private void Init()
        {
            //string uuidCookie = string.Empty;

            //int size = 1000;
            //StringBuilder cookie = new StringBuilder(size);
            //if (InternetGetCookie("http://www.meituan.com", "uuid", cookie, ref size))
            //{
            //    uuidCookie = cookie.ToString();
            //    uuidCookie = uuidCookie.Split('=')[1];
            //}
            //else
            //{
            //    string error = GetLastError().ToString();
            //}

            //_cookieContainer.Add(new Cookie("uuid", uuidCookie, string.Empty, ".meituan.com"));
            //_cookieContainer.Add(new Uri("http://www.meituan.com"), new Cookie("uuid", uuidCookie));

      
            _basicDatas = new List<TuangouBasicDataModel>();

            Parser parser = new Parser();
            Loader loader = new Loader();
            PageUrl url = new PageUrl();

            // 获取所有分类的信息
            foreach (var item in UrlDic)
            {
                _cookieContainer = new System.Net.CookieContainer();
                _cookieContainer.SetCookies(new Uri(item.Value), _cookieString);

                TuangouBasicDataModel basicData = new TuangouBasicDataModel() { City = item.Key };

                url.Url = item.Value;
                ResultStatus status = loader.ReadStream(url, string.Empty, _proxy, _cookieContainer);

                if (!status.Success)
                {
                    //IterateProxy(url);
                    ChangeCookie(url);
                }

                // 加载分类大类
                IList<TuangouUrlModel> lstCategoryUrls = parser.ParseContent(url, _regexCategoty, true, GetCategoryUrl);
                if (lstCategoryUrls != null && lstCategoryUrls.Count > 0)
                {
                    basicData.DctCategories = new Dictionary<string, IList<string>>();
                    CategoryBLL categoryBll = new CategoryBLL();
                    foreach (TuangouUrlModel category in lstCategoryUrls)
                    {
                        if (_isStore)
                            categoryBll.Insert(new CategoryModel() { Name = category.Text, Code = category.Category });

                        if (loader.ReadStream(category.Url, _proxy, _cookieContainer))
                        {
                            IList<TuangouUrlModel> lstUrls = parser.ParseContent(category.Url, _regexSubCategory, false, GetCategoryUrl);

                            // 数据入库
                            foreach (TuangouUrlModel urlModel in lstUrls)
                            {
                                CategoryModel model = new CategoryModel() { Name = urlModel.Text, Code = urlModel.Category, ParentCategory = category.Category };
                               
                                if (_isStore)
                                    categoryBll.Insert(model);
                            }

                            basicData.DctCategories.Add(category.Category, lstUrls.Select(b => b.Category).ToList());
                        }
                    }
                }

                // 加载区域
                IList<TuangouUrlModel> lstRegionUrls = parser.ParseContent(url, _regexRegion, false, GetRegionUrl);
                if (lstRegionUrls != null && lstRegionUrls.Count > 0)
                {
                    basicData.DctRegions = new Dictionary<string, IList<string>>();
                    RegionBLL regionBll = new RegionBLL();
                    foreach (TuangouUrlModel region in lstRegionUrls)
                    {
                        if (_isStore)
                            regionBll.Insert(new RegionModel() { Name = region.Text, Code = region.Region });

                        if (loader.ReadStream(region.Url, _proxy, _cookieContainer))
                        {
                            IList<TuangouUrlModel> lstUrls = parser.ParseContent(region.Url, _regexBusinessArea, false, GetRegionUrl);

                            // 数据入库
                            foreach (TuangouUrlModel urlModel in lstUrls)
                            {
                                RegionModel model = new RegionModel() { Name = urlModel.Text, Code = urlModel.Region, ParentRegion = region.Region };

                                if (_isStore)
                                    regionBll.Insert(model);
                            }

                            basicData.DctRegions.Add(region.Region, lstUrls.Select(b => b.Region).ToList());
                        }
                    }
                }

                _basicDatas.Add(basicData);

            }

            InitUrls();
        }

        /// <summary>
        /// 初始化部门商圈需要采集的Url
        /// </summary>
        private void InitPartUrls()
        {
            //http://gz.meituan.com/category/xican/tianhequ
            //string urlTemplate = "http://gz.meituan.com/category/{0}/{1}";
            _urls = new List<TuangouUrlModel>();
            _dctUrls = new Dictionary<string, IList<TuangouUrlModel>>();
            InitUrlsBLL bll = new InitUrlsBLL();

            foreach (TuangouBasicDataModel item in _basicDatas)
            {
                if (item != null && item.DctCategories != null && item.DctRegions != null)
                {
                    IList<TuangouUrlModel> urls = new List<TuangouUrlModel>();
                    foreach (string categoryKey in item.DctCategories.Keys)
                    {
                        foreach (string category in item.DctCategories[categoryKey])
                        {
                            foreach (string regionKey in item.DctRegions.Keys)
                            {
                                foreach (string region in item.DctRegions[regionKey])
                                {
                                    TuangouUrlModel urlModel = new TuangouUrlModel()
                                    {
                                        Category = categoryKey,
                                        Region = regionKey,
                                        Url = new PageUrl() { Url = string.Format(_urlTemplate, item.City, category, region) }
                                    };

                                    InitUrlsModel model = new InitUrlsModel()
                                    {
                                        CategoryCode = urlModel.Category,
                                        RegionCode = urlModel.Region,
                                        Url = urlModel.Url.Url
                                    };

                                    if (_isStore)
                                        bll.Insert(model);

                                    urls.Add(urlModel);
                                    _urls.Add(urlModel);
                                }
                            }
                        }
                    }
                    _dctUrls.Add(item.City, urls);
                }
            }
        }

        /// <summary>
        /// 初始化所有需要采集的Url
        /// </summary>
        private void InitUrls()
        {
            //http://gz.meituan.com/category/xican/tianhequ
            //string urlTemplate = "http://{0}.meituan.com/category/{1}/{2}";
            _urls = new List<TuangouUrlModel>();
            _dctUrls = new Dictionary<string, IList<TuangouUrlModel>>();
            InitUrlsBLL bll = new InitUrlsBLL();

            foreach (TuangouBasicDataModel item in _basicDatas)
            {
                if (item != null && item.DctCategories != null && item.DctRegions != null)
                {
                    IList<TuangouUrlModel> urls = new List<TuangouUrlModel>();
                    foreach (string categoryKey in item.DctCategories.Keys)
                    {
                        foreach (string regionKey in item.DctRegions.Keys)
                        {
                            TuangouUrlModel urlModel = new TuangouUrlModel()
                            {
                                Category = categoryKey,
                                Region = regionKey,
                                Url = new PageUrl() { Url = string.Format(_urlTemplate, item.City, categoryKey, regionKey) }
                            };

                            InitUrlsModel model = new InitUrlsModel()
                            {
                                CategoryCode = urlModel.Category,
                                RegionCode = urlModel.Region,
                                Url = urlModel.Url.Url
                            };

                            if (_isStore)
                                bll.Insert(model);

                            urls.Add(urlModel);
                            _urls.Add(urlModel);
                        }

                    }
                    _dctUrls.Add(item.City, urls);
                }
            }
        }

        /// <summary>
        /// 遍历代理列表，寻找可用的代理地址
        /// </summary>
        /// <param name="urlModel"></param>
        private void IterateProxy(PageUrl pageUrl)
        {
            Loader loader = new Loader();

            IList<string> invailableUri = new List<string>();

            if (ProxyUris != null && ProxyUris.Count > 0)
            {
                foreach (string uri in ProxyUris)
                {
                    _proxy = new WebProxy(new Uri("http://" + uri));

                    ResultStatus status = loader.ReadStream(pageUrl, string.Empty, _proxy, _cookieContainer);
                    if (status.Success)
                    {
                        break;
                    }
                    else
                    {
                        invailableUri.Add(uri);
                    }
                }
            }

            foreach (string uri in invailableUri)
            {
                ProxyUris.Remove(uri);
            }
        }

        /// <summary>
        /// 修改当前网站的cookie值
        /// </summary>
        /// <param name="pageUrl"></param>
        private void ChangeCookie(PageUrl pageUrl)
        {
            //Process.Start(pageUrl.Url);

            Loader loader = new Loader();

            Dictionary<string, string> dctCookie = CommonHelper.FullWebBrowserCookie.GetCookieList(new Uri("http://gz.meituan.com"), false);

            string cookies = CommonHelper.FullWebBrowserCookie.GetCookieInternal(new Uri("http://gz.meituan.com"), false);

            cookies = cookies.Replace(';', ',');

            _cookieString = CommonHelper.HttpHelper.HttpWebRequestGetCookies("http://www.meituan.com/multiact/default//", "1=%7B%22act%22%3A%22%2Findex%2Fvipbubble%22%7D&2=%7B%22act%22%3A%22%2Findex%2Fuserinfo%22%7D&3=%7B%22act%22%3A%22%2Findex%2Fmessage%22%7D&4=%7B%22act%22%3A%22%2Findex%2Frvd%22%7D&5=%7B%22act%22%3A%22%2Findex%2Fnavcart%22%7D&6=%7B%22isshowshops%22%3Atrue%2C%22isshopspage%22%3Afalse%2C%22act%22%3A%22%2Findex%2Fhotqueries%22%7D", cookies, string.Empty, string.Empty, string.Empty, new CommonHelper.FirefoxHttpHeader(), _proxy, null);

            //int size = 1000;
            //StringBuilder sbcookie = new StringBuilder(size);
            //if (InternetGetCookie(pageUrl.Url, "ci", sbcookie, ref size))
            //{
            //    string uuidCookie = sbcookie.ToString();
            //    uuidCookie = uuidCookie.Split('=')[1];
            //}

            //_cookieString = string.Empty;

            //foreach (KeyValuePair<string,string> cookie in dctCookie)
            //{
            //    _cookieString += string.Format("{0}={1},", cookie.Key, cookie.Value);
            //}


            //if (!string.IsNullOrEmpty(_cookieString))
            //{
            //    _cookieString = _cookieString.Substring(0, _cookieString.Length - 1);
            //}
            //_cookieString += "SID=lkjhq4n1hvu1gakhh3ng5b32d3";

            _cookieContainer = new CookieContainer();
            _cookieContainer.SetCookies(new Uri(pageUrl.Url), _cookieString);

            ResultStatus status = loader.ReadStream(pageUrl, string.Empty, _proxy, _cookieContainer);


            
        }

        /// <summary>
        /// 存储店铺的信息
        /// </summary>
        /// <param name="model"></param>
        private void InsertShopInformation(TuangouUrlModel model)
        {
            IDictionary<string, string> dctXRequestWith = new Dictionary<string, string>();
            dctXRequestWith.Add("X-Requested-With", "XMLHttpRequest");

            string pageContent = CommonHelper.HttpHelper.GetResponse(string.Format(_poilistLinkFromat, model.Url.Name), string.Empty, CookieString, string.Empty, null, null, _proxy, dctXRequestWith);
            Dictionary<string, IList<Meituan.MTShopModel>> dctShopModel = null;

            try
            {
                dctShopModel = JsonConvert.DeserializeObject<Dictionary<string, IList<Meituan.MTShopModel>>>(pageContent);
            }
            catch (Exception ex)
            {
                //CommonHelper.LogHelper.Log.Error(ex.Message);

                return;
            }

            if (dctShopModel != null && dctShopModel.Count > 0)
            {
                ShopBLL bll = new ShopBLL();

                foreach (var key in dctShopModel.Keys)
                {
                    foreach (MTShopModel item in dctShopModel[key])
                    {
                        item.city = item.city == 0 ? Int32.Parse(key) : item.city;
                        item.category = model.Category;
                        item.region = model.Region;

                        if (_isStore)
                            bll.Insert(item);
                    }
                }
            }
        }

        /// <summary>
        /// 获取匹配正则表达式的结果
        /// </summary>
        /// <param name="matchCollection"></param>
        /// <returns></returns>
        private IList<string> GetMatchResult(MatchCollection matchCollection, string key)
        {
            IList<string> lstResult = new List<string>();

            foreach (Match item in matchCollection)
            {
                string val = item.Groups[key].Value;

                lstResult.Add(val);
            }

            return lstResult;
        }

        /// <summary>
        /// 初始化所有的类别的地址
        /// </summary>
        /// <param name="matchCollection"></param>
        /// <returns></returns>
        private IList<TuangouUrlModel> GetCategoryUrl(MatchCollection matchCollection, bool isFilter)
        {
            IList<TuangouUrlModel> lstResult = new List<TuangouUrlModel>();

            foreach (Match item in matchCollection)
            {
                string category = item.Groups["text"].Value;

                if (!isFilter || _categories.Contains(category))
                {
                    TuangouUrlModel url = new TuangouUrlModel()
                    {
                        Category = item.Groups["key"].Value,
                        Text = category,
                        //Count = Convert.ToInt32(item.Groups["count"].Value),
                        Url = new PageUrl()
                        {
                            Url = item.Groups["url"].Value,
                            Name = category
                        }

                    };

                    lstResult.Add(url);
                }

            }

            return lstResult;
        }

        /// <summary>
        /// 获取区域的链接地址
        /// </summary>
        /// <param name="matchCollection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private IList<TuangouUrlModel> GetRegionUrl(MatchCollection matchCollection, bool isFilter)
        {
            IList<TuangouUrlModel> lstResult = new List<TuangouUrlModel>();

            foreach (Match item in matchCollection)
            {
                string region = item.Groups["text"].Value;

                if (!isFilter || _regions.Contains(region))
                {
                    TuangouUrlModel url = new TuangouUrlModel()
                    {
                        Region = item.Groups["key"].Value,
                        Text = region,
                        //Count = Convert.ToInt32(item.Groups["count"].Value),
                        Url = new PageUrl()
                        {
                            Url = item.Groups["url"].Value,
                            Name = region
                        }

                    };

                    lstResult.Add(url);
                }

            }

            return lstResult;
        }

        /// <summary>
        /// 获取发送请求的参数
        /// </summary>
        /// <param name="matchCollection"></param>
        /// <returns></returns>
        private PostParams GetPostParams(MatchCollection matchCollection)
        {
            PostParams postParams = null;

            if (matchCollection != null && matchCollection.Count == 1)
            {
                postParams = new PostParams()
                {
                    mteventParams = matchCollection[0].Groups["mteventParams"].Value,
                    geotype = matchCollection[0].Groups["geotype"].Value,
                    areaid = matchCollection[0].Groups["areaid"].Value,
                    dealids = matchCollection[0].Groups["dealids"].Value,
                    acms = matchCollection[0].Groups["acms"].Value,
                    offset = "0"
                };
            }

            return postParams;
        }

        private IList<TuangouUrlModel> GetDealUrl(MatchCollection matchCollection)
        {
            IList<TuangouUrlModel> lstResult = new List<TuangouUrlModel>();

            foreach (Match item in matchCollection)
            {
                string text = item.Groups["xtitle"].Value;

                TuangouUrlModel url = new TuangouUrlModel()
                {
                    Text = text,
                    Url = new PageUrl()
                    {
                        Url = item.Groups["url"].Value.Replace("\\", ""),
                        Name = item.Groups["dealid"].Value
                    }
                };
                lstResult.Add(url);
            }

            return lstResult;
        }

        private IList<TuangouUrlModel> GetRailwayUrl(MatchCollection matchCollection, string category)
        {
            IList<TuangouUrlModel> lstResult = new List<TuangouUrlModel>();

            foreach (Match item in matchCollection)
            {
                string railway = item.Groups["text"].Value;

                if (_railways.Contains(railway))
                {
                    TuangouUrlModel url = new TuangouUrlModel()
                    {
                        Category = category,
                        Railway = railway,
                        Count = Convert.ToInt32(item.Groups["count"].Value),
                        Url = new PageUrl()
                        {
                            Url = item.Groups["url"].Value,
                            Name = railway
                        }

                    };

                    lstResult.Add(url);
                }

            }

            return lstResult;
        }

        private void PushRegionUrl()
        {

        }
    }
}
