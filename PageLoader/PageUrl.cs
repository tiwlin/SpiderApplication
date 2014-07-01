using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageLoader
{
	public class PageUrl
	{
		private string _url = string.Empty;
		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

        public string Name { get; set; }

		private byte[] _arrayData = null;
		/// <summary>
		/// 数据字节组
		/// </summary>
        public byte[] ArrayData
		{
			get
			{
				return _arrayData;
			}
			set
			{
				_arrayData = value;
			}
		}

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

		public State State { get; set; }

		public PageUrl()
		{
			_arrayData = new byte[1024];
		}

		public PageUrl(int size)
		{
			_arrayData = new byte[size];
		}

		public PageUrl(string url)
			: this()
		{
			_url = url;
		}

		public PageUrl(string url, int size)
			: this(size)
		{
			_url = url;
		}

	}

	public enum State
	{
		Init = 0,
		Completed = 1,
		Open = 2,
		Closed = 3,
	}
}
