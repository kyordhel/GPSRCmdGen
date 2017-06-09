using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	[Serializable, XmlRoot("location")]
	[XmlInclude(typeof(Room)), XmlInclude(typeof(SpecificLocation)), XmlInclude(typeof(SpecificLocation))]
	public abstract class Location : INameable, IComparable<Location>, IEquatable<Location>
	{

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Location"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public Location() : this(String.Empty) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Location"/> class.
		/// </summary>
		/// <param name="name">The name of the location.</param>
		public Location(string name) : this(name, false, false)
		{
			this.Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Location"/> class.
		/// </summary>
		/// <param name="name">The name of the location.</param>
		/// <param name="placement">Indicates whether the location is
		/// suitable for placing objects.</param>
		/// <param name="beacon">Indicates whether the location is
		/// suitable for placing objects.</param>
		public Location(string name, bool placement, bool beacon)
		{
			this.Name = name;
			this.IsPlacement = placement;
			this.IsBeacon = beacon;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the location
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets a value indicating whether the location is
		/// suitable for placing a person.
		/// </summary>
		[XmlIgnore]
		public abstract bool IsBeacon { get; set; }

		/// <summary>
		/// Gets a value indicating whether the location is
		/// suitable for placing objects.
		/// </summary>
		[XmlIgnore]
		public abstract bool IsPlacement { get; set; }

		public int CompareTo(Location other)
		{
			if (other == null)
				return -1;
			return String.Compare(this.Name, other.Name, true);
		}

		/// <summary>
		/// Determines whether the specified <see cref="RoboCup.AtHome.CommandGenerator.Location"/> is equal to the current <see cref="RoboCup.AtHome.CommandGenerator.Location"/> by comparing their names.
		/// </summary>
		/// <param name="other">The <see cref="RoboCup.AtHome.CommandGenerator.Location"/> to compare with the current <see cref="RoboCup.AtHome.CommandGenerator.Location"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="RoboCup.AtHome.CommandGenerator.Location"/> is equal to the current
		/// <see cref="RoboCup.AtHome.CommandGenerator.Location"/>; otherwise, <c>false</c>.</returns>
		public bool Equals(Location other)
		{
			return String.Compare(this.Name, other.Name, true) == 0;
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
		/// Serves as a hash function for a <see cref="RoboCup.AtHome.CommandGenerator.Location"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Location"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Location"/>.</returns>
		public override string ToString()
		{
			return this.Name;
		}

		#endregion

		#region Static Members

		/// <summary>
		/// Compares two locations for equality based on the values of their names
		/// </summary>
		/// <param name="a">A location.</param>
		/// <param name="b">A location.</param>
		public static bool operator ==(Location a, Location b)
		{
			if (Object.Equals(a, null) && Object.Equals(b, null))
				return true;
			if (Object.Equals(a, null) || Object.Equals(b, null))
				return false;
			return String.Compare(a.Name, b.Name, true) == 0;
		}

		/// <summary>
		/// Compares two locations for inequality based on the values of their names
		/// </summary>
		/// <param name="a">A location.</param>
		/// <param name="b">A location.</param>
		public static bool operator !=(Location a, Location b)
		{
			if (Object.Equals(a, null) && Object.Equals(b, null))
				return false;
			if (Object.Equals(a, null) || Object.Equals(b, null))
				return true;
			return String.Compare(a.Name, b.Name, true) != 0;
		}

		#endregion
	}
}

