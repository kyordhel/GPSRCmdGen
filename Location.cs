using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	[XmlRoot("location")]
	public class Location : INameable, IComparable<Location>, IEquatable<Location>
	{
		public Location()
			: this(String.Empty, false)
		{
		}

		public Location(string name, bool isPlacement)
		{
			this.Name = name;
			this.IsPlacement = isPlacement;
		}

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("isPlacement"), DefaultValue(false)]
		public bool IsPlacement { get; set; }

		public int CompareTo(Location other)
		{
			if (other == null)
				return -1;
			return String.Compare(this.Name, other.Name, true);
		}

		public bool Equals(Location other)
		{
			return String.Compare(this.Name, other.Name, true) == 0;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return this.Name;
		}

		public static Location TrashBin { get { return new Location("bin", true); } }

		public static bool operator ==(Location a, Location b)
		{
			if (Object.Equals(a, null) && Object.Equals(b, null))
				return true;
			if (Object.Equals(a, null) || Object.Equals(b, null))
				return false;
			return String.Compare(a.Name, b.Name, true) == 0;
		}

		public static bool operator !=(Location a, Location b)
		{
			if (Object.Equals(a, null) && Object.Equals(b, null))
				return false;
			if (Object.Equals(a, null) || Object.Equals(b, null))
				return true;
			return String.Compare(a.Name, b.Name, true) != 0;
		}
	}
}

