using System;
using System.Collections.Generic;

namespace GPSRCmdGen
{


	public static class Factory
	{

		/*
			 * 
Go to the bedroom, find a person and tell the time.
Navigate to the kitchen, find a person and follow her.
Attend to the dinner-table, grasp the crackers, and take them to the side-table.
Go to the shelf, count the drinks and report to me.
Take this object and bring it to Susan at the hall.
Bring a coke to the person in the living room and answer him a question.
Offer a drink to the person at the door (robot needs to solve which drink will be delivered).

			*/

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

		public static List<Location> GetDefaultLocations ()
		{
			List<Location> locations = new List<Location>();
			locations.Add (new Location ("bath room", false));
			locations.Add (new Location ("bed room", false));
			locations.Add (new Location ("dining room", false));
			locations.Add (new Location ("hall", false));
			locations.Add (new Location ("kitchen", false));
			locations.Add (new Location ("corridor", false));
			return locations;
		}

		public static List<Name> GetDefaultNames ()
		{
			List<Name> names = new List<Name> ();

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
				names.Add(new Name(s, Gender.Female));

			foreach(string s in male)
				names.Add(new Name(s, Gender.Male));

			return names;
		}

		public static GPSRObjectManager GetDefaultObjects ()
		{
			GPSRObjectManager man = new GPSRObjectManager ();

			Location shelf = new Location ("shelf", true);
			Category beverages = new Category ("beverages", shelf);
			beverages.AddObject ("milk", GPSRObjectType.Known );
			beverages.AddObject ("coke", GPSRObjectType.Known );
			beverages.AddObject ("orange juice", GPSRObjectType.Known );
			beverages.AddObject ("beer", GPSRObjectType.Known, DifficultyDegree.High );
			man.Add (beverages);

			Location kitchenTable = new Location ("kitchen table", true);
			Category fruits = new Category ("fruits", kitchenTable);
			fruits.AddObject ("apple", GPSRObjectType.Alike );
			fruits.AddObject ("banana", GPSRObjectType.Alike );
			fruits.AddObject ("orange", GPSRObjectType.Alike );
			fruits.AddObject ("pear", GPSRObjectType.Alike );
			man.Add (fruits);

			Location dinnerTable = new Location ("dinner table", true);
			Category snacks = new Category ("", dinnerTable);
			snacks.AddObject ("lays", GPSRObjectType.Known, DifficultyDegree.Moderate );
			snacks.AddObject ("crackers", GPSRObjectType.Known );
			snacks.AddObject ("pringles", GPSRObjectType.Known );
			snacks.AddObject ("chocolate", GPSRObjectType.Known );
			man.Add (snacks);

			Location bathroomLocker = new Location ("bathroom locker", true);
			Category cleaningStuff = new Category ("cleaning stuff", bathroomLocker);
			cleaningStuff.AddObject ("cloth", GPSRObjectType.Alike, DifficultyDegree.High );
			cleaningStuff.AddObject ("detergent", GPSRObjectType.Known, DifficultyDegree.High);
			cleaningStuff.AddObject ("sponge", GPSRObjectType.Known );
			cleaningStuff.AddObject ("brush", GPSRObjectType.Known, DifficultyDegree.High );
			man.Add (cleaningStuff);


			return man;

		}
	}
}

