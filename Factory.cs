using System;
using System.Collections.Generic;

namespace GPSRCmdGen
{
	public static class Factory
	{
		public static List<Action> GetDefaultActions ()
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
			Action action;
			List<Action> actions = new List<Action>();

			// 	answering
			action = new Action ("answer", DifficultyDegree.Easy);
			action.Requires.Add (Actor.Operator);
			actions.Add (action);

			// 	counting,
			action = new Action ("count people", DifficultyDegree.High);
			action.Requires.Add (Actor.Crowd);
			action.Requires.Add (Actor.Location);
			actions.Add (action);

			action = new Action ("count objects", DifficultyDegree.High);
			action.Requires.Add (Actor.LocationPlacement);
			actions.Add (action);

			// 	finding,
			action = new Action ("find person", DifficultyDegree.Easy);
			action.Requires.Add (Actor.Location);
			actions.Add (action);

			/// 	following
			action = new Action ("follow", DifficultyDegree.Moderate);
			action.Requires.Add (Actor.Operator);
			actions.Add (action);

			/// 	grasping,
			/// 	handling,
			/// 	navigating,
			action = new Action ("go place", DifficultyDegree.Easy);
			action.Requires.Add (Actor.Location);
			actions.Add (action);

			action = new Action ("go person", DifficultyDegree.Easy);
			action.Requires.Add (Actor.Location);
			actions.Add (action);

			/// 	opening,
			/// 	pouring,
			/// 	retrieving,
			/// 	saying

			return actions;
		}

		public static List<Location> GetDefaultLocations ()
		{
			return new List<Location>();
		}

		public static List<Name> GetDefaultNames ()
		{
			return new List<Name>();
		}

		public static List<GPSRObject> GetDefaultObjects ()
		{
			return new List<GPSRObject>();
		}
	}
}

