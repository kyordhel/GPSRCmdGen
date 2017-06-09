using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	/// <summary>
	/// Represents a place within a room on which an object can lie
	/// </summary>
	[Serializable, XmlRoot("location")]
	public class SpecificLocation : Location, IEquatable<SpecificLocation>
	{
		#region Variables

		/// <summary>
		/// Stores the room to which the placement belongs
		/// </summary>
		private Room room;

		/// <summary>
		///Indicates whether the location is suitable for placing a person.
		/// </summary>
		private bool isBeacon;

		/// <summary>
		/// Indicates whether the location is suitable for placing objects.
		/// </summary>
		private bool isPlacement;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Placement"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public SpecificLocation() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Placement"/> class.
		/// </summary>
		/// <param name="name">The name of the location.</param>
		/// <param name="placement">Indicates whether the location is
		/// suitable for placing objects.</param>
		/// <param name="beacon">Indicates whether the location is
		/// suitable for placing objects.</param>
		public SpecificLocation(string name, bool placement, bool beacon) : base(name, placement, beacon) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether the location is suitable for placing a person.
		/// </summary>
		[XmlAttribute("isBeacon"), DefaultValue(false)]
		public override bool IsBeacon
		{
			get { return this.isBeacon; }
			set { this.isBeacon = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the location is suitable for placing objects.
		/// </summary>
		[XmlAttribute("isPlacement"), DefaultValue(false)]
		public override bool IsPlacement
		{
			get { return this.isPlacement; }
			set { this.isPlacement = value; }
		}

		/// <summary>
		/// Gets or sets the room to which the placement belongs
		/// </summary>
		[XmlIgnore]
		public Room Room
		{
			get { return this.room; }
			set
			{
				if (this.room == value)
					return;
				this.room = value;
				if (value != null)
					value.AddLocation(this);
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="RoboCup.AtHome.CommandGenerator.SpecificLocation"/> is equal to the current <see cref="RoboCup.AtHome.CommandGenerator.SpecificLocation"/> by comparing their names.
		/// </summary>
		/// <param name="other">The <see cref="RoboCup.AtHome.CommandGenerator.SpecificLocation"/> to compare with the current <see cref="RoboCup.AtHome.CommandGenerator.SpecificLocation"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="RoboCup.AtHome.CommandGenerator.SpecificLocation"/> is equal to the current
		/// <see cref="RoboCup.AtHome.CommandGenerator.Location"/>; otherwise, <c>false</c>.</returns>
		public bool Equals(SpecificLocation other)
		{
			return String.Compare(this.Name, other.Name, true) == 0;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="RoboCup.AtHome.CommandGenerator.Location"/> at object level.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="RoboCup.AtHome.CommandGenerator.Location"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="RoboCup.AtHome.CommandGenerator.Location"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Creates a new instance of SpecificLocation with the given name and the properties
		/// IsBeacon=true, IsPlacement=false
		/// </summary>
		/// <param name="name">The name of the beacon location</param>
		/// <returns>A Specific Location</returns>
		public static SpecificLocation Beacon(string name) { return new SpecificLocation(name, false, true); }

		/// <summary>
		/// Creates a new instance of SpecificLocation with the given name and the properties
		/// IsBeacon=false, IsPlacement=true
		/// </summary>
		/// <param name="name">The name of the placement location</param>
		/// <returns>A Specific Location</returns>
		public static SpecificLocation Placement(string name) { return new SpecificLocation(name, true, false); }


		#endregion
	}
}
