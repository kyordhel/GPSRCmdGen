using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RoboCup.AtHome.CommandGenerator.ReplaceableTypes;
using Object = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.Object;

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
		private List<Object> avObjects;

		/// <summary>
		/// List of available objects
		/// </summary>
		private List<PredefinedQuestion> avQuestions;

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
			FillAvailabilityLists ();
		}

		#endregion

		#region Evaluate Methods

		/// <summary>
		/// Evaluates a <c>location</c> wildcard, asigning a replacement.
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		private void EvaluateCategory(Wildcard w)
		{
			if (w.Replacement != null)
				return;
			w.Replacement = !String.IsNullOrEmpty(w.Where) ? this.avCategories.PopFirst(w.Where) : this.avCategories.PopLast();
			w.Obfuscated = new Obfuscator("objects");
		}

		/// <summary>
		/// Evaluates a <c>location</c> wildcard, asigning a replacement.
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		private void EvaluateGesture(Wildcard w)
		{
			if (w.Replacement != null)
				return;
			w.Replacement = !String.IsNullOrEmpty(w.Where) ? this.avGestures.PopFirst(w.Where) : this.avGestures.PopLast();
			// w.Obfuscated = ;
		}

		/// <summary>
		/// Evaluates a <c>location</c> wildcard, asigning a replacement.
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		private void EvaluateLocation(Wildcard w)
		{
			if(w.Replacement != null) return;
			if ((w.Name == "location") && String.IsNullOrEmpty(w.Where))
				w.Keyword = w.Type ?? generator.RandomPick ("beacon", "room", "placement");
			switch (w.Keyword)
			{
				case "beacon":
					w.Replacement = this.avLocations.PopFirst(l => l.IsBeacon, w.Where);
					w.Obfuscated = ((SpecificLocation)w.Replacement).Room;
					break;

				case "room":
					w.Replacement = this.avLocations.PopFirst(l => l is Room, w.Where);
					w.Obfuscated = new Obfuscator("apartment");
					break;

				case "placement":
					w.Replacement = this.avLocations.PopFirst(l => l.IsPlacement, w.Where);
					w.Obfuscated = ((SpecificLocation)w.Replacement).Room;
					break;

				default:
					w.Replacement = !String.IsNullOrEmpty(w.Where) ? this.avLocations.PopFirst(w.Where) : this.avLocations.PopLast();
					w.Obfuscated = new Obfuscator("somewhere");
					break;
			}
		}

		/// <summary>
		/// Evaluates a <c>name</c> wildcard, asigning a replacement.
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		private void EvaluateName(Wildcard w)
		{
			if(w.Replacement != null) return;
			if ((w.Name == "name") && String.IsNullOrEmpty(w.Where))
				w.Keyword = w.Type ?? generator.RandomPick ("male", "female");



			switch(w.Keyword)
			{
				case "male":
					w.Replacement = this.avNames.PopFirst(n => n.Gender == Gender.Male, w.Where);
					break;

				case "female":
					w.Replacement = this.avNames.PopFirst(n => n.Gender == Gender.Female, w.Where);
					break;

				default:
					w.Replacement = !String.IsNullOrEmpty(w.Where) ? this.avNames.PopFirst(w.Where) : this.avNames.PopLast();
					break;
			}
			w.Obfuscated = new Obfuscator("a person");
		}

		/// <summary>
		/// Evaluates an <c>object</c> wildcard, asigning a replacement.
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		private void EvaluateObject(Wildcard w)
		{
			if(w.Replacement != null) return;

			if ((w.Name == "object") && String.IsNullOrEmpty(w.Where))
				w.Keyword = (w.Type == null) ? generator.RandomPick ("kobject", "aobject") : String.Format("{0}object", w.Type[0]);

			switch(w.Keyword)
			{
				case "aobject":
					w.Replacement = this.avObjects.PopFirst(o => o.Type == ObjectType.Alike, w.Where);
					break;

				case "kobject":
					w.Replacement = this.avObjects.PopFirst(o => o.Type == ObjectType.Known, w.Where);
					break;

				// case "uobject":
				// 	w.Replacement = GPSRObject.Unknown;
				// 	break;

				case "sobject":
					w.Replacement = this.avObjects.PopFirst(o => o.Type == ObjectType.Special, w.Where);
					break;

				default:
					w.Replacement = !String.IsNullOrEmpty(w.Where) ? this.avObjects.PopFirst(w.Where) : this.avObjects.PopLast();
					break;
			}
			w.Obfuscated = ((Object)w.Replacement).Category;
		}

		/// <summary>
		/// Evaluates a <c>pron</c> wildcard, replacing the string with the adequate pronoun
		/// regarding the last token found. 
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		/// <returns>An appropiate replacement for the wildcard.</returns>
		private void EvaluatePronoun(Wildcard w){
			Wildcard prev = null;
			string keycode;
			string keyword;

			for (int i = currentWildcardIx - 1; i >= 0; --i) {
				keycode = textWildcards [i].Keycode;
				keyword = wildcards [keycode].Keyword;
				if ((keyword != null) && keyword.IsAnyOf ("name", "male", "female")) {
					// prev = textWildcards [i];
					prev = wildcards [keycode];
					break;
				}
			}
			for (int i = currentWildcardIx - 1; (prev == null) && (i >= 0); --i) {
				keycode = textWildcards [i].Keycode;
				keyword = wildcards [keycode].Keyword;
				if ((keyword != null) && keyword.IsAnyOf ("void", "pron"))
					continue;
				// prev = textWildcards [i];
				prev = wildcards [keycode];
				break;
			}
				
			w.Replacement = new NamedTaskElement (Pronoun.Personal.FromWildcard (w, prev));
		}

		/// <summary>
		/// Evaluates a <c>location</c> wildcard, asigning a replacement.
		/// </summary>
		/// <param name="w">The wilcard to find a replacement for</param>
		private void EvaluateQuestion(Wildcard w)
		{
			if(w.Replacement != null) return;
			w.Replacement = !String.IsNullOrEmpty(w.Where) ? this.avQuestions.PopFirst(w.Where) : this.avQuestions.PopLast ();
			w.Obfuscated = new Obfuscator("question");
		}

		#endregion

		#region Random Generation Methos

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
				new NamedTaskElement(String.Format("{{{0}{1}}}", w.Keycode, w.Obfuscated ? "?" : "")),
				// The wildcard has not been added (will be the next one) so this counter needs to be incremented
				new string[]{(currentWildcardIx+1).ToString()}
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
			ParseNestedWildcards (w);
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
			if ((w == null) || String.IsNullOrEmpty (w.Metadata))
				return null;
			ReplaceNestedWildcards(w);
			return w.Metadata.Split (new string[]{"\r", "\n", @"\\", @"\\r", @"\\n"}, StringSplitOptions.None);
		}

		/// <summary>
		/// Evaluates a Wildcards and assignd a replacement to it.
		/// </summary>
		private void FindReplacement(Wildcard w)
		{
			switch (w.Name)
			{
				case "category":
					EvaluateCategory(w);
					break;

				case "gesture":
					EvaluateGesture(w);
					break;

				case "name":
				case "female":
				case "male":
					EvaluateName (w);
					break;

				case "beacon":
				case "placement":
				case "location":
				case "room":
					EvaluateLocation (w);
					break;

				case "object":
				case "aobject":
				case "kobject": 
				case "sobject":
					EvaluateObject (w);
					break;

				case "question":
					EvaluateQuestion(w);
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
			// List<Wildcard> whereless = new List<Wildcard>(this.wildcards.Count);
			// List<Wildcard> whereSimple = new List<Wildcard>(this.wildcards.Count);
			Queue<Wildcard> whereNested = new Queue<Wildcard>(this.wildcards.Count);
			// STEP 1: Wildcards not containing nested wildcards in the WHERE clause
			//         are replaced on the fly, while others are left for later.
			foreach (KeyValuePair<string,Wildcard> p in this.wildcards)
			{
				if (p.Value.Where.IndexOf('{') == -1)
					FindReplacement(p.Value);
				else
					whereNested.Enqueue(p.Value);
			}

			// STEP 2: Replace nested wildcards in WHERE clauses
			//         This will iterate over the list of wildcards with nested ones
			//         in WHERE clauses until the list is empty or until a loop has
			//         been done with no changes.
			int initial = whereNested.Count;
			while (whereNested.Count > 0)
			{
				// Step 2.1: Set the initial counter
				initial = whereNested.Count;
				// STEP 2.2: Dequeue the wildcard
				Wildcard w = whereNested.Dequeue();
				bool allReplaced = true;
				// STEP 2.3: Look for TextWildcards with nested elements in WHERE clause
				foreach (TextWildcard tw in w)
				{
					if ((tw.Where == null) || (tw.Where.IndexOf('{') == -1))
						continue;
					string twWhere = tw.Where;
					// STEP 2.4: Attempt to replace nested wildcards in each TextWildcard.
					if (ReplaceNestedWildcardsHelper(ref twWhere))
						tw.Where = twWhere;
					else
						allReplaced = false;
				}
				// STEP 2.5: If all nested wildcards were replaced, look for a replacement
				//           otherwise, the element is enqueued again.
				if (allReplaced)
					FindReplacement(w);
				else
					whereNested.Enqueue(w);

				// STEP 2.6: Compare the initial counter with the number of elements in the queue.
				//           If the number matches, then no replacement was made.
				//           We quit to prevent infinite loop
				if (initial == whereNested.Count())
					break;
			}
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
			currentWildcardIx = -1;

			// STEP 1: Assembly the token list, fetching all text wildcards and performing their unification
			ParseTaskPrototype(taskPrototype);

			// STEP 2: Find replacements for all wildcards.
			FindReplacements();

			// STEP 3: Replace all tokenized text wildcards with the appropriate replacement token
			ReplaceTokens();

			// Build the task, add the tokens, and return it.
			Task task = new Task () { Tokens = tokens };
			return task;
		}

		/// <summary>
		/// Parses the where clauses and metadata in a text wildcard to extract nested wildcards.
		/// </summary>
		/// <param name="w">The TextWildcard to parse.</param>
		private void ParseNestedWildcards(TextWildcard textWildcard)
		{
			string s;
			if (!String.IsNullOrEmpty(s = textWildcard.Metadata))
			{
				ParseNestedWildcardsHelper(ref s);
				textWildcard.Metadata = s;
			}
				
			if (!String.IsNullOrEmpty(s = textWildcard.Where))
			{
				ParseNestedWildcardsHelper(ref s);
				textWildcard.Where = s;
			}
		}

		/// <summary>
		/// Parses the where clauses and metadata in a text wildcard to extract nested wildcards (helper)
		/// </summary>
		/// <param name="w">The string containing nested TextWildcards to parse.</param>
		private void ParseNestedWildcardsHelper(ref string s){
			int cc = 0;
			TextWildcard inner;
			s = s ?? String.Empty;
			StringBuilder sb = new StringBuilder(s.Length);

			do {
				// Read the string from cc till the next open brace (wildcard delimiter).
				while ((cc < s.Length) && (s[cc] != '{'))
					sb.Append(s[cc++]);
				// Otherwise, extract the nested text wildcard
				inner = TextWildcard.XtractWildcard (s, ref cc);
				// If the extraction failed, continue
				if (inner == null)
					continue;
				// Replace the wildcard entry with its unique keycode
				sb.AppendFormat("{{{0}}}", inner.Keycode);
				// Add the text wildcard to the reference lists
				// When a wildcard is added, all nested wildcards are also processed
				AddWildcard (inner);
			} while(cc < s.Length);
			s = sb.ToString();
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
				// Update read header backup
				bcc = cc;
				// Read the string from cc till the next open brace (wildcard delimiter).
				while ((cc < s.Length) && (s [cc] != '{'))
					++cc;
				// Wildcard found. Extract the string to the left
				string left = s.Substring (bcc, cc - bcc);
				// Store the string at the left of the wildcard as token
				tokens.Add (new Token (left));
				// If the end of the string has been reached, quit
				if (cc >= s.Length)
					break;
				// Otherwise, extract the text wildcard
				tWildcard = TextWildcard.XtractWildcard (s, ref cc);
				// If the extraction failed, continue
				if (tWildcard == null)
					continue;
				// Convert the text wildcard into a token
				token = TokenizeTextWildcard (tWildcard);
				// Add the text wildcard to the reference lists
				// When a wildcard is added, all nested wildcards are also processed
				AddWildcard (tWildcard);
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

		/// <summary>
		/// Replaces all wildcards contained within the given wildcard metadata.
		/// </summary>
		/// <returns>The input string with all wildcards replaced.</returns>
		/// <param name="s">The input string</param>
		public void ReplaceNestedWildcards(TextWildcard textWildcard)
		{
			string s = textWildcard.Metadata;
			if(ReplaceNestedWildcardsHelper(ref s))
				textWildcard.Metadata = s;

			s = textWildcard.Where;
			if(ReplaceNestedWildcardsHelper(ref s))
				textWildcard.Where = s;
		}

		private bool ReplaceNestedWildcardsHelper(ref string s){
			int cc = 0;
			TextWildcard inner;
			if (String.IsNullOrEmpty (s))
				return false;
			StringBuilder sb = new StringBuilder (s.Length);

			do {
				// Fetch the string from cc till the next open brace (wildcard delimiter).
				while ((cc < s.Length) && (s[cc] != '{'))
					sb.Append(s[cc++]);
				// If the end of the string has been reached, quit
				if (cc >= s.Length)
					break;
				// Otherwise, extract the text wildcard
				inner = TextWildcard.XtractWildcard (s, ref cc);
				// If the extraction failed, continue
				if (inner == null)
					continue;
				// Otherwise, if the wildcard has where clauses or metadata, they need to be replaced first.
				if(!String.IsNullOrEmpty(inner.Metadata) || !String.IsNullOrEmpty(inner.Where))
					ReplaceNestedWildcards(inner);
				// After replacing nested wildcards in where clauses and metadata, the inner wildcard value is replaced
				if(!wildcards.ContainsKey(inner.Keycode) || (wildcards[inner.Keycode] == null)) return false;
				sb.Append(wildcards[inner.Keycode].Replacement.Name);
			} while(cc < s.Length);
			// Finally, the metadata of the wildcard is updated.
			s = sb.ToString();
			return true;
		}

		#endregion

		#region Static Methods

		#endregion
	}
}

