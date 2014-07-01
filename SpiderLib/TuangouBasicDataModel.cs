using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderLib
{
    public class TuangouBasicDataModel
    {
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 分类字典
        /// </summary>
        public IDictionary<string, IList<string>> DctCategories
        {
            get;
            set;
        }

        /// <summary>
        /// 区域字典
        /// </summary>
        public IDictionary<string, IList<string>> DctRegions
        {
            get;
            set;
        }
    }
}
