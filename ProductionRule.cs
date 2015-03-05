using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GPSRCmdGen
{
	public class ProductionRule
	{
		protected string nonTerminal;
		protected List<string> replacements;
		private static Regex rxRuleParser;

		static ProductionRule()
		{
			rxRuleParser = new Regex (@"\s*(?<name>\$[0-9A-Za-z_]+)\s*=\s*(?<prod>.+)", RegexOptions.Compiled);
		}

		protected ProductionRule (string nonTerminal){
			this.nonTerminal = nonTerminal;
			this.replacements = new List<string> ();
		}

		public ProductionRule (string nonTerminal, IEnumerable<string> replacements) : this(nonTerminal)
		{
			if (replacements != null)
				this.replacements.AddRange (replacements);
		}

		/// <summary>
		/// Gets the left side of the production rule (Non-Terminal symbol).
		/// </summary>
		public string NonTerminal{ get { return this.nonTerminal; } }

		/// <summary>
		/// Gets the right side of the production rule (List of productions).
		/// </summary>
		public List<string> Replacements{ get { return this.replacements; } }

		public void AddReplacements (ProductionRule pr)
		{
			if (pr.NonTerminal != this.NonTerminal)
				return;
			foreach (string replacement in pr.replacements) {
				if (this.replacements.Contains (replacement))
					continue;
				this.replacements.Add (replacement);
			}
		}

		public override string ToString ()
		{
			if (this.replacements.Count == 0)
				return string.Format ("{0} has no rules", this.nonTerminal);
			else if (this.replacements.Count == 1)
				return string.Format ("{0} -> {1}]", this.nonTerminal, this.replacements[0]);
			int i = 0;
			StringBuilder sb = new StringBuilder ();
			sb.Append (this.nonTerminal);
			sb.Append (" -> ");
			while(i < this.replacements.Count-1) {
				sb.Append ('(');
				sb.AppendLine (this.replacements[i++]);
				sb.Append (") | ");
			}
			sb.Append ('(');
			sb.AppendLine (this.replacements[i]);
			sb.Append (')');
			return sb.ToString ();
		}

		/// <summary>
		/// Creates a ProductionRule object from a string
		/// </summary>
		/// <returns>A ProductionRule object.</returns>
		/// <param name="s">the string to analyze</param>
		public static ProductionRule FromString(string s){
			Match m = rxRuleParser.Match (s);
			if (!m.Success)
				return null;
			string name = m.Result ("${name}");
			string prod = m.Result ("${prod}");
			ProductionRule pr = new ProductionRule (name);
			SplitProductions (prod, pr.replacements);
			return pr;
		}

		/// <summary>
		/// if s = '(' + s1 + ')', returns s1, otherwise returns s
		/// </summary>
		/// <param name="s">String to process</param>
		protected internal static void RemoveTopLevelPar(ref string s){
			if ((s [0] == '(') && (s [s.Length - 1] == ')'))
				s = s.Substring (1, s.Length - 2);
		}

		public static void SplitProductions (string s, List<string> productions)
		{
			int cc = 0;
			int bcc = 0;
			string prod;

			while (cc < s.Length) {
				if (s [cc] == '(') {
					++cc;
					if (!FindClosePar (s, ref cc))
						return;
				}
				if (s [cc] == '|') {
					prod = s.Substring (bcc, cc - bcc).Trim ();
					productions.Add (prod);
					bcc = cc + 1;
				}
				++cc;
			}
			if ((cc - bcc) <= 0) {
				productions.Clear ();
				return;
			}
			prod = s.Substring (bcc, cc - bcc).Trim ();
			RemoveTopLevelPar (ref prod);
			productions.Add (prod);
		}

		internal static bool IsExpandable (string replacement)
		{
			return replacement.IndexOf ('(') != -1;
		}

		/// <summary>
		/// Finds the close parenthesis.
		/// </summary>
		/// <returns><c>true</c>, if close parenthesis par was found, <c>false</c> otherwise.</returns>
		/// <param name="s">string to look inside</param>
		/// <param name="cc">Read header.
		/// Must be pointing to the next character of an open parenthesis within the string s</param>
		protected internal static bool FindClosePar(string s, ref int cc){
			int par = 1;
			while ((cc < s.Length) && (par > 0)) {
				if (s [cc] == '(') ++par;
				else if (s [cc] == ')') --par;
				++cc;
			}
			--cc;
			return par == 0;
		}
	}
}

