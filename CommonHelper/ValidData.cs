using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace CommonHelper
{

    /// <summary>
    ///ValidData 的摘要说明
    /// </summary>
    public class ValidData
    {
        public ValidData()
        {
          
        }


        #region 验证是否为邮编
        public bool IsValidPostalcode(string str)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d{6}$");
        }

        #endregion


        #region 用正则表达式实现 手机号码
        public bool IsValidMobile(string str)
        {
            return Regex.IsMatch(str, @"^(13|15|18)\d{9}$", RegexOptions.IgnoreCase);
        }
        #endregion



        #region 用正则表达式实现.验证输入是否是数字
        public bool IsValidNumer(string str)
        {
            System.Text.RegularExpressions.Regex reg1
             = new System.Text.RegularExpressions.Regex(@"^[-]?\d+[.]?\d*$");
            return reg1.IsMatch(str);
        }
        #endregion

        #region 验证是否为小数
        public bool IsValidDecimal(string str)
        {
            return Regex.IsMatch(str, @"[0].\d{1,2}|[1]");
        }

        #endregion

        #region 验证Email地址
        public bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
			return Regex.IsMatch(strIn, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }
        #endregion

        #region dd-mm-yy 的日期形式代替 mm/dd/yy 的日期形式。
        public string MDYToDMY(String input)
        {
            return Regex.Replace(input, "\\b(?\\d{1,2})/(?\\d{1,2})/(?\\d{2,4})\\b", "${day}-${month}-${year}");
        }
        #endregion

        #region 验证是否为电话号码
        public bool IsValidTelNum(string strIn)
        {
            return Regex.IsMatch(strIn, @"(\d+-)?(\d{4}-?\d{7}|\d{3}-?\d{8}|^\d{7,8})(-\d+)?");
        }
        #endregion

        #region 验证年月日
        bool IsValidDate(string strIn)
        {
            return Regex.IsMatch(strIn, @"^2\d{3}-(?:0?[1-9]|1[0-2])-(?:0?[1-9]|[1-2]\d|3[0-1])(?:0?[1-9]|1\d|2[0-3]):(?:0?[1-9]|[1-5]\d):(?:0?[1-9]|[1-5]\d)$");
        }
        #endregion

        #region 验证后缀名
        bool IsValidPostfix(string strIn)
        {
            return Regex.IsMatch(strIn, @"\.(?i:gif|jpg)$");
        }
        #endregion

        #region 验证字符是否在4至12之间
        bool IsValidByte(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[a-z]{4,12}$");
        }
        #endregion

        #region 验证IP
        bool IsValidIp(string strIn)
        {
            return Regex.IsMatch(strIn, @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        }
        #endregion

        #region 弹出javascript对话框,是否返回或结束。
        public void WriteMessage(string strMsg, bool Back, bool End)
        {
            HttpContext Context = HttpContext.Current;
            strMsg = strMsg.Replace("'", "");
            strMsg = strMsg.Replace("\r\n", "");
            if (strMsg != "" && strMsg != null)
                Context.Response.Write("<script language=javascript>alert('" + strMsg + "');</script>");
            if (Back)
                Context.Response.Write("<script language=javascript>history.back();</script>");
            if (End)
                Context.Response.End();
        }
        #endregion
        
        
        /// <summary>
        /// 验证密码结构
        /// </summary>
        /// <param name="password">密码明文</param>
        /// <param name="length">密码长度要求</param>
        /// <param name="complex">密码复杂度要求</param>
        /// <returns></returns>
        public bool PasswordValidate(string password, int length, int complex)
        {
			password = password.Trim();
			bool flag = true;
			int c = 0;
			if(password.Length < length) flag = false;
			int[] a = new int[4] { 0, 0, 0, 0 };
			foreach(int aa in password)
			{
				if (aa >= 48 && aa <= 57) a[0] = 1;
				else if (aa >= 65 && aa <= 90) a[1] = 1;
				else if (aa >= 97 && aa <= 122) a[2] = 1;
				else a[3] = 1;
			}
			if((a[1] + a[2] + a[3] + a[4]) < complex) flag = false;
			
			return flag;
        }

		#region 用正则表达式实现.验证输入是否数字和字母
		public bool IsValidNumerAndLetter(string str)
		{
			System.Text.RegularExpressions.Regex reg1
			 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
			return reg1.IsMatch(str);
		}
		#endregion
    }

}
 

