using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;


namespace CommonHelper
{
	//Author	: lijy
	//date		: 2010-11-15
	
	public static partial class HttpHelper
	{
        public static HttpWebResponse HttpWebRequest(string _url, string content, string cookie, string referer)
        {
            return HttpWebRequest(_url, content, cookie, referer, null, null, null, null, null);
        }

		public static HttpWebResponse HttpWebRequest(string _url, string content, string cookie, string referer, WebProxy proxy, IDictionary<string, string> dctExtendHeads)
		{
            return HttpWebRequest(_url, content, cookie, referer, null, null, null, proxy, dctExtendHeads);
		}
		
		public static HttpWebResponse HttpWebRequest(string _url, string content, string cookie, string referer, string username, string password)
		{
            return HttpWebRequest(_url, content, cookie, referer, username, password, null, null, null);
		}

		/// <summary>
		/// 请求指定网址
		/// </summary>
		/// <param name="_url">网址</param>
		/// <param name="content">post的数据 null的话则使用get方式请求</param>
		/// <param name="cookie">set-cookie的数据</param>
		/// <param name="referer">来源网址</param>
		/// <param name="username">http用户名</param>
		/// <param name="password">http密码</param>
		/// <returns>web返回对象</returns>
		public static HttpWebResponse HttpWebRequest(string _url, string content, string cookie, string referer, string username, string password, IHttpHeaders heads, WebProxy proxy, IDictionary<string, string> dctExtendHeads)
		{
			HttpWebResponse response = null;
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);

				if ((cookie ?? string.Empty).Length > 0)
				{
					request.CookieContainer = new CookieContainer();
					request.CookieContainer.SetCookies(new Uri(_url), cookie);
				}

				if ((username ?? string.Empty).Length > 0)
				{
					request.Credentials = new System.Net.NetworkCredential(username, password ?? string.Empty);
				}

				if ((referer ?? string.Empty).Length > 0)
				{
					request.Referer = referer;
				}

                if (proxy != null)
                {
                    request.Proxy = proxy;
                }

				if(heads != null)
				{
					if(heads.Accept != null)
					{
						request.Accept = heads.Accept;
					}
					if(heads.AcceptCharset != null)
					{
						request.Headers.Add(HttpRequestHeader.AcceptCharset, heads.AcceptCharset);
					}
					if(heads.AcceptLanguage != null)
					{
						request.Headers.Add(HttpRequestHeader.AcceptLanguage, heads.AcceptLanguage);
					}
					if(heads.UserAgent != null)
					{
						request.UserAgent = heads.UserAgent;
					}
				}

                if (dctExtendHeads != null && dctExtendHeads.Count > 0)
                {
                    foreach (KeyValuePair<string, string> item in dctExtendHeads)
                    {
                        request.Headers.Add(item.Key, item.Value);
                        //request.Headers[item.Key] = item.Value;
                    }
                }

				if ((content ?? string.Empty).Length > 0)
				{
					request.Method = "POST";
                    request.Timeout = 60000;
					request.ContentType = "application/x-www-form-urlencoded";
					byte[] bytes = Encoding.UTF8.GetBytes(content);
					request.ContentLength = bytes.Length;

					Stream stream = request.GetRequestStream();
					stream.Write(bytes, 0, bytes.Length);
					stream.Close();
				}

				response = request.GetResponse() as HttpWebResponse;

			}
			catch (Exception ex)
			{
			}
			finally
			{
			}

			return response;
		}


		public static HttpWebResponse HttpWebRequestPipelining(string _url, string content, string cookie, string referer, string username, string password, IHttpHeaders heads)
		{
			HttpWebResponse response = null;
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
				request.Pipelined = true;
				request.KeepAlive = true;

				if ((cookie ?? string.Empty).Length > 0)
				{
					request.CookieContainer = new CookieContainer();
					request.CookieContainer.SetCookies(new Uri(_url), cookie);
				}

				if ((username ?? string.Empty).Length > 0)
				{
					request.Credentials = new System.Net.NetworkCredential(username, password ?? string.Empty);
				}

				if ((referer ?? string.Empty).Length > 0)
				{
					request.Referer = referer;
				}

				if (heads != null)
				{
					if (heads.Accept != null)
					{
						request.Accept = heads.Accept;
					}
					if (heads.AcceptCharset != null)
					{
						request.Headers.Add(HttpRequestHeader.AcceptCharset, heads.AcceptCharset);
					}
					if (heads.AcceptLanguage != null)
					{
						request.Headers.Add(HttpRequestHeader.AcceptLanguage, heads.AcceptLanguage);
					}
					if (heads.UserAgent != null)
					{
						request.UserAgent = heads.UserAgent;
					}
				}

				if ((content ?? string.Empty).Length > 0)
				{
					request.Method = "POST";
					request.Timeout = 2000;
					request.ContentType = "application/x-www-form-urlencoded";
					byte[] bytes = Encoding.UTF8.GetBytes(content);
					request.ContentLength = bytes.Length;

					Stream stream = request.GetRequestStream();
					stream.Write(bytes, 0, bytes.Length);
					stream.Close();
				}



				response = request.GetResponse() as HttpWebResponse;

			}
			catch (Exception ex)
			{
			}
			finally
			{
			}

			return response;
		}


		public static XmlDocument GetResponseXML(string _url, string content, string cookie, string referer)
		{
			XmlDocument rv = new XmlDocument();
			HttpWebResponse response = HttpWebRequest(_url, content, cookie, referer);
			if (response == null) return rv;
			try
			{
				using (Stream stream = response.GetResponseStream())
				{
					rv = new XmlDocument();
					rv.Load(stream);
				}
			}
			catch (Exception ex)
			{
				rv = null;
			}
			finally
			{
				if (response!= null)
				{
					response.Close();
				}
			}
			return rv;
		}

		#region 获取返回的文本
		public static string GetResponse(string _url)
		{
			return GetResponse(_url, null, null, null, Encoding.UTF8);
		}
		
		public static string GetResponse(string _url, Encoding encode)
		{
			return GetResponse(_url, null, null, null, encode);
		}

		public static string GetResponse(string _url, string content, string cookie, string referer)
		{
			return GetResponse(_url, content, cookie, referer, null);
		}

		public static string GetResponse(string _url, string content, string cookie, string referer, Encoding encode)
		{
			return GetResponse(_url, content, cookie, referer, encode, null);
		}

        public static string GetResponse(string _url, string content, string cookie, string referer, Encoding encode, IHttpHeaders heads)
        {
            return GetResponse(_url, content, cookie, referer, encode, null, null, null);
        }

        public static string GetResponse(string _url, string content, string cookie, string referer, Encoding encode, IHttpHeaders heads, WebProxy proxy, IDictionary<string, string> dctExtendHeads)
		{
			string rv = string.Empty;
			HttpWebResponse response = null;
			
			try
			{
                response = HttpWebRequest(_url, content, cookie, referer, null, null, heads, proxy, dctExtendHeads);
				if (response == null) return rv;
				
				using (Stream stream = response.GetResponseStream())
				{
					using (StreamReader sr = new StreamReader(stream, encode ?? GetEncodingByString(response.CharacterSet)))
					{
						rv = sr.ReadToEnd();
					}
				}
			}
			catch (Exception ex)
			{

			}
			finally
			{
				if (response != null)
				{
					response.Close();
				}
			}
			return rv;
		}
		
		private static Encoding GetEncodingByString(string encodingString)
		{
			encodingString = encodingString.ToLower();
			Encoding encoding = Encoding.Default;
			switch(encodingString)
			{
				case "gbk":
					encoding = Encoding.GetEncoding("GB2312");
					break;
				default:
					try{ encoding = Encoding.GetEncoding(encodingString);}catch{ encoding = Encoding.Default; }
					break;
			}
			return encoding;
		}
		#endregion
		
		
		/// <summary>
		/// 向指定网址发送请求
		/// </summary>
		/// <param name="_url">地址</param>
		/// <param name="content">post的正文（为null则使用Get方式请求网址）</param>
		/// <param name="cookie">set-cookie的数据</param>
		/// <returns>是否成功</returns>
		public static bool HttpWebRequest(string _url, string content, string cookie)
		{
			bool reutrnValue = false;
			HttpWebResponse response = null;
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);

				if ((cookie ?? string.Empty).Length > 0)
				{
					request.CookieContainer = new CookieContainer();
					request.CookieContainer.SetCookies(new Uri(_url), cookie);
				}

				if ((content ?? string.Empty).Length > 0)
				{
					request.Method = "POST";
					request.ContentType = "application/x-www-form-urlencoded";
					byte[] bytes = Encoding.UTF8.GetBytes(content);
					request.ContentLength = bytes.Length;

					Stream stream = request.GetRequestStream();
					stream.Write(bytes, 0, bytes.Length);
					stream.Close();
				}


				response = request.GetResponse() as HttpWebResponse;

				reutrnValue = response.StatusCode == HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				reutrnValue = false;
			}
			finally
			{
				if(response != null)
				{
					response.Close();
				}
			}

			return reutrnValue;
		}

		
		/// <summary>
		/// 获取当前请求的IP地址
		/// </summary>
		public static string RequestIPAddress
		{
			get
			{
				IPAddress ipa;
				string ipaddress = null;
				if (IPAddress.TryParse(HttpContext.Current.Request.Headers["X-Real-IP"] ?? HttpContext.Current.Request.UserHostAddress, out ipa))
				{
					if(ipa.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					{
						ipaddress = ipa.ToString();
					}
				}
				return ipaddress;
			}
		}

	}
	
	public class FirefoxHttpHeader : IHttpHeaders
	{
		
		#region IHttpHeaders 成员

		public string  UserAgent
		{
			get { return "Mozilla/5.0 (Windows; U; Windows NT 5.2; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 QQDownload/1.7"; }
		}

		public string  Accept
		{
			get { return "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"; }
		}

		public string  AcceptLanguage
		{
			get { return "zh-cn,zh;q=0.5"; }
		}

		public string  AcceptCharset
		{
			get { return "GB2312,utf-8;q=0.7,*;q=0.7"; }
		}

		#endregion
}


	public interface IHttpHeaders
	{
		string UserAgent { get; }
		string Accept { get; }
		string AcceptLanguage { get; }
		string AcceptCharset { get; }
	}
}
