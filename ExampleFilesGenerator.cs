using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing gestures  
	/// </summary>
	[XmlRoot(ElementName = "gestures", Namespace = "")]
	public class GestureContainer{
		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.GestureContainer"/> class.
		/// </summary>
		public GestureContainer(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.GestureContainer"/> class.
		/// </summary>
		/// <param name="gestures">List of gestures</param>
		public GestureContainer(List<Gesture> gestures){this.Gestures=gestures;}

		/// <summary>
		/// Gets or sets the list of gestures.
		/// </summary>
		[XmlElement("gesture")]
		public List<Gesture> Gestures{ get; set; } 
	}

	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing locations
	/// </summary>
	[XmlRoot(ElementName = "locations", Namespace = "")]
	public class LocationContainer{

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.LocationContainer"/> class.
		/// </summary>
		public LocationContainer(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.LocationContainer"/> class.
		/// </summary>
		/// <param name="locations">List of locations.</param>
		public LocationContainer(List<Location> locations){this.Locations=locations;}

		/// <summary>
		/// Gets or sets the list of locations.
		/// </summary>
		[XmlElement("location")]
		public List<Location> Locations{ get; set; } 
	}

	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing names  
	/// </summary>
	[XmlRoot(ElementName = "names", Namespace = "")]
	public class NameContainer{
		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.NameContainer"/> class.
		/// </summary>
		public NameContainer(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.NameContainer"/> class.
		/// </summary>
		/// <param name="names">List of names.</param>
		public NameContainer(List<PersonName> names){this.Names=names;}

		/// <summary>
		/// Gets or sets the list of names.
		/// </summary>
		[XmlElement("name")]
		public List<PersonName> Names{ get; set; } 
	}

	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing categories
	/// </summary>
	[XmlRoot(ElementName = "categories", Namespace = "")]
	public class CategoryContainer{

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.CategoryContainer"/> class.
		/// </summary>
		public CategoryContainer(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.CategoryContainer"/> class.
		/// </summary>
		/// <param name="categories">List of categories.</param>
		public CategoryContainer(List<Category> categories){this.Categories=categories;}

		/// <summary>
		/// Gets or sets the list of categories.
		/// </summary>
		[XmlElement("category")]
		public List<Category> Categories{ get; set; } 
	}

	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing categories
	/// </summary>
	[XmlRoot(ElementName = "questions", Namespace = "")]
	public class QuestionsContainer{

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.QuestionsContainer"/> class.
		/// </summary>
		public QuestionsContainer(){}

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.QuestionsContainer"/> class.
		/// </summary>
		/// <param name="questions">List of questions.</param>
		public QuestionsContainer(List<PredefindedQuestion> questions) { this.Questions = questions; }

		/// <summary>
		/// Gets or sets the list of questions.
		/// </summary>
		/// <value>The questions.</value>
		[XmlElement("question")]
		public List<PredefindedQuestion> Questions { get; set; }
	}

	/// <summary>
	/// Example files generator.
	/// </summary>
	public static class ExampleFilesGenerator
	{

		/// <summary>
		/// Writes down a set of example data files with information gathered from the Factory
		/// </summary>
		public static void GenerateExampleFiles(){
			GestureContainer gestures = new GestureContainer(Factory.GetDefaultGestures());
			CategoryContainer categories = new CategoryContainer(Factory.GetDefaultObjects().Categories);
			LocationContainer locations = new LocationContainer (Factory.GetDefaultLocations ());
			NameContainer names = new NameContainer (Factory.GetDefaultNames ());
			foreach (Category cat in categories.Categories) {
				if (!locations.Locations.Contains (cat.DefaultLocation))
					locations.Locations.Add (cat.DefaultLocation);
			}
			QuestionsContainer questions = new QuestionsContainer(Factory.GetDefaultQuestions());

			SaveGrammars ();
			WriteDatafiles (gestures, categories, locations, names, questions);
		}

		/// <summary>
		/// Queries the user for file overwritten permission
		/// </summary>
		/// <param name="file">The name of the file which will be overwritten</param>
		/// <returns><c>true</c> if the user authorizes the overwrite, otherwise <c>false</c></returns>
		private static bool Overwrite (string file)
		{
			FileInfo fi = new FileInfo (file);
			if (!fi.Exists)
				return true;
			Console.Write ("File {0} already exists. Overwrite? [yN]", fi.Name);
			string answer = Console.ReadLine().ToLower();
			if ((answer == "y") || (answer == "yes")) {
				fi.Delete ();
				return true;
			}
			return false;
		}

		static void SaveGrammarFile (string name, string header, string formatSpec, string content)
		{
			string fileName = String.Format ("{0}.txt", name);
			fileName = Loader.GetPath("grammars", fileName);
			if (!Overwrite (fileName))
				return;
			string Name = name.Substring (0, 1).ToUpper () + name.Substring (1);
			header = header.Replace ("${GrammarName}", Name);
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.WriteLine (header);
				writer.WriteLine (formatSpec);
				writer.WriteLine (content);
				writer.Close ();
			}

		}

		/// <summary>
		/// Writes down a set of example grammar files in the grammars sub directory
		/// </summary>
		private static void SaveGrammars()
		{
			string path = Loader.GetPath("grammars");
			if(!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string formatSpec = Resources.FormatSpecification;
			string authoring = Resources.GrammarHeader;

			Dictionary<string, string> grammars = new Dictionary<string, string> ();
			grammars.Add ("count", Resources.CountGrammar);
			grammars.Add("incomplete", Resources.IncompleteCommandsGrammar);
			grammars.Add("incongruent", Resources.IncongruentCommandsGrammar);
			grammars.Add("category1", Resources.Category1Grammar);
			grammars.Add("category2", Resources.Category2Grammar);
			grammars.Add("category3", Resources.Category3Grammar);

			foreach (KeyValuePair<string, string> g in grammars) {
				try{
					SaveGrammarFile (g.Key, authoring, formatSpec, g.Value);
				}catch{}
			}
		}

		/// <summary>
		/// Writes the datafiles, asking the user for overwriting.
		/// </summary>
		/// <param name="gestures">Gestures list container</param>
		/// <param name="categories">Categories list container.</param>
		/// <param name="locations">Locations list container.</param>
		/// <param name="names">Names list container.</param>
		/// <param name="questions">Questions list container.</param>
		private static void WriteDatafiles (GestureContainer gestures, CategoryContainer categories, LocationContainer locations, NameContainer names, QuestionsContainer questions)
		{
			string path = Loader.GetPath ("Gestures.xml");
			if (Overwrite (path))
				Loader.Save (path, gestures);
			path = Loader.GetPath ("Locations.xml");
			if (Overwrite (path))
				Loader.Save (path, locations);
			path = Loader.GetPath ("Names.xml");
			if (Overwrite (path))
				Loader.Save (path, names);
			path = Loader.GetPath ("Objects.xml");
			if (Overwrite (path))
				Loader.Save (path, categories);
			path = Loader.GetPath ("Questions.xml");
			if (Overwrite (path))
				Loader.Save (path, questions);
		}
	}
}

