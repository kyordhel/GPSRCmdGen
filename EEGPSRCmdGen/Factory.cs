using System;
using System.Collections.Generic;
using RoboCup.AtHome.CommandGenerator;

namespace RoboCup.AtHome.EEGPSRCmdGen
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
		public static List<Gesture> GetDefaultGestures()
		{
			List<Gesture> gestures = new List<Gesture>();
			gestures.Add(new Gesture("waving", DifficultyDegree.Easy));
			gestures.Add(new Gesture("rising left arm", DifficultyDegree.Easy));
			gestures.Add(new Gesture("rising right arm", DifficultyDegree.Easy));
			gestures.Add(new Gesture("pointing left", DifficultyDegree.Easy));
			gestures.Add(new Gesture("pointing right", DifficultyDegree.Easy));
			return gestures;
		}

		/// <summary>
		/// Gets a list with predefined locations.
		/// </summary>
		/// <returns>A list with predefined locations.</returns>
		public static List<Room> GetDefaultLocations()
		{
			List<Room> tmp = new List<Room>();
			Room bedroom = new Room("bedroom");
			bedroom.AddBeacon("chair");
			bedroom.AddPlacement("bin");
			bedroom.AddLocation("bed", true, true);
			tmp.Add(bedroom);
			tmp.Add(new Room("bathroom"));
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
		public static List<PersonName> GetDefaultNames()
		{
			List<PersonName> names = new List<PersonName>();

			string[] male = new string[] {
				"Noah",				"Liam",				"Mason",				"Jacob",				"William",				"Ethan",				"Michael",				"Alexander",				"James",				"Daniel"
			};

			string[] female = new string[] {
				"Emma",				"Taylor",				"Sophia",				"Isabella",				"Ava",				"Robin",				"Emily",				"Angel",				"Madison",				"Charlotte"
			};
			foreach (string s in female)
				names.Add(new PersonName(s, Gender.Female));

			foreach (string s in male)
				names.Add(new PersonName(s, Gender.Male));

			return names;
		}

		/// <summary>
		/// Gets a GPSRObjectManager which contains example GPSRObjects grouped by category.
		/// </summary>
		/// <returns>A GPSRObjectManager with default objects.</returns>
		public static List<Category> GetDefaultObjects()
		{
			List<Category> tmp = new List<Category>();

			SpecificLocation shelf = SpecificLocation.Placement("shelf");
			shelf.Room = new Room("dining room");
			Category beverages = new Category("beverages", shelf);
			beverages.AddObject("milk", GPSRObjectType.Known);
			beverages.AddObject("coke", GPSRObjectType.Known);
			beverages.AddObject("orange juice", GPSRObjectType.Known);
			beverages.AddObject("beer", GPSRObjectType.Known, DifficultyDegree.High);
			tmp.Add(beverages);

			SpecificLocation kitchenTable = SpecificLocation.Placement("kitchen table");
			kitchenTable.Room = new Room("kitchen");
			Category fruits = new Category("fruits", kitchenTable);
			fruits.AddObject("apple", GPSRObjectType.Alike);
			fruits.AddObject("banana", GPSRObjectType.Alike);
			fruits.AddObject("orange", GPSRObjectType.Alike);
			fruits.AddObject("pear", GPSRObjectType.Alike);
			tmp.Add(fruits);

			SpecificLocation dinnerTable = SpecificLocation.Placement("dinner table");
			dinnerTable.Room = new Room("dining room");
			Category snacks = new Category("snacks", dinnerTable);
			snacks.AddObject("lays", GPSRObjectType.Known, DifficultyDegree.Moderate);
			snacks.AddObject("crackers", GPSRObjectType.Known);
			snacks.AddObject("pringles", GPSRObjectType.Known);
			snacks.AddObject("chocolate", GPSRObjectType.Known);
			tmp.Add(snacks);

			SpecificLocation bathroomLocker = SpecificLocation.Placement("bathroom locker");
			bathroomLocker.Room = new Room("bathroom");
			Category cleaningStuff = new Category("cleaning stuff", bathroomLocker);
			cleaningStuff.AddObject("cloth", GPSRObjectType.Alike, DifficultyDegree.High);
			cleaningStuff.AddObject("detergent", GPSRObjectType.Known, DifficultyDegree.High);
			cleaningStuff.AddObject("sponge", GPSRObjectType.Known);
			cleaningStuff.AddObject("brush", GPSRObjectType.Known, DifficultyDegree.High);
			tmp.Add(cleaningStuff);

			return tmp;

		}

		/// <summary>
		/// Gets a list with predefined questions.
		/// </summary>
		/// <returns>A list with predefined questions.</returns>
		internal static List<PredefindedQuestion> GetDefaultQuestions()
		{
			List<PredefindedQuestion> q = new List<PredefindedQuestion>();
			q.Add(new PredefindedQuestion("Who are the inventors of the C programming language?", "Ken Thompson and Dennis Ritchie "));
			q.Add(new PredefindedQuestion("Who is the inventor of the Python programming language?", "Guido van Rossum"));
			q.Add(new PredefindedQuestion("Which robot was the star in the movie Wall-E?", "Wall-E"));
			q.Add(new PredefindedQuestion("Where does the term computer bug come from?", "From a moth trapped in a relay"));
			q.Add(new PredefindedQuestion("What is the name of the round robot in the new Star Wars movie?", "BB-8"));
			q.Add(new PredefindedQuestion("How many curry sausages are eaten in Germany each year?", "About 800 million currywurst every year"));
			q.Add(new PredefindedQuestion("Who is president of the galaxy in The Hitchhiker's Guide to the Galaxy?", "Zaphod Beeblebrox"));
			q.Add(new PredefindedQuestion("Which robot is the love interest in Wall-E?", "EVE"));
			q.Add(new PredefindedQuestion("Which company makes ASIMO?", "Honda"));
			q.Add(new PredefindedQuestion("What company makes Big Dog?", "Boston Dynamics"));
			q.Add(new PredefindedQuestion("What is the funny clumsy character of the Star Wars prequals?", "Jar-Jar Binks"));
			q.Add(new PredefindedQuestion("How many people live in the Germany?", "A little over 80 million"));
			q.Add(new PredefindedQuestion("What are the colours of the German flag?", "Black red and yellow"));
			q.Add(new PredefindedQuestion("What city is the capital of the Germany?", "Berlin"));
			q.Add(new PredefindedQuestion("How many arms do you have?", "..."));
			q.Add(new PredefindedQuestion("What is the heaviest element?", "Plutonium when measured by the mass of the element but Osmium is densest"));
			q.Add(new PredefindedQuestion("What did Alan Turing create?", "Many things like Turing machines and the Turing test"));
			q.Add(new PredefindedQuestion("Who is the helicopter pilot in the A-Team?", "Captain Howling Mad Murdock"));
			q.Add(new PredefindedQuestion("What Apollo was the last to land on the moon?", "Apollo 17"));
			q.Add(new PredefindedQuestion("Who was the last man to step on the moon?", "Gene Cernan"));
			q.Add(new PredefindedQuestion("In which county is the play of Hamlet set?", "Denmark"));
			q.Add(new PredefindedQuestion("What are names of Donald Duck's nephews?", "Huey Dewey and Louie Duck"));
			q.Add(new PredefindedQuestion("How many metres are in a mile?", "About 1609 metres"));
			q.Add(new PredefindedQuestion("Name a dragon in The Lord of the Rings?", "Smaug"));
			q.Add(new PredefindedQuestion("Who is the Chancellor of Germany?", "Angela Merkel"));
			q.Add(new PredefindedQuestion("Who developed the first industrial robot?", "The American physicist Joseph Engelberg. He is also considered the father of robotics."));
			q.Add(new PredefindedQuestion("What's the difference between a cyborg and an android?", "Cyborgs are biological being with electromechanical enhancements. Androids are human-shaped robots."));
			q.Add(new PredefindedQuestion("Do you know any cyborg?", "Professor Kevin Warwick. He implanted a chip in in his left arm to remotely operate doors an artificial hand and an electronic wheelchair."));
			q.Add(new PredefindedQuestion("In which city is this year's RoboCup hosted?", "In Leipzig Germany."));
			q.Add(new PredefindedQuestion("Which city hosted last year's RoboCup?", "In Hefei China."));
			q.Add(new PredefindedQuestion("In which city will next year's RoboCup be hosted?", "It hasn't been announced yet."));
			q.Add(new PredefindedQuestion("Name the main rivers surrounding Leipzig", "The Parthe Pleisse and the White Elster."));
			q.Add(new PredefindedQuestion("What is the Cospudener See?", "The Cospudener See is a lake situated south of Leipzig on the site of a former open cast mine."));
			q.Add(new PredefindedQuestion("Where started the peaceful revolution of 1989?", "The peaceful revolution started in September 4 1989 in Leipzig at the St. Nicholas Church."));
			q.Add(new PredefindedQuestion("Where is the world's oldest trade fair hosted?", "The world's oldest trade fair is in Leipzig."));
			q.Add(new PredefindedQuestion("Where is one of the world's largest dark music festivals hosted?", "Leipzig hosts one of the world's largest dark music festivals."));
			q.Add(new PredefindedQuestion("Where is Europe's oldest continuous coffee shop hosted?", "Europe's oldest continuous coffee shop is in Leipzig."));
			q.Add(new PredefindedQuestion("Name one of the greatest German composers", "Johann Sebastian Bach."));
			q.Add(new PredefindedQuestion("Where is Johann Sebastian Bach buried?", "Johann Sebastian Bach is buried in St. Thomas' Church here in Leipzig."));
			q.Add(new PredefindedQuestion("Do you have dreams?", "I dream of Electric Sheeps."));
			q.Add(new PredefindedQuestion("Hey what's up?", "I don't know since I've never been there."));
			q.Add(new PredefindedQuestion("There are seven days in a week. True or false?", "True there are seven days in a week."));
			q.Add(new PredefindedQuestion("There are eleven days in a week. True or false?", "False there are seven days in a week not eleven."));
			q.Add(new PredefindedQuestion("January has 31 days. True or false?", "True January has 31 days."));
			q.Add(new PredefindedQuestion("January has 28 days. True or false?", "False January has 31 days not 28."));
			q.Add(new PredefindedQuestion("February has 28 days. True or false?", "True but in leap-years has 29."));
			q.Add(new PredefindedQuestion("February has 31 days. True or false?", "False February has either 28 or 29 days. Depend on the year."));
			q.Add(new PredefindedQuestion("Do you have dreams?", "I dream of Electric Sheep."));
			q.Add(new PredefindedQuestion("Who used first the word Robot?", "The word robot was first used by Czech writer Karel Capek."));
			q.Add(new PredefindedQuestion("What origin has the word Robot?", "The Czech word robota that means forced work or labour"));
			return q;
		}
	}
}

