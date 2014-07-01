using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Login
{
    public partial class Login : Form
    {
        private string _key = "w9s7d_w918@#$80q2!@#s970";
        private byte[] _iv = { 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00, 0X00 };

        public Login()
        {
            InitializeComponent();
            this.txtAccount.Text = "testtest";
            this.txtPwd.Text = "z1x2c3ok";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (CheckEmpty())
            {
                string strHash = GetHashParam();

                string result = UserLogin(strHash);
            }

        }

        private bool CheckEmpty()
        {
            bool isPass = true;

            if (string.IsNullOrEmpty(this.txtAccount.Text.Trim()))
            {
                this.lblTips.Text = "请填写用户账号！";
                isPass = false; 
            }

            else if (string.IsNullOrEmpty(this.txtPwd.Text.Trim()))
            {
                this.lblTips.Text = "请填写登陆密码！";
                isPass = false; 
            }

            return isPass;
        }

        private string GetHashParam()
        {
            string strHash = string.Empty;

            try
            {
                //string strAccount = string.Format("username={0}&password={1}", this.txtAccount.Text.Trim(), System.Web.HttpUtility.UrlEncode(this.txtPwd.Text.Trim()));

                //strHash = CommonHelper.EncryptHelper.Encrypt3DES(strAccount, _key, _iv);

                //strHash = System.Web.HttpUtility.UrlEncode(strHash);

                strHash = string.Format("username={0}&password={1}", this.txtAccount.Text.Trim(), CommonHelper.EncryptHelper.Md5(System.Web.HttpUtility.UrlEncode(this.txtPwd.Text.Trim())));

                string str = CommonHelper.EncryptHelper.Decrypt3DES("7D5EymNyX730V+XuBX/h7CEq45ybMhmG2E7PT5fwuL0DPwaLbE5VCA==", _key, _iv);
            }
            catch (System.Exception ex)
            {

            }

            return strHash;
        }

        private string UserLogin(string strHash)
        {
            string urlLogin = "http://apitest.weijuyi.com/rest?method=public.auth.login&" + strHash;

            string result = CommonHelper.HttpHelper.GetResponse(urlLogin);

            return result;
        }
    }
}