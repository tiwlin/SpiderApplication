using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace CommonHelper
{
    /// <summary>
    /// 数据库记录日志
    /// </summary>
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    public class DBLogListener : CustomTraceListener
    {
        public override void TraceData(TraceEventCache eventCache, string source,
          TraceEventType eventType, int id, object data)
        {
            if (data is LogEntry && this.Formatter != null)
            {
                this.Write(this.Formatter.Format(data as LogEntry));
            }
            else
            {
                this.Write(data.ToString());
            }
        }

        public override void Write(string message)
        {
          
        }

        public override void WriteLine(string message)
        {
            
        }
    }
}
