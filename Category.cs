using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	/// <summary>
	/// Represents a real-wolrd object category
	/// according to the RoboCup@Home Rulebook 2015
	/// </summary>
	[Serializable]
	public class Category : INameable
	{
		#region Variables
		/// <summary>
		/// Stores the default placement default location for objects in the category
		/// </summary>
		protected Placement defaultLocation;

		/// <summary>
		/// Stores the list of objects in the category
		/// </summary>
		protected List<GPSRObject> objects;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Category"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes</remarks>
		public Category() : this("Unknown objects", Placement.TrashBin){
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Category"/> class.
		/// </summary>
		/// <param name="name">The name of the category.</param>
		/// <param name="defaultLocation">The default placement location for objects in the category.</param>
		public Category(string name, Placement defaultLocation){
			this.Name = name;
			this.defaultLocation = defaultLocation;
			this.objects = new List<GPSRObject> ();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the Category
		/// </summary>
		[XmlAttribute("name")]
		public string Name{get;set;}

		/// <summary>
		/// Gets or sets the name of the default location for objects in the category
		/// </summary>
		/// <remarks>Use for (de)serialization purposes only</remarks>
		[XmlAttribute("defaultLocation")]
		public string LocationString {
			get {
				if (this.defaultLocation == null)
					return null;
				return this.defaultLocation.Name;
			}
			set {
				if (this.defaultLocation == null)
					this.defaultLocation = new Placement(value);
				else
					this.defaultLocation.Name = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the default location's room for objects in the category
		/// </summary>
		/// <remarks>Use for (de)serialization purposes only</remarks>
		[XmlAttribute("room")]
		public string RoomString
		{
			get
			{
				if (this.defaultLocation == null)
					return null;
				return this.defaultLocation.Room.Name;
			}
			set
			{
				if(this.defaultLocation == null)
					this.defaultLocation = new Placement("unknown");
				this.defaultLocation.Room = new Room(value);
			}
		}

		/// <summary>
		/// Gets or sets the default location for objects in the category
		/// </summary>
		[XmlIgnore]
		public Placement DefaultLocation {
			get{ return this.defaultLocation;}
			set {
				this.defaultLocation = value;
			}
		}

		/// <summary>
		/// Gets or sets the list of objects in the category.
		/// </summary>
		/// <remarks>Use for (de)serialization purposes only</remarks>
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

		#endregion

		#region Methods

		/// <summary>
		/// Adds an object to the category
		/// </summary>
		/// <param name="item">The ibject to add to the category</param>
		public void AddObject(GPSRObject item){
			if (this.Objects.Contains (item))
				return;
			item.Category = this;
			this.Objects.Add (item);
		}
		
		/// <summary>
		/// Adds an object to the category
		/// </summary>
		/// <param name="name">The name of the object to add.</param>
		/// <param name="type">The type of the object to add.</param>
		/// <remarks>Object difficulty degree is set automatically
		/// based on type as follows:
		/// Alike -> Moderate /
		/// Known -> Easy
		/// Unknown -> Unknown</remarks>
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

		/// <summary>
		/// Adds an object to the category
		/// </summary>
		/// <param name="name">The name of the object to add.</param>
		/// <param name="type">The type of the object to add.</param>
		/// <param name="type">The difficulty degree of the object to add.</param>
		public void AddObject(string name, GPSRObjectType type, DifficultyDegree tier){
			GPSRObject o = new GPSRObject (name, type, tier);
			this.AddObject (o);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="GPSRCmdGen.Category"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="GPSRCmdGen.Category"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("{0} [{2} Objects | {1} ]", Name, DefaultLocation.Name, Objects.Count);
		}

		#endregion
	}
}

