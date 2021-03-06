﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.Data.SqlClient;
using System.Data;
using DBUtility;

namespace DAL
{
    public class RegionDAL
    {
        public bool Insert(RegionModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO Region ( ");

            strSql.Append("ParentRegion, Name, Code,CreateTime,UpdateTime ");
            strSql.Append(") VALUES (");
            strSql.Append("@ParentRegion, @Name, @Code,GETDATE(),GETDATE() ");
            strSql.Append(")");
            SqlParameter[] parameters = {
                    new SqlParameter("@ParentRegion", SqlDbType.VarChar),
                    new SqlParameter("@Name", SqlDbType.VarChar),
                    new SqlParameter("@Code", SqlDbType.VarChar)};
            parameters[0].Value = model.ParentRegion;
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
