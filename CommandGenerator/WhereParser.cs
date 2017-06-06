using System;

namespace RoboCup.AtHome.CommandGenerator
{
	public class WhereParser
	{
		/**
		 * Characterr code table
		 * o	binary operator (non comparison)
		 * u	unary operator
		 * b	binary boolean operator (comparison)
		 * i	identifier
		 * n	number
		 * s	string
		 * e	error
		 * $	end of string
		*/


		public WhereParser ()
		{
			// ReadWhereClause(s, ref cc);
			// Other clauses might exist preceded by a boolean operator
		}

		public static string Parse(string input, ref int cc){
			throw new NotImplementedException();	
		}

		public static string Fetch(string input, ref int cc){
			// A where clause starts with an identifier followed by a binary operator
			// and ends with a value. The type pattern is: io[sn]
			// 
			// Several clauses can be concatenated together by means of boolean binary
			// operators. The type pattern would be: io[sn](bio[sn])*
			//
			// For now, there is no support for parentheses

			char type;
			string clause;
			int bcc = cc;

			do {
				// STEP 1: Read first clause. If none, restore read header and leave
				clause = ReadClause(input, ref cc);
				if (clause == null) {
					cc = bcc;
					return null;
				}

				// STEP 2: Read comparison operator. While there is one, read more clauses!!!
				clause = ReadNext(input, ref cc, out type);
			} while(type == 'b');

			// STEP 3: Return the where substring when the end has been reached.
			//         In case of error, rewind and return null.
			if (type == '$')
				return input.Substring(bcc, cc-bcc);
			cc = bcc;
			return null;
		}


		private static string ReadNext(string s, ref int cc, out char type){
			double d;
			string next = null;

			type = '$';
			Scanner.SkipSpaces(s, ref cc);
			if (cc >= s.Length)
				return null;

			type = 'e';
			switch (s[cc]) {
				// End of wildcard delimiter. This leads to immediate exit.
				case '}':
					type = '$';
					return null;			

				case '\'':
					if (!Scanner.XtractSingleQuotedString(s, ref cc, out next))
						return null;
					type = 's';
					return next;

				case '"':
					if (!Scanner.XtractDoubleQuotedString(s, ref cc, out next))
						return null;
					type = 's';
					return next;

				case '=':
					++cc;
					type = 'o';
					return "=";

				case '!':
					++cc;
					if (!Scanner.ReadChar('=', s, ref cc))
						return null;
					type = 'o';
					return "!=";

				case '>':
					++cc;
					type = 'o';
					if (Scanner.ReadChar('=', s, ref cc))
						return ">=";
					return ">";

				case '<':
					++cc;
					type = 'o';
					if (Scanner.ReadChar('=', s, ref cc))
						return "<=";
					else if (Scanner.ReadChar('>', s, ref cc))
						return "!=";
					return "<";

				case '-':
					if (!Scanner.XtractDouble(s, ref cc, out d))
						return null;
					type = 'n';
					return d.ToString();					
			}

			if (Scanner.IsNumeric(s[cc])) {
				if (!Scanner.XtractDouble(s, ref cc, out d))
					return null;
				type = 'n';
				return d.ToString();
			}

			if (Scanner.IsAlpha(s[cc]) || (s[cc] == '_')) {
				Scanner.XtractIdentifier(s, ref cc, out next);
				CheckIdentifier(s, ref cc, ref next, out type);
				return next;
			}
			return next;
		}

		private static void CheckIdentifier(string s, ref int cc, ref string next, out char type){
			// The code "meta:" is not an identifier.
			// When found the read header must be rewinded and the end has reached
			if ((next == "meta") && Scanner.ReadChar(':', s, ref cc)) {
				type = '$';
				cc -= 5;
				next = null;
				return;
			}
			if (next.ToLower().IsAnyOf("and", "or", "xor")) {
				type = 'b';
				next = next.ToLower();
				return;
			}
			if (next.ToLower() == "not") {
				type = 'u';
				next = next.ToLower();
				return;
			}

			type = 'i';
		}

		private static string ReadClause(string s, ref int cc){
			// A where clause starts with an identifier followed by a binary operator
			// and ends with a value. The type pattern is: io[sn]

			int bcc = cc;
			char type;
			ReadNext(s, ref cc, out type);
			if (type != 'i')
				return null;
			ReadNext(s, ref cc, out type);
			if (type != 'o')
				return null;
			ReadNext(s, ref cc, out type);
			if ((type != 's') && (type != 'n'))
				return null;
			return s.Substring(bcc, cc - bcc);
		}
	}
}

