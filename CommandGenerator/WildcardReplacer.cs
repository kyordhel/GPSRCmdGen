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
		private List<TextWildcard> textWildcards;

		/// <summary>
		/// Groups all wildcards stored by keycode
		/// </summary>
		private Dictionary<string, Wildcard> wildcards;

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
			this.textWildcards = new List<TextWildcard> ();
			this.wildcards = new Dictionary<string, Wildcard> ();
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
			if (w.Name == "location")
				w.Keyword = w.Type ?? generator.RandomPick ("beacon", "room", "placement");
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
			if (w.Name == "name")
				w.Keyword = w.Type ?? generator.RandomPick ("male", "female");
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
			if (w.Name == "object") 
				w.Keyword = (w.Type == null) ? generator.RandomPick ("kobject", "aobject") : String.Format("{0}object", w.Type[0]);
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
			string keyword;

			for (int i = currentWildcardIx - 1; i >= 0; --i) {
				keyword = wildcards [textWildcards [i].Keycode].Keyword;
				if ((keyword != null) && keyword.IsAnyOf ("name", "male", "female")) {
					// prev = textWildcards [i];
					prev = wildcards [keyword];
					break;
				}
			}
			for (int i = currentWildcardIx - 1; (prev == null) && (i >= 0); --i) {
				keyword = wildcards [textWildcards [i].Keycode].Keyword;
				if ((keyword != null) && keyword.IsAnyOf ("void", "pron"))
					continue;
				// prev = textWildcards [i];
				prev = wildcards [keyword];
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
			if(replacements.ContainsKey(w.Keycode))
				return (T) replacements[w.Keycode];
			T t = fetcher();
			SetReplacement (w, t);
			return t;
		}

		private T GetFromList<T>(Wildcard w, Func<string, T> fetcher) where T: INameable{
			if(replacements.ContainsKey(w.Keycode))
				return (T) replacements[w.Keycode];
			T t = fetcher(w.Keyword);
			SetReplacement (w, t);
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
		private Token TokenizeLeftLiteralString(string taskPrototype, ref int cc, TextWildcard w)
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
		private Token TokenizeTextWildcard(TextWildcard w)
		{
			return new Token(
				w.Value,
				new NamedTaskElement(String.Format("{{0}{1}}", w.Keycode, w.Obfuscated ? "?" : "")),
				new string[]{currentWildcardIx.ToString()}
			);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Safely adds a wildcard to the wildcards and wildcardsByKeycode storages
		/// </summary>
		/// <param name="w">The wildcard to add.</param>
		private void AddWildcard(TextWildcard w){
			if ((w == null) || !w.Success)
				return;
			if (!wildcards.ContainsKey (w.Keycode))
				wildcards.Add (w.Keycode, new Wildcard(w));
			else 
				wildcards[w.Keycode].Add(w);
			textWildcards.Add(w);
			++this.currentWildcardIx;
		}

		/// <summary>
		/// Adds a wildcard to the replacement list. If the wildcard is not in the wildcardsByKeycode dicctionary, is also added.
		/// </summary>
		/// <param name="w">The wildcard to be added to the replacement list.</param>
		/// <param name="replacement">The replacement of the wildcard</param>
		private void SetReplacement(Wildcard w, INameable replacement){
			if (!wildcards.ContainsKey (w.Keycode))
				throw new InvalidOperationException ("Provided wildcard does not belong to the wildcards dictionary");
			wildcards [w.Keycode].Replacement = replacement;
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
			if (!wildcards.ContainsKey (keycode))
				throw new InvalidOperationException ("Unknown keycode. No wildcard has been added with the provided keycode");
			wildcards[keycode].Keyword = keyword;
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
		private string[] FetchMetadata(TextWildcard w){
			string sMeta = w.Metadata;
			if(String.IsNullOrEmpty(sMeta)) return null;
			sMeta = ReplaceNestedWildcards(sMeta);
			return sMeta.Split (new string[]{"\r", "\n", @"\\", @"\\r", @"\\n"}, StringSplitOptions.None);
		}

		/// <summary>
		/// Evaluates a Wildcards and assignd a replacement to it.
		/// </summary>
		private void FindReplacement(Wildcard w)
		{
			switch (w.Name)
			{
				case "category":
					w.Replacement = GetFromList (w, GetCategory);
					w.Obfuscated = new Obfuscator("objects");
					break;

				case "gesture":
					w.Replacement = GetFromList (w, GetGesture);
					// w.Obfuscated = ;
					break;

				case "name":
				case "female":
				case "male":
					EvaluateName (w);
					w.Obfuscated = new Obfuscator("a person");
					break;

				case "beacon":
				case "placement":
					EvaluateLocation (w);
					w.Obfuscated = ((SpecificLocation)w.Replacement).Room;
					break;
				
				case "location":
					EvaluateLocation (w);
					w.Obfuscated = new Obfuscator("somewhere");
					break;

				case "room":
					EvaluateLocation (w);
					w.Obfuscated = new Obfuscator("apartment");
					break;

				case "object":
				case "aobject":
				case "kobject": 
					EvaluateObject (w);
					w.Obfuscated = ((GPSRObject)w.Replacement).Category;
					break;

				case "question":
					w.Replacement = GetFromList (w, GetQuestion);
					w.Obfuscated = new Obfuscator("question");
					break;

				case "void":
				default:
					w.Replacement = new HiddenTaskElement ();
					w.Obfuscated = new HiddenTaskElement ();
					break;

				case "pron":
					EvaluatePronoun (w);
					break;
			}
		}

		private void FindReplacements(){
			foreach (KeyValuePair<string,Wildcard> p in this.wildcards)
				FindReplacement (p.Value);
		}

		public void FindWildcards(string s)
		{
			int cc = 0;
			this.textWildcards.Clear ();
			this.wildcards.Clear ();
			TextWildcard w;

			while (cc < s.Length)
			{
				if (s[cc] == '{')
				{
					w = TextWildcard.XtractWildcard(s, ref cc);
					if (w == null) continue;
					AddWildcard (w);
				}
				++cc;
			}
		}

		/// <summary>
		/// Produces a Task from a task prototype string by replacing all the wildcards
		/// within it
		/// </summary>
		/// <param name="taskPrototype">The task prototype string</param>
		/// <returns>A Task based on the provided task prototype.</returns>
		public Task GetTask(string taskPrototype)
		{
			// Initialize lists and clear
			tokens = new List<Token>(100);
			textWildcards.Clear ();
			wildcards.Clear ();
			replacements.Clear ();
			currentWildcardIx = 0;

			// STEP 1: Assembly the token list, fetching all text wildcards and performing their unification
			ParseTaskPrototype(taskPrototype);

			// STEP 2: Find replacements for all wildcards.
			FindReplacements();

			// STEP 3: Replace all tokenized text wildcards with the appropriate replacement token
			ReplaceTokens();

			// STEP 4: Replace wildcard metadata
			ReplaceWildcardsInMetadata();


			// Build the task, add the tokens, and return it.
			Task task = new Task () { Tokens = tokens };
			return task;
		}

		/// <summary>
		/// Parses the task prototype string converting it in a list of tokens.
		/// All found wildcards are added to the wildcard lists but no replacements are made.
		/// </summary>
		/// <param name="taskPrototype">The task prototype to parse.</param>
		private void ParseTaskPrototype(string taskPrototype)
		{
			int cc = 0;

			ParseString (taskPrototype, ref cc, this.tokens);
		}

		/// <summary>
		/// Parses the given string, splitting it into tokens whic are added to the provided token list.
		/// All found wildcards are added to the wildcard lists but no replacements are made.
		/// </summary>
		/// <param name="s">The string to parse.</param>
		/// <param name="cc">The read header where the parse must start</param>
		/// <param name="tokens">The list where found tokens will be added</param>
		private void ParseString(string s, ref int cc, List<Token> tokens)
		{
			int bcc = 0;
			Token token;
			TextWildcard tWildcard;

			do {
				// Read the string from cc till the next open brace (wildcard delimiter).
				while ((cc < s.Length) && (s [cc] != '{'))
					++cc;
				// Wildcard found. Extract the string to the left
				string left = s.Substring (bcc, cc - bcc);
				// Store the string at the left of the wildcard as token
				tokens.Add (new Token (left));
				// If no text wildcard was found, continue
				if ((cc >= s.Length) || (s [cc] != '{'))
					continue;
				// Otherwise, extract the text wildcard and update the backup read header
				tWildcard = TextWildcard.XtractWildcard (s, ref cc);
				bcc = cc;
				// If the extraction failed, continue
				if (tWildcard == null)
					continue;
				// Add the text wildcard to the reference lists
				// When a wildcard is added, all nested wildcards are also processed
				AddWildcard (tWildcard);
				// Convert the text wildcard into a token
				token = TokenizeTextWildcard (tWildcard);
				// Add the token
				tokens.Add (token);
			} while(cc < s.Length);
		}

		private void ReplaceTokens(){
			for (int i = 0; i < tokens.Count; ++i){
				string tValue = (tokens [i].Value == null) ? String.Empty : tokens [i].Value.Name;
				if (!tValue.StartsWith ("{"))
					continue;
				bool obfuscated = false;
				int keyCodeLength = tValue.Length - 2;
				if (tValue [tValue.Length - 2] == '?') {
					--keyCodeLength;
					obfuscated = true;
				}
				string keycode = tValue.Substring (1, keyCodeLength);

				string[] meta = FetchMetadata (textWildcards[Int32.Parse(tokens[i].Metadata[0])]);
				if(obfuscated){
					tokens [i] = new Token(tokens[i].Key, wildcards[keycode].Obfuscated, meta);
					tokens [i].Metadata.Add(wildcards[keycode].Replacement.Name);
				}
				else
					tokens [i] = new Token(tokens[i].Key, wildcards[keycode].Replacement, meta);
			}
		}
		private void ReplaceWildcardsInMetadata()
		{
		}

		/*

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
					TextWildcard w = TextWildcard.XtractWildcard(s, ref cc);
					if(w == null) continue;
					AddWildcard (w);
					sb.Append(FindReplacement (w).Name);
				}
				else
					sb.Append( s[cc++] );
			}
			return sb.ToString ();
		}
		*/

		#endregion

		#region Static Methods

		#endregion
	}
}

