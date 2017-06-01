using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Replaces wildcards in a task prototype string
	/// with random values from data stored in a generator.
	/// </summary>
	public class WildcardReplacer
	{
		#region Variables

		/// <summary>
		/// The random generator that contains all required data
		/// </summary>
		private readonly Generator generator;

		/// <summary>
		/// The maximum difficulty degree to be used during the replacement
		/// </summary>
		private DifficultyDegree tier;

		/// <summary>
		/// Dicctionary of used categories
		/// </summary>
		private Dictionary<string, Category> categories; 

		/// <summary>
		/// Dicctionary of used gestures
		/// </summary>
		private Dictionary<string, Gesture> gestures;

		/// <summary>
		/// Dicctionary of used locations
		/// </summary>
		private Dictionary<string, Location> locations;

		/// <summary>
		/// Dicctionary of used names
		/// </summary>
		private Dictionary<string, PersonName> names;

		/// <summary>
		/// Dicctionary of used objects
		/// </summary>
		private Dictionary<string, GPSRObject> objects;

		/// <summary>
		/// Dicctionary of used questions
		/// </summary>
		private Dictionary<string, PredefindedQuestion> questions;

		/// <summary>
		/// List of available categories
		/// </summary>
		private List<Category> avCategories; 
		
		/// <summary>
		/// List of available gestures
		/// </summary>
		private List<Gesture> avGestures;
		
		/// <summary>
		/// List of available locations
		/// </summary>
		private List<Location> avLocations;
		
		/// <summary>
		/// List of available names
		/// </summary>
		private List<PersonName> avNames;
		
		/// <summary>
		/// List of available objects
		/// </summary>
		private List<GPSRObject> avObjects;

		/// <summary>
		/// List of available objects
		/// </summary>
		private List<PredefindedQuestion> avQuestions;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.WildcardReplacer"/> class.
		/// </summary>
		/// <param name="g">A Generator which contains the databases and the random generator.</param>
		/// <param name="tier">The maximum difficulty degree for wildcard replacements</param>
		public WildcardReplacer(Generator g, DifficultyDegree tier){
			this.generator = g;
			this.tier = tier;
			this.categories = new Dictionary<string, Category> (); 
			this.gestures = new Dictionary<string, Gesture> ();
			this.locations = new Dictionary<string, Location> ();
			this.names = new Dictionary<string, PersonName> ();
			this.objects = new Dictionary<string, GPSRObject> ();
			this.questions = new Dictionary<string, PredefindedQuestion>();
			FillAvailabilityLists ();
		}

		#endregion

		#region Evaluate Methods

		/// <summary>
		/// Evaluates a <c>category</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateCategory (Wildcard w)
		{
			return GetFromList (w.Name.ToLower(), w.Id, GetCategory, categories);
		}

		/// <summary>
		/// Evaluates a <c>gesture</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateGesture(Wildcard w)
		{
			return GetFromList (w.Name.ToLower(), w.Id, GetGesture, gestures);
		}

		/// <summary>
		/// Evaluates a <c>location</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateLocation(Wildcard w)
		{
			string keycode = w.Name.ToLower ();
			if (keycode == "location")
			{
				switch (w.Type.ToLower())
				{
					case "beacon":
						keycode = "beacon"; break;

					case "room":
						keycode = "room"; break;

					case "placement":
						keycode = "placement"; break;
				}
			}
		
			return GetFromList (keycode, w.Id, GetLocation, locations);
		}

		/// <summary>
		/// Evaluates a <c>name</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateName(Wildcard w)
		{
			string keycode = w.Name.ToLower ();
			if (keycode == "name") {
				string type = w.Type.ToLower ();
				if (type == "male")
					keycode = "male";
				else if (type == "female")
					keycode = "female";
			}

			return GetFromList (keycode, w.Id, GetName, names);
		}

		/// <summary>
		/// Evaluates an <c>object</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateObject(Wildcard w)
		{
			string keycode = w.Name.ToLower ();
			if (keycode == "object") {
				string type = w.Type.ToLower ();
				if (type == "alike")
					keycode = "aobject";
				else if (type == "known")
					keycode = "kobject";
			}

			return GetFromList (keycode, w.Id, GetObject, objects);
		}

		/// <summary>
		/// Evaluates a <c>question</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateQuestion(Wildcard w)
		{
			return GetFromList (w.Name, w.Id, GetQuestion, questions);
		}

		/// <summary>
		/// Evaluates a void wildcard
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateVoid(Wildcard w)
		{
			return new HiddenTaskElement();
		}

		#endregion

		#region Random Generation Methos

		/// <summary>
		/// Retrieves a <c>Category</c> object from the corresponding availability
		/// list, also removing the element to avoid duplicates.
		/// </summary>
		/// <returns>A category</returns>
		private Category GetCategory ()
		{
			return this.avCategories.PopLast();
		}

		/// <summary>
		/// Retrieves a <c>Category</c> object from the corresponding availability
		/// list, also removing the element to avoid duplicates.
		/// </summary>
		/// <returns>A category</returns>
		private Gesture GetGesture ()
		{
			return this.avGestures.PopLast();
		}

		/// <summary>
		/// Retrieves a <c>Location</c> object from the corresponding availability
		/// list, also removing the element to avoid duplicates.
		/// </summary>
		/// <param name="keycode">The specific type of location to fetch<param>
		/// <returns>A location</returns>
		private Location GetLocation(string keycode)
		{
			Location item;
			switch (keycode)
			{
				case "beacon":
					item = this.avLocations.First(l => l.IsBeacon);
					break;

				case "room":
					item = this.avLocations.First(l => l is Room);
					break;

				case "placement":
					item = this.avLocations.First(l => l.IsPlacement);
					break;

				default:
					item = this.avLocations[this.avLocations.Count - 1];
					break;
			}
			this.avLocations.Remove(item);
			return item;
		}

		/// <summary>
		/// Retrieves a <c>Name</c> object from the corresponding availability
		/// list, also removing the element to avoid duplicates.
		/// </summary>
		/// <param name="keycode">The specific type of name to fetch<param>
		/// <returns>A name</returns>
		private PersonName GetName (string keycode)
		{
			PersonName item;
			switch(keycode){
				case "male":
				item = this.avNames.First(n => n.Gender == Gender.Male);
				break;

				case "female":
				item = this.avNames.First(n => n.Gender == Gender.Female);
				break;

				default:
				item = this.avNames [this.avNames.Count - 1];
				break;
			}
			this.avNames.Remove (item);
			return item;
		}

		/// <summary>
		/// Retrieves a <c>GPSRObject</c> object from the corresponding availability
		/// list, also removing the element to avoid duplicates.
		/// </summary>
		/// <param name="keycode">The specific type of object to fetch<param>
		/// <returns>An object</returns>
		private GPSRObject GetObject (string keycode)
		{
			GPSRObject item;
			switch(keycode){
				case "aobject":
				item = this.avObjects.First(o => o.Type == GPSRObjectType.Alike);
				break;

				case "kobject":
				item = this.avObjects.First(o => o.Type == GPSRObjectType.Known);
				break;

				// case "uobject":
				// return GPSRObject.Unknown;

				default:
				item = this.avObjects [this.avObjects.Count - 1];
				break;
			}
			this.avObjects.Remove (item);
			return item;
		}

		private T GetFromList<T>(string keycode, int id, Func<T> fetcher, Dictionary<string, T> assigned){
			if (id == -1)
				return fetcher();
			string key = keycode + id;
			if(assigned.ContainsKey(key))
				return assigned[key];
			T t = fetcher();
			assigned.Add (key, t);
			return t;
		}

		private T GetFromList<T>(string keycode, int id, Func<string, T> fetcher, Dictionary<string, T> assigned){
			if (id == -1)
				return fetcher(keycode);
			string key = keycode + id;
			if(assigned.ContainsKey(key))
				return assigned[key];
			T t = fetcher(keycode);
			assigned.Add (key, t);
			return t;
		}

		/// <summary>
		/// Retrieves a <c>Question</c> object from the corresponding availability
		/// list, also removing the element to avoid duplicates.
		/// </summary>
		/// <returns>A question</returns>
		private PredefindedQuestion GetQuestion()
		{
			return this.avQuestions.PopLast ();
		}

		/// <summary>
		/// Gets a subset of the provided list on which every element has at most the specified difficulty degree.
		/// </summary>
		/// <param name="baseList">Base list which contains all objects.</param>
		/// <typeparam name="T">The type of objects to fetch. Must be ITiered.</typeparam>
		/// <returns>The tiered subset.</returns>
		private List<T> GetTieredList<T>(List<T> baseList) where T : ITiered
		{
			if ((baseList == null) || (baseList.Count < 1))
				throw new Exception("Requested too many elements (count is greater than objects in baseList)");

			IEnumerable<T> ie = baseList.Where(item => (int)item.Tier <= (int)this.tier);
			List<T> tieredList = new List<T>(ie);
			return tieredList;
		}

		#endregion

		#region Tokenize Methods

		/// <summary>
		/// Converts the wildcard contained within the provided regular expression
		/// match object into a Token object, considering obfuscation
		/// </summary>
		/// <param name="w">The regular expression match object that contains the wildcard.</param>
		private Token TokenizeObfuscatedWildcard(Wildcard w)
		{
			INameable obfuscated = null;
			INameable inam = FindReplacement(w);
			switch (w.Name)
			{
				case "beacon":
				case "placement":
					obfuscated = ((SpecificLocation)inam).Room;
					break;

				case "object":
				case "aobject":
				case "kobject":
					obfuscated = ((GPSRObject)inam).Category;
					break;

				case "name":
				case "male":
				case "female":
					obfuscated = new Obfuscator("a person");
					break;

				case "category":
					obfuscated = new Obfuscator("objects");
					break;

				case "location":
					obfuscated = new Obfuscator("somewhere");
					break;

				case "room":
					obfuscated = new Obfuscator("apartment");
					break;
			}
			if(obfuscated == null)
				return new Token(w.Value, inam, FetchMetadata(w));
			Token token = new Token(w.Value, obfuscated, FetchMetadata(w));
			token.Metadata.Add(inam.Name);
			return token;
		}

		/// <summary>
		/// Converts the literal substring string, starting at the given position,
		/// into a Token object.
		/// </summary>
		/// <param name="taskPrototype">The task prototype string.</param>
		/// <param name="index">Start position within the task prototype.</param>
		private Token TokenizeSubstring(string taskPrototype, int index)
		{
			string ss = taskPrototype.Substring (index);
			return String.IsNullOrEmpty (ss) ? null : new Token (ss);
		}

		/// <summary>
		/// Converts the literal string to the left of the provided wildcard match
		/// into a Token object.
		/// </summary>
		/// <param name="taskPrototype">The task prototype string.</param>
		/// <param name="cc">Read header for the task prototupe string pointing to
		/// the start of the literal string</param>
		/// <param name="m">The regular expression match object that contains the next
		/// wildcard.</param>
		private Token TokenizeLeftLiteralString(string taskPrototype, ref int cc, Wildcard w)
		{
			string ss = taskPrototype.Substring (cc, w.Index - cc);
			cc = w.Index + w.Value.Length;
			return String.IsNullOrEmpty (ss) ? null : new Token (ss);
		}

		/// <summary>
		/// Converts the wildcard contained within the provided regular expression
		/// match object into a Token object.
		/// </summary>
		/// <param name="m">The regular expression match object that contains the wildcard.</param>
		private Token TokenizeWildcard(Wildcard w)
		{
			// Handle obfuscated wildcards
			if (w.Obfuscated)
				return TokenizeObfuscatedWildcard(w);
			// Get a replacement for the wildcard
			INameable inam = FindReplacement(w);
			// Create the token
			return new Token(w.Value, inam, FetchMetadata(w));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Evaluates the provided regular expression match object producing
		/// an INameable replacement object
		/// </summary>
		/// <param name="m">The regular expression match object to evaluate.</param>
		/// <returns>An INameable replacement object if an adequate replacement
		/// could be found or produced, null otherwise.</returns>
		private INameable FindReplacement(Wildcard w)
		{
			if (!w.Success)
				return null;

			switch (w.Name)
			{
				case "category":
					return EvaluateCategory(w);

				case "gesture":
					return EvaluateGesture(w);

				case "name": case "female": case "male":
					return EvaluateName(w);

				case "location": case "beacon": case "placement": case "room":
					return EvaluateLocation(w);

				case "object": case "aobject": case "kobject": 
					return EvaluateObject(w);

				case "question":
					return EvaluateQuestion(w);

				case "void":
					return EvaluateVoid(w);

				default:
					return null;
			}
		}

		/// <summary>
		/// Evaluates the provided regular expression match object producing
		/// an replacement string
		private string Evaluator(Wildcard w)
		{
			INameable inam = FindReplacement(w);

			if (inam != null)
				return inam.Name;
			return w.Value;
		}

		/// <summary>
		/// Fills the availability lists with information from the Generator databases
		/// </summary>
		private void FillAvailabilityLists()
		{
			// Copy objects from generator's lists
			this.avCategories = new List<Category>(generator.AllObjects.Categories);
			this.avGestures = GetTieredList(generator.AllGestures);
			this.avLocations = new List<Location>(generator.AllLocations);
			this.avNames = new List<PersonName>(generator.AllNames);
			this.avObjects = GetTieredList(generator.AllObjects.Objects);
			this.avQuestions = GetTieredList(generator.AllQuestions);

			// Shuffle the availability list once in O(1) so just retrieve last
			// item every time as random
			this.avCategories.Shuffle(generator.Rnd);
			this.avGestures.Shuffle(generator.Rnd);
			this.avLocations.Shuffle(generator.Rnd);
			this.avNames.Shuffle(generator.Rnd);
			this.avObjects.Shuffle(generator.Rnd);
			this.avQuestions.Shuffle(generator.Rnd);
		}

		/// <summary>
		/// Extracts the metadata strings from a regular expression match object that
		/// contains the wildcard
		/// </summary>
		/// <param name="m">The regular expression match object that contains the wildcard.</param>
		private string[] FetchMetadata(Wildcard w){
			string sMeta = w.Metadata;
			if(String.IsNullOrEmpty(sMeta)) return null;
			sMeta = ReplaceWildcards(sMeta);
			return sMeta.Split (new string[]{"\r", "\n", @"\\", @"\\r", @"\\n"}, StringSplitOptions.None);
		}

		/// <summary>
		/// Produces a Task from a task prototype string by replacing all the wildcards
		/// within it
		/// </summary>
		/// <param name="taskPrototype">The task prototype string</param>
		/// <returns>A Task based on the provided task prototype.</returns>
		public Task GetTask(string taskPrototype)
		{
			int bcc = 0;
			Token token;
			// Find all wildcards in the task prototype
			List<Wildcard> wildcards = FindWildcards(taskPrototype);
			// Having n wildcards interlaced with literal strings and starting with a
			// literal string there will be n+1 literal strings. Therefore, the worst
			// number of tokens will never be greater than 2n+1
			// Since the list may be reallocated, 2n+2 is used for performance
			List<Token> tokens = new List<Token>(2 * wildcards.Count + 2);
			// For each wildcard found
			foreach (Wildcard w in wildcards) {
				// Add the string on the left (if any) as a token
				token = TokenizeLeftLiteralString (taskPrototype, ref bcc, w);
				if(token != null) tokens.Add (token);
				token = TokenizeWildcard(w);
				tokens.Add (token);
			}
			// If there is more text to the right, add it as last token
			token = TokenizeSubstring (taskPrototype, bcc);
			if(token != null) tokens.Add (token);
			// Build the task, add the tokens, and return it.
			Task task = new Task () { Tokens = tokens };
			return task;
		}

		/// <summary>
		/// Replaces all wildcards in the input string with random values.
		/// </summary>
		/// <returns>The input string with all wildcards replaced.</returns>
		/// <param name="taskPrototype">The input string</param>
		public string ReplaceWildcards(string taskPrototype)
		{
			return GetTask(taskPrototype).ToString();
		}

		#endregion

		#region Static Methods

		public static List<Wildcard> FindWildcards(string s)
		{
			int cc = 0;
			List<Wildcard> wildcards = new List<Wildcard>(100);
			Wildcard w;

			while (cc < s.Length)
			{
				if (s[cc] == '{')
				{
					w = Wildcard.XtractWildcard(s, ref cc);
					if (w == null) continue;
					wildcards.Add(w);
				}
				++cc;
			}
			return wildcards;
		}

		#endregion
	}
}

