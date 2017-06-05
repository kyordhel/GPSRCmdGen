using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Represents an unified Wildcard that encapsulates all TextWildcards with a common keycode
	/// </summary>
	public class Wildcard : IWildcard
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
		/// Stores the Wildcard metadata
		/// </summary>
		private string metadata;

		/// <summary>
		/// Stores the name of the wildcard
		/// </summary>
		private string name;

		/// <summary>
		/// Stores the list text wildcards this Wildcard unifies
		/// </summary>
		private List<TextWildcard> textWildcards;

		/// <summary>
		/// Stores the replacement for all the unified wildcards
		/// </summary>
		private INameable replacement;

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
			Add (textWildcard);
		}

		#endregion

		#region Properties

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
			get{ return String.IsNullOrEmpty (this.keyword) ? this.name : this.keyword; }
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
		/// Gets or sets the replacement for all the unified wildcards
		/// </summary>
		public INameable Replacement { get { return this.replacement; } set { this.replacement = value; } }

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

		#endregion

		#region Static Methods

		#endregion
	}
}
