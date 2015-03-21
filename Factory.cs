using System;
using System.Collections.Generic;

namespace GPSRCmdGen
{
	/// <summary>
	/// Helper class that produces lists and containers with predefined example data
	/// </summary>
	public static class Factory
	{
		/// <summary>
		/// Gets a list with predefined gestures.
		/// </summary>
		/// <returns>A list with predefined gestures.</returns>
		public static List<Gesture> GetDefaultGestures ()
		{
			List<Gesture> gestures = new List<Gesture> ();
			gestures.Add (new Gesture("waving", DifficultyDegree.Easy));
			gestures.Add (new Gesture("rising left arm", DifficultyDegree.Easy));
			gestures.Add (new Gesture("rising right arm", DifficultyDegree.Easy));
			gestures.Add (new Gesture("pointing left", DifficultyDegree.Easy));
			gestures.Add (new Gesture("pointing right", DifficultyDegree.Easy));
			return gestures;
		}

		/// <summary>
		/// Gets a list with predefined locations.
		/// </summary>
		/// <returns>A list with predefined locations.</returns>
		public static LocationManager GetDefaultLocations ()
		{
			LocationManager man = new LocationManager();
			man.Add (new Room ("bath room"));
			man.Add(new Room("bed room"));
			man.Add(new Room("dining room"));
			man.Add(new Room("hall"));
			man.Add(new Room("kitchen"));
			man.Add(new Room("corridor"));
			return man;
		}

		/// <summary>
		/// Gets a list with predefined names.
		/// </summary>
		/// <returns>A list with predefined names.</returns>
		public static List<PersonName> GetDefaultNames ()
		{
			List<PersonName> names = new List<PersonName> ();

			string[] male = new string[] {
				"Alfred",
				"Charles",
				"Daniel",
				"James",
				"John",
				"Luis",
				"Paul",
				"Richard",
				"Robert",
				"Steve"
			};

			string[] female = new string[] {
				"Anna",
				"Beth",
				"Carmen",
				"Jennifer",
				"Jessica",
				"Kimberly",
				"Kristina",
				"Laura",
				"Mary",
				"Sarah"
			};
			foreach(string s in female)
				names.Add(new PersonName(s, Gender.Female));

			foreach(string s in male)
				names.Add(new PersonName(s, Gender.Male));

			return names;
		}

		/// <summary>
		/// Gets a GPSRObjectManager which contains example GPSRObjects grouped by category.
		/// </summary>
		/// <returns>A GPSRObjectManager with default objects.</returns>
		public static GPSRObjectManager GetDefaultObjects ()
		{
			GPSRObjectManager man = new GPSRObjectManager ();

			Placement shelf = new Placement("shelf");
			shelf.Room = new Room("dining room");
			Category beverages = new Category ("beverages", shelf);
			beverages.AddObject ("milk", GPSRObjectType.Known );
			beverages.AddObject ("coke", GPSRObjectType.Known );
			beverages.AddObject ("orange juice", GPSRObjectType.Known );
			beverages.AddObject ("beer", GPSRObjectType.Known, DifficultyDegree.High );
			man.Add (beverages);

			Placement kitchenTable = new Placement("kitchen table");
			kitchenTable.Room = new Room("kitchen");
			Category fruits = new Category ("fruits", kitchenTable);
			fruits.AddObject ("apple", GPSRObjectType.Alike );
			fruits.AddObject ("banana", GPSRObjectType.Alike );
			fruits.AddObject ("orange", GPSRObjectType.Alike );
			fruits.AddObject ("pear", GPSRObjectType.Alike );
			man.Add (fruits);

			Placement dinnerTable = new Placement("dinner table");
			dinnerTable.Room = new Room("dining room");
			Category snacks = new Category ("", dinnerTable);
			snacks.AddObject ("lays", GPSRObjectType.Known, DifficultyDegree.Moderate );
			snacks.AddObject ("crackers", GPSRObjectType.Known );
			snacks.AddObject ("pringles", GPSRObjectType.Known );
			snacks.AddObject ("chocolate", GPSRObjectType.Known );
			man.Add (snacks);

			Placement bathroomLocker = new Placement("bathroom locker");
			bathroomLocker.Room = new Room("bath room");
			Category cleaningStuff = new Category ("cleaning stuff", bathroomLocker);
			cleaningStuff.AddObject ("cloth", GPSRObjectType.Alike, DifficultyDegree.High );
			cleaningStuff.AddObject ("detergent", GPSRObjectType.Known, DifficultyDegree.High);
			cleaningStuff.AddObject ("sponge", GPSRObjectType.Known );
			cleaningStuff.AddObject ("brush", GPSRObjectType.Known, DifficultyDegree.High );
			man.Add (cleaningStuff);


			return man;

		}

		/// <summary>
		/// Gets a list with predefined questions.
		/// </summary>
		/// <returns>A list with predefined questions.</returns>
		internal static List<PredefindedQuestion> GetDefaultQuestions()
		{
			List<PredefindedQuestion> q = new List<PredefindedQuestion>();
			q.Add(new PredefindedQuestion("How many rings has the Olympic flag?", "Five"));
			q.Add(new PredefindedQuestion("Who was the first man in space?", "Yuri Gagarin"));
			q.Add(new PredefindedQuestion("In which year RoboCup was founded?", "1997"));
			q.Add(new PredefindedQuestion("Who played and lost against Deep Blue in 1996?", "Gary Kaspárov"));
			q.Add(new PredefindedQuestion("What is the name of the Simpson's oldest daughter?", "Lisa"));
			q.Add(new PredefindedQuestion("How many countries are in Europe?", "50"));
			q.Add(new PredefindedQuestion("Who was Aristotle's teacher?", "Plato"));
			q.Add(new PredefindedQuestion("What is the capital of Japan?", "Tokyo"));
			q.Add(new PredefindedQuestion("What is the capital of Poland?", "Warsaw"));
			q.Add(new PredefindedQuestion("What time is it?", "(Robot must answer with the current time)"));
			q.Add(new PredefindedQuestion("What is the name of the USS Enterprise's commander?", "James T. Kirk"));
			q.Add(new PredefindedQuestion("What is the name of Luke Skaywalker's father?", "Anakin Skywalker / Darth Vader"));
			q.Add(new PredefindedQuestion("What's the name of the composer of The Four Seasons?", "Antonio Vivaldi"));
			q.Add(new PredefindedQuestion("For which name is better known La Gioconda?", "Mona Lisa"));
			q.Add(new PredefindedQuestion("What was the first James Bond film?", "Dr No"));
			q.Add(new PredefindedQuestion("What was discovered in 1922 by Howard Carter?", "Tutankamen tomb"));
			q.Add(new PredefindedQuestion("What is the national flower of Japan?", "Chrysanthemum"));
			q.Add(new PredefindedQuestion("Where in France do claret wines come from?", "Bordeaux"));
			q.Add(new PredefindedQuestion("What is your name?", "(Robot must answer with its own name)"));
			q.Add(new PredefindedQuestion("What is your team's name?", "(Robot must answer with the team name)"));
			q.Add(new PredefindedQuestion("What is the national national fruit of Serbia?", "Plum"));
			q.Add(new PredefindedQuestion("On which national flag is there an eagle and a snake?", "Mexico"));
			q.Add(new PredefindedQuestion("St Boniface is the Patron Saint of which country?", "Germany"));
			q.Add(new PredefindedQuestion("A pearmain is what type of fruit?", "Apple"));
			q.Add(new PredefindedQuestion("In Chinese mythology what is Taimut?", "A Dragon"));
			q.Add(new PredefindedQuestion("What is ikebana?", "Flower arranging"));
			q.Add(new PredefindedQuestion("A bind is a group of what type of fish?", "Salmon"));
			q.Add(new PredefindedQuestion("Which leader lives in the Potola?", "Dalai Lama"));
			q.Add(new PredefindedQuestion("Who saved Andromeda from the sea monster?", "Perseus"));
			q.Add(new PredefindedQuestion("What is the answer to the ultimate question about life, the universe and everything?", "42"));
			q.Add(new PredefindedQuestion("What is the oldest most widely used drug on earth?", "Alcohol"));
			q.Add(new PredefindedQuestion("What is the worlds most popular green vegetable?", "Lettuce"));
			q.Add(new PredefindedQuestion("What is Erse?", "Irish Gaelic language"));
			q.Add(new PredefindedQuestion("Which famous person invented the cat flap?", "Isaac Newton"));
			q.Add(new PredefindedQuestion("Which country grows the most potatoes?", "Russia"));
			q.Add(new PredefindedQuestion("What fish can hold objects in its tail?", "Sea Horse"));
			q.Add(new PredefindedQuestion("Greek mathematician cylinder enclosed sphere carved on grave?", "Archimedes"));
			q.Add(new PredefindedQuestion("Who was Agrippa's son?", "Nero"));
			q.Add(new PredefindedQuestion("An alloy of Iron - Chromium and Nickel makes what?", "Stainless Steel"));
			q.Add(new PredefindedQuestion("Freyr was the Norse god of what?", "Fertility"));
			q.Add(new PredefindedQuestion("Who was known as the Little Brown Saint?", "Ghandi"));
			q.Add(new PredefindedQuestion("Who was the Goddess of the rainbow?", "Iris"));
			q.Add(new PredefindedQuestion("What is a quadriga?", "Roman 4 horse chariot"));
			q.Add(new PredefindedQuestion("Which country grew the first Orange?", "China"));
			q.Add(new PredefindedQuestion("Which country was the first to introduce old age pensions?", "Germany"));
			q.Add(new PredefindedQuestion("Which race destroyed Vulcan Planet?", "Romulans"));
			q.Add(new PredefindedQuestion("William Hartnell was the first to play what TV character?", "Dr. Who"));
			q.Add(new PredefindedQuestion("The murder of Gonzago was performed in what Shakespeare play?", "Hamlet"));
			q.Add(new PredefindedQuestion("Which goddess sprang full grown from the forehead of her father Zeus?", "Athena"));
			q.Add(new PredefindedQuestion("Which insect has the best eyesight?", "Dragonfly"));
			return q;
		}
	}
}

