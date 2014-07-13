using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace DALHelper
{
    /// <summary>
    /// 参数的辅助类
    /// </summary>
    public class DbParameterHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pName">无需加数据库的标识前缀(如@),已备以后切换数据库</param>
        /// <param name="type"></param>
        /// <param name="parmValue"></param>
        /// <returns></returns>
        public static DbParameterInfo GetDbParameter(string pName, System.Data.DbType type, object parmValue)
        {
            return new DbParameterInfo(pName, type, parmValue);
        }

        public static DbParameterInfo GetDbParameter(string pName, System.Data.DbType type, int size, object parmValue)
        {
            return new DbParameterInfo(pName, type, size, parmValue);
        }

        public static DbParameterInfo GetDbParameter(string pName, System.Data.DbType type, System.Data.ParameterDirection direction, object parmValue)
        {
            return new DbParameterInfo(pName, type, direction, parmValue);
        }

        public static DbParameterInfo GetDbParameter(string pName, System.Data.DbType type, System.Data.ParameterDirection direction, int size, object parmValue)
        {
            return new DbParameterInfo(pName, type, direction, size, parmValue);
        }


        private static string[,] DBTypeConversionKey = new string[,] 
        {
         {"UniqueIdentifier","System.Guid"},
         {"NVarChar",  "System.String"},
         {"Int",    "System.Int32"},
         {"Decimal",   "System.Decimal"},
         {"DateTime",  "System.DateTime"},
         {"Bit",    "System.Boolean"},
         {"SmallDateTime", "System.DateTime"}, 
         {"Money",   "System.Decimal"},
         {"SmallMoney",  "System.Decimal"},
         {"TinyInt",   "System.Byte"},
         {"SmallInt",  "System.Int16"},
         {"BigInt",   "System.Int64"},
         {"VarChar",   "System.String"},
         {"Text",   "System.String"},
         {"Char",   "System.String"},
         {"Float",   "System.Double"},
         {"Real",   "System.Single"},
         {"NChar",   "System.String"},
         {"NText",   "System.String"},
         {"Image",   "System.Byte[]"},
         {"Binary",   "System.Byte[]"},
         {"Timestamp",  "System.Byte[]"},
         {"VarBinary",  "System.Byte[]"},
         {"Variant",   "System.Object"}
        };

        private static Dictionary<string, DbType> cacheDbType = new Dictionary<string, DbType>();
        /// <summary>
        /// 系统类型转换Sql类型
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static SqlDbType SystemTypeToSqlType(Type sourceType)
        {
            string typeName = string.Empty;
            int keyCount = DBTypeConversionKey.GetLength(0);
            for (int i = 0; i < keyCount; i++)
            {
                if (DBTypeConversionKey[i, 1].Equals(sourceType.FullName))
                {
                    typeName = DBTypeConversionKey[i, 0];
                    break;
                }
            }
            if (typeName == String.Empty) typeName = "Variant";//此时是默认的即对应Object

            return (SqlDbType)Enum.Parse(typeof(SqlDbType), typeName);
        }

        /// <summary>
        /// Sql类型转换为DbType
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static DbType SqlTypeToDbType(SqlDbType sourceType)
        {
            SqlParameter parmConvert = new SqlParameter();  //通过SqlParameter把 SqlDbType --> DbType
            parmConvert.SqlDbType = sourceType;
            return parmConvert.DbType;
        }

        /// <summary>
        /// 系统类型转换DbType
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static DbType SystemTypeToDbType(Type sourceType)
        {
            if (cacheDbType.ContainsKey(sourceType.FullName))
            {
                return cacheDbType[sourceType.FullName];
            }

            DbType dbType = SqlTypeToDbType(SystemTypeToSqlType(sourceType));
            cacheDbType.Add(sourceType.FullName, dbType);
            return dbType;
        }
    }
}
