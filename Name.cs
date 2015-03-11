using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	public enum Gender{Female, Male}

	[Serializable]
	public class Name : INameable
	{
		private string value;

		public Name () : this(String.Empty, Gender.Female){}
		public Name (string value) : this(value, Gender.Female){}
		public Name (string value, Gender gender){ this.value = value; this.Gender = gender;}

		[XmlText]
		public string Value
		{
			get{ return this.value; }
			set{ this.value = value; }
		}

		[XmlIgnore]
		string INameable.Name { get { return this.Value; } }

		[XmlAttribute("gender"), DefaultValue(Gender.Female)]
		public Gender Gender{ get; set; }

		public override string ToString ()
		{
			return this.Value;
		}

		public static implicit operator Name(string s){
			return new Name (s);
		}
	}
}

