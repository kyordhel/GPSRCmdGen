using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	[Serializable]
	public enum GPSRObjectType
	{
		Known = 0,
		Alike = 1,
		Unknown = -1
	}

	[Serializable]
	public class GPSRObject : INameable, ITiered
	{
		public GPSRObject():this("object"){
		}

		public GPSRObject(string name) : 
		this(name, GPSRObjectType.Known, DifficultyDegree.Unknown){}

		public GPSRObject(string name, GPSRObjectType type, DifficultyDegree tier){
			this.Name = name;
			this.Tier = tier;
			this.Type = type;
		}

		[XmlAttribute("name")]
		public string Name{get;set;}

		[XmlAttribute("type"), DefaultValue(GPSRObjectType.Known)]
		public GPSRObjectType Type{ get; set; }

		[XmlAttribute("difficulty"), DefaultValue(DifficultyDegree.Unknown)]
		public DifficultyDegree Tier{ get; set; }

		[XmlIgnore]
		public Category Category{ get; set;}

		public override string ToString()
		{
			return String.Format("{0} ({1}, {2})", this.Name, this.Type, this.Tier);
		}
	}
}

