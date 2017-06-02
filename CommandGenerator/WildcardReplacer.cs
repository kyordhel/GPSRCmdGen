using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		/// Stores all found wildcards contained the working task
		/// </summary>
		private List<Wildcard> wildcards;

		/// <summary>
		/// Groups all wildcards stored by keycode
		/// </summary>
		private Dictionary<string, List<Wildcard>> wildcardsByKeycode;

		/// <summary>
		/// Stores the index of the wildcard being processed in the wildcards list
		/// </summary>
		private int currentWildcardIx;

		/// <summary>
		/// Stores tokens produced after wildcard replacement
		/// </summary>
		private List<Token> tokens;

		/// <summary>
		/// The random generator that contains all required data
		/// </summary>
		private readonly Generator generator;

		/// <summary>
		/// The maximum difficulty degree to be used during the replacement
		/// </summary>
		private DifficultyDegree tier;

		/// <summary>
		/// Stores all replacements
		/// </summary>
		private Dictionary<string, INameable> replacements; 

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
			this.wildcards = new List<Wildcard> ();
			this.wildcardsByKeycode = new Dictionary<string, List<Wildcard>> ();
			this.generator = g;
			this.tier = tier;
			this.replacements = new Dictionary<string, INameable> ();
			FillAvailabilityLists ();
		}

		#endregion

		#region Evaluate Methods

		/// <summary>
		/// Evaluates a <c>location</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateLocation(Wildcard w)
		{
			if (w.Name == "location") {
				if (w.Type.IsAnyOf ("beacon", "room", "placement"))
					ChangeWildcardKeyword(w.Keycode, w.Type);
				else if (String.IsNullOrEmpty (w.Type))
					ChangeWildcardKeyword(w.Keycode, generator.RandomPick ("beacon", "room", "placement"));
			}
			return GetFromList (w, GetLocation);
		}

		/// <summary>
		/// Evaluates a <c>name</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateName(Wildcard w)
		{
			if (w.Name == "name") {
				if (w.Type.IsAnyOf ("male", "female"))
					ChangeWildcardKeyword(w.Keycode, w.Type);
				else if (String.IsNullOrEmpty (w.Type))
					ChangeWildcardKeyword(w.Keycode, generator.RandomPick ("male", "female"));
			}

			return GetFromList (w, GetName);
		}

		/// <summary>
		/// Evaluates an <c>object</c> wildcard, creating a new replacement if the wildcard
		/// has not been defined before, or retrieving the replacement otherwise. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluateObject(Wildcard w)
		{
			if (w.Name == "object") {
				if (w.Type.IsAnyOf ("alike", "known"))
					ChangeWildcardKeyword(w.Keycode, String.Format("{0}object", w.Type[0]));
				else if (String.IsNullOrEmpty (w.Type))
					ChangeWildcardKeyword(w.Keycode, generator.RandomPick ("kobject", "aobject"));
			}

			return GetFromList (w, GetObject);
		}

		/// <summary>
		/// Evaluates a <c>pron</c> wildcard, replacing the string with the adequate pronoun
		/// regarding the last token found. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private INameable EvaluatePronoun(Wildcard w){
			Wildcard prev = null;

			for (int i = currentWildcardIx - 1; i >= 0; --i) {
				if (wildcards [i].Keyword.IsAnyOf ("name", "male", "female")) {
					prev = wildcards [i];
					break;
				}
			}
			for (int i = currentWildcardIx - 1; (prev == null) && (i >= 0); --i) {
				if (wildcards [i].Keyword.IsAnyOf ("void", "pron"))
					continue;
					prev = wildcards [i];
					break;
			}

			return new NamedTaskElement (Pronoun.Personal.FromWildcard (w, prev));
			/*
			if (prev == null)
				return new NamedTaskElement ("them");

			switch (prev.Keyword) {
				case "name":
				case "male":
				return new NamedTaskElement ("him");

				case "female":
					return new NamedTaskElement ("her");

			case "object": case "kobject": case "aobject":
			case "beacon": case "room": case "placement": case "location":
					return new NamedTaskElement ("it");

				default:
					return new NamedTaskElement ("them");
			}
			*/
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
			switch (keycode)
			{
				case "beacon":
					return this.avLocations.PopFirst(l => l.IsBeacon);

				case "room":
					return this.avLocations.PopFirst(l => l is Room);

				case "placement":
					return this.avLocations.PopFirst(l => l.IsPlacement);

				default:
					return this.avLocations.PopLast();
			}
		}

		/// <summary>
		/// Retrieves a <c>Name</c> object from the corresponding availability
		/// list, also removing the element to avoid duplicates.
		/// </summary>
		/// <param name="keycode">The specific type of name to fetch<param>
		/// <returns>A name</returns>
		private PersonName GetName (string keycode)
		{
			switch(keycode){
				case "male":
					return this.avNames.PopFirst(n => n.Gender == Gender.Male);

				case "female":
					return this.avNames.PopFirst(n => n.Gender == Gender.Female);

				default:
					return this.avNames.PopLast();
			}
		}

		/// <summary>
		/// Retrieves a <c>GPSRObject</c> object from the corresponding availability
		/// list, also removing the element to avoid duplicates.
		/// </summary>
		/// <param name="keycode">The specific type of object to fetch<param>
		/// <returns>An object</returns>
		private GPSRObject GetObject (string keycode)
		{
			switch(keycode){
				case "aobject":
					return this.avObjects.PopFirst(o => o.Type == GPSRObjectType.Alike);

				case "kobject":
					return this.avObjects.PopFirst(o => o.Type == GPSRObjectType.Known);

				// case "uobject":
				// return GPSRObject.Unknown;

				default:
					return this.avObjects.PopLast();
			}
		}

		private T GetFromList<T>(Wildcard w, Func<T> fetcher) where T: INameable{
			if (w.Id >= 1000)
				return fetcher();
			if(replacements.ContainsKey(w.Keycode))
				return (T) replacements[w.Keycode];
			T t = fetcher();
			AddReplacement (w, t);
			return t;
		}

		private T GetFromList<T>(Wildcard w, Func<string, T> fetcher) where T: INameable{
			if (w.Id >= 1000)
				return fetcher(w.Keyword);
			if(replacements.ContainsKey(w.Keycode))
				return (T) replacements[w.Keycode];
			T t = fetcher(w.Keyword);
			AddReplacement (w, t);
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

				// Pronouns shouldn't be obfuscated.
				case "pron":
					inam = FindReplacement(w);
					return new Token(w.Value, inam, FetchMetadata(w));
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
			if (cc > w.Index)
				return null;
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
		/// Safely adds a wildcard to the wildcards and wildcardsByKeycode storages
		/// </summary>
		/// <param name="w">The wildcard to add.</param>
		private void AddWildcard(Wildcard w){
			if ((w == null) || !w.Success)
				return;
			if (!wildcardsByKeycode.ContainsKey (w.Keycode))
				wildcardsByKeycode.Add (w.Keycode, new List<Wildcard> ());
			wildcards.Add(w);
			wildcardsByKeycode[w.Keycode].Add(w);
		}

		/// <summary>
		/// Adds a wildcard to the replacement list. If the wildcard is not in the wildcardsByKeycode dicctionary, is also added.
		/// </summary>
		/// <param name="w">The wildcard to be added to the replacement list.</param>
		/// <param name="replacement">The replacement of the wildcard</param>
		private void AddReplacement(Wildcard w, INameable replacement){
			if (!wildcardsByKeycode.ContainsKey (w.Keycode)) {
				wildcardsByKeycode.Add (w.Keycode, new List<Wildcard> ());
				wildcardsByKeycode[w.Keycode].Add(w);
			}
			if (!replacements.ContainsKey (w.Keycode))
				replacements.Add (w.Keycode, replacement);
		}

		/// <summary>
		/// Changes the keyword for all wildcards with the same Keycode
		/// </summary>
		/// <param name="w">The unified wildcard keycode.</param>
		/// <param name="keyword">The new keyword.</param>
		private void ChangeWildcardKeyword(string keycode, string keyword){
			if (String.IsNullOrEmpty (keycode))
				throw new ArgumentNullException ("keycode is null");
			if (!wildcardsByKeycode.ContainsKey (keycode))
				throw new InvalidOperationException ("Unknown keycode. No wildcard has been added with the provided keycode");
			foreach (Wildcard w in wildcardsByKeycode[keycode])
				w.Keyword = keyword;
		}

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

			if (replacements.ContainsKey (w.Keycode))
				return replacements [w.Keycode];

			switch (w.Name)
			{
				case "category":
					return GetFromList (w, GetCategory);

				case "gesture":
					return GetFromList (w, GetGesture);

				case "name": case "female": case "male":
					return EvaluateName(w);

				case "location": case "beacon": case "placement": case "room":
					return EvaluateLocation(w);

				case "object": case "aobject": case "kobject": 
					return EvaluateObject(w);

				case "question":
					return GetFromList (w, GetQuestion);

				case "void":
					return new HiddenTaskElement();

				case "pron":
					return EvaluatePronoun(w);

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

		public void FindWildcards(string s)
		{
			int cc = 0;
			this.wildcards.Clear ();
			this.wildcardsByKeycode.Clear ();
			Wildcard w;

			while (cc < s.Length)
			{
				if (s[cc] == '{')
				{
					w = Wildcard.XtractWildcard(s, ref cc);
					if (w == null) continue;
					AddWildcard (w);
				}
				++cc;
			}
		}

		/// <summary>
		/// Extracts the metadata strings from a regular expression match object that
		/// contains the wildcard
		/// </summary>
		/// <param name="m">The regular expression match object that contains the wildcard.</param>
		private string[] FetchMetadata(Wildcard w){
			string sMeta = w.Metadata;
			if(String.IsNullOrEmpty(sMeta)) return null;
			sMeta = ReplaceNestedWildcards(sMeta);
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
			int cc = 0;
			Token token;
			Wildcard wildcard;

			// Initialize lists and clear
			tokens = new List<Token>(100);
			wildcards.Clear ();
			wildcardsByKeycode.Clear ();
			currentWildcardIx = 0;

			do {
				// Read the string from left to right till the next open brace (wildcard delimiter).
				while ((cc < taskPrototype.Length) && (taskPrototype [cc] != '{'))
					++cc;
				// Wildcard found. Extract the string to the left
				string left = taskPrototype.Substring (bcc, cc - bcc);
				// Store the string at the left of the wildcard as token
				tokens.Add (new Token (left));
				// If a wildcard was not found, continue
				if ((cc >= taskPrototype.Length) || (taskPrototype [cc] != '{'))
					continue;
				// Otherwise, extract the wildcard and update the backup read header
				wildcard = Wildcard.XtractWildcard (taskPrototype, ref cc);
				bcc = cc;
				// If the extraction failed, continue
				if (wildcard == null)
					continue;
				// Add the wildcard to the reference lists
				AddWildcard (wildcard);
				// Convert the wildcard into a token
				++currentWildcardIx;
				token = TokenizeWildcard (wildcard);
				// Add te token
				tokens.Add (token);
			} while(cc < taskPrototype.Length);

			// Build the task, add the tokens, and return it.
			Task task = new Task () { Tokens = tokens };
			return task;
		}

		/*

		/// <summary>
		/// Replaces all wildcards in the input string with random values.
		/// </summary>
		/// <returns>The input string with all wildcards replaced.</returns>
		/// <param name="taskPrototype">The input string</param>
		public string ReplaceWildcards(string taskPrototype)
		{
			return GetTask(taskPrototype).ToString();
		}
		*/

		/// <summary>
		/// Replaces all wildcards in the input string with random values.
		/// </summary>
		/// <returns>The input string with all wildcards replaced.</returns>
		/// <param name="s">The input string</param>
		public string ReplaceNestedWildcards(string s)
		{
			int cc= 0;
			StringBuilder sb = new StringBuilder (s.Length);
			// Read the string from left to right till the next open brace (wildcard delimiter).
			while(cc < s.Length) {
				if (s [cc] == '{') {
					Wildcard w = Wildcard.XtractWildcard(s, ref cc);
					if(w == null) continue;
					AddWildcard (w);
					sb.Append(FindReplacement (w).Name);
				}
				else
					sb.Append( s[cc++] );
			}
			return sb.ToString ();
		}

		#endregion

		#region Static Methods

		/*
		 * 
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
					wildcards.Add (w);
				}
				++cc;
			}
			return wildcards;
		}
		*/

		#endregion
	}
}

