using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Data;
using System.Web.UI.WebControls;

using FastReflectionLib;
namespace CommonHelper
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtendMethod
    {
        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> lst = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T model = Activator.CreateInstance<T>();
                WebCommon.DataRowToModel<T>(dr, model);
                lst.Add(model);
            }

            return lst;
        }

        /// <summary>
        /// 将Enum类型直接绑定到DropDownList上
        /// </summary>
        /// <param name="ddl">DropDownList实例</param>
        public static void SetEnumSource<T>(this DropDownList ddl)
        {
            Type oType = typeof(T);
            if (!oType.IsEnum)
            {
                throw new Exception("SetEnumSource must be have a enum type parameters!");
            }
            foreach (string s in Enum.GetNames(oType))
            {
                FieldInfo mi = oType.GetField(s);
                EnumMemberAttribute ea = (EnumMemberAttribute)Attribute.GetCustomAttribute(mi, typeof(EnumMemberAttribute));
                string name = ea == null ? s : ea.Value; //如果没有Attribute 的话就用Name来做；

                int value = (int)Enum.Parse(oType, s, true);
                ddl.Items.Add(new ListItem(name, value.ToString()));
            }
        }
        /// <summary>
        /// 将Enum类型直接绑定到DropDownList上
        /// </summary>
        /// <param name="ddl">DropDownList实例</param>
        /// <param name="DefaultName">是否有默认值,如果默认值不在Enum中的话，就加入空值的Item</param>
        public static void SetEnumSource<T>(this DropDownList ddl, string DefaultName)
        {
            Type oType = typeof(T);
            bool hasDefault = false;
            if (!oType.IsEnum)
            {
                throw new Exception("SetEnumSource must be have a enum type parameters!");
            }
            foreach (string s in Enum.GetNames(oType))
            {
                FieldInfo mi = oType.GetField(s);
                EnumMemberAttribute ea = (EnumMemberAttribute)Attribute.GetCustomAttribute(mi, typeof(EnumMemberAttribute));
                string name = ea == null ? s : ea.Value; //如果没有Attribute 的话就用Name来做；

                int value = (int)Enum.Parse(oType, s, true);
                ListItem item = new ListItem(name, value.ToString());
                if (name == DefaultName)
                {
                    item.Selected = true;
                    hasDefault = true;
                }
                ddl.Items.Add(new ListItem(name, value.ToString()));
            }
            if (!hasDefault)
            {
                ddl.Items.Insert(0, new ListItem(DefaultName, ""));
            }
        }

        /// <summary>
        /// DataSet直接加载xml
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="xml"></param>
        public static void LoadXml(this DataSet ds, string xml)
        {
            System.IO.StringReader sr = new System.IO.StringReader(xml);
            ds.ReadXml(sr, XmlReadMode.Auto);
        }

        /// <summary>
        ///DataTable转换为string数组(只有一列)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="xml"></param>
        public static string[] ToStringList(this DataTable dt)
        {
            List<string> lst = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(dr[0].ToString());
            }
            return lst.ToArray();
        }

        /// <summary>
        /// 增加一自增列（种子为1）
        /// </summary>
        /// <param name="dt"></param>
        public static void AddAutoIncrement(this DataTable dt,string colName)
        {
            DataColumn dc = new DataColumn(colName, typeof(int));
            //dc.AutoIncrement = true;
            //dc.AutoIncrementSeed = 1;
            //dc.AutoIncrementStep = 1;
            dt.Columns.Add(dc);

            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                dt.Rows[i - 1][dc] = i;
            }
        }

        private static IEnumerable<TSource> WhereIterator<TSource,T>(IEnumerable<TSource> source, T arg, Func<TSource, T, bool> predicate)
        {
            foreach (TSource iteratorVariable1 in source)
            {
                if (predicate.Invoke(iteratorVariable1, arg))
                {
                    yield return iteratorVariable1;
                }
            }
        }

        public static IEnumerable<TSource> Where<TSource,T>(this IEnumerable<TSource> source, T arg, Func<TSource, T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return WhereIterator<TSource,T>(source, arg, predicate);
        }
    }
}
