using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using GPSRCmdGen.Containers;

namespace GPSRCmdGen
{
	/// <summary>
	/// Helper class for loading and storing Xml serialized data
	/// </summary>
	public static class XmlLoader
	{
		#region Variables

		/// <summary>
		/// Stores the namespace strings for serialized objects
		/// </summary>
		private static readonly XmlSerializerNamespaces ns;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes the <see cref="GPSRCmdGen.XmlLoader"/> class.
		/// </summary>
		static XmlLoader(){
			XmlLoader.ns = new XmlSerializerNamespaces();
			XmlLoader.ns.Add ("", "");
		}

		#endregion

		#region Methods

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
				item = (T)serializer.Deserialize(reader);
				reader.Close();

			}
			return item;
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
		public static List<Grammar> LoadGrammars ()
		{
			Grammar grammar;
			string grammarsPath = Loader.GetPath ("grammars");
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
		public static void LoadLocations(string filePath)
		{
			RoomContainer container = Load<RoomContainer>(filePath);
			if (container == null)
				throw new Exception("No objects found");
			LocationManager manager = LocationManager.Instance;
			foreach (Room r in container.Rooms)
				manager.Add(r);
		}

		/// <summary>
		/// Loads the set of GPSRObjects and Categories from the Objects.xml file.
		/// </summary>
		/// <returns>A GPSRObjectManager that contains the set of objects and categories</returns>
		public static void LoadObjects (string filePath)
		{
			CategoryContainer container = Load<CategoryContainer> (filePath);
			if (container == null)
				throw new Exception ("No objects found");
			GPSRObjectManager manager = GPSRObjectManager.Instance;
			foreach (Category c in container.Categories)
				manager.Add (c);
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

