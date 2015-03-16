using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	public static class Loader
	{
		private static readonly string exePath;
		private static readonly XmlSerializerNamespaces ns;

		static Loader(){
			Loader.exePath = AppDomain.CurrentDomain.BaseDirectory;
			Loader.ns = new XmlSerializerNamespaces();
			Loader.ns.Add ("", "");
		}

		public static string ExePath{get {return Loader.exePath;} }

		public static string GetPath(string fileName){
			return Path.Combine (Loader.exePath, fileName);
		}

		public static string GetPath(string subdir, string fileName){
			return Path.Combine (Path.Combine(Loader.exePath, subdir), fileName);
		}

		public static List<T> LoadArray<T>(string filePath)
		{
			T[] array = null;
			List<T> list = new List<T>();
			using (StreamReader reader = new StreamReader(filePath, ASCIIEncoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer (typeof(T[]));
				array = (T[])serializer.Deserialize(reader);
				reader.Close();
			}
			if(array != null)
				list = new List<T>(array);
			return list;
		}

		public static T Load<T>(string filePath)
		{
			T item;
			using (StreamReader reader = new StreamReader(filePath, ASCIIEncoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				item = (T)serializer.Deserialize(reader);
				reader.Close();

			}
			return item;
		}

		public static void Save(string filePath, object o){
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
			settings.Indent = true;
			settings.OmitXmlDeclaration = false;
			using (StreamWriter stream = new StreamWriter(filePath))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
				{
					XmlSerializer serializer = new XmlSerializer(o.GetType());
					serializer.Serialize(xmlWriter, o, ns);
					xmlWriter.Close();
				}
				stream.Close();
			}
		}

		public static List<Grammar> LoadGrammars ()
		{
			Grammar grammar;
			string grammarsPath = GetPath ("grammars");
			string[] gfs = Directory.GetFiles (grammarsPath, "*.txt", SearchOption.TopDirectoryOnly);
			List<Grammar> grammars = new List<Grammar> (gfs.Length);
			foreach (string gf in gfs) {
				grammar = Grammar.LoadFromFile (gf);
				if (grammar != null)
					grammars.Add (grammar);
			}
			if (grammars.Count < 1)
				throw new Exception ("No grammars could be loaded");
			return grammars;
		}

		public static GPSRObjectManager LoadObjects (string filePath)
		{
			CategoryContainer categories = Load<CategoryContainer> (filePath);
			if (categories == null)
				throw new Exception ("No objects found");
			GPSRObjectManager manager = new GPSRObjectManager();
			foreach (Category c in categories.Categories)
				manager.Add (c);
			return manager;
		}

		public static string Serialize<T>(List<T> list)
		{
			string serialized;
			T[] array = list.ToArray ();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
			settings.Indent = true;
			settings.OmitXmlDeclaration = false;
			using (StringWriter textWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings)) {
					XmlSerializer serializer = new XmlSerializer (typeof(T[]));
					serializer.Serialize (xmlWriter, array, ns);
				}
				serialized = textWriter.ToString ();
			}
			return serialized;
		}
	}
}

