using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class InitUrlsModel
    {
        public int ID { get; set; }

        public int CategoryID { get; set; }

        public string CategoryCode { get; set; }

        public int RegionID { get; set; }

        public string RegionCode { get; set; }

        public string Url { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public long TimeStamp { get; set; }
    }
}
