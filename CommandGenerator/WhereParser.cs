using System;
using System.Collections.Generic;
using System.Linq;

namespace RoboCup.AtHome.CommandGenerator
{
	public partial class WhereParser
	{
		public interface IEvaluable
		{
			bool Evaluate(object o);
		}

		/**
		 * Characterr code table
		 * 0    null
		 * B    binary values: either true or false
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

		/*
		public static T First<T>(this List<T> available, string query){
			ConditionalStatement statement = Parse(query);
			if (statement == null)
				return available.PopLast();
			List<T> candidates = new List<T>(available.Where(o => statement.Evaluate(o)));
			return (candidates.Count > 0) ? candidates.PopLast() : available.PopLast();
		}
		*/

		/// <summary>
		/// Parse the input string s and extracts the conditional statement contained within
		/// </summary>
		/// <param name="s">The string to evaluate or null if the parsing failed.</param>
		public static ConditionalStatement Parse(string s){
			// This far, unary operators and parentheses are not supported

			char type;
			ConditionalStatement statement = new ConditionalStatement();
			int cc = 0;
			int bcc = cc;
			string next;

			// STEP 1: Read first element.
			next = ReadNext(s, ref cc, out type);
			// There are three valid options: identifier, unary and end $
			// Identifier resets cc and goes to Condition.Parse
			// Unary writes the operator in statement and expects Condition
			// Whatever other option has the same effect of end: terminate (return null).
			switch (type)
			{
				case 'i':
					cc = bcc;
					statement.A = Condition.Parse(s, ref cc);
					break;

				case 'u':
					statement.Operator = next;
					statement.A = Condition.Parse(s, ref cc);
					// As an unary operator works as a whole statement, the statement
					// is stacked and left clean.
					statement = new ConditionalStatement(){ A = statement };
					break;

				default: 
					return null;
			}

			while (cc < s.Length)
			{
				// STEP 2: Whatever happened before, either $ or a binary operator is expected;
				// Again, any other option works like end: termination (return null).
				next = ReadNext(s, ref cc, out type);
				if (type != 'b')
					return  statement;
				statement.Operator = next;

				// STEP 3: Read second element.
				bcc = cc;
				next = ReadNext(s, ref cc, out type);
				// Again, there are three valid options: identifier, unary and end $
				// Identifier resets cc and goes to Condition.Parse
				// Unary writes the operator in statement and expects Condition
				// Whatever other option has the same effect of end: terminate (return null).
				switch (type)
				{
					case 'i':
						cc = bcc;
						statement.B = Condition.Parse(s, ref cc);
						break;

					case 'u':
					// As an unary operator works as a whole statement, another statement
					// the negated conditional is stored as a statement within B
						statement.B = 
						new ConditionalStatement()
						{
							Operator = next,
							A = Condition.Parse(s, ref cc)
						};
						break;

					default: 
						return statement;
				}

				// The statement is complete, it is now stacked and we return to STEP 2;
				statement = new ConditionalStatement(){ A = statement };
			}
			return statement;
		}

		public static string Fetch(string input, ref int cc){
			// A where condition starts with an identifier followed by a binary operator
			// and ends with a value. The type pattern is: io[sn]
			// 
			// Several conditions can be concatenated together by means of boolean binary
			// operators to make a clause. The type pattern would be: io[sn](bio[sn])*
			//
			// For now, there is no support for parentheses

			char type;
			Condition condition;
			int bcc = cc;

			do {
				// STEP 1: Read first condition. If none, restore read header and leave
				condition = Condition.Parse(input, ref cc);
				if (condition == null) {
					cc = bcc;
					return null;
				}

				// STEP 2: Read comparison operator. While there is one, read more clauses!!!
				ReadNext(input, ref cc, out type);
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
				case '"':
					return XtractString(s, ref cc, ref type);

				case '=':
					++cc;
					type = 'o';
					return "=";

				case '!':
					++cc;
					if (!Scanner.ReadChar('=', s, ref cc))
					{
						type = 'u';
						return "NOT";
					}
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
			if (next.IsAnyOf("true", "false", "TRUE", "FALSE")) {
				type = 'B';
				next = next.ToLower();
				return;
			}
			if (next.IsAnyOf("null", "NULL")) {
				type = '0';
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

		private static string XtractString(string s, ref int cc, ref char type)
		{
			string next;
			if (!Scanner.XtractDoubleQuotedString(s, ref cc, out next) && !Scanner.XtractSingleQuotedString(s, ref cc, out next))
				return null;
			if (next.IsAnyOf("true", "false", "TRUE", "FALSE"))
			{
				type = 'B';
				next = next.ToLower();
			}
			else
				type = 's';
			return next;
		}
	}
}

