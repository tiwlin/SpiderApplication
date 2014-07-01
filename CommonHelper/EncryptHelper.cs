using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;
using System.Security.Cryptography;

namespace CommonHelper
{
    public class EncryptHelper
    {
        private readonly static CryptographyManager crypt = EnterpriseLibraryContainer.Current.GetInstance<CryptographyManager>();

        private const string symmProvider = "SymmProvider";
        private const string hashProvider = "HashProvider";

        /// <summary>
        /// 对称加密(Dpapi)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return crypt.EncryptSymmetric(symmProvider, text);
        }

        /// <summary>
        /// 解密(Dpapi)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            return crypt.DecryptSymmetric(symmProvider, text);
        }

        /// <summary>
        /// 创建hash(不可逆)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CreateHash(string text)
        {
            return crypt.CreateHash(hashProvider, text);
        }

        public static byte[] CreateHash(byte[] stream)
        {
            return crypt.CreateHash(hashProvider, stream);
        }

        /// <summary>
        /// 比较Hash
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hashText"></param>
        /// <returns></returns>
        public static bool CompareHash(string text, string hashText)
        {
            return crypt.CompareHash(hashProvider, text, hashText);
        }

        public static bool CompareHash(byte[] stream, byte[] hashStream)
        {
            return crypt.CompareHash(hashProvider, stream, hashStream);
        }

        public static string Md5(string text)
        {
            if (string.IsNullOrEmpty(text))
            { 
                return "";
            }

            byte[] hashvalue = (new MD5CryptoServiceProvider()).ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashvalue).Replace("-","");
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToBase64(string text)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FromBase64(string text)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }

        /// <summary>
        /// 进行DES加密。
        /// </summary>
        /// <param name="pToEncrypt">要加密的字符串。</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <returns>以Base64格式返回的加密字符串。</returns>
        public static string DESEncrypt(string pToEncrypt, string sKey)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = HexStringFromByteArray(ms.ToArray());
                //string str = Convert.ToBase64String(ms.ToArray());  
                ms.Close();
                return str;
            }
        }
        public static string HexStringFromByteArray(byte[] bytes)
        {
            string s = "";
            foreach (byte b in bytes)
            {
                s += string.Format("{0:X2}", b);
            }
            return s;
        }

        /// <summary>
        /// 进行DES解密。
        /// </summary>
        /// <param name="pToDecrypt">要解密的以Base64</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <returns>已解密的字符串。</returns>
        public static string DESDecrypt(string pToDecrypt, string sKey)
        {
            //byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int i = 0; i < (pToDecrypt.Length / 2); i++)
            {
                int num2 = Convert.ToInt32(pToDecrypt.Substring(i * 2, 2), 0x10);
                inputByteArray[i] = (byte)num2;
            }

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        /// <summary>
        /// 进行3DES加密。
        /// </summary>
        /// <param name="pToEncrypt">要加密的字符串。</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <param name="arrIV">IV</param>
        /// <returns></returns>
        public static string Encrypt3DES(string pToEncrypt, string sKey, byte[] arrIV)
        {
            sKey = ToBase64(sKey);

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            byte[] b = ASCIIEncoding.ASCII.GetBytes(pToEncrypt);
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = arrIV;
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(pToEncrypt);

            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        /// <summary>
        /// 进行3DES解密。
        /// </summary>
        /// <param name="pToEncrypt">要解密的字符串。</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <param name="arrIV">IV</param>
        /// <returns></returns>
        public static string Decrypt3DES(string pToEncrypt, string sKey, byte[] arrIV)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = arrIV;
            DES.Mode = CipherMode.ECB;
            DES.Padding = System.Security.Cryptography.PaddingMode.Zeros;
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            string result = "";
            try
            {
                byte[] Buffer = Convert.FromBase64String(pToEncrypt);
                result = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception e)
            {

            }
            return result;
        }
    }
}
