using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PageLoader;

namespace SpiderLib
{
    public class TuangouUrlModel
    {
        public string Category { get; set; }

        public string Region { get; set; }

        public string Railway { get; set; }

        public string Text { get; set; }

        public string Price { get; set; }

        public string NumOfPeople { get; set; }

        public PageUrl Url { get; set; }

        public int Count { get; set; }
    }
}
