using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace DALHelper
{
    public class DataAccessManger
    {
        private static IDictionary<string, DataAccess> _dbConnection = new Dictionary<string,DataAccess>();

        public static DataAccess DefaultConnection
        {
            get
            {
                return GetAccess("DefaultConnection");
            }
        }

        public static DataAccess GetAccess(string cnName)
        {
            //hxb 2011.1.12 modify
            if (!_dbConnection.ContainsKey(cnName))
            {
                _dbConnection.Add(cnName, new DataAccess()
                    {
                        ConnectionName = cnName,
                        Db = EnterpriseLibraryContainer.Current.GetInstance<Database>(cnName)
                    });
            }

            //if (_dbConnection == null)
            //{
            //    IDictionary<string, DataAccess> dic = new Dictionary<string, DataAccess>();
            //    foreach (ConnectionStringSettings settings in ConfigurationManager.ConnectionStrings)
            //    {
            //        dic.Add(settings.Name, new DataAccess()
            //        {
            //            ConnectionName = settings.Name,
            //            Db = EnterpriseLibraryContainer.Current.GetInstance<Database>(settings.Name)
            //        });
            //    }

            //    _dbConnection = dic;
            //}

            //if (!_dbConnection.ContainsKey(cnName))
            //{
            //    throw new Exception(string.Format("连接{0}未定义", cnName));
            //}

            return _dbConnection[cnName];
        }
    }
}
