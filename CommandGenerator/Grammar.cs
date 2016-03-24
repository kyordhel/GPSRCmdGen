using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// A grammar which can be used to produce random sentences.
	/// </summary>
	public class Grammar : ITiered
	{
		#region Variables

		/// <summary>
		/// Stores the set of production rules (accessible by rule name)
		/// </summary>
		private Dictionary<string, ProductionRule> productionRules;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Grammar"/> class.
		/// </summary>
		public Grammar(){
			this.productionRules = new Dictionary<string, ProductionRule> ();
			this.Tier = DifficultyDegree.Unknown;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the grammar.
		/// </summary>
		public string Name{ get; set; }

		/// <summary>
		/// Gets the difficulty degree (tier) of the grammar
		/// </summary>
		public DifficultyDegree Tier{ get; set;	}

		/// <summary>
		/// Gets the set of production rules (accessible by rule name)
		/// </summary>
		internal Dictionary<string, ProductionRule> ProductionRules {
			get{ return this.productionRules;}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Retrieves the non-terminal symbol within the input string pointed by cc
		/// </summary>
		/// <param name="s">S.</param>
		/// <param name="cc">A read header that points to the first character at
		/// the right of.</param>
		/// <returns>The non-terminal symbol found.</returns>
		private string FetchNonTerminal (string s, ref int cc)
		{
			char c;
			int bcc = cc++;
			while (cc < s.Length) {
				c = s [cc];
				if (((c >= '0') && (c <= '9')) || ((c >= 'A') && (c <= 'Z')) || ((c >= 'a') && (c <= 'z')) || (c == '_') )
					++cc;
				else
					break;
			}
			return s.Substring (bcc, cc - bcc);
		}

		/// <summary>
		/// Gets a random replacement for the provided non-terminal symbol.
		/// </summary>
		/// <param name="nonTerminal">The non-terminal symbol for which a
		/// random replacement will be searched</param>
		/// <param name="rnd">Random number generator used to choose a replacement</param>
		/// <returns>A replacement string. If the non-terminal symbol does not
		/// belong to the grammar or contains no productions, an empty string
		/// is returned.</returns>
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

		/// <summary>
		/// Generates a random sentence.
		/// </summary>
		/// <returns>A randomly generated sentence.</returns>
		public string GenerateSentence(){
			return GenerateSentence (new Random (DateTime.Now.Millisecond));
		}

		/// <summary>
		/// Generates a random sentence.
		/// </summary>
		/// <param name="rnd">Random number generator used to choose the
		/// productions and generate the sentence</param>
		/// <returns>A randomly generated sentence.</returns>
		public string GenerateSentence(Random rnd){
			string option = FindReplacement("$Main", rnd);
			return SolveNonTerminals (option, rnd);
		}

		/// <summary>
		/// Solves all the non-terminal symbols within the given sentence.
		/// </summary>
		/// <param name="sentence">A string with non-terminal symbols to replace.</param>
		/// <param name="rnd">Random number generator used to choose the
		/// productions and generate the sentence</param>
		/// <returns>A string with terminal symbols only</returns>
		private string SolveNonTerminals (string sentence, Random rnd)
		{
			return SolveNonTerminals(sentence, rnd, 0);
		}

		/// <summary>
		/// Solves all the non-terminal symbols within the given sentence.
		/// </summary>
		/// <param name="sentence">A string with non-terminal symbols to replace.</param>
		/// <param name="rnd">Random number generator used to choose the
		/// productions and generate the sentence</param>
		/// <param name="stackCounter">A counter that indicates how many times
		/// this function has called itself. It is used to prevent a stack overflow.
		/// When it reach 1000 the production is aborted.</param>
		/// <returns>A string with terminal symbols only</returns>
		/// <remarks>Recursive function</remarks>
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

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Grammar"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Grammar"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[Grammar: Name={0}, Tier={1}]", Name, Tier);
		}

		#endregion

		#region Static variables

		/// <summary>
		/// Regular expression used to extract the name of a grammar
		/// </summary>
		private static readonly Regex rxGrammarNameXtractor;

		/// <summary>
		/// Regular expression used to extract the difficulty degree of a grammar
		/// </summary>
		private static readonly Regex rxGrammarTierXtractor;

		#endregion

		#region Static constructor

		/// <summary>
		/// Initializes the <see cref="RoboCup.AtHome.CommandGenerator.Grammar"/> class.
		/// </summary>
		static Grammar(){
			rxGrammarNameXtractor = new Regex(@"^\s*grammar\s+name\s+(?<name>.*)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			rxGrammarTierXtractor = new Regex(@"^\s*grammar\s+tier\s+(?<tier>\w+)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Expands all production rules in a set of production rules.
		/// Expansion consists in spliting all parentheses and OR operators in a
		/// replacement into new production rules
		/// </summary>
		/// <param name="ruleDicc">A given rule dictionary having the non-terminal
		/// symbol as key and the production rule as value.</param>
		private static void ExpandRules(Dictionary<string, ProductionRule> ruleDicc)
		{
			List<ProductionRule> ruleList = new List<ProductionRule>(2 * ruleDicc.Count);
			foreach (KeyValuePair<string,ProductionRule> p in ruleDicc)
				ruleList.Add (p.Value);

			for (int ix = 0; ix < ruleList.Count; ++ix) {
				ExpandRule (ix, ruleList, ruleDicc);
			}
		}

		/// <summary>
		/// Expands the addressed production rule in a set of production rules.
		/// Expansion consists in spliting all parentheses and OR operators in a
		/// replacement into new production rules
		/// </summary>
		/// <param name="ix">The index of the rule to expand in the lsit of rules</param>
		/// <param name="ruleList">The set of production rules of the grammar with
		/// elements accessible by index in O(1).</param>
		/// <param name="ruleDicc">The set of production rules of the grammar with
		/// elements accessible by non-terminal symbol in O(1).</param>
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

		/// <summary>
		/// Generates a new symbol for a non-terminal from a existing one
		/// </summary>
		/// <returns>The non terminal.</returns>
		/// <param name="parentNonTerminal">The symbol of the non-terminal which is
		/// going to be split and which will be used as base for generating the
		/// new non-terminal symbol.</param>
		/// <param name="ix">The index of the current production</param>
		/// <param name="ruleDicc">The dictionary containing all the rules of the grammar</param>
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

		/// <summary>
		/// Parses a set of strings containing each a production rule, converting them into
		/// a non-terminal symbol addressable set of expanded production rules.
		/// </summary>
		/// <param name="rules">The set of strings to parse</param>
		/// <returns>A set of production rules of with elements accessible by its
		/// non-terminal symbol</returns>
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

		/// <summary>
		/// Loads a grammar from a text file.
		/// </summary>
		/// <param name="filePath">The grammar file path.</param>
		/// <returns>The grammar represented within the provided file, or null
		/// if the grammar could not be loadder.</returns>
		public static Grammar LoadFromFile (string filePath)
		{
			if (!File.Exists (filePath))
				return null;

			Grammar grammar = new Grammar ();
			List<string> lines = new List<string>(File.ReadAllLines (filePath));
			StripCommentsAndEmptyLines (lines, grammar);

			grammar.productionRules = ParseProductionRules (lines);
			if (!grammar.productionRules.ContainsKey ("$Main"))
				return null;
			return grammar;
		}

		/// <summary>
		/// Removes a multi-line comment form a list of text lines 
		/// </summary>
		/// <param name="lines">The list of text lines in the grammar text file.</param>
		/// <param name="i">The index of the line in the list where the multi-line comment was found.</param>
		/// <param name="j">The index within the line of the first character to the
		/// right of the multi-line comment start symbol.</param>
		private static void ParseMultiLineComment (List<string> lines, int i, int j)
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

		/// <summary>
		/// Parses the single line comment looking for the name and tier
		/// (difficulty degree) of the grammar
		/// </summary>
		/// <param name="grammar">The grammar file where the found data will be stored</param>
		/// <param name="line">The string that contains the comment</param>
		/// <param name="j">The position in the line where the comment starts</param>
		private static void ParseSingleLineComment (Grammar grammar, string line, int j)
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

		/// <summary>
		/// Strips comments from the content of a grammar text file
		/// </summary>
		/// <param name="lines">The set of lines read of a grammar text file</param>
		/// <param name="i">The index of the current line being analized</param>
		/// <param name="grammar">A grammar object on which additional information may
		/// be dumped</param>
		private static void StripComments(List<string> lines, int i, Grammar grammar){
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
		}

		/// <summary>
		/// Strips comments and empty lines from the content of a grammar text file
		/// </summary>
		/// <param name="lines">The set of lines read of a grammar text file</param>
		/// <param name="grammar">A grammar object on which additional information may
		/// be dumped</param>
		private static void StripCommentsAndEmptyLines(List<string> lines, Grammar grammar)
		{
			for (int i = 0; i < lines.Count; ++i) {
				lines [i] = lines [i].Trim ();
				StripComments (lines, i, grammar);
				if(lines[i].Length < 1)
					lines.RemoveAt(i--);
			}
		}

		#endregion
	}
}
	