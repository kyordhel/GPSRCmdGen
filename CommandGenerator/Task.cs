using System;
using System.Collections.Generic;
using System.Text;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Represents a task randomly generated from a grammar.
	/// The task is composed by list of tokens (original strings or
	/// wildcards with their replacements and metadata).
	/// </summary>
	public class Task
	{
		/// <summary>
		/// Stores the list of grammar's tokens
		/// </summary>
		private List<Token> tokens;

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Task"/> class.
		/// </summary>
		public Task (){
			this.tokens = new List<Token>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Task"/> class.
		/// </summary>
		/// <param name="tokens">The list of tokens to be used to build the task</param>
		public Task (List<Token> tokens)
		{
			if(tokens == null)return;
			this.tokens = new List<Token>(tokens);
		}

		/// <summary>
		/// Gets the list of tokens that compose the task.
		/// </summary>
		public List<Token> Tokens
		{
			get { return this.tokens; }
			internal set { this.tokens = value; }
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Task"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Task"/>.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < tokens.Count; ++i)
				sb.Append (tokens [i].Name);
			string s = sb.ToString ();
			while(s.Contains("  "))
				s = s.Replace ("  ", " ");
			s = s.Replace (" ,", ",");
			s = s.Replace (" ;", ";");
			s = s.Replace (" .", ".");
			s = s.Replace (" :", ":");
			s = s.Replace (" ?", "?");
			return s;
		}

		
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

			/// 	answering,
			/// 	counting,
			/// 	finding,
			/// 	following,
			/// 	grasping,
			/// 	handling,
			/// 	navigating,
			/// 	opening,
			/// 	pouring,
			/// 	retrieving,
			/// 	saying
			/*
			 *********************************************************
			 * Count
			 *********************************************************
			 * Count the (ObjectCategory|AlikeObjects) at the (PlacementLocation)...
			 * Count the (People|PeopleByGender|PeopleByGesture) at the (Room)...
			 * ...and report to (me|Name (at|in|which is in|) the (Room).
			 * 
			 * (go|navigate) to the (PlacementLocation), count the (ObjectCategory|AlikeObjects)...
			 * (go|navigate) to the (Room) count the (People|PeopleByGender|PeopleByGesture)...
			 * ...and report to (me|Name (at|in|which is in) the (Room)).
			 * 
			 * Tell (me|to Name (at|in|which is in) the (Room))...
			 * ...how many (ObjectCategory|AlikeObjects) are in the (PlacementLocation).
			 * ...how many (People|PeopleByGender|PeopleByGesture) are in the (Room).
			 * 
			 * Grammar:
			 * $count  = $count1 | $count2 | $count3
			 * 
			 * $count1 = count the $cntxat $report
			 * $cntxat = $cntoat | $cntpat
			 * $cntoat = $object at the $PlacementLocation
			 * $cntpat = $people at the $Room
			 * 
			 * $count2 = $navigt $docntx $report 
			 * $navigt = $GoVerb to the 
			 * $docntx = $docnto | $docntp
			 * $docnto = $PlacementLocation, count the $object
			 * $docntp = $Room, count the $people
			 * 
			 * $count3 = Tell $target how many $ctable
			 * $ctable = $objain | $pplain
			 * $objain = $object are in the $PlacementLocation
			 * $pplain = $people are in the $Room
			 * 
			 * $object = objects | $ObjectCategory | $AlikeObjects
			 * $people = people | $PeopleByGender | $PeopleByGesture
			 * $report = and report to $target
			 * $target = me | ($Name (at | in | which is in) the $Room)
			 * 
			 * 
			 * 
			 *********************************************************
			 * 
			 *********************************************************
			 */
	}


}

