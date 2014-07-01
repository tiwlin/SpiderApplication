using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace CommonHelper
{
    public sealed class XMLSerializer
    {
        /// <summary>
        /// 序列化(无空格,无NameSpace)
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public static string SerializeNonNS(object ins)
        {
            XmlSerializer xml = new XmlSerializer(ins.GetType());
            using(MemoryStream stream = new MemoryStream())
            {
				XmlWriterSettings setting = new XmlWriterSettings
				{
					Encoding = new UTF8Encoding(false),
					Indent = false,
					NewLineOnAttributes = false,
					OmitXmlDeclaration = true
				};

				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				using (XmlWriter writer = XmlWriter.Create(stream, setting))
				{
					xml.Serialize(writer, ins, ns);
				}
				stream.Position = 0;
			
				using(StreamReader sr = new StreamReader(stream, Encoding.UTF8))
				{
					return sr.ReadToEnd();
				}
			}
            //return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// 直接序列化
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public static string Serialize(object ins)
        {
            XmlSerializer xml = new XmlSerializer(ins.GetType());
            using(MemoryStream stream = new MemoryStream())
            {
				using (XmlWriter writer = XmlWriter.Create(stream))
				{
					xml.Serialize(writer, ins);
				}
				stream.Position = 0;
				
				using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
				{
					return sr.ReadToEnd();
				}
			}
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string ins)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            T obj;
            using(StringReader sr=new StringReader(ins))
            {
				obj = (T)xml.Deserialize(sr);
            }
            return obj;
        }
    }
}
