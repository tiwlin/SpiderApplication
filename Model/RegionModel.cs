using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class RegionModel
    {
        public int ID { get; set; }

        public int RegionID { get; set; }

        public string ParentRegion { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public long TimeStamp { get; set; }
    }
}
