using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using DALHelper;
using CommonHelper;

namespace Model
{
    public abstract class BaseEntity<T> where T : BaseEntity<T>, new()
    {
        #region 验证信息
        /// <summary>
        /// 验证后实体的错误信息,无错则为空
        /// </summary>
        public string ValidateMessage { get; protected set; }

        /// <summary>
        /// 验证实体
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            var validationResults = Validation.Validate<T>(this as T);
            if (!validationResults.IsValid)
            {
                foreach (var result in validationResults)
                {
                    ValidateMessage += string.Format("{0}:{1}\n", result.Key, result.Message);
                }
                return false;
            }

            return true;
        }
        #endregion

        #region 抽象
        /// <summary>
        /// 主键名
        /// </summary>
        protected abstract string primaryKey { get; }

        /// <summary>
        /// 查询的Sql
        /// </summary>
        protected abstract string baseSql
        {
            get;
        }

        /// <summary>
        /// 根据baseSql执行返回的dataReader进行创建实体
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        protected abstract T SetEntity(IDataReader dataReader);

        private readonly static T ins = new T();
        #endregion

        #region 获取实体
        /// <summary>
        /// 根据主键值获取实体
        /// </summary>
        /// <param name="colValue"></param>
        /// <returns></returns>
        public static T Get(object colValue)
        {
            if (string.IsNullOrEmpty(ins.primaryKey))
            {
                throw new ArgumentNullException("primaryKey未设置");
            }

            ICriteria criteria = CriteriaHelper.CreateCriteria().Add(ins.primaryKey, colValue);

            IList<T> retLst = GetAll(criteria);
            if (retLst.Count != 0)
            {
                return retLst[0];
            }

            return null;
        }

        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IList<T> Get(ICriteria criteria)
        {
            return GetAll(criteria);
        }


        /// <summary>
        /// 根据自己输入的条件进行过滤,条件格式 a={0} and b={1}
        /// </summary>
        /// <param name="queryCon"></param>
        /// <param name="arrVal">参数值 要对应</param>
        /// <returns></returns>
        public static IList<T> GetList(string queryCon, params object[] arrVal)
        {
            List<DbParameterInfo> parmsLst = new List<DbParameterInfo>();
            ArrayList nameLst = new ArrayList();
            for (int i = 0; i < arrVal.Length; i++)
            {
                string name = DataAccessManger.DefaultConnection.BuildParameterName("a" + i);
                nameLst.Add(name);
                parmsLst.Add(DbParameterHelper.GetDbParameter(name, DbParameterHelper.SystemTypeToDbType(arrVal[i].GetType()), arrVal[i]));
            }

            string sql = ins.baseSql + " where " + string.Format(queryCon, nameLst.ToArray());

            return GetEntityList(sql, parmsLst.ToArray());
        }

        public static IList<T> GetTop(int count)
        {
            return GetTop(count, null);
        }

        /// <summary>
        /// 取前N条
        /// </summary>
        /// <param name="count"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IList<T> GetTop(int count, ICriteria criteria)
        {
            List<DbParameterInfo> parmsLst = new List<DbParameterInfo>();
            string queryCon = criteria == null ? "1=1" : criteria.GetCriteria(parmsLst);

            string sql = ins.baseSql + " where " + queryCon;
            sql = sql.Insert(sql.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) + 6, " Top " + count);

            return GetEntityList(sql, parmsLst.ToArray());
        }

        public static IList<T> GetPage(int pageIndex, int pageCount, out int recordCount)
        {
            return GetPage(pageIndex, pageCount, null,out recordCount);
        }

        /// <summary>
        /// 分页去N条
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IList<T> GetPage(int pageIndex, int pageCount, ICriteria criteria, out int recordCount)
        {
            List<DbParameterInfo> parmsLst = new List<DbParameterInfo>();
            string queryCon = criteria == null ? "1=1" : criteria.GetCriteria(parmsLst);

            string orderBy;
            int lastIndex = queryCon.LastIndexOf("ORDER", StringComparison.OrdinalIgnoreCase);
            if (lastIndex != -1)
            {
                orderBy = queryCon.Substring(lastIndex);
                queryCon = queryCon.Remove(lastIndex);
            }
            else
            {
                orderBy = "ORDER BY " + ins.primaryKey;
            }
            string sql = ins.baseSql + " where " + queryCon;
            int pageFirst = (pageIndex - 1) * pageCount + 1;
            int pageEnd = pageFirst + pageCount - 1;
            sql = "SELECT * FROM (" + sql.Insert(sql.IndexOf("FROM", StringComparison.OrdinalIgnoreCase) - 1, string.Format(",ROW_NUMBER() OVER({0}) NumberIndex", orderBy))
                + ") a WHERE NumberIndex BETWEEN " + pageFirst + " AND " + pageEnd;
            sql += " SELECT @RecordCount=COUNT(1) FROM (" + ins.baseSql + " where " + queryCon + ") a";

            parmsLst.Add(DbParameterHelper.GetDbParameter("RecordCount", DbType.Int32, ParameterDirection.Output, 0));
            var retLst = GetEntityList(sql, parmsLst.ToArray());
            recordCount = TypeHelper.ToInt(parmsLst[parmsLst.Count - 1].Value);
            return retLst;
        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public static IList<T> GetAll()
        {
            return GetAll(null);
        }

        private static IList<T> GetAll(ICriteria criteria)
        {
            List<DbParameterInfo> parmsLst = new List<DbParameterInfo>();
            string queryCon = criteria == null ? "1=1" : criteria.GetCriteria(parmsLst);

            string sql = ins.baseSql + " where " + queryCon;

            return GetEntityList(sql, parmsLst.ToArray());
        }

        private static IList<T> GetEntityList(string sql, DbParameterInfo[] parms)
        {
            List<T> retLst = new List<T>();
            using (IDataReader reader = DataAccessManger.DefaultConnection.ExecuteReader(CommandType.Text, sql, parms))
            {
                while (reader.Read())
                {
                    T model = ins.SetEntity(reader);
                    retLst.Add(model);
                }
            }
            return retLst;
        }

        /// <summary>
        /// 根据单条件判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsExists(string colName, object colValue)
        {
            return IsExists(CriteriaHelper.CreateCriteria().Add(colName, colValue));
        }

        public static bool IsExists(ICriteria criteria)
        {
            List<DbParameterInfo> parmsLst = new List<DbParameterInfo>();
            string queryCon = criteria == null ? "1=1" : criteria.GetCriteria(parmsLst);
            string tableName = typeof(T).Name;
            string sql = string.Format("select top 1 1 from {0} where {1}", tableName, queryCon);

            object res = DataAccessManger.DefaultConnection.ExecuteScalar(CommandType.Text, sql, parmsLst.ToArray());
            return !res.Equals(string.Empty);
        }

        /// <summary>
        /// 延迟加载列
        /// </summary>
        /// <param name="colName"></param>
        public void LazyLoad(params string[] colName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            foreach (string str in colName)
            {
                sb.Append(str);
                sb.Append(",");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            Type t = this.GetType();
            object keyValue = t.GetProperty(primaryKey).GetValue(this, null);
            sb.AppendFormat(" FROM {0} WHERE {1}={2}", t.Name, primaryKey, DataAccessManger.DefaultConnection.BuildParameterName(primaryKey));
            DbParameterInfo parm = DbParameterHelper.GetDbParameter(primaryKey, DbParameterHelper.SystemTypeToDbType(keyValue.GetType()), keyValue);

            using (IDataReader dataReader = DataAccessManger.DefaultConnection.ExecuteReader(CommandType.Text, sb.ToString(), parm))
            {
                if (dataReader.Read())
                {
                    foreach (string str in colName)
                    {
                        PropertyInfo p = t.GetProperty(str);
                        p.SetValue(this, dataReader[str], null);
                    }
                }
            }
        }
        #endregion
    }

}
