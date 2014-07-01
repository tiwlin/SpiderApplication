using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CommonHelper
{
    /// <summary>
    /// 读取web.Config的辅助类
    /// </summary>
    public sealed class ConfigHelper
    {
		private static Dictionary<string,string> configContainer = new Dictionary<string,string>();
		
        public static string GetConfigString(string key, string value)
        {
            string tempvalue;
			if (!configContainer.TryGetValue(key,out tempvalue))
			{
				tempvalue = ConfigurationManager.AppSettings[key];
				if(tempvalue != null)
				{
					configContainer[key] = tempvalue;
				}
			}
			return tempvalue;
        }
        
        public static string GetConfigString(string key)
        {
			string value = GetConfigString(key,null);
			if(value == null)
			{
                return string.Empty;//throw new ArgumentNullException("配置项" + key + "不存在");
			}
			return value;
        }


        /// <summary>
        /// 得到AppSettings中的配置Bool信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetConfigBool(string key)
        {
            bool result = false;
            string cfgVal = GetConfigString(key);
            if (!bool.TryParse(cfgVal, out result))
            {
                throw new FormatException("配置项" + key + "值的格式不对");
            }
            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置Decimal信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static decimal GetConfigDecimal(string key)
        {
            decimal result = 0;
            string cfgVal = GetConfigString(key);
            if (!decimal.TryParse(cfgVal, out result))
            {
                throw new FormatException("配置项" + key + "值的格式不对");
            }

            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置int信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetConfigInt(string key)
        {
            int result = 0;
            string cfgVal = GetConfigString(key);
            if (!int.TryParse(cfgVal, out result))
            {
                throw new FormatException("配置项" + key + "值的格式不对");
            }
            return result;
        }
    }
}
