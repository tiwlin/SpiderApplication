using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
	/// <summary>
	/// 类型转换的辅助类
	/// </summary>
	public sealed class TypeHelper
	{
		#region Int16 / short
		public static short ToInt16(object o, short _default)
		{
			try { return Convert.ToInt16(o); }
			catch { }
			short result = default(short);
			if (!short.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static short ToInt16(object o)
		{
			return ToInt16(o, default(short));
		}
        #endregion

        public static string ToStringNoPoint(string str)
        {
            //return str.TrimEnd(new char[] { '0', '.' });
            return str.Replace(".00","").Replace(".0","");
        }

		#region Int32 / int
		/// <summary>
		/// 转换为int类型
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static int ToInt(object o, int _default)
		{
			try { return Convert.ToInt32(o); }
			catch { }
			int result = default(int);
			if (!int.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static int ToInt(object o)
		{
			return ToInt(o, default(int));
		}


		public static int ToInt32(object o, int _default)
		{
			try { return Convert.ToInt32(o); }
			catch { }
			int result = default(int);
			if (!int.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static int ToInt32(object o)
		{
			return ToInt32(o, default(int));
		}
		#endregion
		
		#region Int64 / long
		public static long ToInt64(object o, long _default)
		{
			try { return Convert.ToInt64(o); }
			catch { }
			long result = default(long);
			if (!long.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static long ToInt64(object o)
		{
			return ToInt64(o, default(long));
		}
		#endregion
		
		#region UInt16

		public static UInt16 ToUInt16(object o, UInt16 _default)
		{
			try { return Convert.ToUInt16(o); }
			catch { }
			UInt16 result = default(UInt16);
			if (!UInt16.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static int ToUInt16(object o)
		{
			return ToUInt16(o, default(int));
		}

		#endregion	
		
		#region UInt32

		public static UInt32 ToUInt32(object o, UInt32 _default)
		{
			try { return Convert.ToUInt32(o); }
			catch { }
			UInt32 result = default(UInt32);
			if (!UInt32.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static UInt32 ToUInt32(object o)
		{
			return ToUInt32(o, default(int));
		}

		#endregion	

		#region UInt64

		public static UInt64 ToUInt64(object o, UInt64 _default)
		{
			try { return Convert.ToUInt64(o); }
			catch { }
			UInt64 result = default(UInt64);
			if (!UInt64.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static UInt64 ToUInt64(object o)
		{
			return ToUInt64(o, default(int));
		}

		#endregion	


		#region Single / single
		public static float ToSingle(object o, float _default)
		{
			try { return Convert.ToSingle(o); }	catch { }
			float result = default(float);
			if (!float.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}
		public static float ToSingle(object o)
		{
			return ToSingle(o, default(float));
		}
		#endregion

		#region Double / double
		public static double ToDouble(object o, double _default)
		{
			try { return Convert.ToDouble(o); } catch { }
			double result = default(double);
			if (!double.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static double ToDouble(object o)
		{
			return ToDouble(o, default(double));
		}
		#endregion
		
		#region Boolean / bool
		/// <summary>
		/// 转换为Bool类型
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static bool ToBoolean(object o, bool _default)
		{
			try { return Convert.ToBoolean(o); }
			catch { }
			bool result = default(bool);
			if (!bool.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static bool ToBoolean(object o)
		{
			return ToBoolean(o, default(bool));
		}
		#endregion

		#region Byte / byte
		/// <summary>
		/// 转换为byte类型
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static byte ToByte(object o, byte _default)
		{
			try { return Convert.ToByte(o); }
			catch { }
			byte result = default(byte);
			if (!byte.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static byte ToByte(object o)
		{
			return ToByte(o, default(byte));
		}
		#endregion

		#region Decimal / decimal
		/// <summary>
		/// 转换为decimal类型
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static decimal ToDecimal(object o, decimal _default)
		{
			try { return Convert.ToDecimal(o); }
			catch { }
			decimal result = default(decimal);
			if (!decimal.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static decimal ToDecimal(object o)
		{
			return ToDecimal(o, default(decimal));
		}
		#endregion
		
		#region DateTime / datetime
		/// <summary>
		/// 转换为DateTime类型
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(object o, DateTime _default)
		{
            if (o == null)
            {
                return _default;
            }

			try { return Convert.ToDateTime(o); }
			catch { }
			DateTime result = DateTime.MinValue;
			if (!DateTime.TryParse(o.ToString(), out result))
				result = _default;
			return result;
		}

		public static DateTime ToDateTime(object o)
		{
			return ToDateTime(o, DateTime.Parse("1900-01-01"));
		}
		#endregion

		#region String / string
		/// <summary>
		/// 转换为String
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static string ToString(object o, string _default)
		{
            if (o == DBNull.Value)
            {
                return string.Empty;
            }
			return (o ?? _default).ToString();
		}

		public static string ToString(object o)
		{
			return ToString(o, string.Empty);
		}
		#endregion
		
		public static decimal ToRound(decimal o)
		{
            return TypeHelper.ToInt((o / 10).ToString("f0")) * 10;
		}
	}
}
