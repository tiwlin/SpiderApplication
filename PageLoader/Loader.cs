using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace PageLoader
{
	public class Loader : ILoader
	{
		private PageUrl _currentPageUrl = null;

		private bool _isAsyWrite = false;
		public bool IsAsyWrite
		{
			get
			{
				return _isAsyWrite;
			}
			set
			{
				_isAsyWrite = value;
			}
		}

		private string _savaDirectory = string.Empty;
		public string SavaDirectory
		{
			get
			{
				return _savaDirectory;
			}
			set
			{
				_savaDirectory = value;
			}
		}

		#region ILoader 成员

		public void LoadPage(PageUrl url)
		{
            LoadPage(url, AsyWriteFile);

		}

        public void LoadPage(PageUrl url, Action<PageUrl> action)
        {
            _currentPageUrl = url;

            bool isReadCompleted = ReadStream(url);

            if (isReadCompleted)
            {
                action(url);
            }
        }

		/// <summary>
		/// 发起请求连接的，并获取数据
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public bool ReadStream(PageUrl url, WebProxy proxy)
		{
            return ReadStream(url, "application/x-www-form-urlencoded", proxy);
		}


        public bool ReadStream(PageUrl url)
        {
            return ReadStream(url, null);
        }

        public bool ReadStream(PageUrl url, string contentType, WebProxy proxy)
        {
            int work = 0, io = 0;
            
            ThreadPool.GetAvailableThreads(out work, out io);
            //Console.WriteLine("工作线程总数：{0}，IO线程总数：{1}，当前ULR：{2}", work, io, url.Url);
            //Console.WriteLine("读的线程ID：{0}", Thread.CurrentThread.ManagedThreadId);

            bool isReadCompleted = true;

            ///请求地址
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.Url);
            request.Timeout = 60000;

            if (!string.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }
            
            if (proxy != null)
            {
                request.Proxy = proxy;
            }

            //request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 QQDownload/1.7";

            ///发起请求
            HttpWebResponse response = null;
           
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                ///获取返回来的数据
                using (Stream st = response.GetResponseStream())
                {
                    ///创建流读取器
                    using (StreamReader sr = new StreamReader(st))
                    {
                        ///读取返回的数据
                        string content = sr.ReadToEnd();

                        url.Content = content;
                        url.ArrayData = Encoding.GetEncoding(response.CharacterSet.ToLower()).GetBytes(content);
                    }
                }
            }
            catch (Exception ex)
            {
                isReadCompleted = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return isReadCompleted;
        }

		/// <summary>
		/// 异步把请求数据写进文档
		/// </summary>
		/// <param name="url"></param>
		private void AsyWriteFile(PageUrl url)
		{
			Uri uri = new Uri(url.Url);

			CreateDirectory();

			FileStream fs = new FileStream(string.Format(@"{0}\{1}.html", _savaDirectory, uri.Segments[uri.Segments.Length -1]), FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);

			fs.BeginWrite(url.ArrayData, 0, url.ArrayData.Length, (b) => { CloseFileStream(b); }, fs);
		}

		/// <summary>
		/// 关闭文件流
		/// </summary>
		/// <param name="result"></param>
		private void CloseFileStream(IAsyncResult result)
		{
			int work = 0, io = 0;

			ThreadPool.GetAvailableThreads(out work, out io);
			//Console.WriteLine("工作线程总数：{0}，IO线程总数：{1}，当前ULR：{2}", work, io, _currentPageUrl.Url);
			//Console.WriteLine("写的线程ID：{0}", Thread.CurrentThread.ManagedThreadId);

			FileStream fs = (FileStream)result.AsyncState;

			PageUrlManager.PageUrLCompleted(this._currentPageUrl);

			fs.Close();

			Thread.Sleep(2000);
		}

		private void CreateDirectory()
		{
			if (!Directory.Exists(_savaDirectory))
			{
				Directory.CreateDirectory(_savaDirectory);
			}
		}
		#endregion
	}
}
