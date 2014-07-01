using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace CommonHelper
{
	public static class EnumHelper
	{
		/// <summary>
		/// 枚举转换为字典
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<string, int> EnumDictionary<T>()
		{
			Dictionary<string, int> dic = new Dictionary<string, int>();
			if (typeof(T) == typeof(Enum))
			{
				throw new ArgumentOutOfRangeException("T只能是Enum类型");
			}

			Type enumType = typeof(T);
			foreach (string key in Enum.GetNames(enumType))
			{
				int val = (int)enumType.GetField(key).GetValue(null);
				dic.Add(key, val);
			}
			return dic;
		}

		/// <summary>
		/// 枚举转换为字典
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<int, string> EnumToDictionary<T>()
		{
			Dictionary<int, string> dic = new Dictionary<int, string>();
			if (typeof(T) == typeof(Enum))
			{
				throw new ArgumentOutOfRangeException("T只能是Enum类型");
			}

			Type enumType = typeof(T);
			foreach (string key in Enum.GetNames(enumType))
			{
				int val = (int)enumType.GetField(key).GetValue(null);
				dic.Add(val, key);
			}
			return dic;
		}

		/// <summary>
		/// 枚举的描述和Key转为字典
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<string, string> EnumKeyAndDescriptionToDictionary<T>()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			if (typeof(T) == typeof(Enum))
			{
				throw new ArgumentOutOfRangeException("T只能是Enum类型");
			}

			Type enumType = typeof(T);

			foreach (string key in Enum.GetNames(enumType))
			{
				FieldInfo finfo = enumType.GetField(key);
				object[] cAttr = finfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (cAttr.Length > 0)
				{
					DescriptionAttribute desc = cAttr[0] as DescriptionAttribute;
					if (desc != null)
					{
						dic[key] = desc.Description;
					}
				}
			}

			return dic;
		}

		/// <summary>
		/// 枚举的描述和Key转为字典
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<string, string> EnumDescriptionAndKeyToDictionary<T>()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();

			if (typeof(T) == typeof(Enum))
			{
				throw new ArgumentOutOfRangeException("T只能是Enum类型");
			}

			Type enumType = typeof(T);

			foreach (string key in Enum.GetNames(enumType))
			{
				FieldInfo finfo = enumType.GetField(key);
				object[] cAttr = finfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (cAttr.Length > 0)
				{
					DescriptionAttribute desc = cAttr[0] as DescriptionAttribute;
					if (desc != null)
					{
						dic[desc.Description] = key;
					}
				}
			}

			return dic;
		}
		
		/// <summary>
		/// 枚举的描述和Value转为字典
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<int, string> EnumValueAndDescriptionToDictionary<T>()
		{
			Dictionary<int, string> dic = new Dictionary<int, string>();

			if (typeof(T) == typeof(Enum))
			{
				throw new ArgumentOutOfRangeException("T只能是Enum类型");
			}

			Type enumType = typeof(T);

			foreach (string key in Enum.GetNames(enumType))
			{
				int val = (int)enumType.GetField(key).GetValue(null);

				FieldInfo finfo = enumType.GetField(key);
				object[] cAttr = finfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (cAttr.Length > 0)
				{
					DescriptionAttribute desc = cAttr[0] as DescriptionAttribute;
					if (desc != null)
					{
						dic[val] = desc.Description;
					}
				}
			}

			return dic;
		}

		/// <summary>
		/// 枚举的描述和Value转为字典
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<string, int> EnumDescriptionAndValueToDictionary<T>()
		{
			Dictionary<string, int> dic = new Dictionary<string, int>();

			if (typeof(T) == typeof(Enum))
			{
				throw new ArgumentOutOfRangeException("T只能是Enum类型");
			}

			Type enumType = typeof(T);

			foreach (string key in Enum.GetNames(enumType))
			{
				int val = (int)enumType.GetField(key).GetValue(null);

				FieldInfo finfo = enumType.GetField(key);
				object[] cAttr = finfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (cAttr.Length > 0)
				{
					DescriptionAttribute desc = cAttr[0] as DescriptionAttribute;
					if (desc != null)
					{
						dic[desc.Description] = val;
					}
				}
			}

			return dic;
		}

        /// <summary>
        /// 枚举描述
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetEnumDesc(Enum e)
        {
            FieldInfo fieldInfo = e.GetType().GetField(e.ToString());
			if (fieldInfo != null)
			{
				DescriptionAttribute[] EnumAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (EnumAttributes.Length > 0)
				{
					return EnumAttributes[0].Description;
				}
			}
            return e.ToString();
        }
	}
	
	public class EnumView<T>
	{
		private static Dictionary<int, string> s_DictCodeToText = null;
		private static Dictionary<string, int> s_DictTextToCode = null;

		static EnumView()
		{
            s_DictCodeToText = WebCommon.EnumToDictionary<T>();
            s_DictTextToCode = WebCommon.EnumDictionary<T>();
		}

		private EnumView() { }

		public static Dictionary<int, string> DictCodeToText { get { return s_DictCodeToText; } }
		public static Dictionary<string, int> DictTextToCode { get { return s_DictTextToCode; } }


		public static int GetCode(string text)
		{
			if (s_DictTextToCode.ContainsKey(text))
			{
				return s_DictTextToCode[text];
			}
			return int.MinValue;
		}

		public static string GetText(int code)
		{
			if (s_DictCodeToText.ContainsKey(code))
			{
				return s_DictCodeToText[code] ?? string.Empty;
			}
			return string.Empty;
		}

	}
}
