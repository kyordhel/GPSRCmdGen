using System;
using System.Collections.Generic;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	/// <summary>
	/// Provides pronouns for English language given the pronoun class and person
	/// </summary>
	public static class Pronoun
	{

		#region Enumerations

		/// <summary>
		/// Classes of pronouns
		/// </summary>
		private enum Class
		{
			/// <summary>
			/// Personal pronouns in subjective case.
			/// I, you, he/she/it, we, you, they
			/// </summary>
			PersonalSubjective = 0x0000,
			/// <summary>
			/// Personal pronouns in objective case.
			/// me, you, him/her/it, us, you, them
			/// </summary>
			PersonalObjective = 0x1000,
			/// <summary>
			/// Possessive pronouns.
			/// mine, yours, his/hers/its, ours, theirs
			/// </summary>
			PossessiveAbsolute = 0x2000,
			/// <summary>
			/// Possessive pronouns.
			/// my, your, his/her/its, our, their
			/// </summary>
			PossessiveAdjective = 0x3000,
			/// <summary>
			/// Reflexive pronouns.
			/// myself, yourself, himself, herself, itself, oneself, ourselves, yourselves, themselves
			/// </summary>
			Reflexive = 0x4000,
			/// <summary>
			/// Reciprocal pronouns
			/// each other, one another
			/// </summary>
			Reciprocal = 0x5000,
			/// <summary>
			/// Relative pronouns.
			/// that, which, who, whose, whom, where, when
			/// </summary>
			Relative = 0x6000,
			/// <summary>
			/// Demonstrative pronouns.
			/// this, that, these, those
			/// </summary>
			Demonstrative = 0x7000,
			/// <summary>
			/// Interrogative pronouns.
			/// who, what, why, where, when, whatever
			/// </summary>
			Interrogative = 0x8000,
			/// <summary>
			/// Indefinite pronouns.
			/// anything, anybody, anyone, something, somebody, someone, nothing, nobody, none, no one
			/// </summary>
			Indefinite = 0x9000
		}

		/// <summary>
		/// Pronoun forms: singular and plural.
		/// </summary>
		public enum Form
		{
			/// <summary>
			/// Represents singular forms (one noun).
			/// </summary>
			Singular=0x0000,
			/// <summary>
			/// Represents plural forms (two or more nouns).
			/// </summary>
			Plural=0x0010
		}

		/// <summary>
		/// Pronoun gender: Masculine, femenine, and neutral (objects).
		/// </summary>
		public enum Gender
		{
			/// <summary>
			/// The masculine gender
			/// </summary>
			Masculine = 0x0001,
			/// <summary>
			/// The femenine gender
			/// </summary>
			Femenine = 0x0002,
			/// <summary>
			/// The neutral gender.
			/// </summary>
			Neutral = 0x0000
		}

		/// <summary>
		/// Pronoun persons: First, Second, and Third.
		/// </summary>
		public enum Person
		{
			/// <summary>
			/// Represents the first person: the speaker side.
			/// </summary>
			First=0x0000,
			/// <summary>
			/// Represents the second person: the hearer side.
			/// </summary>
			Second=0x0100,
			/// <summary>
			/// Represents the third person: the other one.
			/// </summary>
			Third=0x0200
		}

		#endregion

		#region Variables

		private static Dictionary<int, string> pronouns;

		#endregion

		#region Constructor

		static Pronoun(){
			pronouns = new Dictionary<int, string> (100);

			// Class + Person + Form + Gender?
			pronouns.Add( (int)Class.PersonalSubjective | (int)Person.First  | (int)Form.Singular, "I");
			pronouns.Add( (int)Class.PersonalSubjective | (int)Person.Second | (int)Form.Singular, "you");
			pronouns.Add( (int)Class.PersonalSubjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Masculine, "he");
			pronouns.Add( (int)Class.PersonalSubjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Femenine, "she");
			pronouns.Add( (int)Class.PersonalSubjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Neutral, "it");
			pronouns.Add( (int)Class.PersonalSubjective | (int)Person.First  | (int)Form.Plural, "we");
			pronouns.Add( (int)Class.PersonalSubjective | (int)Person.Second | (int)Form.Plural, "you");
			pronouns.Add( (int)Class.PersonalSubjective | (int)Person.Third  | (int)Form.Plural, "they");

			pronouns.Add( (int)Class.PersonalObjective | (int)Person.First  | (int)Form.Singular, "me");
			pronouns.Add( (int)Class.PersonalObjective | (int)Person.Second | (int)Form.Singular, "you");
			pronouns.Add( (int)Class.PersonalObjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Masculine, "him");
			pronouns.Add( (int)Class.PersonalObjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Femenine, "her");
			pronouns.Add( (int)Class.PersonalObjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Neutral, "it");
			pronouns.Add( (int)Class.PersonalObjective | (int)Person.First  | (int)Form.Plural, "us");
			pronouns.Add( (int)Class.PersonalObjective | (int)Person.Second | (int)Form.Plural, "you");
			pronouns.Add( (int)Class.PersonalObjective | (int)Person.Third  | (int)Form.Plural, "them");

			pronouns.Add( (int)Class.PossessiveAbsolute | (int)Person.First  | (int)Form.Singular, "mine");
			pronouns.Add( (int)Class.PossessiveAbsolute | (int)Person.Second | (int)Form.Singular, "yours");
			pronouns.Add( (int)Class.PossessiveAbsolute | (int)Person.Third  | (int)Form.Singular | (int)Gender.Masculine, "his");
			pronouns.Add( (int)Class.PossessiveAbsolute | (int)Person.Third  | (int)Form.Singular | (int)Gender.Femenine, "hers");
			pronouns.Add( (int)Class.PossessiveAbsolute | (int)Person.Third  | (int)Form.Singular | (int)Gender.Neutral, "its");
			pronouns.Add( (int)Class.PossessiveAbsolute | (int)Person.First  | (int)Form.Plural, "ours");
			pronouns.Add( (int)Class.PossessiveAbsolute | (int)Person.Second | (int)Form.Plural, "yours");
			pronouns.Add( (int)Class.PossessiveAbsolute | (int)Person.Third  | (int)Form.Plural, "theirs");

			pronouns.Add( (int)Class.PossessiveAdjective | (int)Person.First  | (int)Form.Singular, "my");
			pronouns.Add( (int)Class.PossessiveAdjective | (int)Person.Second | (int)Form.Singular, "your");
			pronouns.Add( (int)Class.PossessiveAdjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Masculine, "his");
			pronouns.Add( (int)Class.PossessiveAdjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Femenine, "her");
			pronouns.Add( (int)Class.PossessiveAdjective | (int)Person.Third  | (int)Form.Singular | (int)Gender.Neutral, "its");
			pronouns.Add( (int)Class.PossessiveAdjective | (int)Person.First  | (int)Form.Plural, "our");
			pronouns.Add( (int)Class.PossessiveAdjective | (int)Person.Second | (int)Form.Plural, "your");
			pronouns.Add( (int)Class.PossessiveAdjective | (int)Person.Third  | (int)Form.Plural, "their");
		}

		#endregion

		#region Methods
		/// <summary>
		/// Gets a pronoun based on the referral and referee wildcards
		/// </summary>
		/// <returns>A pronoun to refer the wilcard received as referee.</returns>
		/// <param name="referral">The refering wildcard.</param>
		/// <param name="referee">The wildcard being referred.</param>
		public static string FromWildcard(Wildcard referral, Wildcard referee){
			if ((referee == null) || (referral == null)){
				Console.WriteLine("referral or referee is null");
				throw new NullReferenceException ("referral or referee is null");
			}
			if (referral.Name != "pron"){
				Console.WriteLine("referral is not a {pron} wildcard");
				throw new InvalidOperationException ("referral is not a {pron} wildcard");
			}

			Gender g;
			switch (referee.Keyword) {
				case "male":
					g = Gender.Masculine;
					break;
				case "female":
					g = Gender.Femenine;
					break;
				default:
					g = Gender.Neutral;
					break;
			}

			if (!String.IsNullOrEmpty(referral.Type)) {
				switch(referral.Type.Substring(0, 3)){
					case "sub":
						return Personal.Subjective (Form.Singular, Person.Third, g);

					case "pab":
						return Possessive.PossessiveAbsolute (Form.Singular, Person.Third, g);

					case "paj":
						return Possessive.PossessiveAdjective (Form.Singular, Person.Third, g);

					case "pos":
						if(referral.Type.StartsWith("posabs"))
							return Possessive.PossessiveAbsolute (Form.Singular, Person.Third, g);
						return Possessive.PossessiveAdjective (Form.Singular, Person.Third, g);
				}

			}
			return Personal.Objective (Form.Singular, Person.Third, g);
		}

		#endregion

		#region Subclases

		/// <summary>
		/// Provides personal pronouns for English language given the pronoun class, case, and person
		/// </summary>
		public static class Personal
		{
			public static string[] AllObjective
			{
				get
				{
					List<string> all = new List<string>(9);
					foreach (Pronoun.Person p in Enum.GetValues(typeof(Pronoun.Person)))
					{
						foreach (Pronoun.Form f in Enum.GetValues(typeof(Pronoun.Form)))
						{
							foreach (Pronoun.Gender g in Enum.GetValues(typeof(Pronoun.Gender)))
							{
								string pron = Objective(f, p, g);
								if(all.Contains(pron)) continue;
								all.Add(pron);
							}
						}
					}
					return all.ToArray();
				}
			}

			public static string[] AllSubjective
			{
				get
				{
					List<string> all = new List<string>(9);
					foreach (Pronoun.Person p in Enum.GetValues(typeof(Pronoun.Person)))
					{
						foreach (Pronoun.Form f in Enum.GetValues(typeof(Pronoun.Form)))
						{
							foreach (Pronoun.Gender g in Enum.GetValues(typeof(Pronoun.Gender)))
							{
								string pron = Subjective(f, p, g);
								if(all.Contains(pron)) continue;
								all.Add(pron);
							}
						}
					}
					return all.ToArray();
				}
			}

			public static string Objective(Pronoun.Form form, Pronoun.Person person, Pronoun.Gender gender = Pronoun.Gender.Neutral)
			{
				int key = (int)Class.PersonalObjective | (int)person | (int)form;
				if ((form == Form.Singular) && (person == Person.Third))
					key |= (int)gender;
				return  pronouns [key];
			}

			public static string Subjective(Pronoun.Form form, Pronoun.Person person,  Pronoun.Gender gender = Pronoun.Gender.Neutral)
			{
				int key = (int)Class.PersonalSubjective | (int)person | (int)form;
				if ((form == Form.Singular) && (person == Person.Third))
					key |= (int)gender;
				return  pronouns [key];
			}



		}

		/// <summary>
		/// Provides possessive pronouns for English language given the pronoun class, case, and person
		/// </summary>
		public static class Possessive
		{
			public static string PossessiveAbsolute(Pronoun.Form form, Pronoun.Person person, Pronoun.Gender gender = Pronoun.Gender.Neutral)
			{
				int key = (int)Class.PossessiveAbsolute | (int)person | (int)form;
				if ((form == Form.Singular) && (person == Person.Third))
					key |= (int)gender;
				return  pronouns [key];

			}

			public static string PossessiveAdjective(Pronoun.Form form, Pronoun.Person person, Pronoun.Gender gender = Pronoun.Gender.Neutral)
			{
				int key = (int)Class.PossessiveAdjective | (int)person | (int)form;
				if ((form == Form.Singular) && (person == Person.Third))
					key |= (int)gender;
				return  pronouns [key];

			}
		}

		#endregion
	}
}

