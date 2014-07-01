using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
    public static class Const
    {
        //PC接口成功标识  const可以归纳到CommonHelper中
        public const string SUCCESSCODE = "00";
        public const string OLDCHAR = "\n";
        public const string NEWCHAR = "@BR@";
        public const string SHOWCHAR = "<BR/>";
        public const string DOUBLESLASH = "//";
        public const string DEFAULTFORMAT = "未知";
        public const int defaultRouteDetailInfoID = 1;
        public const int defaultFlightID = 1;
            
        public const char SEPARATOR1 = '-';
        public const char SEPARATOR2 = ':';
        public const char SEPARATOR3 = ',';

        public const string ELSEAIRCODE = "ELSE";
        public const string DEFAULAIRCODE = "*";

        /// <summary>
        /// 所有航空公司名称
        /// </summary>
        public const string AllAIRNAME = "全部航空公司";
        /// <summary>
        /// 默认销售价为0，表示暂不可销售
        /// </summary>
        public const decimal DEFAULTSALEFARE = 0.0M;

        /// <summary>
        /// 国际机票PNR长度，如果不等于6，则表示此PNR为非法PNR
        /// </summary>
        public const int PNRLENGTH = 6;

        /// <summary>
        /// 缓存默认容量
        /// </summary>
        public const int CACHE_CAPACITY = 10;// Capacity
    }
}
