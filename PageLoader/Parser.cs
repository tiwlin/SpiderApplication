using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PageLoader
{
    public class Parser
    {
        //public IList<string> ParseContent(PageUrl pageUrl, string pattern)
        //{
        //    return ParseContent<string>(pageUrl, pattern, GetUrlFromContent);
        //}

        /// <summary>
        /// 解析URL内容，并通过正则表达式，或许匹配的内容
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="pageUrl">需要分析的url对象</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="func">处理匹配内容的方法</param>
        /// <returns>匹配正则表达式的内容</returns>
        public IList<T> ParseContent<T>(PageUrl pageUrl, string pattern, Func<MatchCollection, IList<T>> func)
        {
            if (string.IsNullOrEmpty(pageUrl.Content))
            {
                return null;
            }

            Regex regex = new Regex(pattern);
            MatchCollection matchCollection = regex.Matches(pageUrl.Content);

            return func(matchCollection);
        }

        /// <summary>
        /// 解析URL内容，并通过正则表达式，或许匹配的内容
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="pageUrl">需要分析的url对象</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="isFilter">是否过滤</param>
        /// <param name="func">处理匹配内容的方法</param>
        /// <returns>匹配正则表达式的内容</returns>
        public IList<T> ParseContent<T>(PageUrl pageUrl, string pattern, bool isFilter, Func<MatchCollection, bool, IList<T>> func)
        {
            if (string.IsNullOrEmpty(pageUrl.Content))
            {
                return null;
            }

            Regex regex = new Regex(pattern);
            MatchCollection matchCollection = regex.Matches(pageUrl.Content);

            return func(matchCollection, isFilter);
        }

        /// <summary>
        /// 解析URL内容，并通过正则表达式，或许匹配的内容
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="pageUrl">需要分析的url对象</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="func">处理匹配内容的方法</param>
        /// <returns>匹配正则表达式的内容</returns>
        public T ParseContent<T>(PageUrl pageUrl, string pattern, Func<MatchCollection, T> func)
        {
            if (string.IsNullOrEmpty(pageUrl.Content))
            {
                return default(T);
            }

            Regex regex = new Regex(pattern);
            MatchCollection matchCollection = regex.Matches(pageUrl.Content);

            return func(matchCollection);
        }

        /// <summary>
        /// 解析URL内容，并通过正则表达式，或许匹配的内容
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="content">需要分析的内容</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="func">处理匹配内容的方法</param>
        /// <returns>匹配正则表达式的内容</returns>
        public T ParseContent<T>(string content, string pattern, Func<MatchCollection, T> func)
        {
            Regex regex = new Regex(pattern);
            MatchCollection matchCollection = regex.Matches(content);

            return func(matchCollection);
        }

        /// <summary>
        /// 获取URL地址
        /// </summary>
        /// <param name="matchCollection"></param>
        /// <returns></returns>
        private IList<string> GetUrlFromContent(MatchCollection matchCollection)
        {
            IList<string> lstResult = new List<string>();

            foreach (Match item in matchCollection)
            {
                lstResult.Add(item.Groups[0].Value);
            }

            return lstResult;
        }
    }
}
