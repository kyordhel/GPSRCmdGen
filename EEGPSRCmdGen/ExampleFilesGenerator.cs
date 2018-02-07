using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using RoboCup.AtHome.CommandGenerator;
using RoboCup.AtHome.CommandGenerator.Containers;
using RoboCup.AtHome.CommandGenerator.ReplaceableTypes;

namespace RoboCup.AtHome.EEGPSRCmdGen
{
	/// <summary>
	/// Example files generator.
	/// </summary>
	public static class ExampleFilesGenerator
	{
		/// <summary>
		/// Adds the location of the objects in the given category list to the given rooms list
		/// </summary>
		/// <param name="rooms">The list of locations grouped by room</param>
		/// <param name="categories">The list of objects grouped by category</param>
		private static void AddObjectLocations(List<Room> rooms, List<Category> categories)
		{
			foreach (Category cat in categories)
			{
				int ix = -1;
				for (int i = 0; i < rooms.Count; ++i)
				{
					if (rooms[i].Name == cat.RoomString) ix = i;
				}
				if (ix == -1)
				{
					ix = rooms.Count;
					rooms.Add(new Room(cat.RoomString));
				}
				rooms[ix].AddLocation(cat.DefaultLocation);
			}
		}

		/// <summary>
		/// Writes down a set of example data files with information gathered from the Factory
		/// </summary>
		public static void GenerateExampleFiles(){
			SaveGrammars ();
			WriteDatafiles ();
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
			fileName = Loader.GetPath("eegpsr_grammars", fileName);
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
			string path = Loader.GetPath("eegpsr_grammars");
			if(!Directory.Exists(path))
				Directory.CreateDirectory(path);

			string formatSpec = Resources.FormatSpecification;
			string authoring = Resources.GrammarHeader;

			Dictionary<string, string> grammars = new Dictionary<string, string> ();
			grammars.Add("eegpsr_cat1", Resources.EEGPSR_Category1);
			grammars.Add("eegpsr_cat2", Resources.EEGPSR_Category2);
			grammars.Add("eegpsr_cat3", Resources.EEGPSR_Category3);
			grammars.Add("eegpsr_cat1e", Resources.EEGPSR_Category1e);
			grammars.Add("eegpsr_cat2e", Resources.EEGPSR_Category2e);
			grammars.Add("eegpsr_cat3e", Resources.EEGPSR_Category3e);
			grammars.Add("eegpsr_cat1i", Resources.EEGPSR_Category1i);
			grammars.Add("eegpsr_cat2i", Resources.EEGPSR_Category2i);
			grammars.Add("eegpsr_cat3i", Resources.EEGPSR_Category3i);
			grammars.Add("common", Resources.CommonRules);

			foreach (KeyValuePair<string, string> g in grammars) {
				try{
					SaveGrammarFile (g.Key, authoring, formatSpec, g.Value);
				}catch{}
			}
		}

		/// <summary>
		/// Writes the default datafiles.
		/// </summary>
		private static void WriteDatafiles (){
			string path = Loader.GetPath ("Gestures.xml");
			if (Overwrite (path))
				File.WriteAllText(path, Resources.Gestures);
			path = Loader.GetPath ("Locations.xml");
			if (Overwrite (path))
				File.WriteAllText(path, Resources.Locations);
			path = Loader.GetPath ("Names.xml");
			if (Overwrite (path))
				File.WriteAllText(path, Resources.Names);
			path = Loader.GetPath ("Objects.xml");
			if (Overwrite (path))
				File.WriteAllText(path, Resources.Objects);
			path = Loader.GetPath ("Questions.xml");
			if (Overwrite (path))
				File.WriteAllText(path, Resources.Questions);
		}

		/// <summary>
		/// Writes the datafiles, asking the user for overwriting.
		/// </summary>
		/// <param name="gestures">Gestures list container</param>
		/// <param name="categories">Categories list container.</param>
		/// <param name="locations">Locations list container.</param>
		/// <param name="names">Names list container.</param>
		/// <param name="questions">Questions list container.</param>
		private static void WriteDatafiles (GestureContainer gestures, CategoryContainer categories, RoomContainer locations, NameContainer names, QuestionsContainer questions)
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

