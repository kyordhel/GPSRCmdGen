using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
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
		protected SpecificLocation defaultLocation;

		/// <summary>
		/// Stores the list of objects in the category
		/// </summary>
		protected Dictionary<string, Object> objects;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Category"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes</remarks>
		public Category() : this("Unknown objects", null){
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Category"/> class.
		/// </summary>
		/// <param name="name">The name of the category.</param>
		/// <param name="defaultLocation">The default placement location for objects in the category.</param>
		public Category(string name, SpecificLocation defaultLocation){
			this.Name = name;
			this.defaultLocation = defaultLocation;
			this.objects = new Dictionary<string, Object>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns the number of objects in the category
		/// </summary>
		public int ObjectCount { get { return this.objects.Count; } }

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
					this.defaultLocation = new SpecificLocation(value, true, false);
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
					this.defaultLocation = new SpecificLocation("unknown", true, false);
				this.defaultLocation.Room = new Room(value);
			}
		}

		/// <summary>
		/// Gets or sets the default location for objects in the category
		/// </summary>
		[XmlIgnore]
		public SpecificLocation DefaultLocation {
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
		public Object[] Objects
		{
			get { return new List<Object>(this.objects.Values).ToArray(); }
			set {
				if(value == null) return;
				foreach (Object o in value)
					AddObject(o);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds an object to the category
		/// </summary>
		/// <param name="item">The ibject to add to the category</param>
		public void AddObject(Object item){
			if (item == null)
				return;
			if ((item.Category != null) && (item.Category != this))
				item.Category.RemoveObject(item);
			if (!this.objects.ContainsKey(item.Name))
				this.objects.Add(item.Name, item);
			if (item.Category != this)
				item.Category = this;
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
		public void AddObject(string name, ObjectType type){
			DifficultyDegree tier;
			switch (type) {
				case ObjectType.Alike:
					tier = DifficultyDegree.Moderate;
					break;

				case ObjectType.Known:
					tier = DifficultyDegree.Easy;
					break;

				default:
				case ObjectType.Unknown:
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
		public void AddObject(string name, ObjectType type, DifficultyDegree tier){
			Object o = new Object (name, type, tier);
			this.AddObject (o);
		}

		/// <summary>
		/// Gets a value indicating if the category contains an object with the given name
		/// </summary>
		/// <param name="objectName">The name of the object to look for</param>
		/// <returns>true if the category contains a object with the given name, false otherwise</returns>
		public bool Contains(string objectName)
		{
			return this.objects.ContainsKey(objectName);
		}

		/// <summary>
		/// Removes all objects
		/// </summary>
		public void Clear()
		{
			this.objects.Clear();
		}

		/// <summary>
		/// Removes the given GPSRObject from the Category
		/// </summary>
		/// <param name="item">The GPSRObject to remove</param>
		/// <returns>true if the GPSRObject was in the collection, false otherwise</returns>
		private bool RemoveObject(Object item)
		{
			if (item == null)
				return false;
			return RemoveObject(item.Name);
		}

		/// <summary>
		/// Removes the given GPSRObject from the room
		/// </summary>
		/// <param name="objectName">The name of the GPSRObject to remove</param>
		/// <returns>true if the GPSRObject was in the collection, false otherwise</returns>
		private bool RemoveObject(string objectName)
		{
			return this.objects.Remove(objectName);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Category"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Category"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("{0} [{2} Objects | {1} ]", Name, DefaultLocation.Name, ObjectCount);
		}

		#endregion
	}
}

