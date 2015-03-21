using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	/// <summary>
	/// Represents a place within a room on which a person can lie
	/// </summary>
	[Serializable, XmlRoot("beacon")]
	public class Beacon : Location
	{
		#region Variables

		/// <summary>
		/// Stores the room to which the beacon belongs
		/// </summary>
		private Room room;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Beacon"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public Beacon() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Beacon"/> class.
		/// </summary>
		/// <param name="name">The name of the location.</param>
		/// <param name="isBeacon">Flag indicating whether the location is
		/// suitable for placing objects.</param>
		public Beacon(string name) : base(name) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether the location is
		/// suitable for placing objects.
		/// </summary>
		public override bool IsPlacement { get { return false; } }

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
				{
					value.AddBeacon(this);
				}
			}
		}

		#endregion
	}
}
