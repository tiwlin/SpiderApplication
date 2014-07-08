using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class ShopModel
    {
        public string category { get; set; }

        public string region { get; set; }

        public int ShopID { get; set; }

        public string name { get; set; }

        public string address { get; set; }

        public string range { get; set; }

        public int rangeid { get; set; }

        public int disid { get; set; }

        public string disname { get; set; }

        public int dpshopid { get; set; }

        public string mapurl { get; set; }

        public string trafficinfo { get; set; }

        public string phone { get; set; }

        public string latlng { get; set; }

        public int city { get; set; }

        public string url { get; set; }

        public int poiid { get; set; }

        public string cityname { get; set; }

        public int status { get; set; }

        public string subwayname { get; set; }

        public string subwaydis { get; set; }

        public string subwayslug { get; set; }

        public string appointmentDay { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public long TimeStamp { get; set; }
    }
}
