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
	public partial class Grammar : ITiered
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

		/// <summary>
		/// Regular expression used to extract import directives
		/// </summary>
		private static readonly Regex rxGrammarImportXtractor;

		#endregion

		#region Static constructor

		/// <summary>
		/// Initializes the <see cref="RoboCup.AtHome.CommandGenerator.Grammar"/> class.
		/// </summary>
		static Grammar(){
			rxGrammarNameXtractor = new Regex(@"^\s*grammar\s+name\s+(?<name>.*)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			rxGrammarTierXtractor = new Regex(@"^\s*grammar\s+tier\s+(?<tier>\w+)\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			rxGrammarImportXtractor = new Regex(@"^\s*(?<directive>(load)|(import))\s+(?<path>(\S+|(""([^""]|(\\""))+"")))(\s+as\s+(?<nt>\$[A-Za-z_][0-9A-Za-z_]+))?\s*", RegexOptions.Compiled);
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Loads a grammar from a text file.
		/// </summary>
		/// <param name="filePath">The grammar file path.</param>
		/// <param name="requireMainNT">Specifies whether a Main rule is required to load the grammar.</param>
		/// <returns>The grammar represented within the provided file, or null
		/// if the grammar could not be loadder.</returns>
		public static Grammar LoadFromFile (string filePath)
		{
			if (!File.Exists (filePath))
				return null;

			return new GrammarLoader ().FromFile (filePath);
		}

		#endregion
	}
}
	