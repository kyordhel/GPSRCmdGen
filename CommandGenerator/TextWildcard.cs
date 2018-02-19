using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup.AtHome.CommandGenerator
{
	public class TextWildcard : IWildcard
	{
		#region Variables

		/// <summary>
		/// Stores the Wildcard id
		/// </summary>
		private int id;

		/// <summary>
		/// Stores the
		/// </summary>
		private int index;

		/// <summary>
		/// Stores the Wildcard metadata
		/// </summary>
		private string metadata;

		/// <summary>
		/// Stores the name of the wildcard
		/// </summary>
		private string name;

		/// <summary>
		/// Stores the next automatically calculated identifier.
		/// </summary>
		private static int nextAutoId = 1000;

		/// <summary>
		///Indicates if the wildcard is obfuscated
		/// </summary>
		private bool obfuscated;

		/// <summary>
		/// Stores the type of the wildcard
		/// </summary>
		private string type;

		/// <summary>
		/// Stores the original text from which the text wildcard was created
		/// </summary>
		private string value;

		#endregion

		#region Constructors

		private TextWildcard() { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets the keycode associated to each wildcard group unique replacements
		/// </summary>
		public string Keycode
		{
			get{ return Name + this.id.ToString().PadLeft(4, '0'); }
		}

		/// <summary>
		/// Gets the Wildcard id
		/// </summary>
		public int Id
		{
			get { return this.id; }
			internal set { this.id = (value < 0) ? TextWildcard.nextAutoId++ : value; }
		}

		/// <summary>
		/// Gets the
		/// </summary>
		public int Index { get { return this.index; } }

		/// <summary>
		/// Gets the Wildcard metadata
		/// </summary>
		public string Metadata { get { return this.metadata; } internal set { this.metadata = value;} }

		/// <summary>
		/// Gets the name of the wildcard
		/// </summary>
		public string Name
		{
			get { return this.name; }
			protected set{ this.name = String.IsNullOrEmpty (value) ? null : value.ToLower (); }
		}

		/// <summary>
		/// Gets a value indicating if the wildcard is obfuscated
		/// </summary>
		public bool Obfuscated { get { return this.obfuscated; } }

		/// <summary>
		/// Gets a value indicating if the wildcard is valid
		/// </summary>
		public bool Success { get { return !String.IsNullOrEmpty(this.Name); } }

		/// <summary>
		/// Gets the type of the wildcard
		/// </summary>
		public string Type
		{
			get { return this.type; }
			protected set{ this.type = String.IsNullOrEmpty (value) ? null : value.ToLower (); }
		}

		/// <summary>
		/// Gets the original text from which the text wildcard was created
		/// </summary>
		public string Value { get { return this.value; } }

        /// <summary>
        /// Gets the string of where clauses
        /// </summary>
        public string Where{ get; internal set; }

		#endregion

		#region Methods

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
			return this.value;
		}

		#endregion

		#region Static Methods

		public static TextWildcard XtractWildcard(string s, ref int cc)
		{
			if ((cc >= s.Length) || (s[cc] != '{'))
				return null;

			// Create Wildcard and set index
			TextWildcard wildcard = new TextWildcard();
			wildcard.index = cc;
			// Read wildcard name
			++cc;
			wildcard.Name = ReadWildcardName(s, ref cc);
			if (String.IsNullOrEmpty(wildcard.Name)) return null;

			// Read obfuscator
			wildcard.obfuscated = Scanner.ReadChar('?', s, ref cc);

			// Read wildcard type
			wildcard.Type = ReadWildcardType(s, ref cc);

			// Read wildcard id
			wildcard.Id = ReadWildcardId(s, ref cc);

			// Read wildcard where clauses (query)
            wildcard.Where = ReadWhereClauses(s, ref cc);

			// Read wildcard metadata
			wildcard.metadata = ReadWildcardMetadata(s, ref cc);

			// Set wildcard value
			if (cc < s.Length) ++cc;
			wildcard.value = s.Substring(wildcard.index, cc - wildcard.index);
			return wildcard;
		}

		private static string ReadWildcardName(string s, ref int cc)
		{
			Scanner.SkipSpaces(s, ref cc);
			int bcc = cc;
			while ((cc < s.Length) && Scanner.IsLAlpha(s[cc])) ++cc;
			return s.Substring(bcc, cc - bcc);
		}

		private static string ReadWildcardType(string s, ref int cc)
		{
			Scanner.SkipSpaces(s, ref cc);
			int bcc = cc;
			while ((cc < s.Length) && Scanner.IsLAlpha(s[cc])) ++cc;
			string type = s.Substring(bcc, cc - bcc);
			if ((type != null) && type.IsAnyOf("meta", "where"))
			{
				cc -= type.Length;
				return String.Empty;
			}
			return type;
		}

		private static string ReadWhereClauses(string s, ref int cc)
		{
			Scanner.SkipSpaces(s, ref cc);
			int bcc = cc;
			// First, read the "where" literal string
			char[] where = new char[]{'w','h','e','r', 'e'};
			foreach(char c in where){
				if (Scanner.ReadChar(c, s, ref cc)) continue;
				cc = bcc;
				return null;
			}
			// After the "where" literal string, the where clauses come
			string clauses = WhereParser.Fetch(s, ref cc);
			return clauses;
		}

		private static string ReadWildcardMetadata(string s, ref int cc)
		{
			Scanner.SkipSpaces(s, ref cc);
			char[] meta = new char[]{'m','e','t','a'};
			foreach(char c in meta){
				if (Scanner.ReadChar(c, s, ref cc)) continue;
				FindCloseBrace(s, ref cc);
				return null;
			}
			Scanner.SkipSpaces(s, ref cc);
			if (!Scanner.ReadChar(':', s, ref cc))
			{
				FindCloseBrace(s, ref cc);
				return null;
			}
			int bcc = cc;
			string metaContent = null;
			FindCloseBrace(s, ref cc, out metaContent);
			return metaContent;
		}

		private static int ReadWildcardId(string s, ref int cc)
		{
			ushort usId;
			Scanner.SkipSpaces(s, ref cc);
			if (!Scanner.XtractUInt16(s, ref cc, out usId)) return -1;
			return usId;
		}

		/// <summary>
		/// Finds the close brace.
		/// </summary>
		/// <param name="s">string to look inside</param>
		/// <param name="cc">Read header.
		/// Must be pointing to the next character of an open brace within the string s</param>
		/// <returns><c>true</c>, if closing brace par was found, <c>false</c> otherwise.</returns>
		protected internal static bool FindCloseBrace(string s, ref int cc)
		{
			int braces = 1;
			while ((cc < s.Length) && (braces > 0))
			{
				if (s[cc] == '\\'){
					cc+=2;
					continue;
				}
				if (s[cc] == '{') ++braces;
				else if (s[cc] == '}') --braces;
				++cc;
			}
			--cc;
			return braces == 0;
		}

		/// <summary>
		/// Finds the close brace.
		/// </summary>
		/// <param name="s">string to look inside</param>
		/// <param name="cc">Read header.
		/// Must be pointing to the next character of an open brace within the string s</param>
		/// <returns><c>true</c>, if closing brace par was found, <c>false</c> otherwise.</returns>
		protected internal static bool FindCloseBrace(string s, ref int cc, out string subs)
		{
			int braces = 1;
			StringBuilder sb = new StringBuilder(s.Length);

			while ((cc < s.Length) && (braces > 0))
			{
				if (s[cc] == '\\'){
					if (++cc < s.Length) sb.Append(s[cc]);
					++cc;
					continue;
				}
				if (s[cc] == '{') ++braces;
				else if (s[cc] == '}') --braces;
				sb.Append(s[cc]);
				++cc;
			}
			--cc;
			if(sb.Length > 0) --sb.Length;
			subs = sb.ToString();
			return braces == 0;
		}

		#endregion
	}
}
