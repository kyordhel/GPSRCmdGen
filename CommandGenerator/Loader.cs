using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using RoboCup.AtHome.CommandGenerator.Containers;
using RoboCup.AtHome.CommandGenerator.ReplaceableTypes;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Helper class for loading and storing Xml serialized data
	/// </summary>
	public static class Loader
	{
		#region Variables

		/// <summary>
		/// Stores the path of the executable file
		/// </summary>
		private static readonly string exePath;

		/// <summary>
		/// Stores the namespace strings for serialized objects
		/// </summary>
		private static readonly XmlSerializerNamespaces ns;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes the <see cref="RoboCup.AtHome.CommandGenerator.Loader"/> class.
		/// </summary>
		static Loader(){
			Loader.exePath = AppDomain.CurrentDomain.BaseDirectory;
			Loader.ns = new XmlSerializerNamespaces();
			Loader.ns.Add ("", "");
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the path of the executable file
		/// </summary>
		public static string ExePath{get {return Loader.exePath;} }

		#endregion

		#region Methods

		/// <summary>
		/// Gets a full path for the given filename using the executable
		/// file path as base path path.
		/// </summary>
		/// <param name="fileName">The name of the file to combine into a path</param>
		/// <returns>A full path for the given fileName.</returns>
		public static string GetPath(string fileName){
			return Path.Combine (Loader.exePath, fileName);
		}

		/// <summary>
		/// Gets a full path for the given filename using the executable
		/// file path as base path path and a subdirectory.
		/// </summary>
		/// <param name="subdir">The name of the subdirectory that will contain the file</param>
		/// <param name="fileName">The name of the file to combine into a path</param>
		/// <returns>A full path for the given fileName.</returns>
		public static string GetPath(string subdir, string fileName){
			return Path.Combine (Path.Combine(Loader.exePath, subdir), fileName);
		}

		/// <summary>
		/// Loads an array of T objects from a XML file.
		/// </summary>
		/// <param name="filePath">The path of the XML file</param>
		/// <typeparam name="T">The type of objects contained in the file.</typeparam>
		/// <returns>The array of T objects in the XML file</returns>
		public static List<T> LoadArray<T>(string filePath)
		{
			T[] array = null;
			List<T> list = new List<T>();
			using (StreamReader reader = new StreamReader(filePath, ASCIIEncoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer (typeof(T[]));
				serializer.UnknownAttribute+= new XmlAttributeEventHandler(serializer_UnknownAttribute);
				array = (T[])serializer.Deserialize(reader);
				reader.Close();
			}
			if(array != null)
				list = new List<T>(array);
			return list;
		}

		/// <summary>
		/// Loads an object from a XML file.
		/// </summary>
		/// <param name="filePath">The path of the XML file</param>
		/// <typeparam name="T">The type of object to load.</typeparam>
		/// <returns>The object in the XML file</returns>
		public static T Load<T>(string filePath)
		{
			T item;
			using (StreamReader reader = new StreamReader(filePath, ASCIIEncoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				serializer.UnknownAttribute+= new XmlAttributeEventHandler(serializer_UnknownAttribute);
				item = (T)serializer.Deserialize(reader);
				reader.Close();

			}
			return item;
		}

		/// <summary>
		/// Loads an object from a XML string.
		/// </summary>
		/// <param name="xml">An XML encoded string</param>
		/// <typeparam name="T">The type of object to load.</typeparam>
		/// <returns>The object in the XML string</returns>
		public static T LoadXmlString<T>(string xml)
		{
			T item;
			using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml ?? String.Empty)))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				serializer.UnknownAttribute+= new XmlAttributeEventHandler(serializer_UnknownAttribute);
				item = (T)serializer.Deserialize(ms);
				ms.Close();

			}
			return item;
		}

		private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e){
			IDescribable desc = e.ObjectBeingDeserialized as IDescribable;
			if (desc == null)
				return;
			desc.Properties.Add(e.Attr.Name, e.Attr.Value);
		}

		/// <summary>
		/// Stores an object into a XML file.
		/// </summary>
		/// <param name="filePath">The path of the XML file to store the objectt within.</param>
		/// <param name="o">The object to serialize and save.</param>
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

		/// <summary>
		/// Loads a list of Grammar objects from the grammars subdirectory.
		/// </summary>
		/// <returns>A list of Grammar objects</returns>
		public static List<Grammar> LoadGrammars()
		{
			return LoadGrammars("grammars");
		}

		/// <summary>
		/// Loads a list of Grammar objects from the grammars subdirectory.
		/// </summary>
		/// <returns>A list of Grammar objects</returns>
		public static List<Grammar> LoadGrammars (string grammarsDirectoryName)
		{
			Grammar grammar;
			string grammarsPath = GetPath(grammarsDirectoryName);
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

		/// <summary>
		/// Loads the set of Locations grouped by room from the Locations.xml file.
		/// </summary>
		/// <returns>A LocationManager that contains the set of objects and categories</returns>
		public static LocationManager LoadLocations(string filePath)
		{
			RoomContainer container = Load<RoomContainer>(filePath);
			if (container == null)
				throw new Exception("No objects found");
			LocationManager manager = LocationManager.Instance;
			foreach (Room r in container.Rooms)
				manager.Add(r);
			return manager;
		}

		/// <summary>
		/// Loads the set of GPSRObjects and Categories from the Objects.xml file.
		/// </summary>
		/// <returns>A GPSRObjectManager that contains the set of objects and categories</returns>
		public static ObjectManager LoadObjects (string filePath)
		{
			CategoryContainer container = Load<CategoryContainer> (filePath);
			if (container == null)
				throw new Exception ("No objects found");
			ObjectManager manager = ObjectManager.Instance;
			foreach (Category c in container.Categories)
				manager.Add (c);
			return manager;
		}

		/// <summary>
		/// Serializes the specified list of T objects into a string.
		/// </summary>
		/// <param name="list">The list to serialize.</param>
		/// <typeparam name="T">The type of serializable objects of the list.</typeparam>
		/// <returns>A string containing the XML representation of the list.</returns>
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
		#endregion
	}
}

