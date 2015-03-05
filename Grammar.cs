using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GPSRCmdGen
{
	public class Grammar
	{
		private Dictionary<string, ProductionRule> productionRules;

		public static string g = @"
$count  = $count1 | $count2 | $count3

$count1 = count the $cntxat $report
$cntxat = $cntoat | $cntpat
$cntoat = $object at the $PlacementLocation
$cntpat = $people at the $Room

$count2 = $navigt $docntx $report 
$navigt = $GoVerb to the 
$docntx = $docnto | $docntp
$docnto = $PlacementLocation, count the $object
$docntp = $Room, count the $people
 
$count3 = Tell $target how many $ctable
$ctable = $objain | $pplain
$objain = $object are in the $PlacementLocation
$pplain = $people are in the $Room
 
$object = objects | $ObjectCategory | $AlikeObjects
$people = people | $PeopleByGender | $PeopleByGesture
$report = and report to $target
$target = me | ($Name (at | in | which is in) the $Room)";

		public Grammar(){
			this.productionRules = new Dictionary<string, ProductionRule> ();

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

		static void ExpandRule (int ix, List<ProductionRule> ruleList, Dictionary<string, ProductionRule> ruleDicc)
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
			ProductionRule pr;
			Dictionary<string, ProductionRule> prsd;

			string[] gl = g.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			prsd = new Dictionary<string, ProductionRule> ();
			foreach (string line in gl) {
				pr = ProductionRule.FromString (line);
				if ((pr == null) || (pr.Replacements.Count < 1))
					continue;
				if (prsd.ContainsKey (pr.NonTerminal))
					prsd[pr.NonTerminal].AddReplacements(pr);
				else
					prsd.Add (pr.NonTerminal, pr);
			}
			ExpandRules (prsd);
			grammar = new Grammar ();
			grammar.productionRules = prsd;
			return grammar;
		}

		private void ParseLine(string line){



		}

	}
}

