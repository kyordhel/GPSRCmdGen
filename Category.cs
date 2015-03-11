using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	[Serializable]
	public class Category : INameable
	{
		protected Location defaultLocation;
		protected List<GPSRObject> objects;

		public Category() : this("Unknown objects", Location.TrashBin){
		}

		public Category(string name, Location defaultLocation){
			this.Name = name;
			this.defaultLocation = defaultLocation;
			this.defaultLocation.IsPlacement = true;
			this.objects = new List<GPSRObject> ();
		}

		[XmlAttribute("name")]
		public string Name{get;set;}

		[XmlAttribute("defaultLocation")]
		public string LocationString {
			get {
				if (this.defaultLocation == null)
					return null;
				return this.defaultLocation.Name;
			}
			set {
				this.defaultLocation = new Location (value, true);
			}
		}

		[XmlIgnore]
		public Location DefaultLocation {
			get{ return this.defaultLocation;}
			set {
				this.defaultLocation = value;
                if(value != null)
				    this.defaultLocation.IsPlacement = true;
			}
		}

		//[XmlArray("objects")]
		//[XmlArrayItem("object")]
		[XmlElement("object")]
		public List<GPSRObject> Objects
		{
			get { return this.objects; }
			set {
				if(value == null) return;
				foreach (GPSRObject o in value)
					AddObject(o);
			}
		}

		public void AddObject(GPSRObject item){
			if (this.Objects.Contains (item))
				return;
			item.Category = this;
			this.Objects.Add (item);
		}
		

		public void AddObject(string name, GPSRObjectType type){
			DifficultyDegree tier;
			switch (type) {
				case GPSRObjectType.Alike:
					tier = DifficultyDegree.Moderate;
					break;

				case GPSRObjectType.Known:
					tier = DifficultyDegree.Easy;
					break;

				default:
				case GPSRObjectType.Unknown:
					tier = DifficultyDegree.Unknown;
					break;
			}
			AddObject (name, type, tier);
		}

		public void AddObject(string name, GPSRObjectType type, DifficultyDegree tier){
			GPSRObject o = new GPSRObject (name, type, tier);
			this.AddObject (o);
		}

		public override string ToString ()
		{
			return string.Format ("{0} [{2} Objects | {1} ]", Name, DefaultLocation.Name, Objects.Count);
		}
	}
}

