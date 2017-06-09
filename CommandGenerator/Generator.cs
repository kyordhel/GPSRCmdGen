using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RoboCup.AtHome.CommandGenerator.Containers;
using RoboCup.AtHome.CommandGenerator.ReplaceableTypes;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Generates Random Sentences for the GPSR test
	/// </summary>
	public abstract class Generator
	{
		#region Variables

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
		protected List<Gesture> allGestures;
		/// <summary>
		/// Stores all known locations
		/// </summary>
		protected LocationManager allLocations;
		/// <summary>
		/// Stores all known names
		/// </summary>
		protected List<PersonName> allNames;
		/// <summary>
		/// Stores all known objects
		/// </summary>
		protected ObjectManager allObjects;
		/// <summary>
		/// Stores all known questions
		/// </summary>
		protected List<PredefinedQuestion> allQuestions;
		/// <summary>
		/// Stores all generation grammars
		/// </summary>
		protected List<Grammar> allGrammars;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of Generator
		/// </summary>
		public Generator ()
		{
			// Initialize all objects
			this.rnd = new Random (DateTime.Now.Millisecond);
			this.allGestures = new List<Gesture> ();
			this.allLocations = LocationManager.Instance;
			this.allNames = new List<PersonName> ();
			this.allObjects = ObjectManager.Instance;
			this.allGrammars = new List<Grammar> ();
			this.allQuestions = new List<PredefinedQuestion>();
			GenerateSortedDifficultyDegreesArray ();
			this.Quiet = false;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the list that stores all known locations
		/// </summary>
		internal LocationManager AllLocations { get{return this.allLocations; } }

		/// <summary>
		/// Gets the list that stores all known gestures
		/// </summary>
		internal List<Gesture> AllGestures { get{return this.allGestures; } }

		/// <summary>
		/// Gets the list that stores all known names
		/// </summary>
		internal List<PersonName> AllNames{ get { return this.allNames; } }

		/// <summary>
		/// Stores all known objects
		/// </summary>
		internal ObjectManager AllObjects { get { return this.allObjects; } }

		/// <summary>
		/// Stores all known questions
		/// </summary>
		internal List<PredefinedQuestion> AllQuestions { get { return this.allQuestions; } }

		/// <summary>
		/// Gets the random numbers generator
		/// </summary>
		internal Random Rnd{ get { return this.rnd; } }

		/// <summary>
		/// Gets the list that stores sorted difficulty degrees from the hardes to the easiest
		/// </summary>
		internal List<DifficultyDegree> SortedDD{ get{ return this.sdd; } }

		public bool Quiet { get; set; }

		#endregion

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

		/// <summary>
		/// Randomly generates a task with the requested difficulty degree
		/// </summary>
		/// <param name="tier">The maximum difficulty degree allowed to produce the task</param>
		/// <returns></returns>
		public Task GenerateTask (DifficultyDegree tier)
		{
			for (int i = 0; i < 3; ++i)
			{
				try
				{
					string taskPrototype = GetTaskPrototype(tier);
					WildcardReplacer replacer = new WildcardReplacer(this, tier);
					if (String.IsNullOrEmpty(taskPrototype))
						return null;
					return replacer.GetTask(taskPrototype);
				}
				catch
				{
					continue;
				}
			}
			return null;
		}

		/// <summary>
		/// Randomly generates a task with the given grammar
		/// </summary>
		/// <param name="grammarName">The name of the grammar to generate the task</param>
		/// <param name="tier">The maximum difficulty degree allowed to produce the task</param>
		/// <returns></returns>
		public Task GenerateTask (string grammarName, DifficultyDegree tier)
		{
			for (int i = 0; i < 3; ++i)
			{
				try
				{
					string taskPrototype = GetTaskPrototype(grammarName);
					WildcardReplacer replacer = new WildcardReplacer(this, tier);
					if (String.IsNullOrEmpty(taskPrototype))
						return null;
					return replacer.GetTask(taskPrototype);
				}
				catch
				{
					continue;
				}
			}
			return null;
		}

		/// <summary>
		/// Obtains a random grammar with the highest difficulty degree possible
		/// </summary>
		/// <param name="tier">The maximum difficulty degree allowed</param>
		/// <returns>A grammar</returns>
		private Grammar GetRandomGrammar (DifficultyDegree tier)
		{
			int idd;
			Grammar g;
			foreach (DifficultyDegree dd in sdd) {
				idd = (int)dd;
				if ( (idd > (int)tier) || (idd < 1) )
					continue;
				g = GetTieredElement (dd, allGrammars);
				if (g != null)
					return g;
				if(!Quiet)
					Warn("No grammars were found for {0} difficulty degree. {1}", dd, idd > 1 ? "Grammar tier reduced." : String.Empty);
			}
			return null;
		}

		/// <summary>
		/// Randomly selects a grammar of the specified difficulty degree (or lower)
		/// and uses it to produce a random task sentence prototype. The prototype
		/// contains several tokens to be replaced by random values
		/// </summary>
		/// <param name="tier">The maximum difficulty degree allowed</param>
		/// <returns>A task sentence prototype with unsolved constructs</returns>
		protected string GetTaskPrototype (DifficultyDegree tier)
		{	
			Grammar g;

			g = GetRandomGrammar (tier);
			if (g == null) {
				Err ("No grammars could be selected. Aborting.");
				return String.Empty;
			}

			if(!Quiet){
				if (String.IsNullOrEmpty (g.Name))
					Console.WriteLine ("Selected {0} difficulty degree grammar.", g.Tier);
				else
					Console.WriteLine ("Selected {0} ({1} difficulty degree grammar).", g.Name, g.Tier);
			}

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
		/// Produces a random task sentece prototype from the grammar with the given name.
		///  The prototype contains several tokens to be replaced by random values
		/// </summary>
		/// <param name="grammarName">The name of the grammar to generate the task</param>
		/// <returns>A task sentence prototype with unsolved constructs</returns>
		protected string GetTaskPrototype (string grammarName)
		{	
			Grammar g = this.allGrammars.FirstOrDefault( o => o.Name == grammarName );
			if (g == null)
			{
				Err("Grammar " + grammarName + "does not exist. Aborting.");
				return String.Empty;
			}
			if(!Quiet)
				Console.WriteLine ("Selected {0} grammar.", grammarName);

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

		/// <summary>
		/// Returns a randomly chosen element from the source set using the generator seed.
		/// </summary>
		/// <returns>An element from source chosen randomly</returns>
		/// <param name="source">The source array</param>
		/// <typeparam name="T">The type of the elements of source.</typeparam>
		public T RandomPick<T>(IList <T> source){
			if (source == null)
				throw new ArgumentNullException ("Source is null.");
			if (source.Count < 1)
				throw new InvalidOperationException ("The source sequence is empty.");
			return source [rnd.Next (0, source.Count)];
		}

		/// <summary>
		/// Returns a randomly chosen element from the source set using the generator seed.
		/// </summary>
		/// <returns>An element from source chosen randomly</returns>
		/// <param name="source">The source array</param>
		/// <typeparam name="T">The type of the elements of source.</typeparam>
		public T RandomPick<T>(params T [] source){
			if (source == null)
				throw new ArgumentNullException ("Source is null.");
			if (source.Length < 1)
				throw new InvalidOperationException ("The source sequence is empty.");
			return source [rnd.Next (0, source.Length)];
		}

		#region Load Methods

		/// <summary>
		/// Loads the set of gestures from disk. If no gestures file is found, 
		/// the default set is loaded from Factory
		/// </summary>
		public virtual void LoadGestures ()
		{
				this.allGestures = Loader.Load<GestureContainer> (Loader.GetPath("Gestures.xml")).Gestures;
				Green("Done!");
		}

		/// <summary>
		/// Loads the grammars from disk. If no grammars are found, the application is
		/// terminated.
		/// </summary>
		public virtual void LoadGrammars()
		{
			this.allGrammars = Loader.LoadGrammars();
			Green("Done!");
		}

		/// <summary>
		/// Loads the set of locations from disk. If no locations file is found, 
		/// the default set is loaded from Factory
		/// </summary>
		public virtual void LoadLocations ()
		{
			this.allLocations = Loader.LoadLocations(Loader.GetPath("Locations.xml"));
			Green("Done!");
		}

		/// <summary>
		/// Loads the set of names from disk. If no names file is found, 
		/// the default set is loaded from Factory
		/// </summary>
		public virtual void LoadNames ()
		{
			this.allNames = Loader.Load<NameContainer>(Loader.GetPath("Names.xml")).Names;
			Green("Done!");
		}

		/// <summary>
		/// Loads the set of objects and categories from disk. If no objects file is found, 
		/// the default set is loaded from Factory
		/// </summary>
		public virtual void LoadObjects()
		{
			this.allObjects = Loader.LoadObjects(Loader.GetPath("Objects.xml"));
			Green("Done!");
		}

		/// <summary>
		/// Loads the set of questions from disk. If no questions file is found, 
		/// the default set is loaded from Factory
		/// </summary>
		public virtual void LoadQuestions()
		{
			this.allQuestions = Loader.Load<QuestionsContainer>(Loader.GetPath("Questions.xml")).Questions;
			Green("Done!");
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
		public static void Err(Exception ex){
			Err (null, ex);
		}

		public static void Err(string format, params object[] args)
		{
			Err (String.Format (format, args));
		}

		/// <summary>
		/// Writes the provided message string to the console in RED text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void Err(string message)
		{
			Err(message, (Exception)null);
		}

		/// <summary>
		/// Writes the provided message string and exception's Message to the console in RED text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		/// <param name="ex">Exception to be written.</param>
		public static void Err(string message, Exception ex)
		{
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
		public static void Warn(Exception ex)
		{
			Err (null, ex);
		}

		public static void Warn(string format, params object[] args)
		{
			Err (String.Format (format, args));
		}

		/// <summary>
		/// Writes the provided message string to the console in YELLOW text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void Warn(string message)
		{
			Err(message, (Exception)null);
		}

		/// <summary>
		/// Writes the provided message string and exception's Message to the console in YELLOW text
		/// </summary>
		/// <param name="message">The message to be written.</param>
		/// <param name="ex">Exception to be written.</param>
		public static void Warn(string message, Exception ex)
		{
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
		public static void Green(string message)
		{
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine (message);
			Console.ForegroundColor = pc;
		}

		#endregion
	}
}

