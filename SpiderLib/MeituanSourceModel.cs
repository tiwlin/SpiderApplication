using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PageLoader;
using System.Text.RegularExpressions;
using System.Net;
using SpiderLib.Meituan;

namespace SpiderLib
{
    public class MeituanSourceModel : SourceModelBase
    {
        private string[] _categories = { "美食", "电影", "休闲娱乐", "丽人", "生活服务", "酒店", "旅游", "购物", "抽奖" };

        private string[] _regions = { "越秀区", "天河区", "番禺区", "海珠区", "白云区", "荔湾区", "黄埔区", "萝岗区", "增城市", "花都区", "从化市", "南沙区" };

        private string[] _railways = { "1号线", "2号线", "3号线", "3号线支线", "4号线", "5号线", "6号线", "8号线", "APM线", "广佛线" };

        private static readonly string _domainFormat = "http://{0}.meituan.com";

        private static readonly string _deallistLinkFormat = "http://{0}.meituan.com/index/deallist";

        /// <summary>
        /// 所有筛选条件的正则表达式
        /// </summary>
        private static string _regexFilter = @"(?is)<a[^>]*?href=(['""]?)(?<url>[^'""\s>]+)\1[^>]*>(?<text>(\w+))<span>(?<count>(\d+))</span></a>";

        /// <summary>
        /// 所有市区县的正则表达式
        /// </summary>
        private static string _regexRegion = @"(?is)<a[^>]*?href=(['""]?)(?<url>[^'""\s>]+/category/all/(?<key>\w+))\1[^>]*>(?<text>([\w]+[市区县]))<span>(?<count>(\d+))</span></a>";

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
        private static string _regexAsynLoadParams = @"(?is)(['""]?)data\1:{\1params\1:(?<mteventParams>{\1mteventParams\1:{[^}]+}}),\s*\1geotype\1:(?<geotype>\d+),\s*\1areaid\1:(?<areaid>\1*\w+\1*).*\1asyncPageviewData\1:{\1deals\1:\1(?<dealids>[\d,]+)\1";

        /// <summary>
        /// 返回的列表的产品数据的正则表达式
        /// </summary>
        private static string _regexProduct = "(?is)<div class=\\\\\"deal-tile.*?class=\\\\\"xtitle\\\\\">(?<xtitle>[\\s\\\\/\\w+]+).*?<\\\\/div><\\\\/div>";


        private WebProxy proxy = new WebProxy("172.16.1.2:8080", false) { Credentials = new NetworkCredential("frankielin@schmidtelectronics.com", "tiwlintiw520") };

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

        public override void Spider()
        {
            Loader loader = new Loader();
            Parser parser = new Parser();

            IDictionary<string, string> dctXRequestWith = new Dictionary<string, string>();
            dctXRequestWith.Add("X-Requested-With", "XMLHttpRequest");

            foreach (string key in _dctUrls.Keys)
            {
                foreach (TuangouUrlModel urlModel in _dctUrls[key])
                {
                    loader.ReadStream(urlModel.Url, proxy);
                    PostParams postParams = parser.ParseContent(urlModel.Url, _regexAsynLoadParams, GetPostParams);

                    string pageContent = CommonHelper.HttpHelper.GetResponse(string.Format(_deallistLinkFormat, key), postParams.ToString(), string.Empty, string.Empty, null, null, proxy, dctXRequestWith);

                    IList<TuangouUrlModel> lstProduct = parser.ParseContent(pageContent, _regexProduct, GetProductUrl);
                }
            }

            base.Spider();
        }

        /// <summary>
        /// 初始化所有基础数据
        /// </summary>
        private void Init()
        {
            _basicDatas = new List<TuangouBasicDataModel>();

            Parser parser = new Parser();
            Loader loader = new Loader();
            PageUrl url = new PageUrl();

            //WebProxy proxy = new WebProxy("172.16.1.2:8080", false);
            //proxy.Credentials = new NetworkCredential("frankielin@schmidtelectronics.com", "tiwlintiw520");

            //proxy = null;

            // 获取所有分类的信息
            foreach (var item in UrlDic)
            {
                TuangouBasicDataModel basicData = new TuangouBasicDataModel() { City = item.Key };

                url.Url = item.Value;
                loader.ReadStream(url, proxy);

                // 加载分类大类
                IList<TuangouUrlModel> lstCategoryUrls = parser.ParseContent(url, _regexCategoty, GetCategoryUrl);
                if (lstCategoryUrls != null && lstCategoryUrls.Count > 0)
                {
                    basicData.DctCategories = new Dictionary<string, IList<string>>();
                    foreach (TuangouUrlModel category in lstCategoryUrls)
                    {
                        loader.ReadStream(category.Url, proxy);
                        IList<string> lstUrls = parser.ParseContent(category.Url, _regexSubCategory, GetMatchResult);

                        basicData.DctCategories.Add(category.Category, lstUrls);
                    }
                }

                // 加载区域
                IList<TuangouUrlModel> lstRegionUrls = parser.ParseContent(url, _regexRegion, GetRegionUrl);
                if (lstRegionUrls != null && lstRegionUrls.Count > 0)
                {
                    basicData.DctRegions = new Dictionary<string, IList<string>>();
                    foreach (TuangouUrlModel region in lstRegionUrls)
                    {
                        loader.ReadStream(region.Url, proxy);
                        IList<string> lstUrls = parser.ParseContent(region.Url, _regexBusinessArea, GetMatchResult);

                        basicData.DctRegions.Add(region.Region, lstUrls);
                    }
                }

                _basicDatas.Add(basicData);
            }

            InitUrls();
        }

        /// <summary>
        /// 初始化所有需要采集的Url
        /// </summary>
        private void InitUrls()
        {
            //http://gz.meituan.com/category/xican/tianhequ
            string urlTemplate = "http://gz.meituan.com/category/{0}/{1}";
            _urls = new List<TuangouUrlModel>();
            _dctUrls = new Dictionary<string, IList<TuangouUrlModel>>();

            foreach (TuangouBasicDataModel item in _basicDatas)
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
                                    Url = new PageUrl() { Url = string.Format(urlTemplate, category, region) }
                                };

                                urls.Add(urlModel);
                                _urls.Add(urlModel);
                            }
                        }
                    }
                }
                _dctUrls.Add(item.City, urls);
            }
        }

        /// <summary>
        /// 获取匹配正则表达式的结果
        /// </summary>
        /// <param name="matchCollection"></param>
        /// <returns></returns>
        private IList<string> GetMatchResult(MatchCollection matchCollection)
        {
            IList<string> lstResult = new List<string>();

            foreach (Match item in matchCollection)
            {
                string key = item.Groups["key"].Value;

                lstResult.Add(key);
            }

            return lstResult;
        }

        /// <summary>
        /// 初始化所有的类别的地址
        /// </summary>
        /// <param name="matchCollection"></param>
        /// <returns></returns>
        private IList<TuangouUrlModel> GetCategoryUrl(MatchCollection matchCollection)
        {
            IList<TuangouUrlModel> lstResult = new List<TuangouUrlModel>();

            foreach (Match item in matchCollection)
            {
                string category = item.Groups["text"].Value;

                if (_categories.Contains(category))
                {
                    TuangouUrlModel url = new TuangouUrlModel()
                    {
                        Category = item.Groups["key"].Value,
                        Text = category,
                        Count = Convert.ToInt32(item.Groups["count"].Value),
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
        private IList<TuangouUrlModel> GetRegionUrl(MatchCollection matchCollection)
        {
            IList<TuangouUrlModel> lstResult = new List<TuangouUrlModel>();

            foreach (Match item in matchCollection)
            {
                string region = item.Groups["text"].Value;

                if (_regions.Contains(region))
                {
                    TuangouUrlModel url = new TuangouUrlModel()
                    {
                        Region = item.Groups["key"].Value,
                        Text = region,
                        Count = Convert.ToInt32(item.Groups["count"].Value),
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
                    offset = "0"
                };
            }

            return postParams;
        }

        private IList<TuangouUrlModel> GetProductUrl(MatchCollection matchCollection)
        {
            IList<TuangouUrlModel> lstResult = new List<TuangouUrlModel>();

            foreach (Match item in matchCollection)
            {
                string text = item.Groups["xtitle"].Value;

                TuangouUrlModel url = new TuangouUrlModel()
                {
                    Text = text
                };

                //if (_regions.Contains(region))
                //{
                //    TuangouUrlModel url = new TuangouUrlModel()
                //    {
                //        Region = item.Groups["key"].Value,
                //        Text = region,
                //        Count = Convert.ToInt32(item.Groups["count"].Value),
                //        Url = new PageUrl()
                //        {
                //            Url = item.Groups["url"].Value,
                //            Name = region
                //        }

                //    };

                //    lstResult.Add(url);
                //}

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
