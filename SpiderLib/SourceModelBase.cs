using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using PageLoader;

namespace SpiderLib
{
    public class SourceModelBase
    {
        private static readonly string _regexProxyUrl = @"http://www.youdaili.cn/Daili/http/\d+.html";

        private static readonly string _regexProxyUri = @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,4}";

        private string _proxySourceUrl = "http://www.youdaili.cn/Daili/http/2379.html";
        /// <summary>
        /// 代理网站的地址
        /// </summary>
        public string ProxySourceUrl
        {
            get
            {
                return _proxySourceUrl;
            }
            set
            {
                _proxySourceUrl = value;
            }
        }

        protected WebProxy _proxy = new WebProxy("172.16.1.2:8080", false) { Credentials = new NetworkCredential("frankielin@schmidtelectronics.com", "tiwlintiw520") };
        public WebProxy Proxy
        {
            get
            {
                return _proxy;
            }
            set
            {
                _proxy = value;
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The dictionary which store the Url needed to search
        /// </summary>
        public Dictionary<string, string> UrlDic { get; set; }

        /// <summary>
        /// The sum of the access times
        /// </summary>
        public int AccessCount { get; set; }

        /// <summary>
        /// The limited time
        /// </summary>
        public int MaxAccessCount { get; set; }


        private IList<string> _proxuUris;
        public IList<string> ProxyUris
        {
            get
            {
                return _proxuUris;
            }
            set
            {
                _proxuUris = value;
            }
        }

        public virtual void InitProxyUris()
        {
            string content = CommonHelper.HttpHelper.GetResponse(_proxySourceUrl, null, null, null, null, null, _proxy, null);
            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("获取代理页面内容失败！");
            }

            Regex regex = new Regex(_regexProxyUrl);
            Match match = regex.Match(content);

            if (match.Success)
            {
                string uriContent = CommonHelper.HttpHelper.GetResponse(match.Value, null, null, null, null, null, _proxy, null);

                if (string.IsNullOrEmpty(content))
                {
                    throw new Exception("获取代理页面Uri内容失败！");
                }
                 Parser parser = new Parser();

                regex = new Regex(_regexProxyUri);

                MatchCollection matchs = regex.Matches(uriContent);

                if (matchs.Count > 0)
                {
                    _proxuUris = new List<string>();
                    foreach (Match item in matchs)
                    {
                        _proxuUris.Add(item.Value);
                    }
                }
            }
        }

        public virtual void Spider()
        {
            AccessCount++;

            if (AccessCount >= MaxAccessCount)
            {
                Thread.CurrentThread.Abort();
            }
        }
    }
}
