using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	public class Gesture : INameable, ITiered
	{
		public Gesture () : this("none", DifficultyDegree.Easy){ }
		
		public Gesture (string name) : this(name, DifficultyDegree.Easy){ }

		public Gesture (string name, DifficultyDegree tier){
			this.Name = name;
			this.Tier = tier;
		}

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("difficulty"), DefaultValue(DifficultyDegree.Unknown)]
		public DifficultyDegree Tier{ get; set; }
	}
}

