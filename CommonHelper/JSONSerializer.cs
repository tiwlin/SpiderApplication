using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection.Emit;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace CommonHelper
{
    public class JSONSerializer
    {
        /// <summary>
        /// 其中时间格式为new Date(1234656000000),js直接解析
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public static string Serialize(object ins)
        {
            return JsonConvert.SerializeObject(ins, Formatting.None,new IsoDateTimeConverter());
        }

        /// <summary>
        /// 序列化给js前段用
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public static string SerializeJS(object ins)
        {
            return JsonConvert.SerializeObject(ins, Formatting.None, new JavaScriptDateTimeConverter());
        }

        public static string SerializeStandard(object ins)
        {
            return JsonConvert.SerializeObject(ins, Formatting.Indented);
        }

        /// <summary>
        /// 如果对象的属性未赋值或为null,则不序列化，减少大小
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public static string SerializeNonIncludeNull(object ins)
        {
            return JsonConvert.SerializeObject(ins, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Converters = new List<JsonConverter>() { new IsoDateTimeConverter() } }).Replace("\"", "'");
        }

        public static string SerializeNonIncludeNullJS(object ins)
        {
            return JsonConvert.SerializeObject(ins, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Converters = new List<JsonConverter>() { new JavaScriptDateTimeConverter() } });
        }

        public static string SerializeByCondition(object ins, string[] fliterList)
        {
            return JsonConvert.SerializeObject(ins, Formatting.None, new JsonSerializerSettings() { ContractResolver = new DynamicContractResolver(fliterList), Converters = new List<JsonConverter>() { new IsoDateTimeConverter() } });
        }

        /// <summary>
        /// XML序列化为json
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string SerializeByXml(System.Xml.XmlDocument doc)
        {
            return JsonConvert.SerializeXmlNode(doc);
        }

        public static object Deserialize(string str)
        {
            return JsonConvert.DeserializeObject(str);
        }

        public static T Deserialize<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// DataTable序列化成JSON
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTableToJSON(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                JsonSerializer ser = new JsonSerializer();
                jw.WriteStartObject();
                jw.WritePropertyName(dt.TableName);
                jw.WriteStartArray();
                foreach (DataRow dr in dt.Rows)
                {
                    jw.WriteStartObject();

                    foreach (DataColumn dc in dt.Columns)
                    {
                        jw.WritePropertyName(dc.ColumnName);
                        ser.Serialize(jw, dr[dc].ToString());
                    }

                    jw.WriteEndObject();
                }
                jw.WriteEndArray();
                jw.WriteEndObject();

                sw.Close();
                jw.Close();

            }

            return sb.ToString();
        }
    }

    public class DynamicContractResolver : DefaultContractResolver
    {
        private readonly string[] filterLst;
        public DynamicContractResolver(string[] lst)
        {
            filterLst = lst;
        }

        protected override IList<JsonProperty> CreateProperties(JsonObjectContract contract)
        {
            IList<JsonProperty> properties = base.CreateProperties(contract);

            properties =
              properties.Where(p => filterLst.Contains(p.PropertyName)).ToList();

            return properties;
        }
    }

}
