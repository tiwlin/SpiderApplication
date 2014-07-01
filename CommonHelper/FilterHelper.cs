using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonHelper
{
	public static class FilterHelper
	{
		#region 用正则表达式实现.过滤单双引号
		public static string ReplaceQuotationMarks(string input)
		{
			return Regex.Replace(input, "[\"\']", "");
		}
		#endregion
	}
}
