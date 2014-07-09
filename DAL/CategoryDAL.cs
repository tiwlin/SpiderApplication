using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.Data.SqlClient;
using DBUtility;
using System.Data;

namespace DAL
{
    public class CategoryDAL
    {
        public bool Insert(CategoryModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO Category ( ");

            strSql.Append("ParentCategory, Name, Code,CreateTime,UpdateTime ");
            strSql.Append(") VALUES (");
            strSql.Append("@ParentCategory, @Name, @Code,GETDATE(),GETDATE() ");
            strSql.Append(")");
            SqlParameter[] parameters = {
                    new SqlParameter("@ParentCategory", SqlDbType.VarChar),
                    new SqlParameter("@Name", SqlDbType.VarChar),
                    new SqlParameter("@Code", SqlDbType.VarChar)};
            parameters[0].Value = model.ParentCategory;
            parameters[1].Value = model.Name;
            parameters[2].Value = model.Code;

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
