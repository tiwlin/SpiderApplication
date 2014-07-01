using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Data;

using System.Xml.Linq;
using System.Runtime.Serialization;

namespace CommonHelper
{
    /// <summary>
    /// 处理对象序列化和反序列化
    /// </summary>
    public sealed class SerializeHelper
    {
        /// <summary>
        /// 将 XmlDocument 类型数据转换为 String 数据
        /// </summary>
        /// <param name="xmldoc">xmldocument 参数</param>
        /// <returns>返回xml字符串</returns>
        public static string XmlDocToXmlString(XmlDocument xmldoc)
        {
            try
            {
                if (xmldoc == null) return null;

                return xmldoc.InnerXml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将xml格式的string数据转换成xmldocument
        /// </summary>
        /// <param name="xmlstring">xml格式string</param>
        /// <returns>xmldocument</returns>
        public static XmlDocument XmlStringToXmlDoc(string xmlstring)
        {
            try
            {
                // 如果传入的字符为空，返回null
                if (xmlstring == null || xmlstring.Length == 0) return null;

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xmlstring);

                return xmldoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将xml格式的string串转换为特定类型的实体对象
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="xmlstring">xml格式的string串</param>
        /// <returns>返回泛型类型的实体对象</returns>
        public static T XmlStringToObject<T>(string xmlstring) where T : class
        {
            try
            {
                // 如果传入的字符为空，返回null
                if (xmlstring == null || xmlstring.Length == 0) return null;

                MemoryStream ms;
                ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlstring));

                XmlSerializer xs = new XmlSerializer(typeof(T));
                T t = (T)xs.Deserialize(ms);

                ms.Close();

                return t;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将xml格式文档转换特定类型的实体对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="xmldoc"></param>
        /// <returns>返回泛型类型的实体对象</returns>
        public static T XmlDocToObject<T>(XmlDocument xmldoc) where T : class
        {
            try
            {
                // 先调用xml to string方法，再调用 string to object方法
                return XmlStringToObject<T>(XmlDocToXmlString(xmldoc));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将特定实体对象序列化成xml格式的字符串
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="t">实体对象</param>
        /// <returns>返回xml格式的string</returns>
        public static string ObjectToXmlString<T>(T t) where T : class
        {
            try
            {
                if (t == null) return null;

                MemoryStream ms = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(ms, t);

                // 内存流转成字符串
                //StringBuilder sb = new StringBuilder();
                //foreach (byte b in ms.ToArray())
                //{
                //    sb.Append((char)b);
                //}

                //string result = System.Text.Encoding.GetEncoding("GB2312").GetString(ms.ToArray());
                string result = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                ms.Close();

                //return sb.ToString();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将特定实体对象序列化成xml格式的文档
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="t">实体对象</param>
        /// <returns>返回xml格式的文档</returns>
        public static XmlDocument ObjectToXmlDoc<T>(T t) where T : class
        {
            try
            {
                return XmlStringToXmlDoc(ObjectToXmlString<T>(t));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 提取一个节点下所有一级子节点或所有属性并转化成DataTable保存数据
        /// <summary>
        /// 提取一个节点下所有一级子节点或所有属性并转化成DataTable保存数据
        /// </summary>
        /// <param name="xlist">节点集合 XmlNodeList</param>
        /// <param name="type">0为提取所有属性，1为提取所有子节点</param>
        /// <returns>DataTable</returns>
        public static DataTable ConvertXmlNodeListDataTable(XmlNodeList xlist, int type)
        {
            DataTable Dt = new DataTable();
            DataRow Dr;

            for (int i = 0; i < xlist.Count; i++)
            {
                Dr = Dt.NewRow();
                XmlElement xe = (XmlElement)xlist.Item(i);

                if (type == 0)
                {
                    for (int j = 0; j < xe.Attributes.Count; j++)
                    {
                        if (!Dt.Columns.Contains("@" + xe.Attributes[j].Name))
                        {
                            Dt.Columns.Add("@" + xe.Attributes[j].Name);
                        }

                        Dr["@" + xe.Attributes[j].Name] = xe.Attributes[j].Value;
                    }
                }
                else if (type == 1)
                {
                    for (int j = 0; j < xe.ChildNodes.Count; j++)
                    {
                        if (!Dt.Columns.Contains(xe.ChildNodes.Item(j).Name))
                        {
                            Dt.Columns.Add(xe.ChildNodes.Item(j).Name);
                        }

                        Dr[xe.ChildNodes.Item(j).Name] = xe.ChildNodes.Item(j).InnerText;
                    }
                }

                Dt.Rows.Add(Dr);
            }

            return Dt;
        }
        #endregion
    }

    /// <summary>
    /// 公用方法库
    /// </summary>
    public sealed class CommonHelper
    {
        #region Clone..

        /// <summary>
        /// 返回clone的实体给调用方
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="obj">T类型</param>
        /// <returns>返回T类型的新实体</returns>        
        public static T Clone<T>(T obj) where T : class
        {
            try
            {
                Type t = obj.GetType();
                T newObj = (T)Activator.CreateInstance(t);

                foreach (PropertyInfo pi in t.GetProperties())
                {
                    try
                    {
                        pi.SetValue(newObj, pi.GetValue(obj, null), null);
                    }
                    catch { }
                }
                foreach (FieldInfo fi in t.GetFields())
                {
                    try
                    {
                        fi.SetValue(newObj, fi.GetValue(obj));
                    }
                    catch { }
                }

                return newObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Copy/Clone方法，将Source的数据拷贝到Target对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Clone<T>(T source, T target) where T : class
        {
            try
            {
                Type t = source.GetType();

                if (target == null)
                { return; }

                foreach (PropertyInfo pi in t.GetProperties())
                {
                    try
                    {
                        pi.SetValue(target, pi.GetValue(source, null), null);
                    }
                    catch { }
                }
                foreach (FieldInfo fi in t.GetFields())
                {
                    try
                    {
                        fi.SetValue(target, fi.GetValue(source));
                    }
                    catch { }
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Copy指定列的数据新对象
        /// </summary>
        /// <typeparam name="T">对象类型，仅限于类</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">新对象，传入对象必须已实例化</param>
        /// <param name="propertyNames">Copy列数组</param>
        public static void Clone<T>(T source, T target, string[] propertyNames) where T : class
        {
            try
            {
                Type t = source.GetType();

                if (target == null)
                { return; }

                foreach (PropertyInfo pi in t.GetProperties())
                {
                    try
                    {
                        if (propertyNames.Where(o => o.Trim().ToLower() == pi.Name.Trim().ToLower()).Count() > 0)
                        {
                            pi.SetValue(target, pi.GetValue(source, null), null);
                        }

                    }
                    catch { }
                }
                foreach (FieldInfo fi in t.GetFields())
                {
                    try
                    {
                        if (propertyNames.Where(o => o.Trim().ToLower() == fi.Name.Trim().ToLower()).Count() > 0)
                        {
                            fi.SetValue(target, fi.GetValue(source));
                        }
                    }
                    catch { }
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 两个不同类型的对象数据复制
        /// </summary>
        /// <typeparam name="TSource">数据对象类</typeparam>
        /// <typeparam name="TTarget">要赋值的对象类</typeparam>
        /// <param name="source">数据对象</param>
        /// <returns></returns>
        public static TTarget Clone<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class
        {
            if (source == null)
            {
                return null;
            }

            try
            {
                Type tSource = source.GetType();
                Type tTarget = typeof(TTarget);

                // 催化剂实例对象
                TTarget target = (TTarget)Activator.CreateInstance(tTarget);

                PropertyInfo sProperty;
                foreach (PropertyInfo pi in tTarget.GetProperties())
                {
                    try
                    {
                        sProperty = tSource.GetProperty(pi.Name);

                        pi.SetValue(target, sProperty.GetValue(source, null), null);
                    }
                    catch { }
                }
                FieldInfo sfield;
                foreach (FieldInfo fi in tTarget.GetFields())
                {
                    try
                    {
                        sfield = tSource.GetField(fi.Name);

                        fi.SetValue(target, sfield.GetValue(source));
                    }
                    catch { }
                }
                return target;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 两个不同类型同属性的对象数组复制
        /// </summary>
        /// <param name="arrSourceObj">数据源数组对象</param>
        /// <returns>返回的指定类型的数组对象</returns>
        public static TTarget[] CloneArrayObject<TSource, TTarget>(TSource[] arrSourceObj)
            where TSource : class
            where TTarget : class
        {
            if (arrSourceObj == null)
            {
                return null;
            }
            //复制返回的数组TTarget Clone<TSource, TTarget>(TSource source) where TSource : class where TTarget :class
            List<TTarget> iList = new List<TTarget>();

            for (int i = 0; i < arrSourceObj.Length; i++)
            {
                TTarget newObj;
                newObj = CommonHelper.Clone<TSource, TTarget>(arrSourceObj[i]);
                iList.Add(newObj);
            }

            return iList.ToArray<TTarget>();
        }

        #endregion

        #region 反映处理 ..

        /// <summary>
        /// 通过属性名，获得实例中对应属性名的值
        /// </summary>
        /// <typeparam name="T">实例类型，必须为类</typeparam>
        /// <param name="t">实例</param>
        /// <param name="fieldName">属性名</param>
        /// <returns></returns>
        public static object GetValue<T>(T t, string fieldName) where T : class
        {
            try
            {
                if (fieldName.Trim().Length < 1) return null;

                Type type = t.GetType();

                object obj = null;

                PropertyInfo pi;
                pi = type.GetProperty(fieldName.Trim());
                if (pi == null) pi = type.GetProperty(fieldName.Trim().ToLower());

                if (pi != null)
                {
                    obj = pi.GetValue(t, null);
                }
                else
                {
                    // 获取字段的值                    
                    FieldInfo fi;
                    fi = type.GetField(fieldName.Trim());                    
                    if (fi == null) fi = type.GetField(fieldName.Trim().ToLower());
                    if (fi != null)
                    {
                        obj = fi.GetValue(t);
                    }
                }

                // 没有对应属性
                return obj;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 通过属性名，获得ORM实例中对应属性名的值
        /// </summary>
        /// <typeparam name="T">实例类型，必须为类</typeparam>
        /// <param name="t">实例</param>
        /// <param name="fieldName">属性名</param>
        /// <returns></returns>
        public static object GetTableValue<T>(T t, string fieldName) where T : class
        {
            try
            {
                if (fieldName.Trim().Length < 1) return string.Empty;

                Type type = t.GetType();

                fieldName = fieldName.Trim().ToLower();
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (pi.Name.Trim().ToLower() != fieldName)
                    { continue; }

                    object[] attrs = pi.GetCustomAttributes(false);
                    foreach (object attr in attrs)
                    {
                        if (attr is System.Data.Linq.Mapping.ColumnAttribute)
                        {
                            System.Data.Linq.Mapping.ColumnAttribute column = (System.Data.Linq.Mapping.ColumnAttribute)attr;

                            if (column.Storage.Trim().ToLower() == "_" + fieldName)
                            { return pi.GetValue(t, null); }
                        }
                    }
                }

                // 没有任何条件
                return null;
            }
            catch
            { return null; }
        }

        /// <summary>
        /// 获取实体对应表的字段列，只取实体对应数据库表中的列串
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetTableFieldList(Type t)
        {
            try
            {
                if (t == null) return string.Empty;

                StringBuilder sb = new StringBuilder();

                foreach (PropertyInfo pi in t.GetProperties())
                {
                    object[] attrs = pi.GetCustomAttributes(false);
                    foreach (object attr in attrs)
                    {
                        if (attr is System.Data.Linq.Mapping.ColumnAttribute ||
                            attr is DataMemberAttribute)
                        {
                            if (sb.Length > 0)
                            { sb.Append("|"); }

                            // 追加属性名
                            sb.Append(pi.Name);
                        }
                    }
                }

                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取映射值
        /// </summary>
        /// <param name="paraValue">传入参数值</param>
        /// <param name="paraMappingIndex">传入参数索号，从0开始</param>
        /// <param name="MappingString">格式：“1,2|3,4”</param>
        /// <returns></returns>
        public static string GetMappingValue(string paraValue, int paraMappingIndex, string MappingString)
        {
            try
            {
                if (MappingString.Length == 0) return string.Empty;

                string[] sMapping = MappingString.Split('|');

                for (int i = 0; i < sMapping.Length; i++)
                {
                    string[] sValues = sMapping[i].Split(',');

                    if (sValues.Length < 2)
                    { if (paraValue.Trim() == sValues[0].Trim()) return paraValue.Trim(); }
                    else
                    {
                        if (paraMappingIndex == 0)
                        { if (paraValue.Trim() == sValues[0].Trim()) return sValues[1]; }
                        else
                        { if (paraValue.Trim() == sValues[1].Trim()) return sValues[0]; }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return string.Empty;
        }

        #endregion

        #region List..

        /// <summary>
        /// 获取List中符合条件的条一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstInfos"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T First<T>(List<T> lstInfos, string fieldName, object value) where T : class
        {
            if (lstInfos == null || lstInfos.Count == 0) return null;

            PropertyInfo pi = null;
            foreach (T info in lstInfos)
            {
                try
                {
                    // 获取属性
                    if (pi == null)
                    { pi = info.GetType().GetProperty(fieldName); }

                    // 获取属性值是否与结点中值一致，如一致返回实例
                    if (pi.GetValue(info, null).ToString() == value.ToString())
                    { return info; }
                }
                catch { }
            }

            return null;
        }

        #endregion

        #region Xml方法..

        #region 获取xml结点名字串..

        /// <summary>
        /// 获取xmlNode二级结点的字符串
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static string GetNodeNameList(XElement xmlNode)
        {
            return CommonHelper.GetNodeNameList(xmlNode, "|");
        }

        /// <summary>
        /// 只能是二级结点的xml结点
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="sSplit"></param>
        /// <returns></returns>
        public static string GetNodeNameList(XElement xmlNode, string sSplit)
        {
            try
            {
                if (xmlNode == null) return "";

                StringBuilder sb = new StringBuilder();

                foreach (XElement item in xmlNode.Elements())
                {
                    if (sb.Length > 0) sb.Append(sSplit);
                    sb.Append(item.Name);
                }

                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region 获取xml结点..

        /// <summary>
        /// 通过结点名和值获取结点信息
        /// </summary>
        /// <param name="XName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement GetXElement(string XName, object value)
        {
            try
            {
                return new XElement(XName.Trim(), value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将xml字符串
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XElement GetXElementByString(string xml)
        {
            try
            {
                //// 把xml字符串转换成结点
                //Stream stream = new MemoryStream(Encoding.Default.GetBytes(xml));
                //XmlReader reader = XmlReader.Create(stream);
                //XElement xmlroot = XElement.Load(reader);

                //return xmlroot;

                return GetXElementByString(xml, Encoding.Default);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将xml字符串
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static XElement GetXElementByString(string xml, Encoding encoding)
        {
            try
            {
                if (!xml.ToLower().StartsWith("<?xml version"))
                { xml = GetXmlString(xml, encoding); }

                // 把xml字符串转换成结点
                Stream stream = new MemoryStream(encoding.GetBytes(xml));
                XmlReader reader = XmlReader.Create(stream);
                XElement xmlroot = XElement.Load(reader);

                return xmlroot;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打开xml文件，生成XElement对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XElement GetXElementByFile(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                XElement xml;

                // 把xml字符串转换成结点
                using (Stream stream = new FileStream(path, FileMode.Open))
                {
                    using (XmlReader reader = XmlReader.Create(stream))
                    {
                        xml = XElement.Load(reader);
                    }
                }

                return xml;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过DataTable生成xml结点
        /// </summary>
        /// <param name="dt">datatable生成xml结点</param>
        /// <param name="rowName">行的名称</param>
        /// <returns></returns>
        public static XElement GetXElementByDataTable(DataTable dt, string rowName)
        {
            try
            {
                return GetXElementByDataTable(dt, rowName, "Root");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 通过DataTable生成xml结点
        /// </summary>
        /// <param name="dt">datatable生成xml结点</param>
        /// <param name="rowName">行的名称</param>
        /// <param name="rootName">根结点名称</param>
        /// <returns></returns>
        public static XElement GetXElementByDataTable(DataTable dt, string rowName, string rootName)
        {
            try
            {
                if (dt == null) return null;
                if (dt.Rows.Count == 0) return null;

                if (rowName.Trim().Length == 0) rowName = "DataRow";
                if (rootName.Trim().Length == 0) rootName = "Root";

                XElement xmlRoot = new XElement(rootName);
                XElement xmlRow;
                foreach (DataRow row in dt.Rows)
                {
                    xmlRow = new XElement(rowName);

                    foreach (DataColumn col in dt.Columns)
                    {
                        xmlRow.Add(new XElement(col.ColumnName, row[col.ColumnName].ToString()));
                    }

                    xmlRoot.Add(xmlRow);
                }

                return xmlRoot;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 获取XML结点..

        /// <summary>
        /// Landry add at: 2011-05-24
        /// Description: 返回实体列表对应列的xml节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        /// <param name="NodeName"></param>
        /// <param name="sFields"></param>
        /// <returns></returns>
        public static XElement GetXmlByList<T>(IEnumerable<T> entityList, string NodeName, string[] sFields) where T : class
        {
            return GetXmlByList(entityList, null, NodeName, sFields);
        }

        /// <summary>
        /// Landry add at: 2011-05-24
        /// Description: 返回实体列表对应列的xml节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityList"></param>
        /// <param name="NodeName"></param>
        /// <param name="sFields"></param>
        /// <returns></returns>
        public static XElement GetXmlByList<T>(IEnumerable<T> entityList, string rootNodeName, string NodeName, string[] sFields) where T : class
        {
            if (entityList == null || entityList.Count() == 0) { return null; }
            if (string.IsNullOrEmpty(rootNodeName)) { rootNodeName = "root"; }

            XElement result = new XElement(rootNodeName);
            foreach (var entity in entityList)
            {
                XElement tempNode = GetXml<T>(entity, NodeName, sFields, false);

                if (tempNode == null) { continue; }

                result.Add(tempNode);
            }

            return result;
        }

        /// <summary>
        /// 返回实体对应列的xml结点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="NodeName"></param>
        /// <param name="sFields"></param>
        /// <returns></returns>
        public static XElement GetXml<T>(T entity, string NodeName, string[] sFields) where T : class
        {
            try
            {
                return GetXml<T>(entity, NodeName, sFields, false);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取实体对应列的xml，实体列与xml结点列可不同
        /// Landry modify at: 2011-11-21
        /// Description: 调整有映射时，处理逻辑不正确的bug
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体实例</param>
        /// <param name="NodeName">结点名</param>
        /// <param name="sFields">实体列</param>
        /// <param name="isMapping">实体列与xml结点是否有映射，默认false；
        /// isMapping为false时，xml结点和实体列一致；
        /// isMapping为true时，strFidlds保存列表和结点名，用“,”分隔开，如"FieldName,NodeName"</param>
        /// <returns></returns>
        public static XElement GetXml<T>(T entity, string NodeName, string[] sFields, bool isMapping) where T : class
        {
            try
            {
                if (entity == null) return null;

                // 只有一个并为空时，获取数据库列
                if (sFields == null || sFields.Length == 0 || (sFields.Length == 1 && sFields[0].Trim().Length == 0))
                {
                    string fieldsStr = CommonHelper.GetTableFieldList(entity.GetType());
                    sFields = string.IsNullOrEmpty(fieldsStr) ? null : fieldsStr.Split('|'); 
                }

                XElement xmlInfo = new XElement(NodeName);

                string tempNodeName=string.Empty;
                object tempValue = null;

                #region 未传入列集合时，直接返回实体所有属性的XML值..

                // 未传入列时，返回实体的Xml
                Type type = entity.GetType();
                if (sFields == null || sFields.Length == 0)
                {
                    foreach (PropertyInfo pi in type.GetProperties())
                    {
                        tempNodeName = pi.Name;
                        tempValue = pi.GetValue(entity, null);

                        xmlInfo.Add(new XElement(tempNodeName, tempValue == null ? string.Empty : tempValue.ToString()));
                    }

                    // 直接返回
                    return xmlInfo;
                }

                #endregion

                // 遍历列集合，生成Xml
                string tempFieldName = string.Empty;
                for (int i = 0; i < sFields.Length; i++)
                {
                    // 设置结点名
                    if (!isMapping)
                    {
                        tempNodeName = sFields[i].Trim();
                        tempValue = CommonHelper.GetValue(entity, tempNodeName);
                        // 生成结点信息
                        xmlInfo.Add(new XElement(tempNodeName, tempValue == null ? string.Empty : tempValue.ToString()));
                    }
                    else
                    {
                        // 只有一个值或者第二项值为空
                        var tempFields = sFields[i].Trim().Split(',');
                        if (tempFields.Length == 1 || (tempFields.Length == 2 && tempFields[0].Length == 0))
                        {
                            tempNodeName = sFields[i].Trim();
                            tempFieldName = tempNodeName;
                        }
                        else
                        {
                            tempFieldName = tempFields[0];
                            tempNodeName = tempFields[1];
                        }
                        //if (sFields[i].Trim().Split(',').Length == 1
                        //    || (sFields[i].Trim().Split(',').Length == 2
                        //        && sFields[i].Trim().Split(',')[0].Trim().Length == 0))
                        //{ tempNodeName = sFields[i].Trim(); }
                        //else
                        //{ tempNodeName = sFields[i].Trim().Split(',')[1].Trim(); }

                        tempValue = CommonHelper.GetValue(entity, tempFieldName);
                        // 生成结点信息
                        xmlInfo.Add(new XElement(tempNodeName, tempValue == null ? string.Empty : tempValue.ToString()));
                    }

                }

                return xmlInfo;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取差异xml
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="compareEntity">当前对比对象</param>
        /// <param name="baseObj">基础对象</param>
        /// <param name="nodeName">结点名</param>
        /// <param name="sFields">字段名</param>
        /// <returns></returns>
        public static XElement GetXml<T>(T compareEntity, T baseEntity, string nodeName, string[] sFields) where T : class
        {
            try
            {
                if (compareEntity == null) return null;
                if (baseEntity == null) return null;
                if (sFields.Length == 0) return null;

                if (nodeName.Trim().Length == 0) nodeName = "Root";

                XElement xNode = null;

                Type type = compareEntity.GetType();
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (sFields.FirstOrDefault(f => f.Trim().ToLower() == pi.Name.Trim().ToLower()) == null)
                    { continue; }

                    if (pi.GetValue(compareEntity, null) != pi.GetValue(baseEntity, null))
                    {
                        if (xNode == null) xNode = new XElement(nodeName);
                        xNode.Add(GetXElement(pi.Name, pi.GetValue(compareEntity, null)));
                    }
                }

                return xNode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region DataTable转换成Xml..

        /// <summary>
        /// 通过DataTable获取xml结点
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowNodeName"></param>
        /// <returns></returns>
        public static XElement GetXml(DataTable dt, string rowNodeName)
        {
            return GetXml(dt, rowNodeName, null, false);
        }

        /// <summary>
        /// 通过DataTable获取xml结点
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rootNodeName"></param>
        /// <param name="rowNodeName"></param>
        /// <returns></returns>
        public static XElement GetXml(DataTable dt, string rootNodeName, string rowNodeName)
        {
            return GetXml(dt, rootNodeName, rowNodeName, null, false);
        }

        /// <summary>
        /// 通过DataTable获取xml结点
        /// </summary>
        /// <param name="dt">DataTable数据</param>
        /// <param name="rowNodeName">每行对应生成结点的nodeName</param>
        /// <param name="sFields">字段列</param>
        /// <param name="isMapping">实体列与xml结点是否有映射，默认false；
        /// isMapping为false时，xml结点和实体列一致；
        /// isMapping为true时，strFidlds保存列表和结点名，用“,”分隔开，如"FieldName,NodeName"</param>
        /// <returns></returns>
        public static XElement GetXml(DataTable dt, string rowNodeName, string[] sFields, bool isMapping)
        {
            return GetXml(dt, "root", rowNodeName, sFields, isMapping);
        }

        /// <summary>
        /// 通过DataTable获取xml结点
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rootNodeName">根结点名</param>
        /// <param name="rowNodeName">DataRow对应结点名</param>
        /// <param name="sFields"></param>
        /// <param name="isMapping">实体列与xml结点是否有映射，默认false；
        /// isMapping为false时，xml结点和实体列一致；
        /// isMapping为true时，strFidlds保存列表和结点名，用“,”分隔开，如"FieldName,NodeName"</param>
        /// <returns></returns>
        public static XElement GetXml(DataTable dt, string rootNodeName, string rowNodeName, string[] sFields, bool isMapping)
        {
            try
            {
                if (dt == null) return null;

                // 只有一个并为空时
                if (sFields == null || sFields.Length == 0 || (sFields.Length == 1 && sFields[0].Trim().Length == 0))
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataColumn column in dt.Columns)
                    {
                        if (sb.Length > 0) sb.Append("|");
                        sb.Append(column.ColumnName);

                        // 增加对应实体列
                        if (isMapping) sb.Append("," + column.ColumnName);
                    }

                    sFields = sb.ToString().Split('|');
                }

                XElement xmlRoot = new XElement(rootNodeName);

                XElement xmlNode;
                foreach (DataRow dr in dt.Rows)
                {
                    xmlNode = GetXml(dr, rowNodeName, sFields, isMapping);

                    if (xmlNode != null) xmlRoot.Add(xmlNode);
                }

                return xmlRoot;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 通过DataRow获取XElement结点
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="NodeName"></param>
        /// <param name="sFields"></param>
        /// <param name="isMapping">实体列与xml结点是否有映射，默认false；
        /// isMapping为false时，xml结点和实体列一致；
        /// isMapping为true时，strFidlds保存列表和结点名，用“,”分隔开，如"FieldName,NodeName"</param>
        /// <returns></returns>
        public static XElement GetXml(DataRow dr, string NodeName, string[] sFields, bool isMapping)
        {
            try
            {
                if (dr == null) return null;

                // 只有一个并为空时
                if (sFields.Length == 0 || (sFields.Length == 1 && sFields[0].Trim().Length == 0))
                { return null; }

                XElement xmlNode = new XElement(NodeName);

                if (!isMapping)
                {
                    for (int i = 0; i < sFields.Length; i++)
                    {
                        if (sFields[i].Trim().Length == 0) continue;

                        object obj = dr[sFields[i]];

                        xmlNode.Add(new XElement(sFields[i], obj == null ? "" : obj.ToString()));
                    }
                }
                else
                {
                    for (int i = 0; i < sFields.Length; i++)
                    {
                        if (sFields[i].Trim().Length == 0) continue;

                        object obj = dr[sFields[i]];
                        if (sFields[i].Trim().Split(',').Length == 1)
                        { xmlNode.Add(new XElement(sFields[i], obj == null ? "" : obj.ToString())); }
                        else
                        { xmlNode.Add(new XElement(sFields[i].Trim().Split(',')[1], obj == null ? "" : obj.ToString())); }
                    }
                }

                return xmlNode;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// 保存XElement对象到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static string SaveXElementToFile(string path, XElement xmlNode)
        {
            try
            {
                if (!File.Exists(path)) return null;

                // 把xml字符串转换成结点
                StreamWriter sw = File.CreateText(path);
                sw.Write(xmlNode.ToString());

                sw.Close();

                return "";
            }
            catch (Exception ex)
            {
                return "99|" + ex.Message;
            }
        }

        /// <summary>
        /// 批量增加XElement结点
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="xmlSubNodes"></param>
        public static void AddRange(XElement xmlNode, List<XElement> xmlSubNodes)
        {
            if (xmlNode == null) return;
            if (xmlSubNodes == null || xmlSubNodes.Count == 0) return;

            foreach (XElement item in xmlSubNodes)
            { xmlNode.Add(item); }

            return;
        }

        #region 获取xml字符串..

        /// <summary>
        /// 把结点转换成string类型，并将编码设置为“GB2312”
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static string GetXmlString(XElement xmlNode)
        {
            try
            {
                if (xmlNode == null) return "";

                return GetXmlString(xmlNode.ToString());
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 把结点转换成string类型，并将编码设置为“GB2312”
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string GetXmlString(string xml)
        {
            try
            {
                if (xml.Length == 0) return "";

                return "<?xml version=\"1.0\" encoding=\"GB2312\" ?> " + xml;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取xml结点的xml字符串
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetXmlString(XElement xmlNode, Encoding encoding)
        {
            try
            {
                if (xmlNode == null) return "";

                return GetXmlString(xmlNode.ToString(), encoding);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 按一定编码生成xml字符串
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetXmlString(string xml, Encoding encoding)
        {
            try
            {
                if (xml.Length == 0) return "";

                return "<?xml version=\"1.0\" encoding=\"" + encoding.HeaderName + "\" ?> " + xml;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region xml/实体操作（替换）..

        /// <summary>
        /// 用xml结点的内容替换实例，返回当前实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xmlNode"></param>
        public static void Replace<T>(ref T entity, XElement xmlNode) where T : class
        {
            if (entity == null) return;

            CommonHelper.Replace(ref entity, xmlNode, null);

            #region 旧代码 乐文斌 20101208 ...

            //Type type = entity.GetType();

            //XElement item;
            //foreach (PropertyInfo pi in type.GetProperties())
            //{
            //    try
            //    {
            //        // 是否有对应属性的值，有就赋值
            //        item = xmlNode.Element(pi.Name);
            //        if (item == null) item = xmlNode.Element(pi.Name.ToLower());
            //        if (item != null)
            //        {
            //            pi.SetValue(entity, (object)ConvertByType(pi.PropertyType, item.Value), null);
            //        }
            //    }
            //    catch { }
            //}
            //foreach (FieldInfo fi in type.GetFields())
            //{
            //    try
            //    {
            //        item = xmlNode.Element(fi.Name);
            //        if (item == null) item = xmlNode.Element(fi.Name.ToLower());
            //        if (item != null)
            //        {
            //            fi.SetValue(entity, (object)ConvertByType(fi.FieldType, item.Value));
            //        }
            //    }
            //    catch { }
            //}

            #endregion

            return;
        }

        public static void ReplaceList<T>(ref IList<T> entities, XElement xmlNode, string nodeName, string[] fields) where T : class, new()
        {
            if (entities == null || string.IsNullOrEmpty(nodeName) || xmlNode == null || xmlNode.Elements(nodeName).Count() == 0) { return; }
            foreach (var item in xmlNode.Elements(nodeName))
            {
                T tempT = new T();
                Replace(tempT, item, fields);
                entities.Add(tempT);
            }
        }

        /// <summary>
        /// 用xml结点指定列的内容替换实例，返回当前实例，如果列为空，替换所有结点内容；
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xmlNode"></param>
        /// <param name="fields"></param>
        public static void Replace<T>(ref T entity, XElement xmlNode, string[] fields) where T : class
        {
            try
            {
                if (entity == null) return;
                if (xmlNode == null) return;

                // 如果列数组为空，返回整个替换整个xml结点；
                if (fields == null || fields.Length == 0)
                {
                    // 获取XML结点中的结点名并生成替换列
                    fields = GetNodeNameList(xmlNode).Split('|');
                }
                
                XElement item;
                PropertyInfo pi;
                FieldInfo fi;
                Type type = entity.GetType();

                for (int i = 0; i < fields.Length; i++)
                {
                    try
                    {
                        item = xmlNode.Element(fields[i]);
                        // 默认兼容小写
                        if (item == null) item = xmlNode.Element(fields[i].ToLower());
                        if (item != null)
                        {
                            pi = type.GetProperty(fields[i]);
                            if (pi != null)
                            {
                                pi.SetValue(entity, (object)ConvertByType(pi.PropertyType, item.Value), null);
                            }
                            else
                            {
                                fi = type.GetField(fields[i]);
                                if (fi == null) continue;

                                fi.SetValue(entity, (object)ConvertByType(fi.FieldType, item.Value));
                            }
                        }

                    }
                    catch { }
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 用xml结点的内容替换实例，返回实体与传入实体共址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static T Replace<T>(T entity, XElement xmlNode) where T : class
        {
            // 调用基本方法
            return CommonHelper.Replace(entity, xmlNode, null);

            #region 旧代码 乐文斌 20101208 ..

            //Type type = entity.GetType();

            //// 可对所有属性做复制
            //T newEntity = Clone<T>(entity);

            //XElement item;
            //foreach (PropertyInfo pi in type.GetProperties())
            //{
            //    try
            //    {
            //        // 是否有对应属性的值，有就赋值
            //        item = xmlNode.Element(pi.Name);
            //        if (item == null) item = xmlNode.Element(pi.Name.ToLower());
            //        if (item != null)
            //        {
            //            pi.SetValue(newEntity, (object)ConvertByType(pi.PropertyType, item.Value), null);
            //        }
            //    }
            //    catch { }
            //}
            //foreach (FieldInfo fi in type.GetFields())
            //{
            //    try
            //    {
            //        item = xmlNode.Element(fi.Name);
            //        if (item == null) item = xmlNode.Element(fi.Name.ToLower());
            //        if (item != null)
            //        {
            //            fi.SetValue(newEntity, (object)ConvertByType(fi.FieldType, item.Value));
            //        }
            //    }
            //    catch { }
            //}

            //return newEntity;

            #endregion
        }

        /// <summary>
        /// 替换方法，返回实体与传入实体共址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xmlNode"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static T Replace<T>(T entity, XElement xmlNode, string[] fields) where T : class
        {
            CommonHelper.Replace(ref entity, xmlNode, fields);

            return entity;

            #region 旧代码 乐文斌 20101208 ..

            //// 如果列数组为空，返回整个替换整个xml结点；
            //if (fields == null || fields.Length == 0)
            //{
            //    // 获取XML结点中的结点名并生成替换列
            //    string sFieldList = GetNodeNameList(xmlNode);

            //    fields = sFieldList.Split('|');
            //}

            //Type type = entity.GetType();

            //// 可对所有属性做复制
            //T newEntity = Clone<T>(entity);

            //XElement item;
            //PropertyInfo pi;
            //FieldInfo fi;
            //for (int i = 0; i < fields.Length; i++)
            //{
            //    try
            //    {
            //        item = xmlNode.Element(fields[i]);
            //        // 默认兼容小写
            //        if (item == null) item = xmlNode.Element(fields[i].ToLower());
            //        if (item != null)
            //        {
            //            pi = type.GetProperty(fields[i]);
            //            if (pi == null) pi = type.GetProperty(fields[i].ToLower());
            //            if (pi != null)
            //            {
            //                pi.SetValue(newEntity, (object)ConvertByType(pi.PropertyType, item.Value), null);
            //            }
            //            else
            //            {
            //                fi = type.GetField(fields[i]);
            //                if (fi == null) continue;

            //                fi.SetValue(newEntity, (object)ConvertByType(fi.FieldType, item.Value));
            //            }
            //        }

            //    }
            //    catch { }
            //}

            //return newEntity;

            #endregion
        }

        #endregion

        #region Xml合并

        /// <summary>
        /// 将多个 xml 文件合并成一个xml
        /// 约定：列表中索引值 index 越大的数据其值越新
        /// 合并逻辑合并逻辑为 or (即只要其中一个文档存在的节点都会保留)，若同时存在的节点，用 newData 中的值替换掉 oldData 中的值
        /// </summary>
        /// <param name="datas">需要合并的xml列表</param>
        /// <returns>返回合并后的xml</returns>
        public static XElement IncorporateDistinctBySingle(List<XElement> datas)
        {
            if (datas == null || datas.Count == 0) { return null; }
            XElement result = null;
            foreach (var item in datas)
            {
                if (result == null)
                {
                    result = item;
                    continue;
                }
                result = IncorporateDistinctBySingle(result, item);
            }
            return result;
        }

        /// <summary>
        /// 将 newData 中的值替换至 oldData，并保留差异值(只有其中一个xml有的节点)
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        /// <returns></returns>
        public static XElement DisplayOldDataBySingle(XElement oldData, XElement newData)
        {
            foreach (var item in newData.Elements())
            {
                XElement oldItem = oldData.Elements().Where(p => p.Name == item.Name).FirstOrDefault();
                if (oldItem != null)
                {
                    if (item.HasElements) { oldItem = DisplayOldDataBySingle(oldItem, item); }
                    else { oldItem.Value = item.Value; }
                }
                else
                {
                    oldData.Add(item);
                }
            }
            return oldData;
        }

        /// <summary>
        /// 递归实现 xml 文件的合并
        /// 合并逻辑为 or (即只要其中一个文档存在的节点都会保留)，若同时存在的节点，用 newData 中的值替换掉 oldData 中的值
        /// </summary>
        /// <param name="oldData">旧 xml 数据</param>
        /// <param name="newData">新 xml 数据</param>
        /// <returns>返回合并后的 xml 数据</returns>
        public static XElement IncorporateDistinctBySingle(XElement oldData, XElement newData)
        {
            if (oldData == null) { return newData == null ? new XElement("root") : newData; }
            if (newData == null) { return oldData; }

            foreach (var item in newData.Elements())
            {
                // 若该节点没有子节点，则在旧xml文档中添加该节点或修改对应节点的值为该节点的值，并跳过本次循环
                if (!item.HasElements)
                {
                    XElement oldTempElement = oldData.Elements().Where(p => p.Name == item.Name && !p.HasElements).FirstOrDefault();
                    if (oldTempElement == null) { oldData.Add(item); }
                    else { oldTempElement.Value = item.Value; }
                    continue;
                }

                // 处理有字节点的情况
                XElement newIdElement = item.Elements().Where(p => p.Name == string.Format("{0}id", item.Name)).FirstOrDefault();

                // 若该节点没有Id子节点，则进行递归调用本方法或在旧xml文档中添加该节点
                if (newIdElement == null)
                {
                    IEnumerable<XElement> oldTempElements = oldData.Elements().Where(p => p.Name == item.Name);

                    // 若旧xml文档中不存在与该新节点同名的节点时，则往旧xml文档中，添加该新节点
                    if (oldTempElements == null || oldTempElements.Count() == 0)
                    {
                        oldData.Add(item);
                    }
                    else
                    {
                        // 若该新接点不为单据内容接点，则递归调用本方法
                        XElement oldIdElement = oldTempElements.Where(p => p.Elements(string.Format("{0}id", item.Name)) == null).FirstOrDefault();
                        if (oldIdElement == null && oldTempElements.FirstOrDefault() != null
                            && oldTempElements.FirstOrDefault().HasElements && item.Elements().FirstOrDefault().HasElements)
                        {
                            IncorporateDistinctBySingle(oldTempElements.FirstOrDefault(), item);
                        }
                        else
                        {
                            oldData.Add(item);
                        }
                    }
                }
                else
                {
                    // 若存在Id接点，且其Id的值相等(即同一张单据)，则将旧xml中子节点的值修改为新xml中的值或增加旧xml中没有的子节点；
                    // 若存在Id节点，且不存在其Id值相等的节点(旧xml中不存在该单据)，则在旧xml中添加该节点
                    XElement oldTempElement = oldData.Elements().Where(p => p.Name == item.Name &&
                        p.Elements().Where(q => q.Name == newIdElement.Name && q.Value == newIdElement.Value).FirstOrDefault() != null).FirstOrDefault();
                    if (oldTempElement == null)
                    {
                        oldData.Add(item);
                        continue;
                    }
                    DisplayOldDataBySingle(oldTempElement, item);
                }
            }
            return oldData;
        }
        #endregion

        /// <summary>
        /// 转换二级结点实体列表，二级结点的NodeName需与实体Property一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlNode"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(XElement xmlNode, string nodeName) where T : class
        {
            try
            {
                if (xmlNode == null) return null;

                T obj;
                Type t = typeof(T);
                List<T> lst = new List<T>();

                // 取配送结点
                IEnumerable<XElement> itemNodes = from itemNode in xmlNode.Elements(nodeName)
                                                  select itemNode;
                foreach (XElement itemNode in itemNodes)
                {
                    obj = (T)Activator.CreateInstance(t);

                    lst.Add(CommonHelper.Replace<T>(obj, itemNode));
                }

                return lst;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Convert By Type..

        /// <summary>
        /// 将字符串转换成对应类型的数据
        /// </summary>
        /// <param name="t"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ConvertByType(Type t, string value)
        {
            try
            {
                if (value == null) return null;

                if (t.FullName == typeof(String).FullName)
                { return value; }
                if (t.FullName == typeof(Boolean).FullName || t == typeof(Nullable<Boolean>))
                { return Boolean.Parse(value); }
                if (t.FullName == typeof(Byte).FullName || t == typeof(Nullable<Byte>))
                { return Byte.Parse(value); }
                if (t.FullName == typeof(SByte).FullName || t == typeof(Nullable<SByte>))
                { return SByte.Parse(value); }
                if (t.FullName == typeof(Decimal).FullName || t == typeof(Nullable<Decimal>))
                { return Decimal.Parse(value); }
                if (t.FullName == typeof(Single).FullName || t == typeof(Nullable<Single>))
                { return Single.Parse(value); }
                if (t.FullName == typeof(Double).FullName || t == typeof(Nullable<Double>))
                { return Double.Parse(value); }
                if (t.FullName == typeof(Int16).FullName || t == typeof(Nullable<Int16>))
                { return Int16.Parse(value); }
                if (t.FullName == typeof(UInt16).FullName || t == typeof(Nullable<UInt16>))
                { return UInt16.Parse(value); }
                if (t.FullName == typeof(int).FullName || t == typeof(Nullable<int>))
                { return int.Parse(value); }
                if (t.FullName == typeof(Int32).FullName || t == typeof(Nullable<Int32>))
                { return Int32.Parse(value); }
                if (t.FullName == typeof(UInt32).FullName || t == typeof(Nullable<UInt32>))
                { return UInt32.Parse(value); }
                if (t.FullName == typeof(Int64).FullName || t == typeof(Nullable<Int64>))
                { return Int64.Parse(value); }
                if (t.FullName == typeof(UInt64).FullName || t == typeof(Nullable<UInt64>))
                { return UInt64.Parse(value); }
                if (t.FullName == typeof(DateTime).FullName || t == typeof(Nullable<DateTime>))
                { return DateTime.Parse(value); }

                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region DateTime方法..

        /// <summary>
        /// 求两个时间的不同项的，
        /// </summary>
        /// <param name="Interval">对比类型</param>
        /// <param name="StartDate">开始时间</param>
        /// <param name="EndDate">结束</param>
        /// <returns></returns>
        public static long DateDiff(DateInterval Interval, System.DateTime StartDate, System.DateTime EndDate)
        {
            long lngDateDiffValue = 0;
            System.TimeSpan TS = new System.TimeSpan(EndDate.Ticks - StartDate.Ticks);
            switch (Interval)
            {
                case DateInterval.Second:
                    lngDateDiffValue = (long)TS.TotalSeconds;
                    break;
                case DateInterval.Minute:
                    lngDateDiffValue = (long)TS.TotalMinutes;
                    break;
                case DateInterval.Hour:
                    lngDateDiffValue = (long)TS.TotalHours;
                    break;
                case DateInterval.Day:
                    lngDateDiffValue = (long)TS.Days;
                    break;
                case DateInterval.Week:
                    lngDateDiffValue = (long)(TS.Days / 7);
                    break;
                case DateInterval.Month:
                    lngDateDiffValue = (long)(TS.Days / 30);
                    break;
                case DateInterval.Quarter:
                    lngDateDiffValue = (long)((TS.Days / 30) / 3);
                    break;
                case DateInterval.Year:
                    lngDateDiffValue = (long)(TS.Days / 365);
                    break;
            }
            return (lngDateDiffValue);
        } //end of DateDiff

        #endregion
    }

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extensions
    {
        #region DateTime操作..

        /// <summary>
        /// 获取得时间
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Interval"></param>
        /// <param name="StartDate"></param>
        /// <returns></returns>
        public static long DateDiff(this DateTime dt, DateInterval Interval, System.DateTime StartDate)
        {
            try
            {
                long lngDateDiffValue = 0;
                System.TimeSpan TS = new System.TimeSpan(dt.Ticks - StartDate.Ticks);
                switch (Interval)
                {
                    case DateInterval.Second:
                        lngDateDiffValue = (long)TS.TotalSeconds;
                        break;
                    case DateInterval.Minute:
                        lngDateDiffValue = (long)TS.TotalMinutes;
                        break;
                    case DateInterval.Hour:
                        lngDateDiffValue = (long)TS.TotalHours;
                        break;
                    case DateInterval.Day:
                        lngDateDiffValue = (long)TS.Days;
                        break;
                    case DateInterval.Week:
                        lngDateDiffValue = (long)(TS.Days / 7);
                        break;
                    case DateInterval.Month:
                        lngDateDiffValue = (long)(TS.Days / 30);
                        break;
                    case DateInterval.Quarter:
                        lngDateDiffValue = (long)((TS.Days / 30) / 3);
                        break;
                    case DateInterval.Year:
                        lngDateDiffValue = (long)(TS.Days / 365);
                        break;
                }
                return (lngDateDiffValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region System.Data..

        #region DataTable..
        
        /// <summary>
        /// 合并DataTable中各行的数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyField">合并列时的参照列</param>
        /// <returns>返回合并后的DataTable</returns>
        public static DataTable MergeRow(this DataTable source, string keyField)
        {
            if (source == null || source.Rows.Count == 0)
            { return null; }

            StringBuilder sb = new StringBuilder();
            foreach (DataColumn dc in source.Columns)
            {
                if (sb.Length > 0) { sb.Append("|"); }

                sb.Append(dc.ColumnName);
            }

            return source.MergeRow(keyField.Split('|'), sb.ToString().Split('|'));
        }

        /// <summary>
        /// 合并DataTable中各行的数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyField">合并列时的参照列</param>
        /// <param name="mergeField">合并列，为空时不合并</param>
        /// <returns>返回合并后的DataTable</returns>
        public static DataTable MergeRow(this DataTable source, string keyField, string[] mergeField)
        {
            return source.MergeRow(keyField.Split('|'), mergeField);
        }

        /// <summary>
        /// 合并行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyFields"></param>
        /// <param name="mergeField"></param>
        /// <returns></returns>
        public static DataTable MergeRow(this DataTable source, string[] keyFields, string[] mergeField)
        {
            return source.MergeRow(keyFields, mergeField, "<br>");
        }

        /// <summary>
        /// 合并行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyFields"></param>
        /// <param name="mergeField"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        public static DataTable MergeRow(this DataTable source, string[] keyFields, string[] mergeField, string splitStr)
        {
            try
            {
                if (source == null || source.Rows.Count == 0)
                { return null; }

                // 不合并
                if (keyFields.Length == 0) return source;
                if (mergeField.Length < 1) return source;

                splitStr = splitStr.Trim();
                if (splitStr.Length == 0) splitStr = "<br>";

                DataTable result = source.Clone();

                // 遍历整个记录集
                foreach (DataRow drSource in source.Rows)
                {
                    bool blnHasRow = false;
                    foreach (DataRow drTarget in result.Rows)
                    {
                        if (IsEquals(drSource, drTarget, keyFields))
                        {
                            blnHasRow = true;
                            // 把需要合并的列合并
                            for (int i = 0; i < mergeField.Length; i++)
                            {
                                try
                                {
                                    if (drTarget[mergeField[i]].ToString().IndexOf(drSource[mergeField[i]].ToString()) < 0)
                                    { drTarget[mergeField[i]] += splitStr + drSource[mergeField[i]].ToString(); }
                                }
                                catch
                                { }
                            }
                        }
                    }
                    if (!blnHasRow)
                    { DataRow newData = result.Rows.Add(drSource.ItemArray); }
                }

                return result;
            }
            catch
            {
                return source;
            }
        }

        /// <summary>
        /// 判断两个行相应列的值是否相等，全部相等返回true，否则返回false
        /// </summary>
        /// <param name="drSource"></param>
        /// <param name="drTarget"></param>
        /// <param name="keyFields"></param>
        /// <returns></returns>
        private static bool IsEquals(DataRow drSource, DataRow drTarget, string[] keyFields)
        {
            try
            {
                if (keyFields.Length == 0) return false;

                for (int i = 0; i < keyFields.Length; i++)
                {
                    if (keyFields[i].Trim().Length == 0) continue;
                    if (drSource[keyFields[i]].ToString() != drTarget[keyFields[i]].ToString()) return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region Xml..

        /// <summary>
        /// 转换二级结点实体列表，二级结点的NodeName需与实体Property一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlNode"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this XElement xmlNode, string nodeName) where T : class
        {
            return CommonHelper.ToList<T>(xmlNode, nodeName);
        }

        /// <summary>
        /// 用xml替换实体内容，返回替换后的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static T Replace<T>(this T entity, XElement xmlNode) where T : class
        {
            CommonHelper.Replace(ref entity, xmlNode);

            return entity;
        }

        /// <summary>
        /// 用xml替换实体内容，返回替换后的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="xmlNode"></param>
        /// <param name="fileds"></param>
        /// <returns></returns>
        public static T Replace<T>(this T entity, XElement xmlNode, string[] fileds) where T : class
        {
            CommonHelper.Replace<T>(ref entity, xmlNode, fileds);

            return entity;
        }

        /// <summary>
        /// 在XElement结点中批量增加子结点
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="xmlSubNodes">子结点集合</param>
        public static void AddRange(this XElement xmlNode, List<XElement> xmlSubNodes)
        {
            CommonHelper.AddRange(xmlNode, xmlSubNodes);

            return;
        }

        /// <summary>
        /// 获取结点名，结点名默认按"|"分割
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static string GetNodeNameList(this XElement xmlNode)
        {
            return CommonHelper.GetNodeNameList(xmlNode);
        }

        /// <summary>
        /// 获取结点名
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="sSplit"></param>
        /// <returns></returns>
        public static string GetNodeNameList(this XElement xmlNode, string sSplit)
        {
            return CommonHelper.GetNodeNameList(xmlNode, sSplit);
        }

        #endregion

        #region List..

        /// <summary>
        /// 通过属性名和值过滤实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstInfos"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T First<T>(this List<T> lstInfos, string fieldName, object value) where T : class
        {
            return CommonHelper.First<T>(lstInfos, fieldName, value);
        }

        /// <summary>
        /// 向list中增加实例并判断是否重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstList"></param>
        /// <param name="item">实例对象</param>
        /// <param name="FilterEqual">是否过滤重复实例，true-过滤，如有重复实例不做追加；false-不过滤重复实例，按正常add执行；</param>
        public static void Add<T>(this List<T> lstList, T item, bool FilterEqual) where T : class
        {
            if (lstList == null) return;
            if (item == null) return;

            try
            {
                if (FilterEqual)
                {
                    bool blnIsEqual = false;
                    foreach (var obj in lstList)
                    {
                        if (object.ReferenceEquals(obj, item))
                        {
                            blnIsEqual = true;
                            break;
                        }
                    }

                    // 如果没有重复项才增加
                    if (!blnIsEqual) lstList.Add(item);
                }
                else
                {
                    lstList.Add(item);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 向列表批量增加实例并判断是否重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstList"></param>
        /// <param name="collection">实例集合</param>
        /// <param name="FilterEqual">是否过滤重复实例，true-过滤，如有重复实例不做追加；false-不过滤重复实例，按正常add执行；</param>
        public static void AddRange<T>(this List<T> lstList, List<T> collection, bool FilterEqual) where T : class
        {
            if (lstList == null) return;
            if (collection == null || collection.Count == 0) return;

            try
            {
                foreach (var item in collection)
                {
                    lstList.Add(item, FilterEqual);
                }
            }
            catch
            { }
        }

        #endregion

        #region DataTable..

        /// <summary>
        /// 获取DataTable的Xml
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static XElement GetXml(this DataTable dt)
        {
            return CommonHelper.GetXml(dt, "Root", "Row");
        }

        /// <summary>
        /// 获取DataTable的Xml
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rootNodeName"></param>
        /// <param name="rowNodeName"></param>
        /// <returns></returns>
        public static XElement GetXml(this DataTable dt, string rootNodeName, string rowNodeName)
        {
            return CommonHelper.GetXml(dt, rootNodeName, rowNodeName);
        }

        #endregion
    }

    /// <summary>
    /// 时间类型属性
    /// </summary>
    public enum DateInterval
    {
        /// <summary>
        /// 秒
        /// </summary>
        Second,
        /// <summary>
        /// 分
        /// </summary>
        Minute,
        /// <summary>
        /// 小时
        /// </summary>
        Hour,
        /// <summary>
        /// 天
        /// </summary>
        Day,
        /// <summary>
        /// 周
        /// </summary>
        Week,
        /// <summary>
        /// 月
        /// </summary>
        Month,
        /// <summary>
        /// 季度
        /// </summary>
        Quarter,
        /// <summary>
        /// 年
        /// </summary>
        Year
    }
}
