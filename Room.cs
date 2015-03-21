using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	/// <summary>
	/// Represents a room within the house
	/// </summary>
	[Serializable, XmlRoot("room")]
	public class Room : Location, IEquatable<Room>
	{
		#region Variables

		/// <summary>
		/// Stores the list of beacons in this room
		/// </summary>
		protected List<Beacon> beacons;

		/// <summary>
		/// Stores the list of placements in this room
		/// </summary>
		protected List<Placement> placements;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Room"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public Room() : this(String.Empty) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Room"/> class.
		/// </summary>
		/// <param name="name">The name of the location.</param>
		/// <param name="isPlacement">Flag indicating whether the location is
		/// suitable for placing objects.</param>
		public Room(string name) : base(name) {
			this.beacons = new List<Beacon>();
			this.placements = new List<Placement>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the list of beacons in the room.
		/// </summary>
		/// <remarks>Use for (de)serialization purposes only</remarks>
		[XmlElement("beacon")]
		public List<Beacon> Beacons
		{
			get { return this.beacons; }
			set
			{
				if (value == null) return;
				foreach (Beacon b in value)
					AddBeacon(b);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the location is
		/// suitable for placing objects.
		/// </summary>
		public override bool IsPlacement { get { return false; } }

		/// <summary>
		/// Gets or sets the list of placements in the room.
		/// </summary>
		/// <remarks>Use for (de)serialization purposes only</remarks>
		[XmlElement("placement")]
		public List<Placement> Placements
		{
			get { return this.placements; }
			set
			{
				if (value == null) return;
				foreach (Placement p in value)
					AddPlacement(p);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a beacon to the room
		/// </summary>
		/// <param name="item">The beacon to add to the room.</param>
		public void AddBeacon(Beacon item)
		{
			if (this.Beacons.Contains(item))
				return;
			item.Room = this;
			this.Beacons.Add(item);
		}

		/// <summary>
		/// Adds an beacon to the room
		/// </summary>
		/// <param name="name">The name of the beacon to add.</param>
		public void AddBeacon(string name)
		{
			Beacon b = new Beacon(name);
			this.AddBeacon(b);
		}

		/// <summary>
		/// Adds a placement to the room
		/// </summary>
		/// <param name="item">The placement to add to the room.</param>
		public void AddPlacement(Placement item)
		{
			if (this.Placements.Contains(item))
				return;
			item.Room = this;
			this.Placements.Add(item);
		}

		/// <summary>
		/// Adds an beacon to the room
		/// </summary>
		/// <param name="name">The name of the placement to add.</param>
		public void AddPlacement(string name)
		{
			Placement p = new Placement(name);
			this.AddPlacement(p);
		}

		/// <summary>
		/// Determines whether the specified <see cref="GPSRCmdGen.Room"/> is equal to the current <see cref="GPSRCmdGen.Room"/> by comparing their names.
		/// </summary>
		/// <param name="other">The <see cref="GPSRCmdGen.Location"/> to compare with the current <see cref="GPSRCmdGen.Location"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="GPSRCmdGen.Room"/> is equal to the current
		/// <see cref="GPSRCmdGen.Room"/>; otherwise, <c>false</c>.</returns>
		public bool Equals(Room other)
		{
			return String.Compare(this.Name, other.Name, true) == 0;
		}

		#endregion
	}
}
