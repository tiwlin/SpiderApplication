using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderLib.Meituan
{
    public class PostParams
    {
        public string mteventParams { get; set; }

        public string geotype { get; set; }

        public string areaid { get; set; }

        public string offset { get; set; }

        public string dealids { get; set; }

        public string acms { get; set; }

        public override string ToString()
        {
            string toString = string.Format("params={0}&geotype={1}&areaid={2}&offset={3}&dealids={4}&acms={5}", System.Web.HttpUtility.UrlEncode(mteventParams.Replace("\\", "")), System.Web.HttpUtility.UrlEncode(geotype), System.Web.HttpUtility.UrlEncode(areaid), System.Web.HttpUtility.UrlEncode(offset), System.Web.HttpUtility.UrlEncode(dealids), System.Web.HttpUtility.UrlEncode(acms.Replace("\"", "")));

            return toString;
           //return JsonConvert.SerializeObject(this);
        }
    }
}
