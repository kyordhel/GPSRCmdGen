using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	public class Loader
	{
		public static List<T> Load<T>(string filePath)
		{
			T[] array = null;
			List<T> list = new List<T>();
			using (FileStream fs = File.OpenRead(filePath))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T[]));
				array = (T[])serializer.Deserialize(fs);
				fs.Close();
			}
			if(array != null)
				list = new List<T>(array);
			return list;
		}

		public static string Serialize<T>(List<T> list)
		{
			string serialized;
			T[] array = list.ToArray ();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
			settings.Indent = false;
			settings.OmitXmlDeclaration = false;
			using (StringWriter textWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings)) {
					XmlSerializer serializer = new XmlSerializer (typeof(T[]));
					serializer.Serialize (xmlWriter, array);
				}
				serialized = textWriter.ToString ();
			}
			return serialized;
		}
	}
}

