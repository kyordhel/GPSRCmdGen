using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using RoboCup.AtHome.CommandGenerator.ReplaceableTypes;
using Object = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.Object;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Converts between grammar formats
	/// </summary>
	public class GrammarConverter
	{
		private Grammar grammar;
		protected XmlWriter writer;
		private List<Gesture> gestures;
		private LocationManager locations;
		private List<PersonName> names;
		private ObjectManager objects;
		private List<PredefinedQuestion> questions;
		//private static 

		/// <summary>
		/// Initializes a new instance of GrammarConverter
		/// </summary>
		private GrammarConverter()
		{
			this.objects = ObjectManager.Instance;
			this.locations = LocationManager.Instance;
		}

		/// <summary>
		/// Initializes a new instance of GrammarConverter
		/// </summary>
		/// <param name="grammar">The grammar to convert</param>
		/// <param name="gestures">List of gesture names</param>
		/// <param name="names">List of people name</param>
		/// <param name="questions">List of known questions</param>
		public GrammarConverter(Grammar grammar, List<Gesture> gestures, List<PersonName> names, List<PredefinedQuestion> questions) : this()
		{
			if (grammar == null) throw new ArgumentNullException();
			this.grammar = grammar;
			this.gestures = (gestures != null)? gestures : new List<Gesture>();
			this.names = (names != null)?names  : new List<PersonName>();
			this.questions = (questions != null) ? questions : new List<PredefinedQuestion>();
		}

		/// <summary>
		/// Saves the provided grammar as an SRGSS xml file
		/// </summary>
		/// <param name="grammar">The grammar to convert</param>
		/// <param name="filePath">Path to the SRGS xml file to save the grammar in</param>
		/// <param name="grammar">The grammar to convert</param>
		/// <param name="gestures">List of gesture names</param>
		/// <param name="names">List of people name</param>
		/// <param name="questions">List of known questions</param>
		public static void SaveToSRGS(Grammar grammar, string filePath, List<Gesture> gestures, List<PersonName> names, List<PredefinedQuestion> questions)
		{
			if (grammar == null) throw new ArgumentNullException();
			GrammarConverter converter = new GrammarConverter();
			converter.grammar = grammar;
			converter.gestures = gestures;
			converter.names = names;
			converter.questions = questions;

			converter.ConvertToXmlSRGS(filePath);
		}

		/// <summary>
		/// Converts the grammar to Xml SRGS specification, saving it in the provided stream
		/// </summary>
		public void ConvertToXmlSRGS(TextWriter writer)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			using (this.writer = XmlTextWriter.Create(writer, settings))
			{
				SRGSWriteHeader();
				SRGSWriteMainRule();
				foreach (var productionRule in grammar.ProductionRules.Values)
				{
					if (productionRule.NonTerminal == "$Main") continue;
					SRGSWriteProductionRule(productionRule);
				}
				SRGSWriteWildcardRules();
				SRGSWriteQuestionsRule();
				this.writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Converts the grammar to ABNF SRGS specification, saving it in the provided stream
		/// </summary>
		public void ConvertToAbnfSRGS(TextWriter writer)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts the grammar to ABNF SRGS specification, saving it into the specified file
		/// </summary>
		/// <param name="filePath">The name of the file in which the converted grammar will be stored</param>
		public void ConvertToAbnfSRGS(string filePath)
		{
			using (StreamWriter stream = new StreamWriter(filePath))
			{
				ConvertToAbnfSRGS(stream);
				stream.Close();
			}
		}

		/// <summary>
		/// Converts the grammar to Xml SRGS specification, saving it into the specified file
		/// </summary>
		/// <param name="filePath">The name of the file in which the converted grammar will be stored</param>
		public void ConvertToXmlSRGS(string filePath)
		{
			using (StreamWriter stream = new StreamWriter(filePath))
			{
				ConvertToXmlSRGS(stream);
				stream.Close();
			}
		}

		protected virtual void SRGSWriteHeader()
		{
			writer.WriteStartDocument();
			// writer.WriteDocType("grammar", "-//W3C//DTD GRAMMAR 1.0//EN", "http://www.w3.org/TR/speech-grammar/grammar.dtd", null);
			writer.WriteStartElement("grammar", "http://www.w3.org/2001/06/grammar");
			writer.WriteAttributeString("version", "1.0");
			writer.WriteAttributeString("xml", "lang", null, "en-US");
			// writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
			// writer.WriteAttributeString("xsi", "schemaLocation", null, "http://www.w3.org/2001/06/grammar http://www.w3.org/TR/speech-grammar/grammar.xsd");
			writer.WriteAttributeString("root", "main");


		}

		private string ComputeKeyword(TextWildcard w)
		{
			if (w.Type == null)
				return w.Name;

			switch (w.Name)
			{
				case "location":
					switch(w.Type) { case "beacon": case "placement": case "room": return w.Type; }
					break;

				case "name":
					switch (w.Type) { case "male": case "female": return w.Type; }
					break;

				case "object":
					switch (w.Type)
					{
						case "aobject": case "kobject": case "special":
							return w.Type[0].ToString() + w.Name;
					}
					break;

				case "pron":
					switch (w.Type)
					{
						case "obj": return "pronobj";
						case "sub": return "pronsub";
					}
					break;
			}

			return w.Name;
		}

		private void SRGSWriteMainRule()
		{
			ProductionRule main = grammar.ProductionRules["$Main"];
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "main");
			writer.WriteAttributeString("scope", "public");

			writer.WriteStartElement("one-of");
			foreach (string replacement in main.Replacements)
				SRGSWriteReplacement(replacement);
			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_questions");
			writer.WriteEndElement();
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		private void SRGSWriteProductionRule(ProductionRule productionRule)
		{
			List<string> validReplacements = GetValidReplacements(productionRule);
			
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", SRGSNonTerminalToRuleName(productionRule.NonTerminal));
			writer.WriteAttributeString("scope", "private");
			
			SRGSWriteReplacements(validReplacements);
			
			writer.WriteEndElement();
		}

		private List<string> GetValidReplacements(ProductionRule productionRule)
		{
			List<string> vr = new List<string>(productionRule.Replacements.Count);

			for (int i = 0; i < productionRule.Replacements.Count; ++i){
				string replacement = (productionRule.Replacements[i] ?? String.Empty).Trim();

				for (int cc = 0; cc < replacement.Length; ++cc)
				{
					cc = replacement.IndexOf('{', cc);
					if (cc == -1) break;
					int bcc = cc;
					TextWildcard w = TextWildcard.XtractWildcard(replacement, ref cc);
					if (w.Name != "void")
						continue;
					replacement = replacement.Remove(bcc) + replacement.Substring(cc);
					cc = bcc;
				}

				if(String.IsNullOrEmpty(replacement))
					continue;

				vr.Add(replacement);
			}
			return vr;
		}

		private void SRGSWriteWildcardRules()
		{
			// SRGSWriteCategoriesRules();
			SRGSWriteGesturesRules();
			SRGSWriteLocationsRules();
			SRGSWriteNamesRules();
			SRGSWriteObjectsRules();
			SRGSWritePronounsRules();
		}

		private void SRGSWriteReplacements(List<string> replacements)
		{
			if (replacements.Count == 0)
			{
				writer.WriteStartElement("item");
				writer.WriteEndElement();
			}
			else if (replacements.Count == 1)
				SRGSWriteReplacement(replacements[0]);
			else
			{
				writer.WriteStartElement("one-of");
				foreach (string replacement in replacements)
					SRGSWriteReplacement(replacement);
				writer.WriteEndElement();
			}
		}

		private void SRGSWriteReplacement(string replacement)
		{
			writer.WriteStartElement("item");
			SRGSExpandReplacement(replacement);
			writer.WriteEndElement();
		}

		private void SRGSExpandReplacement(string replacement)
		{
			// Search in sentence for non-terminals
			int cc = 0;
			while ( cc < replacement.Length) {
				if (replacement[cc] == '$')
					SRGSExpandNonTerminal(replacement, ref cc);
				else if (replacement[cc] == '{')
					SRGSExpandWildcard(replacement, ref cc);
				else
				{
					writer.WriteString(replacement.Substring(cc,1));
					++cc;
				}
			}
		}

		private void SRGSWriteRuleRef(string uri)
		{
			writer.WriteStartElement("ruleref");
			writer.WriteAttributeString("uri", uri);
			writer.WriteEndElement();
		}

		private void SRGSWriteItem(string value)
		{
			writer.WriteStartElement("item");
			writer.WriteString(value);
			writer.WriteEndElement();
		}

		/*
		private void SRGSExpandWhereWildcard(TextWildcard w)
		{
			string keyword = ComputeKeyword(w);
			string uri = "#_";
			SRGSWriteRuleRef(uri);
		}
		*/

		private void SRGSExpandWildcard(string s, ref int cc)
		{
			string uri = "#_";
			TextWildcard w = TextWildcard.XtractWildcard(s, ref cc);
			/*
			if (!String.IsNullOrEmpty(w.Where))
			{
				SRGSExpandWhereWildcard(w);
				return;
			}
			*/
			string keyword = ComputeKeyword(w);
			switch (keyword)
			{
				case "category":
					uri += "categories";
					break;

				case "pron":
					uri += "pronobjs";
					break;

				case "gesture":
				case "name":
				case "female":
				case "male":
				case "location":
				case "beacon":
				case "placement":
				case "room":
				case "object":
				case "aobject":
				case "kobject":
				case "sobject":
				case "pronobj":
				case "pronsub":
					uri += w.Name + "s";
					break;

				case "question":
					writer.WriteString("question");
					return;

				case "void": return;

				default: return;
			}
			SRGSWriteRuleRef(uri);
		}

		private void SRGSExpandNonTerminal(string replacement, ref int cc)
		{
			string nonTerminal = FetchNonTerminal(replacement, ref cc);
			string uri = "#" + SRGSNonTerminalToRuleName(nonTerminal);
			SRGSWriteRuleRef(uri);
		}

		private string FetchNonTerminal(string s, ref int cc)
		{
			char c;
			int bcc = cc++;
			while (cc < s.Length)
			{
				c = s[cc];
				if (((c >= '0') && (c <= '9')) || ((c >= 'A') && (c <= 'Z')) || ((c >= 'a') && (c <= 'z')) || (c == '_'))
					++cc;
				else
					break;
			}
			return s.Substring(bcc, cc - bcc);
		}

		private string SRGSNonTerminalToRuleName(string nonTerminal){
			return nonTerminal.Substring(1,1).ToLower() + (nonTerminal.Length > 2 ? nonTerminal.Substring(2) : String.Empty);
		}

		private void SRGSWriteGesturesRules()
		{
			if (gestures == null)
				return;
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_gestures");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (Gesture gesture in gestures)
			{
				SRGSWriteItem(gesture.Name);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		private void SRGSWriteLocationsRules()
		{
			if (locations == null)
				return;
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_locations");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");

			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_beacons");
			writer.WriteEndElement();

			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_placements");
			writer.WriteEndElement();

			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_rooms");
			writer.WriteEndElement();

			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>

			SRGSWriteBeaconsRule();
			SRGSWritePlacementsRule();
			SRGSWriteRoomsRule();
		}

		private void SRGSWriteBeaconsRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_beacons");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			HashSet<string> hsLocations = new HashSet<string>();
			foreach (Location loc in locations)
			{
				if (loc.IsBeacon && !hsLocations.Contains(loc.Name))
					SRGSWriteItem(loc.Name);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWritePlacementsRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_placements");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			HashSet<string> hsLocations = new HashSet<string>();
			foreach (Location loc in locations)
			{
				if (loc.IsPlacement && !hsLocations.Contains(loc.Name))
					hsLocations.Add(loc.Name);
			}
			foreach (Category category in objects.Categories)
			{
				if (category.DefaultLocation.IsPlacement && !hsLocations.Contains(category.DefaultLocation.Name))
					hsLocations.Add(category.DefaultLocation.Name);
			}
			foreach (string loc in hsLocations)
			{
				SRGSWriteItem(loc);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteRoomsRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_rooms");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (Room room in locations.Rooms)
			{
				SRGSWriteItem(room.Name);
			}

			HashSet<string> hsRooms = new HashSet<string>();
			foreach (Room room in locations.Rooms)
			{
				if (!hsRooms.Contains(room.Name))
					hsRooms.Add(room.Name);
			}
			foreach (Category category in objects.Categories)
			{
				if (!hsRooms.Contains(category.RoomString))
					hsRooms.Add(category.RoomString);
			}
			foreach (string room in hsRooms)
			{
				SRGSWriteItem(room);
			}

			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteNamesRules()
		{
			if (names == null)
				return;
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_names");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");

			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_males");
			writer.WriteEndElement();

			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_females");
			writer.WriteEndElement();

			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>

			SRGSWriteMaleNamesRule();
			SRGSWriteFemaleNamesRule();
		}

		private void SRGSWriteFemaleNamesRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_females");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (PersonName name in names)
			{
				if (name.Gender != Gender.Female)
					continue;
				SRGSWriteItem(name.Name);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteMaleNamesRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_males");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (PersonName name in names)
			{
				if (name.Gender != Gender.Male)
					continue;
				SRGSWriteItem(name.Name);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteObjectsRules()
		{
			if (objects == null)
				return;

			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_objects");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");

			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_aobjects");
			writer.WriteEndElement();

			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_kobjects");
			writer.WriteEndElement();

			writer.WriteStartElement("item");
			SRGSWriteRuleRef("#_sobjects");
			writer.WriteEndElement();

			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>

			SRGSWriteCategoriesRule();
			SRGSWriteAObjectsRule();
			SRGSWriteKObjectsRule();
			SRGSWriteSObjectsRule();
		}

		private void SRGSWriteCategoriesRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_categories");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (Category category in objects.Categories)
			{
				SRGSWriteItem(category.Name);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteAObjectsRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_aobjects");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (Object o in objects.Objects)
			{
				if (o.Type != ObjectType.Alike)
					continue;
				SRGSWriteItem(o.Name);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteKObjectsRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_kobjects");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (Object o in objects.Objects)
			{
				if (o.Type != ObjectType.Known)
					continue;
				SRGSWriteItem(o.Name);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteSObjectsRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_sobjects");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (Object o in objects.Objects)
			{
				if (o.Type != ObjectType.Special)
					continue;
				SRGSWriteItem(o.Name);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWritePronounsRules(){
			SRGSWritePronouns_Obj_Rule();
			SRGSWritePronouns_Sub_Rule();
		}

		private void SRGSWritePronouns_Obj_Rule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_pronobjs");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (string s in Pronoun.Personal.AllObjective)
				SRGSWriteItem(s);
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWritePronouns_Sub_Rule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_pronsubs");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (string s in Pronoun.Personal.AllSubjective)
				SRGSWriteItem(s);
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteQuestionsRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_questions");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (PredefinedQuestion question in questions)
			{
				SRGSWriteItem(question.Question);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}
	}
}
