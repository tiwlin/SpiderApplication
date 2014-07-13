using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DALHelper;
namespace Model
{
    public class CriteriaHelper
    {
        public static ICriteria CreateCriteria()
        {
            return new SqlCriteria();
        }
    }

    public interface ICriteria
    {
        string GetCriteria(List<DbParameterInfo> parmLst);

        /// <summary>
        /// 用此条件时用Entity的Name,此时需保证数据库和实体的名称对应
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ICriteria Add(string key, object value);

        /// <summary>
        /// 暂时只支持key字段为整形的情况
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ICriteria In(string key, object value);

        ICriteria Upper(string key, object value);

        ICriteria Lower(string key, object value);

        ICriteria Not(string key, object value);

        ICriteria OrderBy(string colName);

        ICriteria OrderByDesending(string colName);
    }

    /// <summary>
    /// 简单的查询实现
    /// </summary>
    public class SqlCriteria : ICriteria
    {
        //暂时这样，后期整合一起 todo
        private Hashtable queryHt = Hashtable.Synchronized(new Hashtable());
        private Hashtable inHt = Hashtable.Synchronized(new Hashtable());
        private Hashtable upperHt = Hashtable.Synchronized(new Hashtable());
        private Hashtable notHt = Hashtable.Synchronized(new Hashtable());
        private string orderBy;
        private string orderByDes;



        #region ICriteria 成员

        public string GetCriteria(List<DbParameterInfo> parmLst)
        {
            string queryCon = "1=1";
            foreach (string key in queryHt.Keys)
            {
                queryCon += string.Format(" and {0}={1}", key, DataAccessManger.DefaultConnection.BuildParameterName(key));
                parmLst.Add(DbParameterHelper.GetDbParameter(key, DbParameterHelper.SystemTypeToDbType(queryHt[key].GetType()), queryHt[key]));
            }

            foreach (string key in inHt.Keys)
            {
                queryCon += string.Format(" and {0} in (", key);

                string[] arr = inHt[key].ToString().Split(',');
                foreach (string s in arr)
                {
                    queryCon += Convert.ToInt32(s) + ",";
                }

                if (arr.Length > 0) queryCon = queryCon.Remove(queryCon.Length - 1, 1);
                queryCon += ")";
            }

            foreach (string key in upperHt.Keys)
            {
                queryCon += string.Format(" and {0}>{1}", key, DataAccessManger.DefaultConnection.BuildParameterName(key));
                parmLst.Add(DbParameterHelper.GetDbParameter(key, DbParameterHelper.SystemTypeToDbType(upperHt[key].GetType()), upperHt[key]));
            }

            foreach (string key in notHt.Keys)
            {
                queryCon += string.Format(" and {0}!={1}", key, DataAccessManger.DefaultConnection.BuildParameterName(key));
                parmLst.Add(DbParameterHelper.GetDbParameter(key, DbParameterHelper.SystemTypeToDbType(notHt[key].GetType()), notHt[key]));
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                queryCon += " ORDER BY " + orderBy;
            }
            else if (!string.IsNullOrEmpty(orderByDes))
            {
                queryCon += " ORDER BY " + orderByDes + " desc";
            }

            return queryCon;
        }

        public ICriteria Add(string key, object value)
        {
            queryHt.Add(key, value);
            return this;
        }

        public ICriteria In(string key, object value)
        {
            inHt.Add(key, value);
            return this;
        }

        public ICriteria Upper(string key, object value)
        {
            upperHt.Add(key, value);
            return this;
        }

        public ICriteria Lower(string key, object value)
        {
            throw new NotImplementedException();
        }

        public ICriteria Not(string key, object value)
        {
            notHt.Add(key, value);
            return this;
        }

        public ICriteria OrderBy(string colName)
        {
            orderBy = colName;
            return this;
        }

        public ICriteria OrderByDesending(string colName)
        {
            orderByDes = colName;
            return this;
        }
        #endregion
    }

}
