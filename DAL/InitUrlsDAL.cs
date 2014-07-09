using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DBUtility;
using Model;

namespace DAL
{
    public class InitUrlsDAL
    {
        public bool Insert(InitUrlsModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO InitUrls ( ");

            strSql.Append("CategoryCode, RegionCode, Url, Status,CreateTime,UpdateTime ");
            strSql.Append(") VALUES (");
            strSql.Append("@CategoryCode, @RegionCode, @Url, 1, GETDATE(),GETDATE() ");
            strSql.Append(")");
            SqlParameter[] parameters = {
                    new SqlParameter("@CategoryCode", SqlDbType.VarChar),
                    new SqlParameter("@RegionCode", SqlDbType.VarChar),
                    new SqlParameter("@Url", SqlDbType.VarChar)};
            parameters[0].Value = model.CategoryCode;
            parameters[1].Value = model.RegionCode;
            parameters[2].Value = model.Url;

            int rows = DBHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
