using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GPSRCmdGen
{
	public class Generator
	{
		/// <summary>
		/// Random numbers generator
		/// </summary>
		private Random rnd;
		/// <summary>
		/// Sorted difficulty degrees from the hardes to the easiest
		/// </summary>
		private List<DifficultyDegree> sdd;
		/// <summary>
		/// Stores all known gestures
		/// </summary>
		private List<Gesture> allGestures;
		/// <summary>
		/// Stores all known locations
		/// </summary>
		private List<Location> allLocations;
		/// <summary>
		/// Stores all known names
		/// </summary>
		private List<Name> allNames;
		/// <summary>
		/// Stores all known objects
		/// </summary>
		private GPSRObjectManager allObjects;
		/// <summary>
		/// Stores all generation grammars
		/// </summary>
		private List<Grammar> allGrammars;

		public Generator ()
		{
			// Initialize all objects
			this.rnd = new Random (DateTime.Now.Millisecond);
			this.allGestures = new List<Gesture> ();
			this.allLocations = new List<Location> ();
			this.allNames = new List<Name> ();
			this.allObjects = new GPSRObjectManager ();
			this.allGrammars = new List<Grammar> ();
			GenerateSortedDifficultyDegreesArray ();
		}

		/// <summary>
		/// Gets the list that stores all known locations
		/// </summary>
		internal List<Location> AllLocations { get{return this.allLocations; } }

		/// <summary>
		/// Gets the list that stores all known gestures
		/// </summary>
		internal List<Gesture> AllGestures { get{return this.allGestures; } }

		/// <summary>
		/// Gets the list that stores all known names
		/// </summary>
		internal List<Name> AllNames{ get { return this.allNames; } }

		/// <summary>
		/// Stores all known objects
		/// </summary>
		internal GPSRObjectManager AllObjects{ get { return this.allObjects; } }

		/// <summary>
		/// Gets the random numbers generator
		/// </summary>
		internal Random Rnd{ get { return this.rnd; } }

		/// <summary>
		/// Gets the list that stores sorted difficulty degrees from the hardes to the easiest
		/// </summary>
		internal List<DifficultyDegree> SortedDD{ get{ return this.sdd; } }

		/// <summary>
		/// Generates the list of sorted difficulty degrees from the hardes to the easiest
		/// </summary>
		private void GenerateSortedDifficultyDegreesArray()
		{
			DifficultyDegree[] dda = (DifficultyDegree[])Enum.GetValues (typeof(DifficultyDegree));
			this.sdd = new List<DifficultyDegree>(
				dda.Where (dd => (int)dd > -1)
				.OrderByDescending(dd => (int)dd)
				);
		}

		public string GenerateTask (DifficultyDegree tier)
		{
			string task = GetTask (tier);
			WildcardReplacer replacer = new WildcardReplacer (this, tier);
			replacer.GetTokens (task);
			task = replacer.ReplaceWildcards (task);
			return task;
		}

		private Grammar GetRandomGrammar (DifficultyDegree tier)
		{
			int idd;
			Grammar g;
			foreach (DifficultyDegree dd in sdd) {
				idd = (int)dd;
				if ( (idd > (int)tier) || (idd < 1) )
					continue;
				g = GetTieredElement (tier, allGrammars);
				if (g != null)
					return g;
				Warn("No grammars were found for {0} difficulty degree. {1}", dd, idd > 1 ? "Grammar tier reduced." : String.Empty);
			}
			return null;
		}

		public string GetTask (DifficultyDegree tier)
		{	
			Grammar g;

			g = GetRandomGrammar (tier);
			if (g == null) {
				Err ("No grammars could be selected. Aborting.");
				return String.Empty;
			}
			if (String.IsNullOrEmpty (g.Name))
				Console.WriteLine ("Selected {0} difficulty degree grammar.", g.Tier);
			else
				Console.WriteLine ("Selected {0} ({1} difficulty degree grammar).", g.Name, g.Tier);

			try {
				return g.GenerateSentence (rnd);
			} catch (StackOverflowException) {
				Err ("Can't generate grammar. Grammar is recursive");
				return null;
			} catch {
				Err ("Can't generate grammar. Unexpected error");
				return null;
			}
		}	

		/// <summary>
		/// Gets a random element from the provided list with the specified difficulty degree.
		/// </summary>
		/// <param name="tier">The difficulty for fetched object.</param>
		/// <param name="baseList">Base list which contains all objects.</param>
		/// <typeparam name="T">The type of object to fetch. Must be ITiered.</typeparam>
		/// <returns>A random element from the list.</returns>
		private T GetTieredElement<T>(DifficultyDegree tier, List<T> baseList) where T : ITiered{

			if ((baseList == null) || (baseList.Count < 1))
				return default(T); // throw new Exception ("Provided baselist is null or empty");

			// Get a list with all the elements of the requested difficulty degree
			List<T> tieredList = new List<T> (baseList.Where(idd => idd.Tier == tier));
			if (tieredList.Count < 1)
				return default(T); //throw new Exception("No elements were found for the requested difficulty degree");

			// Return random object
			return tieredList[rnd.Next(tieredList.Count)];
		}

		#region Load Methods

		public void LoadGestures ()
		{
			try {
				this.allGestures = Loader.Load<GestureContainer> ("Gestures.xml").Gestures;
				Green("Done!");
			} catch {
				this.allGestures = Factory.GetDefaultGestures ();
				Err ("Failed! Default Gestures loaded");
			}
		}

		public void LoadGrammars ()
		{
			try {
				this.allGrammars = Loader.LoadGrammars ();
				Green("Done!");
			} catch {

				Err ("Failed! Application terminated");
				Environment.Exit (0);
			}
		}

		public void LoadLocations ()
		{
			try {
				this.allLocations = Loader.Load<LocationContainer> ("Locations.xml").Locations;
				Green("Done!");
			} catch {
				this.allLocations = Factory.GetDefaultLocations ();
				Err ("Failed! Default Locations loaded");
			}
		}

		public void LoadNames ()
		{
			try {
				this.allNames = Loader.Load<NameContainer> ("Names.xml").Names;
				Green("Done!");
			} catch {
				this.allNames = Factory.GetDefaultNames ();
				Err ("Failed! Default Names loaded");
			}
		}

		public void LoadObjects ()
		{
			try {
				this.allObjects = Loader.LoadObjects("Objects.xml");
				Green("Done!");
			} catch {
				this.allObjects = Factory.GetDefaultObjects ();
				Err ("Failed! Default Objects loaded");
			}
		}

		/// <summary>
		/// Validates all default locations of categories are contained in the locations array.
		/// </summary>
		public void ValidateLocations()
		{
			foreach(Category c in this.AllObjects.Categories)
			{
				if (!this.AllLocations.Contains (c.DefaultLocation))
					this.AllLocations.Add (c.DefaultLocation);
			}
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Writes the provided exception's Message to the console in RED text
		/// </summary>
		/// <param name="ex">Exception to be written.</param>
		private static void Err(Exception ex){
			Err (null, ex);
		}

		private static void Err(string format, params object[] args){
			Err (String.Format (format, args));
		}

		/// <summary>
		/// Writes the provided message string to the console in RED text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		private static void Err(string message){
			Err(message, (Exception)null);
		}

		/// <summary>
		/// Writes the provided message string and exception's Message to the console in RED text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		/// <param name="ex">Exception to be written.</param>
		private static void Err(string message, Exception ex){
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			if(!String.IsNullOrEmpty(message))
				Console.WriteLine (message);
			if(ex != null)
				Console.WriteLine ("Exception {0}:", ex.Message);
			Console.ForegroundColor = pc;
		}

		/// <summary>
		/// Writes the provided exception's Message to the console in YELLOW text
		/// </summary>
		/// <param name="ex">Exception to be written.</param>
		private static void Warn(Exception ex){
			Err (null, ex);
		}

		private static void Warn(string format, params object[] args){
			Err (String.Format (format, args));
		}

		/// <summary>
		/// Writes the provided message string to the console in YELLOW text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		private static void Warn(string message){
			Err(message, (Exception)null);
		}

		/// <summary>
		/// Writes the provided message string and exception's Message to the console in YELLOW text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		/// <param name="ex">Exception to be written.</param>
		private static void Warn(string message, Exception ex){
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			if(!String.IsNullOrEmpty(message))
				Console.WriteLine (message);
			if(ex != null)
				Console.WriteLine ("Exception {0}:", ex.Message);
			Console.ForegroundColor = pc;
		}

		/// <summary>
		/// Writes the provided message string to the console in GREEN text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		private static void Green(string message){
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine (message);
			Console.ForegroundColor = pc;
		}


		#endregion
	}
}

