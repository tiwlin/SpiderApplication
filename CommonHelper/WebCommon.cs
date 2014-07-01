using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

using FastReflectionLib;
using System.Collections;

namespace CommonHelper
{
    public class WebCommon
    {
        /// <summary>
        /// 判断一个word是否为GB2312编码的汉字
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsGBCode(string word)
        {
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(word);
            if (bytes.Length <= 1) // if there is only one byte, it is ASCII code or other code
            {
                return false;
            }
            else
            {
                byte byte1 = bytes[0];
                byte byte2 = bytes[1];
                if (byte1 >= 176 && byte1 <= 247 && byte2 >= 160 && byte2 <= 254)    //判断是否是GB2312
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<T> DataTableToModel<T>(DataTable dt)
        {
            List<T> lst = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T model = Activator.CreateInstance<T>();
                DataRowToModel<T>(dr, model);
                lst.Add(model);
            }

            return lst;
        }

        public static void DataRowToModel<T>(DataRow dr, T obj)
        {
            foreach (PropertyInfo p in obj.GetType().GetProperties())
            {
                if (dr.Table.Columns.Contains(p.Name) && dr[p.Name] != DBNull.Value)
                {
                    var accessor = FastReflectionCaches.PropertyAccessorCache.Get(p);
                    if (!p.PropertyType.IsEnum)
                    {
                        accessor.SetValue(obj, dr[p.Name]);
                        //p.FastSetValue(obj, dr[p.Name]);
                    }
                    else
                    {
                        accessor.SetValue(obj, Enum.Parse(p.PropertyType, dr[p.Name].ToString()));
                        //p.FastSetValue(obj, Enum.Parse(p.PropertyType, dr[p.Name].ToString()));
                    }
                }
            }
        }

        public static List<T> DataReaderToModel<T>(IDataReader reader)
        {
            List<T> retLst = new List<T>();
            using (reader)
            {
                while (reader.Read())
                {
                    T ins = Activator.CreateInstance<T>();
                    DataReaderToModel<T>(reader, ins);
                    retLst.Add(ins);
                }
            }
            return retLst;
        }

        public static void DataReaderToModel<T>(IDataReader reader, T obj)
        {
            Type t = obj.GetType();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                PropertyInfo pInfo = t.GetProperty(reader.GetName(i));
                if (pInfo != null)
                {
                    var accessor = FastReflectionCaches.PropertyAccessorCache.Get(pInfo);
                    if (!pInfo.PropertyType.IsEnum)
                    {
                        accessor.SetValue(obj, reader[i]);
                    }
                    else
                    {
                        accessor.SetValue(obj, Enum.Parse(pInfo.PropertyType, reader[i].ToString()));
                    }
                }
            }
        }

        public static void ModelToDataRow<T>(DataRow dr, T obj)
        {
            foreach (PropertyInfo p in obj.GetType().GetProperties())
            {
                if (dr.Table.Columns.Contains(p.Name))
                {
                    var accessor = FastReflectionCaches.PropertyAccessorCache.Get(p);
                    dr[p.Name] = accessor.GetValue(obj);
                }
            }
        }

        /// <summary>
        /// 枚举转换为字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, int> EnumDictionary<T>()
        {
            return EnumHelper.EnumDictionary<T>();
        }
        /// <summary>
        /// 枚举转换为字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> EnumToDictionary<T>()
        {
            return EnumHelper.EnumToDictionary<T>();
        }

		/// <summary>
		/// 枚举描述和值转换为字典
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<string, int> EnumDescriptionAndValueToDictionary<T>()
		{
			return EnumHelper.EnumDescriptionAndValueToDictionary<T>();
		}

		public static string GetEnumDesc(Enum e)
		{
			return EnumHelper.GetEnumDesc(e);
		}

        /// <summary>
        /// 检查str是否为空，如果不为空直接返回str,否则返回defaultStr
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultStr"></param>
        /// <returns></returns>
        public static string CheckNullStr(string str, string defaultStr)
        {
            if (str == null || str.Trim() == string.Empty)
            {
                return defaultStr;
            }

            return str;
        }

        /// <summary>
        /// 截取指定长度的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubStringLength(string str, int length)
        {
            return str.Length > length ? str.Substring(0, length) : str;
        }

        /// <summary>
        /// 返回时间的默认值（1900-00-01）
        /// </summary>
        /// <param name="dt"></param>
        public static DateTime GetDateTimeDefault(DateTime dt)
        {
            if (dt != DateTime.MinValue)
            {
                return dt;
            }

            return DateTime.Parse("1900-01-01");
        }

        /// <summary>
        /// 根据英文的Week获取数字
        /// </summary>
        /// <returns></returns>
        public static int GetWeekByWeekEn(string weekEn)
        {
            switch (weekEn)
            {
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                case "Sunday":
                    return 7;
                default: return 0;
            }
        }

        protected static int code = 0;
        private static readonly object lockObj = new object();
        /// <summary>
        /// 生成不重复的编码 (由于购物车可以批量提交订单，所以一定要生成不重复的编码)
        /// </summary>
        /// <returns></returns>
        public static long GetNonRepeatCode()
        {
            lock (lockObj)
            {
                if (code == 1000000)
                {
                    code = 1;
                }
                code++;
            }
            return code + TypeHelper.ToInt64(DateTime.Now.ToString("MMddHHmmss"));
        }

        public static Hashtable GetQueryStringParams(string queryString)
        {
            Hashtable result = new Hashtable();
            char[] splitChar = new char[] { '&' };
            char[] equalChar = new char[] { '=' };
            // Split query string to components
            foreach (string s in queryString.Split(splitChar))
            {
                // split each component to key and value
                string[] keyVal = s.Trim().Split(equalChar, 2);
                string key = keyVal[0];
                string val = String.Empty;
                if (keyVal.Length > 1) val = keyVal[1];
                if (!result.Contains(key))// Add to the hashtable
                    result.Add(key, val);
            }
            // return the resulting hashtable
            return result;
        }

        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="d">double 型数字</param>
        /// <returns>DateTime</returns>
        public static System.DateTime ConvertUnixDateTime(double d)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddSeconds(d);
            return time;
        }

        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static double ConvertDateTimeUnix(System.DateTime time)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return intResult;
        }

    }
}
