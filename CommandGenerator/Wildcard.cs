using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup.AtHome.CommandGenerator
{
	public class Wildcard
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
		///Indicates if the wildcard is obfuscated
		/// </summary>
		private bool obfuscated;

		/// <summary>
		/// Stores the type of the wildcard
		/// </summary>
		private string type;

		/// <summary>
		/// Stores the 
		/// </summary>
		private string value;

		#endregion

		#region Constructors

		private Wildcard() { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets the Wildcard id
		/// </summary>
		public int Id { get { return this.id; } }

		/// <summary>
		/// Gets the 
		/// </summary>
		public int Index { get { return this.index; } }

		/// <summary>
		/// Gets the Wildcard metadata
		/// </summary>
		public string Metadata { get { return this.metadata; } }

		/// <summary>
		/// Gets the name of the wildcard
		/// </summary>
		public string Name { get { return this.name; } }

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
		public string Type { get { return this.type; } }

		/// <summary>
		/// Gets the 
		/// </summary>
		public string Value { get { return this.value; } }

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

		public static Wildcard XtractWildcard(string s, ref int cc)
		{
			if ((cc >= s.Length) || (s[cc] != '{'))
				return null;

			// Create Wildcard and set index
			Wildcard wildcard = new Wildcard();
			wildcard.index = cc;
			// Read wildcard name
			++cc;
			wildcard.name = ReadWildcardName(s, ref cc);
			if (String.IsNullOrEmpty(wildcard.Name)) return null;

			// Read obfuscator
			wildcard.obfuscated = Scanner.ReadChar('?', s, ref cc);

			// Read wildcard type
			wildcard.type = ReadWildcardType(s, ref cc);

			// Read wildcard id
			wildcard.id = ReadWildcardId(s, ref cc);

			// Read wildcard id
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
			if (type == "meta")
			{
				cc -= 4;
				return String.Empty;
			}
			return type;
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
			FindCloseBrace(s, ref cc);
			return s.Substring(bcc, cc - bcc);
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
				if (s[cc] == '{') ++braces;
				else if (s[cc] == '}') --braces;
				++cc;
			}
			--cc;
			return braces == 0;
		}

		#endregion
	}
}
