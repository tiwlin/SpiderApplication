using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SpiderLib
{
    public class SourceModelBase
    {
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
