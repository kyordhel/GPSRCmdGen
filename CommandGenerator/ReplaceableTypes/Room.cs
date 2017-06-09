using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	/// <summary>
	/// Represents a room within the house
	/// </summary>
	[Serializable, XmlRoot("room")]
	public class Room : Location
	{
		#region Variables

		/// <summary>
		/// Stores the list of placements in this room
		/// </summary>
		protected Dictionary<string, SpecificLocation> locations;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Room"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public Room() : this(String.Empty) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Room"/> class.
		/// </summary>
		/// <param name="name">The name of the location.</param>
		/// <param name="isPlacement">Flag indicating whether the location is
		/// suitable for placing objects.</param>
		public Room(string name) : base(name) {
			this.locations = new Dictionary<string, SpecificLocation>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether the location is suitable for placing a person.
		/// </summary>
		[XmlIgnore]
		public override bool IsBeacon { get { return false; } set { } }

		/// <summary>
		/// Gets a value indicating whether the location is suitable for placing objects.
		/// </summary>
		[XmlIgnore]
		public override bool IsPlacement { get { return false; } set { } }

		/// <summary>
		/// Gets or sets the list of placements in the room.
		/// </summary>
		/// <remarks>Use for (de)serialization purposes only</remarks>
		[XmlElement("location")]
		public SpecificLocation[] Locations
		{
			get { return new List<SpecificLocation>(this.locations.Values).ToArray(); }
			set
			{
				if (value == null) return;
				foreach (SpecificLocation location in value)
					AddLocation(location);
			}
		}

		/// <summary>
		/// Returns the number of locations in the room
		/// </summary>
		public int LocationCount { get { return this.locations.Count; } }

		#endregion

		#region Methods

		/// <summary>
		/// Adds an beacon to the room
		/// </summary>
		/// <param name="name">The name of the beacon to add.</param>
		public void AddBeacon(string name)
		{
			SpecificLocation b = new SpecificLocation(name, false, true);
			this.AddLocation(b);
		}

		/// <summary>
		/// Adds a specific location to the room
		/// </summary>
		/// <param name="item">The specific location to add to the room.</param>
		public void AddLocation(SpecificLocation item)
		{
			if(item == null)
				return;
			if ((item.Room != null) && (item.Room != this))
				item.Room.RemoveLocation(item);
			if (this.locations.ContainsKey(item.Name))
			{
				this.locations[item.Name].IsBeacon |= item.IsBeacon;
				this.locations[item.Name].IsPlacement |= item.IsPlacement;
			}
			else
				this.locations.Add(item.Name, item);
			if (item.Room != this)
				item.Room = this;
		}

		/// <summary>
		/// Adds a specific location to the room
		/// </summary>
		/// <param name="name">The name of the location.</param>
		/// <param name="placement">Indicates whether the location is
		/// suitable for placing objects.</param>
		/// <param name="beacon">Indicates whether the location is
		/// suitable for placing objects.</param>
		public void AddLocation(string name, bool placement, bool beacon)
		{
			AddLocation(new SpecificLocation(name, placement, beacon));
		}

		/// <summary>
		/// Adds an placement to the room
		/// </summary>
		/// <param name="name">The name of the placement to add.</param>
		public void AddPlacement(string name)
		{
			SpecificLocation p = new SpecificLocation(name, true, false);
			this.AddLocation(p);
		}

		/// <summary>
		/// Gets a value indicating if the room contains a location with the given name
		/// </summary>
		/// <param name="locationName">The name of the room to look for</param>
		/// <returns>true if the collection contains a room with the given name, false otherwise</returns>
		public bool Contains(string locationName)
		{
			return this.locations.ContainsKey(locationName);
		}

		/// <summary>
		/// Removes all locations
		/// </summary>
		public void Clear()
		{
			this.locations.Clear();
		}

		/// <summary>
		/// Removes the given location from the room
		/// </summary>
		/// <param name="item">The room to remove</param>
		/// <returns>true if the room was in the collection, false otherwise</returns>
		private bool RemoveLocation(SpecificLocation item)
		{
			if (item == null)
				return false;
			return RemoveLocation(item.Name);
		}

		/// <summary>
		/// Removes the given location from the room
		/// </summary>
		/// <param name="locationName">The name of the room to remove</param>
		/// <returns>true if the room was in the collection, false otherwise</returns>
		private bool RemoveLocation(string locationName)
		{
			return this.locations.Remove(locationName);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Room"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Room"/>.</returns>
		public override string ToString()
		{
			return string.Format("{0} ({1} locations)", Name, LocationCount);
		}

		#endregion
	}
}
