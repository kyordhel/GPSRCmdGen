using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GPSRCmdGen
{
	public class Grammar : ITiered
	{
		private static readonly Regex rxGrammarNameXtractor;
		private static readonly Regex rxGrammarTierXtractor;
		private Dictionary<string, ProductionRule> productionRules;

		public Grammar(){
			this.productionRules = new Dictionary<string, ProductionRule> ();
			this.Tier = DifficultyDegree.Unknown;
		}

		static Grammar(){
			rxGrammarNameXtractor = new Regex(@"^\s*grammar\s+name\s+(?<name>.*)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			rxGrammarTierXtractor = new Regex(@"^\s*grammar\s+tier\s+(?<tier>\w+)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

		public string Name{ get; set; }

		public DifficultyDegree Tier{ get; set;	}

		internal Dictionary<string, ProductionRule> ProductionRules {
			get{ return this.productionRules;}
		}

		private string FetchNonTerminal (string s, ref int cc)
		{
			char c;
			int bcc = cc++;
			while (cc < s.Length) {
				c = s [cc];
				if (((c >= '0') && (cc <= '9')) || ((c >= 'A') && (cc <= 'Z')) || ((c >= 'a') && (c <= 'z')) || (c == '_') )
					++cc;
				else
					break;
			}
			return s.Substring (bcc, cc - bcc);
		}

		private string FindReplacement(string nonTerminal, Random rnd){
			ProductionRule rule;

			if (!this.productionRules.ContainsKey (nonTerminal))
				return String.Empty;
			rule = this.productionRules [nonTerminal];
			int max = rule.Replacements.Count;
			if (max < 1)
				return String.Empty;
			return rule.Replacements [rnd.Next (0, max)];
		}

		public string GenerateSentence(){
			return GenerateSentence (new Random (DateTime.Now.Millisecond));
		}

		public string GenerateSentence(Random rnd){
			string option = FindReplacement("$Main", rnd);
			return SolveNonTerminals (option, rnd);
		}

		public override string ToString ()
		{
			return string.Format ("[Grammar: Name={0}, Tier={1}]", Name, Tier);
		}

		private string SolveNonTerminals (string sentence, Random rnd)
		{
			return SolveNonTerminals(sentence, rnd, 0);
		}

		private string SolveNonTerminals (string sentence, Random rnd, int stackCounter)
		{
			if (++stackCounter > 999)
				throw new StackOverflowException ();

			int bcc;
			string replacement;
			string nonTerminal;

			// Search in sentence for non-terminals
			int cc = 0;
			while ( cc < sentence.Length) {
				if (sentence [cc] != '$') {
					++cc;
					continue;
				}
				bcc = cc;
				nonTerminal = FetchNonTerminal (sentence, ref cc);
				replacement = FindReplacement (nonTerminal, rnd);
				replacement = SolveNonTerminals (replacement, rnd, stackCounter);
				sentence = sentence.Substring (0, bcc) + replacement + sentence.Substring (cc);
				cc = bcc + replacement.Length;
			}
			return sentence;
		}

		private static void ExpandRules(Dictionary<string, ProductionRule> ruleDicc)
		{
			List<ProductionRule> ruleList = new List<ProductionRule>(2 * ruleDicc.Count);
			foreach (KeyValuePair<string,ProductionRule> p in ruleDicc)
				ruleList.Add (p.Value);

			for (int ix = 0; ix < ruleList.Count; ++ix) {
				ExpandRule (ix, ruleList, ruleDicc);
			}
		}

		private static void ExpandRule (int ix, List<ProductionRule> ruleList, Dictionary<string, ProductionRule> ruleDicc)
		{
			if (ix >= ruleList.Count)
				return;

			string replacement;
			string nonTerminal;
			int nonTerminalBaseIndex = 0;
			ProductionRule pr = ruleList [ix];
			for (int i = 0; i < pr.Replacements.Count; ++i) {
				replacement = pr.Replacements [i];
				// Find open parenthesis within replacement
				for (int cc = 0; cc < replacement.Length; ++cc) {
					if (replacement [cc] != '(')
						continue;
					// Open parenthesis was found!
					// Increase reading header to the char at the right of the left par
					int bcc = ++cc; 
					// and find the closing parenthesis
					if (!ProductionRule.FindClosePar (replacement, ref cc))
						break;
					// Get the replacement (subchunk)
					string subchunk = replacement.Substring (bcc, cc - bcc);
					// Generate a Non-Terminal symbol (name) for the replacement
					nonTerminal = GenerateNonTerminal(pr.NonTerminal, ref nonTerminalBaseIndex, ruleDicc);
					// Create and add the production rule
					ProductionRule cpr = ProductionRule.FromString(nonTerminal+" = "+subchunk);
					ruleList.Add (cpr);
					ruleDicc.Add (cpr.NonTerminal, cpr);
					// Replace the subchunk with the Non-Terminal symbol
					replacement = replacement.Substring (0, bcc-1) + nonTerminal + replacement.Substring (cc+1);
					pr.Replacements [i] = replacement;
					cc = bcc + nonTerminal.Length -2;
				}
			}
		}

		private static string GenerateNonTerminal (string parentNonTerminal, ref int ix, Dictionary<string, ProductionRule> ruleDicc)
		{
			string nonTerminal;
			string prefix = parentNonTerminal + "_";
			do {
				nonTerminal = prefix + ix.ToString ();
				++ix;
			} while(ruleDicc.ContainsKey(nonTerminal));
			return nonTerminal;
		}

		public static Grammar FromString(string s)
		{
			Grammar grammar;

			string[] gl = s.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			grammar = new Grammar ();
			grammar.productionRules = ParseProductionRules(gl);
			return grammar;
		}

		private static Dictionary<string, ProductionRule> ParseProductionRules(IEnumerable<string> rules)
		{
			ProductionRule pr;
			Dictionary<string, ProductionRule> prsd;

			prsd = new Dictionary<string, ProductionRule> ();
			foreach (string line in rules) {
				pr = ProductionRule.FromString (line);
				if ((pr == null) || (pr.Replacements.Count < 1))
					continue;
				if (prsd.ContainsKey (pr.NonTerminal))
					prsd[pr.NonTerminal].AddReplacements(pr);
				else
					prsd.Add (pr.NonTerminal, pr);
			}
			ExpandRules (prsd);
			return prsd;
		}

		public static Grammar LoadFromFile (string filePath)
		{
			if (!File.Exists (filePath))
				return null;

			Grammar grammar = new Grammar ();
			List<string> lines = new List<string>(File.ReadAllLines (filePath));
			for (int i = 0; i < lines.Count; ++i) {
				lines [i] = lines [i].Trim ();
				for (int j = 0; j < lines[i].Length; ++j) {
					// Double character commenting
					if ((lines [i] [j] == '/') && ((j+1) < lines[i].Length)) {
						++j;
						if (lines [i] [j] == '/') {
							ParseSingleLineComment (grammar, lines [i], j);
							lines [i] = lines [i].Substring (0, j-1);
							break;
						} else if (lines [i] [j] == '*') {
							ParseMultiLineComment (lines, i, j);
							break;
						}
					}
					// Single character commenting
					else if ((lines [i] [j] == '#') || (lines [i] [j] == ';') || (lines [i] [j] == '%')) {
						ParseSingleLineComment (grammar, lines[i], j);
						lines [i] = lines [i].Substring (0, j);
						break;
					}
				}
				if(lines[i].Length < 1)
					lines.RemoveAt(i--);
			}

			grammar.productionRules = ParseProductionRules (lines);
			if (!grammar.productionRules.ContainsKey ("$Main"))
				return null;
			return grammar;
		}

		static void ParseMultiLineComment (List<string> lines, int i, int j)
		{
			lines [i] = lines [i].Substring (0, j-1);
			if(lines[i].Length < 1)
				lines.RemoveAt(i);

			while (i < lines.Count) {
				j = lines [i].IndexOf ("*/");
				if (j != -1)
					break;
				lines.RemoveAt(i);
			}
			lines [i] = lines [i].Substring (j+2);
		}

		static void ParseSingleLineComment (Grammar grammar, string line, int j)
		{
			if (++j >= line.Length)
				return;

			Match m;
			line = line.Substring (j);
			if (String.IsNullOrEmpty (grammar.Name)) {
				m = rxGrammarNameXtractor.Match (line);
				if (m.Success)
					grammar.Name = m.Result ("${name}");
			}
			m = rxGrammarTierXtractor.Match (line);
			if (grammar.Tier == DifficultyDegree.Unknown) {
				m = rxGrammarTierXtractor.Match (line);
				try{
				if(m.Success)
						grammar.Tier = (DifficultyDegree)Enum.Parse(typeof(DifficultyDegree), m.Result("${tier}"));
				}catch{
					return;
				}
			}
		}
	}
}
	