using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace GPSRCmdGen
{

	[XmlRoot(ElementName = "gestures", Namespace = "")]
	public class GestureContainer{

		public GestureContainer(){}
		public GestureContainer(List<Gesture> gestures){this.Gestures=gestures;}

		[XmlElement("gesture")]
		public List<Gesture> Gestures{ get; set; } 
	}

	[XmlRoot(ElementName = "locations", Namespace = "")]
	public class LocationContainer{

		public LocationContainer(){}
		public LocationContainer(List<Location> locations){this.Locations=locations;}

		[XmlElement("location")]
		public List<Location> Locations{ get; set; } 
	}

	[XmlRoot(ElementName = "names", Namespace = "")]
	public class NameContainer{

		public NameContainer(){}
		public NameContainer(List<Name> names){this.Names=names;}

		[XmlElement("name")]
		public List<Name> Names{ get; set; } 
	}

	[XmlRoot(ElementName = "categories", Namespace = "")]
	public class CategoryContainer{

		public CategoryContainer(){}
		public CategoryContainer(List<Category> categories){this.Categories=categories;}

		[XmlElement("category")]
		public List<Category> Categories{ get; set; } 
	}

	[XmlRoot(ElementName = "questions", Namespace = "")]
	public class QuestionsContainer{

		public QuestionsContainer(){}
		public QuestionsContainer(List<PredefindedQuestion> questions) { this.Questions = questions; }

		[XmlElement("question")]
		public List<PredefindedQuestion> Questions { get; set; }
	}

	public static class ExampleFilesGenerator
	{

		public static void GenerateExampleFiles(){
			SaveGrammars ();

			GestureContainer gestures = new GestureContainer(Factory.GetDefaultGestures());
			CategoryContainer categories = new CategoryContainer(Factory.GetDefaultObjects().Categories);
			LocationContainer locations = new LocationContainer (Factory.GetDefaultLocations ());
			NameContainer names = new NameContainer (Factory.GetDefaultNames ());
			foreach (Category cat in categories.Categories) {
				if (!locations.Locations.Contains (cat.DefaultLocation))
					locations.Locations.Add (cat.DefaultLocation);
			}
			QuestionsContainer questions = new QuestionsContainer(Factory.GetDefaultQuestions());

			if (Prompt("Gestures.xml"))
				Loader.Save("Gestures.xml", gestures);
			if (Prompt("Locations.xml"))
				Loader.Save("Locations.xml", locations);
			if (Prompt("Names.xml"))
				Loader.Save("Names.xml", names);
			if(Prompt("Objects.xml"))
				Loader.Save ("Objects.xml", categories);
			if (Prompt("Questions.xml"))
				Loader.Save("Questions.xml", questions);
		}

		private static bool Prompt (string file)
		{
			if (!File.Exists (file))
				return true;
			Console.Write ("File {0} already exists. Overwrite? [yN]", file);
			string answer = Console.ReadLine().ToLower();
			if ((answer == "y") || (answer == "yes")) {
				File.Delete (file);
				return true;
			}
			return false;
		}

		static void SaveGrammarFile (string name, string header, string formatSpec, string content)
		{
			string fileName = String.Format ("{0}.txt", name);
			fileName = Path.Combine("grammars", fileName);
			if (File.Exists (fileName)){
				if(!Prompt (fileName))
					return;
				File.Delete (fileName);
			}
			string Name = name.Substring (0, 1).ToUpper () + name.Substring (1);
			header = header.Replace ("${GrammarName}", Name);
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.WriteLine (header);
				writer.WriteLine (formatSpec);
				writer.WriteLine (content);
				writer.Close ();
			}

		}

		private static void SaveGrammars()
		{
			if(!Directory.Exists("grammars"))
				Directory.CreateDirectory("grammars");

			string formatSpec = Resources.FormatSpecification;
			string authoring = Resources.GrammarHeader;

			Dictionary<string, string> grammars = new Dictionary<string, string> ();
			grammars.Add ("count", Resources.CountGrammar);
			grammars.Add("category1", Resources.Category1Grammar);
			grammars.Add("category2", Resources.Category2Grammar);
			grammars.Add("category3", Resources.Category3Grammar);

			foreach (KeyValuePair<string, string> g in grammars) {
				try{
					SaveGrammarFile (g.Key, authoring, formatSpec, g.Value);
				}catch{}
			}
		}
	}
}

