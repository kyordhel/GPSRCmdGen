using System;
using System.Collections.Generic;
using System.Text;

namespace GPSRCmdGen
{
	public class Task
	{
		private List<Token> tokens;

		public Task (){
			this.tokens = new List<Token>();
		}

		public Task (List<Token> tokens)
		{
			if(tokens == null)return;
			this.tokens = new List<Token>(tokens);
		}

		public List<Token> Tokens
		{
			get { return this.tokens; }
			internal set { this.tokens = value; }
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if(tokens.Count > 0)
			sb.Append(tokens[0].StringValue);
			for (int i = 1; i < tokens.Count; ++i)
			{
				sb.Append(tokens[i].StringValue);
			}
			return sb.ToString();
		}

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

