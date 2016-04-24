using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Converts between grammar formats
	/// </summary>
	public class GrammarConverter
	{
		private Grammar grammar;
		private XmlWriter writer;
		private List<Gesture> gestures;
		private LocationManager locations;
		private List<PersonName> names;
		private GPSRObjectManager objects;
		private List<PredefindedQuestion> questions;
		//private static 

		/// <summary>
		/// Saves the provided grammar as an SRGSS xml file
		/// </summary>
		/// <param name="grammar">The grammar to convert</param>
		/// <param name="filePath">Path to the SRGS xml file to save the grammar in</param>
		public static void SaveToSRGS(Grammar grammar, string filePath, List<Gesture> gestures, LocationManager locations, List<PersonName> names, GPSRObjectManager objects, List<PredefindedQuestion> questions)
		{
			if (grammar == null) throw new ArgumentNullException();
			GrammarConverter converter = new GrammarConverter();
			converter.grammar = grammar;
			converter.gestures = gestures;
			converter.locations = locations;
			converter.names = names;
			converter.objects = objects;
			converter.questions = questions;

			converter.Convert(filePath);
		}

		private void Convert(string filePath)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			using (writer = XmlTextWriter.Create(filePath, settings))
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
				writer.WriteEndElement();
			}
		}

		private void SRGSWriteHeader()
		{
			writer.WriteStartDocument();
			writer.WriteDocType("grammar", "-//W3C//DTD GRAMMAR 1.0//EN", "http://www.w3.org/TR/speech-grammar/grammar.dtd", null);
			writer.WriteStartElement("grammar", "http://www.w3.org/2001/06/grammar");
			writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
			writer.WriteAttributeString("xsi", "schemaLocation", null, "http://www.w3.org/2001/06/grammar http://www.w3.org/TR/speech-grammar/grammar.xsd");
			writer.WriteAttributeString("root", "main");


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
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("root", SRGSNonTerminalToRuleName(productionRule.NonTerminal));
			writer.WriteAttributeString("scope", "private");
			
			SRGSWriteReplacements(productionRule);
			
			writer.WriteEndElement();
		}

		private void SRGSWriteWildcardRules()
		{
			// SRGSWriteCategoriesRules();
			SRGSWriteGesturesRules();
			SRGSWriteLocationsRules();
			SRGSWriteNamesRules();
			SRGSWriteObjectsRules();
		}

		private void SRGSWriteReplacements(ProductionRule productionRule)
		{
			if (productionRule.Replacements.Count == 1)
				SRGSWriteReplacement(productionRule.Replacements[0]);
			else
			{
				writer.WriteStartElement("one-of");
				foreach (string replacement in productionRule.Replacements)
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

		private void SRGSExpandWildcard(string s, ref int cc)
		{
			string uri = "#_";
			Wildcard w = Wildcard.XtractWildcard(s, ref cc);
			switch (w.Name)
			{
				case "category":
					uri += "categories";
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
			int bcc;
			bcc = cc;
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
			writer.WriteAttributeString("id", "_categories");
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

			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>

			SRGSWriteCategoriesRule();
			SRGSWriteAObjectsRule();
			SRGSWriteKObjectsRule();
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
			foreach (GPSRObject o in objects.Objects)
			{
				if (o.Type != GPSRObjectType.Alike)
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
			foreach (GPSRObject o in objects.Objects)
			{
				if (o.Type != GPSRObjectType.Known)
					continue;
				SRGSWriteItem(o.Name);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}

		private void SRGSWriteQuestionsRule()
		{
			writer.WriteStartElement("rule");
			writer.WriteAttributeString("id", "_questions");
			writer.WriteAttributeString("scope", "private");
			writer.WriteStartElement("one-of");
			foreach (PredefindedQuestion question in questions)
			{
				SRGSWriteItem(question.Question);
			}
			writer.WriteEndElement(); // </one-of>
			writer.WriteEndElement(); // </rule>
		}
	}
}
