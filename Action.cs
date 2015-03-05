using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	/// <summary>
	/// Encapsulates single actions which have requirements
	/// Actions may be:
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
	/// </summary>
	[Serializable]
	public class Action
	{
		public Action () : this("Leave", DifficultyDegree.None)
		{
			this.Requires = new List<Actor> ();
			this.Produces = new List<Actor> ();
			this.Consumes = new List<Actor> ();
		}

		public Action (string name, DifficultyDegree tier)
		{
			this.Name = name;
			this.Tier = tier;
		}

		[XmlAttribute("name")]
		public string Name{ get; set; }

		[XmlAttribute("difficulty")]
		public DifficultyDegree Tier{ get; set; }

		[XmlArray("requires")]
		[XmlAnyElement("actor")]
		public List<Actor> Requires{ get; set; }

		[XmlArray("produces")]
		[XmlAnyElement("actor")]
		public List<Actor> Produces{ get; set; }

		[XmlArray("consumes")]
		[XmlAnyElement("actor")]
		public List<Actor> Consumes{ get; set; }
	}
}

