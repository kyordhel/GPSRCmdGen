using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GPSRCmdGen
{
	/// <summary>
	/// Helper class for loading data in legacy text files
	/// </summary>
	public static class TextLoader
	{
		#region Variables

		private static string[] grammarFiles;
		private static Dictionary<string, int> counters;
		private static Regex rxTextWildcard;
		private static MatchEvaluator dlgReplaceTextWildcards;
		private static Room room;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes the <see cref="GPSRCmdGen.TextLoader"/> class.
		/// </summary>
		static TextLoader (){
			TextLoader.grammarFiles = new String[] {
				"cat1Sentences.txt",
				"cat2Sentences.txt",
				"cat3Situations.txt" };
			TextLoader.rxTextWildcard = new Regex(@"\w+");
			TextLoader.counters = new Dictionary<string, int> ();
			TextLoader.dlgReplaceTextWildcards = new MatchEvaluator (ReplaceTextWildcards);
			TextLoader.room = new Room ("home");
		}

		#endregion

		#region Public Methods

		public static bool DataFilesExist(bool silent = true){
			bool missing = false;
			string[] requiredFiles = {
				"locations.txt", "names.txt", "items.txt", "item_categories.txt"};
			List<string> required = new List<string> ();
			required.AddRange (requiredFiles);
			required.AddRange (grammarFiles);
			foreach(string file in required){
				if(!System.IO.File.Exists(Loader.GetPath(file))){
					missing = true;
					if(!silent)
					Generator.Warn(String.Format("{0} not found", file));
				}
			}

			if(missing)
				return false;
			return true;
		}

		public static void LoadData (Generator generator)
		{
			Console.Write ("Loading objects...");
			TextLoader.LoadObjects();
			Console.Write ("Loading names...");
			TextLoader.LoadNames (generator);
			Console.Write ("Loading locations...");
			TextLoader.LoadLocations ();
			Console.Write ("Loading gestures...");
			TextLoader.LoadGestures(generator);
			Console.Write("Loading predefined questions...");
			TextLoader.LoadQuestions(generator);
			Console.Write ("Loading grammars...");
			TextLoader.LoadGrammars (generator);
		}

		#endregion

		#region Private Methods

		private static DifficultyDegree GetGrammarTier (string filePath)
		{
			if (filePath.Length < 3)
				return DifficultyDegree.Unknown;
			switch (filePath [3] - '0') {
			case 1:return DifficultyDegree.Easy;
			case 2:return DifficultyDegree.Moderate;
			case 3:return DifficultyDegree.High;
			default: return DifficultyDegree.Unknown;
			}
		}

		private static Grammar ConvertGrammar (string filePath)
		{
			List<string> lines = ReadFile (filePath);
			if (lines == null)
				return null;

			ProductionRule main = ProductionRule.CreateMainRule ();
			counters.Clear ();
			for(int i = 0; i < lines.Count; ++i) {
				if (lines [i].Contains ("situation:"))
					continue;
				if (lines [i].StartsWith ("question: "))
					lines [i] = lines[i].Substring (10);
				main.Replacements.Add (rxTextWildcard.Replace (lines[i], dlgReplaceTextWildcards));
			}

			Grammar g = new Grammar ();
			g.Tier = GetGrammarTier(filePath);
			g.Name = filePath;
			g.ProductionRules.Add (main.NonTerminal, main);
			return g;
		}

		/// <summary>
		/// Loads the set of gestures from <c>./gestures.txt</c></c>
		/// </summary>
		private static void LoadGestures (Generator gen)
		{
			List<string> lines = ReadFile ("gestures.txt");
			if (lines == null) {
				gen.AllGestures.AddRange (Factory.GetDefaultGestures ());
				Generator.Err ("Failed! Default Gestures loaded");
				return;
			}

			foreach (string gestureName in lines)
				gen.AllGestures.Add (new Gesture (gestureName));
			Generator.Green("Done!");
		}

		/// <summary>
		/// Loads grammars from 
		/// <c>./cat1sentences.txt</c>,
		/// <c>./cat2sentences.txt</c>, and
		/// <c>./cat3sentences.txt</c>
		/// </summary>
		private static void LoadGrammars (Generator gen)
		{
			Grammar g;

			foreach (string grammarFile in grammarFiles) {
				g = ConvertGrammar (grammarFile);
				if (g != null)
					gen.AllGrammars.Add (g);
			}
			Generator.Green("Done!");
		}

		/// <summary>
		/// Loads the set of locations from <c>./locations.txt</c>
		/// </summary>
		private static void LoadLocations ()
		{
			List<string> lines = ReadFile ("locations.txt");
			if (lines == null)
			{
				Factory.GetDefaultLocations ();
				Generator.Err ("Failed! Default Locations loaded");
				return;
			}

			foreach (string locName in lines)
				room.AddLocation(locName, true, true);
			LocationManager.Instance.Add (room);
			Generator.Green("Done!");
		}

		/// <summary>
		/// Loads the set of names from <c>./names.txt</c>
		/// </summary>
		private static void LoadNames (Generator gen)
		{
			List<string> lines = ReadFile ("names.txt");
			if (lines == null)
			{
				gen.AllNames.AddRange (Factory.GetDefaultNames ());
				Generator.Err ("Failed! Default Names loaded");
				return;
			}

			foreach (string name in lines)
				gen.AllNames.Add (new PersonName(name));
			Generator.Green("Done!");
		}

		/// <summary>
		/// Loads the set of objects and categories with their locations from
		/// <c>./items.txt</c>, and
		/// <c>./item_categories.txt</c>, and
		/// </summary>
		private static void LoadObjects ()
		{
			List<string> objNames = ReadFile ("items.txt", true);
			List<string> catNames = ReadFile ("item_categories.txt", true);
			if ((objNames == null) || (catNames == null) || (objNames.Count < catNames.Count))
			{
				Factory.GetDefaultObjects ();
				Generator.Err ("Failed! Default Objects loaded");
				return;
			}

			SpecificLocation loc = new SpecificLocation ("table", true, false);
			loc.Room = room;
			Dictionary<string, Category> categories = new Dictionary<string, Category> ();
			for (int i = 0; i < catNames.Count; ++i) {
				if(!categories.ContainsKey(catNames [i]))
					categories.Add(catNames [i], new Category (catNames [i], loc));
				categories[catNames [i]].AddObject(objNames [i], GPSRObjectType.Known, DifficultyDegree.Easy);
			}
			foreach (Category cat in categories.Values)
				GPSRObjectManager.Instance.Add (cat);
			Generator.Green("Done!");
		}

		/// <summary>
		/// Loads the set of questions from <c>./questions.txt</c>
		/// </summary>
		private static void LoadQuestions(Generator gen)
		{
			List<string> lines = ReadFile ("questions.txt");
			if (lines == null) {
				gen.AllQuestions.AddRange (Factory.GetDefaultQuestions ());
				Generator.Err ("Failed! Default Questions loaded");
				return;
			}

			gen.AllQuestions.AddRange (Factory.GetDefaultQuestions ());
			Generator.Green("Done!");
		}

		private static List<string> ReadFile(string fileName, bool skipEmptyLines = true){
			string[] allLines;

			try{
				fileName = Loader.GetPath (fileName);
				allLines = File.ReadAllLines(fileName);
			}
			catch{ return null; }
			return RetrieveUsefulLines (allLines, skipEmptyLines);
		}

		private static string ReplaceTextWildcards (Match match)
		{
			if (!counters.ContainsKey (match.Value))
				counters.Add (match.Value, 0);
			int count = (counters [match.Value] += 1);

			switch (match.Value) {
			case "LOCATION":
				return string.Format("{{location {0}}}", count);

			case "ITEM":
				return string.Format("{{object {0}}}", count);

			case "NAME":
				return string.Format("{{name {0}}}", count);

			case "ITEM_CATEGORY":
				return string.Format("{{category {0}}}", count);

			// case "LOCATION_CATEGORY":
			//	return string.Format("{name {0}}", count);
			}
			return match.Value;
		}

		private static List<string> RetrieveUsefulLines(string[] allLines, bool skipEmptyLines)
		{
			if(allLines == null)
			return null;

			List<string> lines = new List<string> (allLines.Length);
			foreach (string line in allLines) {
				string trimmed = line.Trim ();
				int padPos = trimmed.IndexOf ('#');
				if (padPos != -1)
					trimmed = trimmed.Substring (0, padPos);
				if (skipEmptyLines && String.IsNullOrEmpty (trimmed))
					continue;
				lines.Add (trimmed);
			}
			return lines.Count > 0 ? lines : null;
		}

		#endregion
	}
}

