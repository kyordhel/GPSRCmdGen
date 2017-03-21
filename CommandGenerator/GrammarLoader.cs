using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RoboCup.AtHome.CommandGenerator
{
	public partial class Grammar{
		/// <summary>
		/// Code for loading grammar files in a (thread) safe way
		/// </summary>
		internal class GrammarLoader
		{
			#region Variables

			/// <summary>
			/// Path of the grammar file being loaded
			/// </summary>
			private string grammarFilepath;

			/// <summary>
			/// The grammar being loaded.
			/// </summary>
			private Grammar grammar;

			/// <summary>
			/// The list of text lines in the grammar text file
			/// </summary>
			private List<string> lines;

			#endregion

			#region Constructor
			#endregion

			#region Methods

			/// <summary>
			/// Loads a grammar from a text file.
			/// </summary>
			/// <param name="filePath">The grammar file path.</param>
			/// <param name="requireMainNT">Specifies whether a Main rule is required to load the grammar.</param>
			/// <returns>The grammar represented within the provided file, or null
			/// if the grammar could not be loadder.</returns>
			public Grammar FromFile (string filePath, bool requireMainNT = true)
			{
				if (!File.Exists (filePath))
					return null;

				grammarFilepath = filePath;
				grammar = new Grammar ();
				lines = new List<string>(File.ReadAllLines (filePath));
				StripCommentsAndEmptyLines ();

				ParseProductionRules ();
				if (requireMainNT && !grammar.productionRules.ContainsKey ("$Main"))
					return null;
				return grammar;
			}

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
			/// Loads another grammar file within the existing one
			/// </summary>
			/// <param name="directive">Specifies the import method used</param>
			/// <param name="nonTerminal">The non-terminal in which the grammar file will be imported</param>
			/// <param name="path">Path to the grammar file to be imported</param>
			/// be dumped</param>
			private void ImportGrammar(string directive, string path, string nonTerminal = null){
				if(directive == "import")
					ImportSubGrammar(directive, path, nonTerminal);
				else if((directive != "load") || !File.Exists(path))
					return;

				Grammar subGrammar = new GrammarLoader().FromFile(path, false);
				if(subGrammar == null)
					return;
				foreach(var item in subGrammar.productionRules){
					if(grammar.productionRules.ContainsKey(item.Key))
						grammar.productionRules[item.Key].AddReplacements(item.Value);
					else
						grammar.productionRules.Add(item.Key, item.Value);
				}

			}

			/// <summary>
			/// Imports production tules from another grammar file
			/// </summary>
			/// <param name="directive">Specifies the import method used</param>
			/// <param name="nonTerminal">The non-terminal in which the grammar file will be imported</param>
			/// <param name="path">Path to the grammar file to be imported</param>
			/// be dumped</param>
			private void ImportSubGrammar(string directive, string path, string nonTerminal=null){
				if(directive != "import")
					return;
				if(!String.IsNullOrEmpty(nonTerminal)){
					ImportSubGrammarIntoNT(directive, path, nonTerminal);
					return;
				}
				if(!File.Exists(path))
					return;

				Grammar subGrammar = new GrammarLoader().FromFile(path, false);
				if(subGrammar == null)
					return;
				subGrammar.productionRules.Remove("$Main");
				foreach(var item in subGrammar.productionRules){
					if(grammar.productionRules.ContainsKey(item.Key))
						grammar.productionRules[item.Key].AddReplacements(item.Value);
					else
						grammar.productionRules.Add(item.Key, item.Value);
				}
			}

			/// <summary>
			/// Imports production tules from another grammar file into a non-terminal
			/// </summary>
			/// <param name="directive">Specifies the import method used</param>
			/// <param name="nonTerminal">The non-terminal in which the grammar file will be imported</param>
			/// <param name="path">Path to the grammar file to be imported</param>
			/// be dumped</param>
			private void ImportSubGrammarIntoNT(string directive, string path, string nonTerminal){
				if(String.IsNullOrEmpty(nonTerminal)){
					ImportSubGrammar(directive, path, nonTerminal);
					return;
				}
				if(directive != "import")
					return;

				ProductionRule pr = null;
				string errMsg = "#ERROR! {void meta:{0}}";
				if(!File.Exists(path)){
					errMsg = String.Format(errMsg, String.Format("File {0} not found", path));
					pr = new ProductionRule(nonTerminal, new String[]{errMsg});
					if(grammar.productionRules.ContainsKey(nonTerminal))
						grammar.productionRules[nonTerminal].AddReplacements(pr);
					else
						grammar.productionRules.Add(nonTerminal, pr);
					return;
				}

				Grammar subGrammar = new GrammarLoader().FromFile(path, true);
				if(subGrammar == null){
					errMsg = String.Format(errMsg, String.Format("Cannot load grammar file {0}", path));
					pr = new ProductionRule(nonTerminal, new String[]{errMsg});
					if(grammar.productionRules.ContainsKey(nonTerminal))
						grammar.productionRules[nonTerminal].AddReplacements(pr);
					else
						grammar.productionRules.Add(nonTerminal, pr);
					return;
				}

				errMsg = String.Format(errMsg, "Not implemented. Sorry =(");
				pr = new ProductionRule(nonTerminal, new String[]{errMsg});
				if(grammar.productionRules.ContainsKey(nonTerminal))
					grammar.productionRules[nonTerminal].AddReplacements(pr);
				else
					grammar.productionRules.Add(nonTerminal, pr);

				// var main = subGrammar.productionRules["$Main"];
				// subGrammar.productionRules.Remove("$Main");
				// foreach(var item in subGrammar.productionRules){
				// 	if(grammar.productionRules.ContainsKey(item.Key))
				// 		grammar.productionRules[item.Key].AddReplacements(item.Value);
				// 	else
				// 		grammar.productionRules.Add(item.Key, item.Value);
				// }
			}

			/// <summary>
			/// Removes a multi-line comment form a list of text lines 
			/// </summary>
			/// <param name="i">The index of the line in the list where the multi-line comment was found.</param>
			/// <param name="j">The index within the line of the first character to the
			/// right of the multi-line comment start symbol.</param>
			private void ParseMultiLineComment (int i, int j)
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
			private void ParseSingleLineComment (string line, int j)
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

				if ((grammar.Tier == DifficultyDegree.Unknown) && (m = rxGrammarTierXtractor.Match (line)).Success ) {
					try{
						grammar.Tier = (DifficultyDegree)Enum.Parse(typeof(DifficultyDegree), m.Result("${tier}"));
					}catch{
						return;
					}
				}

				m = rxGrammarImportXtractor.Match (line);
				if (m.Success) {
					string path = Path.Combine (Path.GetDirectoryName (grammarFilepath), m.Result ("${path}"));
					if (path != grammarFilepath)
						ImportGrammar (m.Result ("${directive}"), path, m.Result ("${nt}"));
				}
			}

			/// <summary>
			/// Parses a set of strings containing each a production rule, converting them into
			/// a non-terminal symbol addressable set of expanded production rules.
			/// </summary>
			private void ParseProductionRules()
			{
				ProductionRule pr;
				Dictionary<string, ProductionRule> prsd = grammar.productionRules;

				// prsd = new Dictionary<string, ProductionRule> ();
				foreach (string line in lines) {
					pr = ProductionRule.FromString (line);
					if ((pr == null) || (pr.Replacements.Count < 1))
						continue;
					if (prsd.ContainsKey (pr.NonTerminal))
						prsd[pr.NonTerminal].AddReplacements(pr);
					else
						prsd.Add (pr.NonTerminal, pr);
				}
				ExpandRules (prsd);
			}

			/// <summary>
			/// Strips comments from the content of a grammar text file
			/// </summary>
			/// <param name="i">The index of the current line being analized</param>
			private void StripComments(int i){
				for (int j = 0; j < lines[i].Length; ++j) {
					// Double character commenting
					if ((lines [i] [j] == '/') && ((j+1) < lines[i].Length)) {
						++j;
						if (lines [i] [j] == '/') {
							ParseSingleLineComment (lines [i], j);
							lines [i] = lines [i].Substring (0, j-1);
							break;
						} else if (lines [i] [j] == '*') {
							ParseMultiLineComment (i, j);
							break;
						}
					}
					// Single character commenting
					else if ((lines [i] [j] == '#') || (lines [i] [j] == ';') || (lines [i] [j] == '%')) {
						ParseSingleLineComment (lines[i], j);
						lines [i] = lines [i].Substring (0, j);
						break;
					}
				}
			}

			/// <summary>
			/// Strips comments and empty lines from the content of a grammar text file
			/// </summary>
			private void StripCommentsAndEmptyLines()
			{
				for (int i = 0; i < lines.Count; ++i) {
					lines [i] = lines [i].Trim ();
					StripComments (i);
					if(lines[i].Length < 1)
						lines.RemoveAt(i--);
				}
			}

			#endregion
			
		}
	}
}

