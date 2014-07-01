using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PageLoader
{
	public class PageUrlManager
	{
		private static int _urlCount = 0;
		private static int _completedCount = 0;
		private static Queue<string> _queenUrl = new Queue<string>();
		private static IList<PageUrl> _lstDeadPageUrl = new List<PageUrl>();
		private static object lockObj = new object();

		public static void Clear()
		{
			_queenUrl = new Queue<string>();
			_lstDeadPageUrl = new List<PageUrl>();
		}

		public static void PageUrLCompleted(PageUrl url)
		{
			// TODO 锁
			if (url.State == State.Init)
			{
				_lstDeadPageUrl.Add(url);
			}

			url.State = State.Completed;
			_completedCount++;
		}

		public static void Enqueue(string url)
		{
			// TODO 锁
			Enqueue(url, true);
		}

		public static void Enqueue(string url, bool isRepeat)
		{
			lock (lockObj)
			{
				bool isEnqueue = true;

				if (!isRepeat && _queenUrl.Contains(url))
				{
					isEnqueue = false;
				}

				if (isEnqueue)
				{
					_queenUrl.Enqueue(url);
					_urlCount++;
				}
			}
		}

		private static string Dequeue()
		{
			// TODO 锁
			string url = _queenUrl.Dequeue();
			return url;
		}

		public static int WaitCompleted()
		{
			while (true)
			{
				if (_urlCount == _completedCount)
				{
					break;
				}
			}

			return _completedCount;
		}

		public static PageUrl GetPageUrl()
		{
			if (_queenUrl.Count == 0)
			{
				return null;
			}

			PageUrl url = _lstDeadPageUrl.FirstOrDefault(b => b.State == State.Completed);

			if (url == null)
			{
				url = new PageUrl() { State = State.Init };
			}
			else
			{
				url.State = State.Open;
			}

			url.Url = _queenUrl.Dequeue();

			return url;
		}
	}
}
