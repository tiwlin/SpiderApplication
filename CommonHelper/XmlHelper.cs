using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace CommonHelper
{
	public static class XmlHelper
	{
		 public static string SelectNodeText(this XmlNode node, string xpath)
		 {
			if(node == null) return null;
			XmlNode temp = node.SelectSingleNode(xpath);
			if(temp != null)
			{
				return temp.InnerText;
			}
			return null;
		 }
		 
		 public static string DecodeXMLChar(string xml)
		 {
			//$str = str_replace("&","&amp;",$str);
			//$str = str_replace("\"","&quot;",$str);
			//$str = str_replace(">","&gt;",$str);
			//$str = str_replace("<", "&lt;" , $str);
			if(xml == null)
			{
				return null;
			}
			else
			{
    			return HttpUtility.HtmlDecode(xml);
    		}
		 }
	}
}
