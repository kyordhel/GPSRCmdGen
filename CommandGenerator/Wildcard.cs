using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Represents an unified Wildcard that encapsulates all TextWildcards with a common keycode
	/// </summary>
	public class Wildcard : IEnumerable<TextWildcard>, IWildcard
	{
		#region Variables

		/// <summary>
		/// Stores the keycode of the Wildcard
		/// </summary>
		private string keycode;

		/// <summary>
		/// Stores the keyword associated to this wildcard
		/// </summary>
		private string keyword;

		/// <summary>
		/// Stores the list text wildcards this Wildcard unifies
		/// </summary>
		private List<TextWildcard> textWildcards;

		/// <summary>
		/// Stores the replacement for all the unified wildcards
		/// </summary>
		private INameable replacement;

		/// <summary>
		/// Stores the obfuscated replacement value for the unified wildcard
		/// </summary>
		private INameable obfuscated;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Wildcard"/> class from a <see cref="RoboCup.AtHome.CommandGenerator.TextWildcard"/> object
		/// </summary>
		/// <param name="tw">The textWildcard to be used to initialize the wildcard</param>
		public Wildcard(TextWildcard textWildcard) {
			if (textWildcard == null)
				throw new ArgumentNullException ("textWildcard cannot be null.");
			this.keycode = textWildcard.Keycode;
			this.textWildcards = new List<TextWildcard> ();
			this.textWildcards.Add (textWildcard);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of TextWildcards contained in the Wildcard.
		/// </summary>
		/// <value>The number of elements contained in the Wildcard.</value>
		public int Count{ get {return this.textWildcards.Count;} }

		/// <summary>
		/// Gets the keycode associated to each wildcard group unique replacements
		/// </summary>
		public string Keycode
		{
			get{ return this.keycode; }
		}

		/// <summary>
		/// Gets or sets the keyword associated to this wildcard
		/// </summary>
		public string Keyword
		{
			get{ return String.IsNullOrEmpty (this.keyword) ? this.Name : this.keyword; }
			set{ this.keyword = String.IsNullOrEmpty (value) ? null : value.ToLower (); }
		}

		/// <summary>
		/// Gets the name of the wildcard
		/// </summary>
		public string Name
		{
			get { return this.textWildcards[0].Name; }
		}

		/// <summary>
		/// Stores the obfuscated replacement value for the unified wildcard
		/// </summary>
		public INameable Obfuscated { get { return this.obfuscated; } set { this.obfuscated = value; } }

		/// <summary>
		/// Gets or sets the replacement for all the unified wildcards
		/// </summary>
		public INameable Replacement { get { return this.replacement; } set { this.replacement = value; } }

		/// <summary>
		/// Gets the dominant type of the TextWildcards in the collection
		/// </summary>
		public string Type 
		{
			get {
				string tNone = String.Empty;
				Dictionary<string, int> tCount=new Dictionary<string, int>(10);
				tCount.Add (tNone, 0);
				foreach (TextWildcard t in this.textWildcards) {
					if (!tCount.ContainsKey (t.Type ?? tNone))
						tCount.Add (t.Type ?? tNone, 0);
					tCount [t.Type ?? tNone]+=1;
				}
				if (tCount [tNone] == this.textWildcards.Count)
					return null;

				int max = 0;
				string key = null;
				foreach(KeyValuePair<string, int> pair in tCount){
					if (String.IsNullOrEmpty(pair.Key) || (max >= pair.Value)) 
						continue;
					max = pair.Value;
					key = pair.Key;
				}
				return String.IsNullOrEmpty(key) ? null : key;
			}
		}

        /// <summary>
        /// Gets the union (AND) of all where clauses in the collection
        /// </summary>
        public string Where{ 
            get{
                Queue<string> clauses = new Queue<string>(this.textWildcards.Count);
                foreach (TextWildcard t in this.textWildcards)
                {
                    if (!String.IsNullOrEmpty(t.Where))
                        clauses.Enqueue(t.Where);
                }
				if (clauses.Count < 1)
					return String.Empty;
                if (clauses.Count == 1)
                    return clauses.Dequeue();
                StringBuilder sb = new StringBuilder();
                // ToDo: Add parentheses support
                // sb.AppendFormat("({0})", clauses.Dequeue());
                sb.AppendFormat("{0}", clauses.Dequeue());
                while (clauses.Count > 0)
                {
                    // ToDo: Add parentheses support
                    // sb.AppendFormat("AND ({0})", clauses.Dequeue());
                    sb.AppendFormat("AND {0}", clauses.Dequeue());
                }
                return sb.ToString();
            }
        }

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the <see cref="RoboCup.AtHome.CommandGenerator.Wildcard"/> at the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public TextWildcard this[int index] { get{ return this.textWildcards [index];} }

		#endregion

		#region Methods

		/// <summary>
		/// Adds the provided TextWildcard to the collection. Wildcards must have the same keycode.
		/// </summary>
		/// <param name="w">The TextWildcard to add to the collection.</param>
		public void Add(TextWildcard w){
			if(w == null)throw new ArgumentNullException ("w cannot be null.");
			if (w.Keycode != this.keycode)
				throw new InvalidOperationException ("Keycode mistmatch. Added wildcards must share the same keycode");
			this.textWildcards.Add (w);
		}

		public override string ToString()
		{
			//string s = String.Empty;
			//if (!String.IsNullOrEmpty(this.Name))
			//    s += "Name=" + this.Name;
			//if (!String.IsNullOrEmpty(this.Type))
			//    s += " Type=" + this.Type;
			//if (this.Id != -1)
			//    s += String.Format(" Id={0}", this.Id);
			//if (!String.IsNullOrEmpty(this.Metadata))
			//    s += " Metadata=" + this.Metadata;
			//return s.TrimStart();
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat ("{0} ({1})", this.keycode, textWildcards.Count);
			return sb.ToString();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<TextWildcard> GetEnumerator (){
			return this.textWildcards.GetEnumerator ();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator (){
			return this.GetEnumerator ();
		}

		#endregion

		#region Static Methods

		#endregion
	}
}
