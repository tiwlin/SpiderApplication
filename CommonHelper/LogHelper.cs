using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
namespace CommonHelper
{
    public static class LogHelper
    {
        /// <summary>
        /// 返回企业库的日志管理
        /// </summary>
        public static ILog Log
        {
            get
            {
                return new EnterLibLog();
            }
        }
        /// <summary>
        /// 记录调试 by add zbh
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message, string title, string category)
        {
            LogEntry logEntry = new LogEntry();

            logEntry.EventId = 1;
            logEntry.Priority = 1;
            logEntry.Title = title;
            logEntry.Message = message;
            logEntry.Categories.Add(category);

            Logger.Writer.Write(logEntry, "Debug");
        }
    }


    #region EnterLibLog
    internal sealed class EnterLibLog : ILog
    {
        private readonly static LogWriter logWriter = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();

        private void Write(object message, string category)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>();
            if (HttpContext.Current != null)
            {
                dic.Add("userid", HttpContext.Current.User.Identity.Name);
                dic.Add("url", HttpContext.Current.Request.RawUrl);
            }

            logWriter.Write(message, category, dic);
        }

        #region ILog 成员
        /// <summary>
        /// 记录操作
        /// </summary>
        /// <param name="message"></param>
        public void Info(object message)
        {
            this.Write(message, "Info");
        }

        /// <summary>
        /// 记录错误
        /// </summary>
        /// <param name="message"></param>
        public void Error(object message)
        {
            this.Write(message, "Error");
        }

        /// <summary>
        /// 记录调试
        /// </summary>
        /// <param name="message"></param>
        public void Debug(object message)
        {
            this.Write(message, "Debug");
        }

        
        #endregion
    }
   

    public interface ILog
    {
        void Info(object message);

        void Error(object message);

        void Debug(object message);

    }
    #endregion
}
