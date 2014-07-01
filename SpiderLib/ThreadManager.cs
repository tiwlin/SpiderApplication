using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SpiderLib
{
    public class ThreadManager
    {
        private static IDictionary<string, SpiderThread> _dctThread;

        private static object _objectLock = new object();

        public static IList<SourceModelBase> UrlList = new List<SourceModelBase>();

        public static IDictionary<string, IList<string>> DctUrls = new Dictionary<string, IList<string>>();

        public static IList<object> OjbectList = new List<object>();
        //public static IList<tnx.collectservice.AirTicketCollectData> AirDataList = new List<tnx.collectservice.AirTicketCollectData>();
        public static int ThreadSize { get; set; }

        private static bool isUploaded = true;

        private static int uploadingCount;

        private static int maxUplodingCount = 20;

        static ThreadManager()
        {
            _dctThread = new Dictionary<string, SpiderThread>();
        }


        public static IDictionary<string, SpiderThread> DctThread
        {
            get
            {
                return _dctThread;
            }
        }

        public ThreadManager(int size, int maxSize)
        {
            ThreadSize = size;
            ThreadPool.SetMaxThreads(maxSize, maxSize);
        }


        public static void AddThread(Thread thread, SourceModelBase spiderModel)
        {
            lock (_objectLock)
            {
                if (!_dctThread.ContainsKey(thread.ManagedThreadId.ToString()))
                {
                    _dctThread.Add(thread.ManagedThreadId.ToString(), new SpiderThread() { Thread = thread, SpiderModel = spiderModel });
                }
            }
        }

        public static void Upload()
        {
            //if (OjbectList != null && OjbectList.Count > 0 && uploadingCount < maxUplodingCount)
            //{
            //    lock (_objectLock)
            //    {
            //        if (OjbectList != null && OjbectList.Count > 0 && uploadingCount < maxUplodingCount)
            //        {
            //            uploadingCount++;

            //            object obj = TManager.OjbectList.First();


            //            // upload data

            //            // update status, remove the object in the list
            //            // is succeed, update isUploaded; else record;
            //            uploadingCount--;
            //        }
            //    }
            //}
        }

        public static void Upload(int count)
        {

            //if (OjbectList != null && OjbectList.Count > maxUplodingCount)
            //{

            //}

            //if (OjbectList != null && OjbectList.Count > 0 && OjbectList.Count > maxUplodingCount)
            //{
            //    lock (_objectLock)
            //    {
            //        if (OjbectList != null && OjbectList.Count > 0 && OjbectList.Count > maxUplodingCount)
            //        {
            //            var obj = TManager.OjbectList.Take(maxUplodingCount).ToList();
            //            // modify the stutas


            //            // upload data

            //            // update status, remove the object in the list
            //            // is succeed, update isUploaded; else record;
            //        }
            //    }
            //}
        }

        public static void CheckIsLimit(SpiderThread thread)
        {
            if (thread.SpiderModel.AccessCount >= thread.SpiderModel.MaxAccessCount)
            {
                thread.Thread.Abort();
            }
        }
    }
    
}
