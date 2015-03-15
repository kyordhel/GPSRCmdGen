using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GPSRCmdGen
{
	public class WildcardReplacer
	{
		#region Variables

		/// <summary>
		/// The random generator that contains all required data
		/// </summary>
		private readonly Generator generator;
		private readonly MatchEvaluator dlgEvaluator;

		/// <summary>
		/// Regular expression for extracting wildcards
		/// </summary>
		private static readonly Regex rxWildcard;

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
		private Dictionary<string, Name> names;

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
		private List<Name> avNames;
		
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

		public WildcardReplacer(Generator g, DifficultyDegree tier){
			this.generator = g;
			this.tier = tier;
			this.dlgEvaluator = new MatchEvaluator (Evaluator);
			this.categories = new Dictionary<string, Category> (); 
			this.gestures = new Dictionary<string, Gesture> ();
			this.locations = new Dictionary<string, Location> ();
			this.names = new Dictionary<string, Name> ();
			this.objects = new Dictionary<string, GPSRObject> ();
			this.questions = new Dictionary<string, PredefindedQuestion>();
			FillAvailabilityLists ();
		}

		static WildcardReplacer ()
		{
			rxWildcard = new Regex (@"\{\s*(?<name>[a-z]+)(\s+(?<type>[a-z]+))?(\s+(?<id>\d+))?\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

		#endregion

		#region Evaluate Methods

		private INameable EvaluateCategory (Match m)
		{
			string keycode = m.Result ("${name}").ToLower ();
			string key = keycode + m.Result("${id}");
			if(this.categories.ContainsKey(key))
				return this.categories[key];
			Category cat = GetCategory ();
			categories.Add (key, cat);
			return cat;
		}

		private INameable EvaluateGesture (Match m)
		{
			string keycode = m.Result ("${name}").ToLower ();
			string key = keycode + m.Result("${id}");
			if(this.gestures.ContainsKey(key))
				return this.gestures[key];
			Gesture ges = GetGesture ();
			gestures.Add (key, ges);
			return ges;
		}

		private INameable EvaluateLocation (Match m)
		{
			string keycode = m.Result ("${name}").ToLower ();
			if (keycode == "location") {
				string type = m.Result ("${type}").ToLower ();
				if (type == "room")
					keycode = "room";
				else if (type == "placement")
					keycode = "placement";
			}
			string key = keycode + m.Result("${id}");
			if(this.locations.ContainsKey(key))
				return this.locations[key];
			Location loc = GetLocation (keycode);
			locations.Add (key, loc);
			return loc;
		}

		private INameable EvaluateName (Match m)
		{
			string keycode = m.Result ("${name}").ToLower ();
			if (keycode == "name") {
				string type = m.Result ("${type}").ToLower ();
				if (type == "male")
					keycode = "male";
				else if (type == "female")
					keycode = "female";
			}
			string key = keycode + m.Result("${id}");
			if(this.names.ContainsKey(key))
				return this.names[key];
			Name nam = GetName (keycode);
			names.Add (key, nam);
			return nam;
		}

		private INameable EvaluateObject (Match m)
		{
			string keycode = m.Result ("${name}").ToLower ();
			if (keycode == "object") {
				string type = m.Result ("${type}").ToLower ();
				if (type == "alike")
					keycode = "aobject";
				else if (type == "known")
					keycode = "kobject";
			}
			string key = keycode + m.Result("${id}");
			if(this.objects.ContainsKey(key))
				return this.objects[key];
			GPSRObject obj = GetObject (keycode);
			objects.Add (key, obj);
			return obj;
		}

		private INameable EvaluateQuestion(Match m)
		{
			string keycode = m.Result("${name}").ToLower();
			string key = keycode + m.Result("${id}");
			if (this.questions.ContainsKey(key))
				return this.questions[key];
			PredefindedQuestion qst = GetQuestion();
			questions.Add(key, qst);
			return qst;
		}

		#endregion

		#region Random Generation Methos

		private Category GetCategory ()
		{
			Category item = this.avCategories [this.avCategories.Count - 1];
			this.avCategories.RemoveAt (this.avCategories.Count - 1);
			return item;
		}

		private Gesture GetGesture ()
		{
			Gesture item = this.avGestures [this.avCategories.Count - 1];
			this.avGestures.RemoveAt (this.avCategories.Count - 1);
			return item;
		}

		private Location GetLocation (string keycode)
		{
			Location item;
			switch(keycode){
			case "room":
				item = this.avLocations.First(l => l.IsPlacement == false);
				break;

			case "placement":
				item = this.avLocations.First(l => l.IsPlacement == true);
				break;

			default:
				item = this.avLocations [this.avLocations.Count - 1];
				break;
			}
			this.avLocations.Remove (item);
			return item;
		}

		private Name GetName (string keycode)
		{
			Name item;
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

		private PredefindedQuestion GetQuestion()
		{
			PredefindedQuestion item = this.avQuestions[this.avQuestions.Count - 1];
			this.avQuestions.RemoveAt(this.avQuestions.Count - 1);
			return item;
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

		#region Methods

		private string Evaluator(Match m, out INameable o)
		{
			o = null;
			if (!m.Success)
				return m.Value;

			int id;
			Int32.TryParse(m.Result("${id}"), out id);

			switch (m.Result("${name}").ToLower())
			{
				case "aobject":
				case "kobject":
				case "object":
					o = EvaluateObject(m);
					break;

				case "category":
					o = EvaluateCategory(m);
					break;

				case "female":
				case "male":
				case "name":
					o = EvaluateName(m);
					break;

				case "gesture":
					o = EvaluateGesture(m);
					break;

				case "location":
				case "placement":
				case "room":
					o = EvaluateLocation(m);
					break;

				case "question":
					o = EvaluateQuestion(m);
					break;
			}

			if (o != null)
				return o.Name;
			return m.Value;
		}

		private string Evaluator(Match m)
		{
			INameable inam;
			return Evaluator(m, out inam);
		}

		private void FillAvailabilityLists()
		{
			this.avCategories = new List<Category>(generator.AllObjects.Categories);
			this.avGestures = GetTieredList(generator.AllGestures);
			this.avLocations = new List<Location>(generator.AllLocations);
			this.avNames = new List<Name>(generator.AllNames);
			this.avObjects = GetTieredList(generator.AllObjects.Objects);
			this.avQuestions = GetTieredList(generator.AllQuestions);

			this.avCategories.Shuffle(generator.Rnd);
			this.avGestures.Shuffle(generator.Rnd);
			this.avLocations.Shuffle(generator.Rnd);
			this.avNames.Shuffle(generator.Rnd);
			this.avObjects.Shuffle(generator.Rnd);
			this.avQuestions.Shuffle(generator.Rnd);
		}

		public string ReplaceWildcards(string s)
		{
			return rxWildcard.Replace (s, dlgEvaluator);
		}

		public Task GetTask(string taskPrototype)
		{
			int bcc = 0;
			string ss;
			INameable o;
			MatchCollection mc = rxWildcard.Matches (taskPrototype);
			List<Token> tokens = new List<Token>(2 + mc.Count);
			foreach (Match m in mc) {
				ss = taskPrototype.Substring (bcc, m.Index - bcc);
				if (!String.IsNullOrEmpty (ss))
					tokens.Add (new Token (ss, null));
				bcc = m.Index + m.Value.Length;
				Evaluator(m, out o);
				tokens.Add (new Token(m.Value, o));
			}
			ss = taskPrototype.Substring (bcc);
			if (!String.IsNullOrEmpty (ss))
				tokens.Add (new Token (ss, null));
			Task task = new Task();
			task.Tokens = tokens;
			return task;
		}

		#endregion
	}
}

