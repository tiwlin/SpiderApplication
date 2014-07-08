using System;
using System.Configuration;
namespace DBUtility
{
    
    public class PubConstant
    {        
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {           
            get 
            {
                string _connectionString = null; 
                try
                {
                    _connectionString = PubConstant.GetConfigString("ConnectionString");
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                return _connectionString; 
            }
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionStringMiddleDB
        {
            get
            {
                string _connectionString = null;
                try
                {
                    _connectionString = PubConstant.GetConfigString("ConnectionStringMiddleDB");
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionStringOracle
        {
            get
            {
                string _connectionString = null;
                try
                {
                    _connectionString = PubConstant.GetConfigString("ConnectionStringOracle");
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// 得到web.config里配置项的字符串。
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConfigString(string configName)
        {
            string _configString = ConfigurationManager.AppSettings[configName];
            string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
            if (ConStringEncrypt == "true")
            {
                _configString = DESEncrypt.Decrypt(_configString);
            }
            if (String.IsNullOrEmpty(_configString))
            {
                throw new Exception(configName + " not config in App.config/Web.config file.");
            }
            return _configString;
        }


        public static string ReportPrinterName
        {
            get
            {
                string _configString = null;
                try
                {
                    _configString = PubConstant.GetConfigString("ReportPrinterName");
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                return _configString;
            }
        }

        public static string WebrootRptSource
        {
            get
            {
                string _configString = null;
                try
                {
                    _configString = PubConstant.GetConfigString("WebrootRptSource");
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                return _configString;
            }
        }

        public static int DIPrintCopies
        {
            get
            {
                string _configString = null;
                int _configInt = 0;
                try
                {
                    _configString = PubConstant.GetConfigString("DIPrintCopies");
                    _configInt = Convert.ToInt16(_configInt);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                return _configInt;
            }
        }

        public static int LMPrintCopies
        {
            get
            {
                string _configString = null;
                int _configInt = 0;
                try
                {
                    _configString = PubConstant.GetConfigString("LMPrintCopies");
                    _configInt = Convert.ToInt16(_configString);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                return _configInt;
            }
        }



    }
}
