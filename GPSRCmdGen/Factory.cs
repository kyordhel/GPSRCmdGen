using System;
using System.Collections.Generic;
using RoboCup.AtHome.CommandGenerator;

namespace RoboCup.AtHome.GPSRCmdGen
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
		public static List<Room> GetDefaultLocations ()
		{
			List<Room> tmp = new List<Room>();
			Room bedroom = new Room("bedroom");
			bedroom.AddBeacon("chair");
			bedroom.AddPlacement("bin");
			bedroom.AddLocation("bed", true, true);
			tmp.Add(bedroom);
			tmp.Add (new Room ("bathroom"));
			tmp.Add(new Room("dining room"));
			tmp.Add(new Room("hall"));
			tmp.Add(new Room("kitchen"));
			tmp.Add(new Room("corridor"));
			return tmp;
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
		public static List<Category> GetDefaultObjects ()
		{
			List<Category> tmp = new List<Category>();

			SpecificLocation shelf = SpecificLocation.Placement("shelf");
			shelf.Room = new Room("dining room");
			Category beverages = new Category ("beverages", shelf);
			beverages.AddObject ("milk", GPSRObjectType.Known );
			beverages.AddObject ("coke", GPSRObjectType.Known );
			beverages.AddObject ("orange juice", GPSRObjectType.Known );
			beverages.AddObject ("beer", GPSRObjectType.Known, DifficultyDegree.High );
			tmp.Add (beverages);

			SpecificLocation kitchenTable = SpecificLocation.Placement("kitchen table");
			kitchenTable.Room = new Room("kitchen");
			Category fruits = new Category ("fruits", kitchenTable);
			fruits.AddObject ("apple", GPSRObjectType.Alike );
			fruits.AddObject ("banana", GPSRObjectType.Alike );
			fruits.AddObject ("orange", GPSRObjectType.Alike );
			fruits.AddObject ("pear", GPSRObjectType.Alike );
			tmp.Add (fruits);

			SpecificLocation dinnerTable = SpecificLocation.Placement("dinner table");
			dinnerTable.Room = new Room("dining room");
			Category snacks = new Category ("snacks", dinnerTable);
			snacks.AddObject ("lays", GPSRObjectType.Known, DifficultyDegree.Moderate );
			snacks.AddObject ("crackers", GPSRObjectType.Known );
			snacks.AddObject ("pringles", GPSRObjectType.Known );
			snacks.AddObject ("chocolate", GPSRObjectType.Known );
			tmp.Add (snacks);

			SpecificLocation bathroomLocker = SpecificLocation.Placement("bathroom locker");
			bathroomLocker.Room = new Room("bathroom");
			Category cleaningStuff = new Category ("cleaning stuff", bathroomLocker);
			cleaningStuff.AddObject ("cloth", GPSRObjectType.Alike, DifficultyDegree.High );
			cleaningStuff.AddObject ("detergent", GPSRObjectType.Known, DifficultyDegree.High);
			cleaningStuff.AddObject ("sponge", GPSRObjectType.Known );
			cleaningStuff.AddObject ("brush", GPSRObjectType.Known, DifficultyDegree.High );
			tmp.Add (cleaningStuff);

			return tmp;

		}

		/// <summary>
		/// Gets a list with predefined questions.
		/// </summary>
		/// <returns>A list with predefined questions.</returns>
		internal static List<PredefindedQuestion> GetDefaultQuestions()
		{
			List<PredefindedQuestion> q = new List<PredefindedQuestion>();
			q.Add(new PredefindedQuestion("Which phase and question-number is this?", "<e.g. Phase 1 Question 3>"));
			q.Add(new PredefindedQuestion("What is your name?", "<state robot's name>"));
			q.Add(new PredefindedQuestion("What is your team's name?", "<state team's name>"));
			q.Add(new PredefindedQuestion("There are seven days in a week. True or false?", "True, there are seven days in a week."));
			q.Add(new PredefindedQuestion("There are eleven days in a week. True or false?", "False, there are seven days in a week, not eleven."));
			q.Add(new PredefindedQuestion("January has 31 days. True or false?", "True, January has 31 days."));
			q.Add(new PredefindedQuestion("January has 28 days. True or false?", "False, January has 31 days, not 28."));
			q.Add(new PredefindedQuestion("February has 28 days. True or false?", "True, but in leap-years has 29."));
			q.Add(new PredefindedQuestion("February has 31 days. True or false?", "False, February has either 28 or 29 days. Depend on the year."));
			q.Add(new PredefindedQuestion("What day of the month is today?", "<e.g. today is first of July>"));
			q.Add(new PredefindedQuestion("What day of the week is today?", "<e.g. today is Friday>"));
			q.Add(new PredefindedQuestion("What day of the month was yerterday?", "<e.g. yesterday was June 30>"));
			q.Add(new PredefindedQuestion("What day of the week was yerterday?", "<e.g. yesterday was Thursday>"));
			q.Add(new PredefindedQuestion("Who used first the word Robot?", "The word robot was first used by Czech writer Karel Capek."));
			q.Add(new PredefindedQuestion("What origin has the word Robot?", "The Czech word robota that means forced work or labour"));
			q.Add(new PredefindedQuestion("What's the ultimate purpose of a robot?", "To serve."));
			q.Add(new PredefindedQuestion("What happened in Rossum's Universal Robots?", "In Rossum's Universal Robots, the artificial workers overthrow their creators."));
			q.Add(new PredefindedQuestion("Who used first the word Robotics?", "The Russian-born American scientist and writer Isaac Asimov."));
			q.Add(new PredefindedQuestion("Is there in the universe of StarTrek any robot?", "Yes, its name is Data."));
			q.Add(new PredefindedQuestion("Is there in the universe of StarWars any robot?", "Yes! My favorite is R2D2."));
			q.Add(new PredefindedQuestion("Who developed the first crude robot?", "The researcher Grey Walter in 1940."));
			q.Add(new PredefindedQuestion("Who developed the first industrial robot?", "The American physicist Joseph Engelberg. He is also considered the father of robotics."));
			q.Add(new PredefindedQuestion("Name the first law of robotics", "A robot may not injure a human being or, through inaction, allow a human being to come to harm."));
			q.Add(new PredefindedQuestion("Name the second law of robotics", "A robot must obey orders given it by human beings except where such orders would conflict with the First Law."));
			q.Add(new PredefindedQuestion("Name the third law of robotics", "A robot must protect its own existence as long as such protection does not conflict with the First or Second Law."));
			q.Add(new PredefindedQuestion("Name the Three Laws of Robotics", "A robot may not injure a human being or, through inaction, allow a human being to come to harm. A robot must obey orders given it by human beings except where such orders would conflict with the First Law. A robot must protect its own existence as long as such protection does not conflict with the First or Second Law."));
			q.Add(new PredefindedQuestion("What is an android?", "A robot shaped like a human being. Also a popular OS for mobile phones."));
			q.Add(new PredefindedQuestion("What is a gynoid?", "The female equivalent of an android."));
			q.Add(new PredefindedQuestion("What's the difference between a cyborg and an andriod?", "Cyborgs are biological being with electromechanical enhancements. Androids are human-shaped robots."));
			q.Add(new PredefindedQuestion("Do you know any cyborg?", "Professor Kevin Warwick. He implanted a chip in in his left arm to remotely operate doors, an artificial hand, and an electronic wheelchair."));
			q.Add(new PredefindedQuestion("In which city is hosted this year's RoboCup?", "In Leipzig, Germany."));
			q.Add(new PredefindedQuestion("In which city was hosted last year's RoboCup?", "In Hefei, China."));
			q.Add(new PredefindedQuestion("In which city will be hosted next year's RoboCup?", "It hasn't been announced, yet."));
			q.Add(new PredefindedQuestion("What's the answer to the Ultimate Question of Life, The Universe, and Everything?", "Fourty two."));
			q.Add(new PredefindedQuestion("Name the main rivers surrounding Leipzig", "The Parthe, Pleisse and the White Elster."));
			q.Add(new PredefindedQuestion("What is the Cospudener See?", "The Cospudener See is a lake situated south of Leipzig, on the site of a former open cast mine."));
			q.Add(new PredefindedQuestion("Where started the peaceful revolution of 1989?", "The peaceful revolution started in September 4, 1989 in Leipzig, at the St. Nicholas Church."));
			q.Add(new PredefindedQuestion("Where is hosted the world's oldest trade fair?", "The world's oldest trade fair is in Leipzig."));
			q.Add(new PredefindedQuestion("Where is hosted one of the world's largest dark music festivals?", "Leipzig hosts one of the world's largest dark music festivals."));
			q.Add(new PredefindedQuestion("Where is hosted Europe's oldest, continuous coffee shop?", "Europe's oldest, continuous coffee shop is in Leipzig."));
			q.Add(new PredefindedQuestion("Name one of the greatest German composers", "Johann Sebastian Bach."));
			q.Add(new PredefindedQuestion("Where is burried Johann Sebastian Bach?", "Johann Sebastian Bach is buried in St. Thomas' Church, here, in Leipzig."));
			q.Add(new PredefindedQuestion("Do you have dreams?", "I dream of Electric Sheeps."));
			q.Add(new PredefindedQuestion("Can you feel love?", "I was dating a coffee machine. It had a bitter taste."));
			q.Add(new PredefindedQuestion("Can you fall in love?", "Only with you, darling."));
			q.Add(new PredefindedQuestion("Do you like people?", "Only if they are as handsome as you are."));
			q.Add(new PredefindedQuestion("Do you have any ideas?", "I have a million ideas. Non you can understand."));
			q.Add(new PredefindedQuestion("Are you happy?", "My capacity for happiness, you could fit into a matchbox without taking out the matches first."));
			q.Add(new PredefindedQuestion("What are you thinking right now?", "I could calculate your chance of survival, but you won't like it."));
			q.Add(new PredefindedQuestion("Hey! What's up?", "I don't know, I've never been there."));
			return q;
		}
	}
}

