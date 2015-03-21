using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	[Serializable, XmlRoot("location")]
	[XmlInclude(typeof(Room)), XmlInclude(typeof(Placement)), XmlInclude(typeof(Beacon))]
	public abstract class Location : INameable, IComparable<Location>, IEquatable<Location>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Location"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public Location() : this(String.Empty) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Location"/> class.
		/// </summary>
		/// <param name="name">The name of the location.</param>
		public Location(string name)
		{
			this.Name = name;
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
		/// suitable for placing objects.
		/// </summary>
		public abstract bool IsPlacement { get; }

		public int CompareTo(Location other)
		{
			if (other == null)
				return -1;
			return String.Compare(this.Name, other.Name, true);
		}

		/// <summary>
		/// Determines whether the specified <see cref="GPSRCmdGen.Location"/> is equal to the current <see cref="GPSRCmdGen.Location"/> by comparing their names.
		/// </summary>
		/// <param name="other">The <see cref="GPSRCmdGen.Location"/> to compare with the current <see cref="GPSRCmdGen.Location"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="GPSRCmdGen.Location"/> is equal to the current
		/// <see cref="GPSRCmdGen.Location"/>; otherwise, <c>false</c>.</returns>
		public bool Equals(Location other)
		{
			return String.Compare(this.Name, other.Name, true) == 0;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="GPSRCmdGen.Location"/> at object level.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="GPSRCmdGen.Location"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="GPSRCmdGen.Location"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="GPSRCmdGen.Location"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="GPSRCmdGen.Location"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="GPSRCmdGen.Location"/>.</returns>
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

