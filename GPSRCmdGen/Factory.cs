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
			bedroom.AddBeacon("bed");
			bedroom.AddPlacement("bedside");
			tmp.Add(bedroom);

			Room livingroom = new Room("living room");
			livingroom.AddPlacement("living shelf");
			livingroom.AddLocation("TV stand", true, true);
			livingroom.AddLocation("living table", true, true);
			tmp.Add(livingroom);

			Room office = new Room("office");
			office.AddPlacement("drawer");
			office.AddLocation("desk", true, true);
			tmp.Add(office);

			Room kitchen = new Room("kitchen");
			kitchen.AddPlacement("bar");
			kitchen.AddPlacement("cupboard");
			kitchen.AddLocation("sink", true, true);
			kitchen.AddPlacement("sideshelf");
			kitchen.AddPlacement("bookcase");
			kitchen.AddLocation("dining table", true, true);
			tmp.Add(kitchen);

			Room corridor = new Room("corridor");
			corridor.AddLocation("cabinet", true, true);
			tmp.Add(corridor);
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
				"Ken",
				"Erik",
				"Samuel",
				"Skyler",
				"Brian",
				"Thomas",
				"Edward",
				"Michael",
				"Charlie",
				"Alex"
			};

			string[] female = new string[] {
				"Hanna",
				"Barbara",
				"Samantha",
				"Erika",
				"Sophie",
				"Jackie",
				"Samantha",
				"Skyler",
				"Charlie",
				"Alex"
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

			SpecificLocation desk = SpecificLocation.Placement("desk");
			desk.Room = new Room("office");
			Category snacks = new Category("snacks", desk);
			snacks.AddObject("chips", GPSRObjectType.Known, DifficultyDegree.Moderate);
			snacks.AddObject("senbei", GPSRObjectType.Known, DifficultyDegree.Moderate);
			snacks.AddObject("pringles", GPSRObjectType.Known);
			snacks.AddObject("peanuts", GPSRObjectType.Known);
			tmp.Add(snacks);

			SpecificLocation centerTable = SpecificLocation.Placement("center table");
			centerTable.Room = new Room("living room");
			Category candies = new Category("candies", centerTable);
			candies.AddObject("chocolate bar", GPSRObjectType.Known, DifficultyDegree.Moderate);
			candies.AddObject("manju", GPSRObjectType.Known, DifficultyDegree.Easy);
			candies.AddObject("mints", GPSRObjectType.Alike, DifficultyDegree.Moderate);
			candies.AddObject("chocolate egg", GPSRObjectType.Known, DifficultyDegree.High);
			tmp.Add(candies);

			SpecificLocation fridge = SpecificLocation.Placement("fridge");
			fridge.Room = new Room("kitchen");
			Category food = new Category("food", fridge);
			food.AddObject("noodles", GPSRObjectType.Known);
			food.AddObject("apple", GPSRObjectType.Alike);
			food.AddObject("paprika", GPSRObjectType.Alike);
			food.AddObject("watermelon", GPSRObjectType.Alike, DifficultyDegree.High);
			food.AddObject("sushi", GPSRObjectType.Alike, DifficultyDegree.High);
			tmp.Add(food);

			SpecificLocation bar = SpecificLocation.Placement("bar");
			bar.Room = new Room("living room");
			Category drinks = new Category("drinks", bar);
			drinks.AddObject("tea", GPSRObjectType.Known);
			drinks.AddObject("beer", GPSRObjectType.Known);
			drinks.AddObject("coke", GPSRObjectType.Known);
			drinks.AddObject("sake", GPSRObjectType.Known, DifficultyDegree.Moderate);
			tmp.Add(drinks);

			SpecificLocation cupboard = SpecificLocation.Placement("cupboard");
			cupboard.Room = new Room("bathroom");
			Category toiletries = new Category("toiletries", cupboard);
			toiletries.AddObject("shampoo", GPSRObjectType.Known, DifficultyDegree.Moderate);
			toiletries.AddObject("soap", GPSRObjectType.Known);
			toiletries.AddObject("cloth", GPSRObjectType.Alike, DifficultyDegree.High);
			toiletries.AddObject("sponge", GPSRObjectType.Known, DifficultyDegree.High);
			tmp.Add(toiletries);

			SpecificLocation counter = SpecificLocation.Placement("counter");
			counter.Room = new Room("kitchen");
			Category containers = new Category("containers", counter);
			containers.AddObject("bowl", GPSRObjectType.Known, DifficultyDegree.High);
			containers.AddObject("tray", GPSRObjectType.Known, DifficultyDegree.High);
			containers.AddObject("plate", GPSRObjectType.Known, DifficultyDegree.High);
			tmp.Add(containers);

			return tmp;

		}

		/// <summary>
		/// Gets a list with predefined questions.
		/// </summary>
		/// <returns>A list with predefined questions.</returns>
		internal static List<PredefindedQuestion> GetDefaultQuestions()
		{
			List<PredefindedQuestion> q = new List<PredefindedQuestion>();
			q.Add(new PredefindedQuestion("Who invented the C programming language?", "Ken Thompson and Dennis Ritchie."));
			q.Add(new PredefindedQuestion("When was invented the C programming language?", "C was developed after B in 1972 at Bell Labs"));
			q.Add(new PredefindedQuestion("When was invented the B programming language?", "B was developed circa 1969 at Bell Labs"));
			q.Add(new PredefindedQuestion("Where does the term computer bug come from?", "From a moth trapped in a relay"));
			q.Add(new PredefindedQuestion("Who invented the first compiler?", "Grace Brewster Murray Hopper invented it"));
			q.Add(new PredefindedQuestion("Which robot is used in the Open Platform League?", "There is no standard defined for OPL"));
			q.Add(new PredefindedQuestion("Which robot is used in the Domestic Standard Platform League?", "The Toyota Human Support Robot"));
			q.Add(new PredefindedQuestion("Which robot is used in the Social Standard Platform League?", "The SoftBank Robotics Pepper"));
			q.Add(new PredefindedQuestion("How many people live in the Japan?", "A little over 127 million"));
			q.Add(new PredefindedQuestion("What are the colours of the Japanese flag?", "Japanese flag is a red circle centred over white"));
			q.Add(new PredefindedQuestion("What city is the capital of the Japan?", "Tokyo"));
			q.Add(new PredefindedQuestion("What is the highest point in Japan?", "The highest point in Japan is Mount Fuji, which reaches 3776m above sea level"));
			q.Add(new PredefindedQuestion("What is a Sakura?", "Sakura is the Japanese term for ornamental cherry blossom and its tree"));
			q.Add(new PredefindedQuestion("Who is the emperor of Japan?", "His Majesty Akihito sama is the emperor in Japan since January 7, 1989"));
			q.Add(new PredefindedQuestion("What's the name of your team?", "..."));
			q.Add(new PredefindedQuestion("What time is it?", "..."));
			q.Add(new PredefindedQuestion("What day is today?", "..."));
			q.Add(new PredefindedQuestion("Do you have dreams?", "I dream of Electric Sheep."));
			q.Add(new PredefindedQuestion("In which city will next year's RoboCup be hosted?", "It hasn't been announced yet."));
			return q;
		}
	}
}

