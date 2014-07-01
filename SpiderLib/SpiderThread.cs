using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SpiderLib
{
    public class SpiderThread
    {
        public SourceModelBase SpiderModel { get; set; }

        public Thread Thread { get; set; }
    }
}
