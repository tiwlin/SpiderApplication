using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Transactions;
using System.Reflection;

using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DALHelper
{
    public class DataAccess
    {
        internal DataAccess() { }

        public string ConnectionName { get; set; }
        public Database Db { get; set; }

        public string ConnectionString
        {
            get
            {
                return Db.ConnectionString;
            }
        }

        #region 执行方法
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(commandType, commandText, null);
        }

        /// <summary>
        /// 执行命令并返回影响行数
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parmsLst"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameterInfo[] parmsLst)
        {
            DbCommand cmd = this.BuildCommand(commandType, commandText, parmsLst);

            int result = Db.ExecuteNonQuery(cmd);

            return result;
        }

        /// <summary>
        /// 执行命令并返回值(此处对DBNull和Null已做处理返回"")
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return ExecuteScalar(commandType, commandText, null);
        }

        /// <summary>
        /// 执行命令并返回值(此处对DBNull和Null已做处理返回"",无需多次判断)
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parmsLst"></param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameterInfo[] parmsLst)
        {
            DbCommand command = BuildCommand(commandType, commandText, parmsLst);

            object result = Db.ExecuteScalar(command);

            if (result == null || result == DBNull.Value)
            {
                return string.Empty;
            }

            return result;
        }

        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            return ExecuteReader(commandType, commandText, null);
        }

        /// <summary>
        /// 执行命令并返回DataReader
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parmsLst"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(CommandType commandType, string commandText, params DbParameterInfo[] parmsLst)
        {
            DbCommand command = BuildCommand(commandType, commandText, parmsLst);

            //IDataReader dataReader = Db.ExecuteReader(command);
            DbConnection cn = this.CreateConnection();
            command.Connection = cn;
            cn.Open();
            IDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            return dataReader;
        }

        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            return ExecuteDataSet(commandType, commandText, null);
        }

        /// <summary>
        /// 执行命令并返回DataSet
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parmsLst"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText, params DbParameterInfo[] parmsLst)
        {
            DbCommand command = BuildCommand(commandType, commandText, parmsLst);

            DataSet res = Db.ExecuteDataSet(command);

            return res;
        }
        #endregion

        #region 事务

        /// <summary>
        /// 执行事务,此时不关闭连接
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbCommand command, DbTransaction tran)
        {
            return Db.ExecuteNonQuery(command, tran);
        }

        /// <summary>
        /// 在事务中执行多个命令
        /// </summary>
        /// <param name="commandLst"></param>
        public void ExecuteSqlList(DbCommand[] commandLst)
        {
            using (DbConnection cn = Db.CreateConnection())
            {
                cn.Open();
                DbTransaction tran = cn.BeginTransaction();

                try
                {
                    foreach (DbCommand cmd in commandLst)
                    {
                        Db.ExecuteNonQuery(cmd, tran);
                    }
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                    throw e;
                }
            }
        }

        /// <summary>
        /// 执行分布式事务,前提要是开启DCOM服务
        /// </summary>
        /// <param name="commandLst"></param>
        public void ExecuteDistributeQuery(IDictionary<DbCommand, DataAccess> commandLst)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (DbCommand cmd in commandLst.Keys)
                {
                    Database db = commandLst[cmd].Db;
                    db.ExecuteNonQuery(cmd);
                }
                scope.Complete();
            }
        }
        #endregion

        #region Command
        public DbConnection CreateConnection()
        {
            return Db.CreateConnection();
        }

        public DbCommand BuildCommand(CommandType commandType, string commandText, DbParameterInfo[] parmsLst)
        {
            DbCommand command = null;
            if (commandType == CommandType.StoredProcedure)
            {
                command = Db.GetStoredProcCommand(commandText);
            }
            else
            {
                command = Db.GetSqlStringCommand(commandText);
            }

            if (parmsLst != null)
            {
                foreach (DbParameterInfo parm in parmsLst)
                {
                    parm.ParameterName = BuildParameterName(parm.ParameterName);
                    //Db.AddParameter(command, parm.ParameterName, parm.DbType, parm.Size, parm.Direction, false, 0, 0, string.Empty, DataRowVersion.Default, parm.Value);
                    command.Parameters.Add(parm.Parm);
                }
            }
            return command;
        }

        public string BuildParameterName(string name)
        {
            return Db.BuildParameterName(name);
        }

        /// <summary>
        /// 处理output参数的返回值
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parmsLst"></param>
        private void ProcessParms(DbCommand cmd, DbParameterInfo[] parmsLst)
        {
            if (parmsLst == null)
                return;

            foreach (DbParameterInfo param in parmsLst)
            {
                if (param.Direction == ParameterDirection.Output || param.Direction == ParameterDirection.InputOutput)
                {
                    param.Value = Db.GetParameterValue(cmd, param.ParameterName);
                }
            }
        }
        #endregion
    }
}
