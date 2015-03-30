using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GPSRCmdGen
{
	/// <summary>
	/// Helper class for loading data in legacy text files
	/// </summary>
	public static class TextLoader
	{
		/// <summary>
		/// Initializes the <see cref="GPSRCmdGen.TextLoader"/> class.
		/// </summary>
		static TextLoader (){ }

		/// <summary>
		/// Loads the set of gestures from <c>./gestures.txt</c></c>
		/// </summary>
		public static List<Gesture> LoadGestures ()
		{
			List<string> lines = ReadFile ("gestures.txt");
			if (lines == null)
				return null;

			List<Gesture> gestures = new List<Gesture> (lines.Count);
			foreach (string gestureName in lines)
				gestures.Add (new Gesture (gestureName));
			return gestures;
		}

		/// <summary>
		/// Loads grammars from 
		/// <c>./cat1sentences.txt</c>,
		/// <c>./cat2sentences.txt</c>, and
		/// <c>./cat3sentences.txt</c>
		/// </summary>
		public static object LoadGrammars ()
		{
			Grammar g;
			List<Grammar> grammars = new List<Grammar> (3);
			List<string> lines = ReadFile (".txt");
			if (lines == null)
				return null;
			return grammars;

		}

		/// <summary>
		/// Loads the set of locations from <c>./locations.txt</c>
		/// </summary>
		public static object LoadLocations ()
		{
			List<string> lines = ReadFile ("locations.txt");
			if (lines == null)
				return null;
			return null;
		}

		/// <summary>
		/// Loads the set of names from <c>./names.txt</c>
		/// </summary>
		public static List<PersonName> LoadNames ()
		{
			List<string> lines = ReadFile ("names.txt");
			if (lines == null)
				return null;

			List<PersonName> names = new List<PersonName> (lines.Count);
			foreach (string name in lines)
				names.Add (new PersonName(name));
			return names;
		}

		/// <summary>
		/// Loads the set of objects and categories with their locations from
		/// <c>./items.txt</c>, and
		/// <c>./item_categories.txt</c>, and
		/// </summary>
		public static List<Category> LoadObjects ()
		{
			List<string> objNames = ReadFile ("items.txt", false);
			List<string> catNames = ReadFile ("item_categories.txt", false);
			if ((objNames == null) || (catNames == null) || (objNames.Count < catNames.Count))
				return null;

			SpecificLocation loc = new SpecificLocation ("table", true, false);
			Dictionary<string, Category> categories = new Dictionary<string, Category> ();
			for (int i = 0; i < catNames.Count; ++i) {
				if(!categories.ContainsKey(catNames [i]))
					categories.Add(catNames [i], new Category (catNames [i], loc));
				categories[catNames [i]].AddObject(objNames [i], GPSRObjectType.Known, DifficultyDegree.Easy);
			}
			return new List<Category>(categories.Values);
		}

		/// <summary>
		/// Loads the set of questions from <c>./questions.txt</c>
		/// </summary>
		public static object LoadQuestions()
		{
			List<string> lines = ReadFile ("questions.txt");
			if (lines == null)
				return null;
			return null;

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

		private static List<string> RetrieveUsefulLines(string[] allLines, bool skipEmptyLines)
		{
			if(allLines == null)
			return null;

			List<string> lines = new List<string> (allLines.Length);
			foreach (string line in allLines) {
				string trimmed = line.Trim ();
				int padPos = trimmed.IndexOf ('#');
				if (padPos != -1)
					trimmed = trimmed.Substring (padPos);
				if (skipEmptyLines && String.IsNullOrEmpty (trimmed))
					continue;
				lines.Add (trimmed);
			}
			return lines.Count > 0 ? lines : null;
		}
	}
}

