using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DBUtility
{
    public class DBHelper
    {
        //连接数据库字符串
        public readonly static string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["ConnString"].ToString();

        /// <summary>
        /// 执行 Sql 语句并返回DataTable。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string strSql)
        {
            return GetDataTable(strSql, null);
        }

        /// <summary>
        /// 执行带参数SQL语句并返回DataTable。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string strSql, params SqlParameter[] pa)
        {
            DataTable dt = null;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(strSql, conn))
                {
                    try
                    {
                        if (pa != null)
                        {
                            da.SelectCommand.Parameters.AddRange(pa);
                        }
                        conn.Open();

                        dt = new DataTable();
                        da.Fill(dt);
                        da.SelectCommand.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //错误处理
                        throw ex;
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 存储过程_执行带参数SQL语句并返回DataTable。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static DataTable Pro_GetDataTable(string strSql, params SqlParameter[] pa)
        {
            DataTable dt = null;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand(strSql, conn))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (pa != null)
                        {
                            //da.SelectCommand.Parameters.AddRange(pa);
                            cmd.Parameters.AddRange(pa);
                        }
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        conn.Open();
                        dt = new DataTable();
                        da.Fill(dt);
                        da.SelectCommand.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //错误处理
                        throw ex;
                    }
                }
            }
            return dt;
        }

        /// <summary>
        ///  执行SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string strSql)
        {
            return ExecuteNonQuery(strSql, null);
        }

        /// <summary>
        /// 执行带参数SQL语句 并返回受影响的行数。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string strSql, params SqlParameter[] pa)
        {
            int iRows = 0;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand(strSql, conn))
                {
                    try
                    {
                        if (pa != null) { cmd.Parameters.AddRange(pa); }
                        conn.Open();
                        iRows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //错误处理
                        throw ex;
                    }
                }
            }
            return iRows;
        }

        /// <summary>
        ///  执行SQL语句并返回受影响的行数。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static int Pro_ExecuteNonQuery(string strSql)
        {
            return Pro_ExecuteNonQuery(strSql, null);
        }

        /// <summary>
        /// 执行带参数SQL语句 并返回受影响的行数。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static int Pro_ExecuteNonQuery(string strSql, params SqlParameter[] pa)
        {
            int iRows = 0;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand(strSql, conn))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (pa != null) { cmd.Parameters.AddRange(pa); }
                        conn.Open();
                        iRows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //错误处理
                        throw ex;
                    }
                }
            }
            return iRows;
        }

        /// <summary>
        /// 执行SQL语句 并返回主键值。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static int ExecuteNonQueryGetID(string strSql)
        {
            return ExecuteNonQueryGetID(strSql, null);
        }

        /// <summary>
        /// 执行带参数SQL语句 并返回主键值。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static int ExecuteNonQueryGetID(string strSql, params SqlParameter[] pa)
        {
            int id = 0;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand(strSql, conn))
                {
                    try
                    {
                        if (pa != null) { cmd.Parameters.AddRange(pa); }
                        conn.Open();
                        id = cmd.ExecuteNonQuery();
                        if (id > 0)
                        {
                            cmd.CommandText = "select @@IDENTITY";
                            id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        cmd.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //错误处理
                        throw ex;
                    }
                }
            }
            return id;
        }

        /// <summary>
        /// 执行SQL语句查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string strSql)
        {
            return ExecuteScalar(strSql, null);
        }

        /// <summary>
        ///  执行 带参数SQL 语句查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string strSql, params SqlParameter[] pa)
        {
            object obj = null;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand(strSql, conn))
                {
                    try
                    {
                        if (pa != null)
                        {
                            cmd.Parameters.AddRange(pa);
                        }
                        conn.Open();
                        obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //错误处理
                        throw ex;
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// 执行 SQL 语句查询，并返回查询所返回的结果集。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string strSql)
        {
            return ExecuteReader(strSql, null);
        }

        /// <summary>
        /// 执行 带参数SQL语句 查询，并返回查询所返回的结果集。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string strSql, params SqlParameter[] pa)
        {
            SqlDataReader rs = null;
            SqlConnection conn = new SqlConnection(strConn);
            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                try
                {
                    if (pa != null)
                    {
                        cmd.Parameters.AddRange(pa);
                    }
                    conn.Open();
                    rs = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    //错误处理
                    throw ex;
                }
                finally
                {
                    if (rs == null)
                        conn.Dispose();
                }
            }
            return rs;
        }

        /// <summary>
        /// 存储过程_执行 SQL 语句查询，并返回查询所返回的结果集。
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static SqlDataReader Pro_ExecuteReader(string strSql)
        {
            return Pro_ExecuteReader(strSql, null);
        }

        /// <summary>
        /// 存储过程_执行带参数SQL语句 查询，并返回查询所返回的结果集。
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static SqlDataReader Pro_ExecuteReader(string strSql, params SqlParameter[] pa)
        {
            SqlDataReader rs = null;
            SqlConnection conn = new SqlConnection(strConn);
            using (SqlCommand cmd = new SqlCommand(strSql, conn))
            {
                try
                {
                    if (pa != null)
                    {
                        cmd.Parameters.AddRange(pa);
                    }
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    rs = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    //错误处理
                    throw ex;
                }
                finally
                {
                    if (rs == null)
                        conn.Dispose();
                }
            }
            return rs;
        }

        public static DataSet returnDs(string sqlStr)
        {
            return returnDs(sqlStr, "temptable");
        }

        public static DataSet returnDs(string sqlStr,string tempTable)
        {
            DataSet ds = new DataSet();
            return returnDs(ds, sqlStr, tempTable);
        }

        public static DataSet returnDs(DataSet ds, string sqlStr, string tempTable)
        {
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                try
                {
                    //conn.Open(); 
                    SqlCommand comm = new SqlCommand(sqlStr, conn);
                    comm.CommandTimeout = 20;
                    conn.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(comm);
                    sda.Fill(ds, tempTable);
                    return ds;
                    conn.Close(); conn.Dispose(); comm.Dispose();

                }
                catch (Exception e)
                {
                    throw (e);
                    // ds = null; 
                    return ds;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static DataSet Pro_GetDataSet(string strSql, string tempTable, params SqlParameter[] pa)
        {
            DataSet ds = null;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand(strSql, conn))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (pa != null)
                        {
                            //da.SelectCommand.Parameters.AddRange(pa);
                            cmd.Parameters.AddRange(pa);
                        }
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;
                        conn.Open();
                        ds = new DataSet();
                        da.Fill(ds, tempTable);
                        da.SelectCommand.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //错误处理
                        throw ex;
                    }
                }
            }
            return ds;
        }

        public static DataSet Pro_GetDataSet(string strSql, params SqlParameter[] pa)
        {

            return Pro_GetDataSet(strSql, "temptable", pa);
        }

        /// <summary>
        /// 多参数查询-(xsj 2011-09-26)
        /// </summary>
        /// <param name="connectionString">连接字符</param>
        /// <param name="cmdType">存储过程或者sql语句选择</param>
        /// <param name="cmdText">存储过程名或sql语句串</param>
        /// <param name="ClerParams">是否清空SqlCommand参数</param>
        /// <param name="commandParameters">语句参数</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, bool ClerParams, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (ClerParams)
                {
                    cmd.Parameters.Clear();
                }
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// Prepare a command for execution -(xsj 2011-09-26)
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        public static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if (parm.Value == null)
                        parm.Value = DBNull.Value;
                    cmd.Parameters.Add(parm);
                }
            }
        }
    }
}
