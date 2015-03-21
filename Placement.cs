using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	/// <summary>
	/// Represents a place within a room on which an object can lie
	/// </summary>
	[Serializable, XmlRoot("placement")]
	public class Placement : Location
	{
		#region Variables

		/// <summary>
		/// Stores the room to which the placement belongs
		/// </summary>
		private Room room;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Placement"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public Placement() : base() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.Placement"/> class.
		/// </summary>
		/// <param name="name">The name of the location.</param>
		/// <param name="isPlacement">Flag indicating whether the location is
		/// suitable for placing objects.</param>
		public Placement(string name) : base(name) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether the location is
		/// suitable for placing objects.
		/// </summary>
		public override bool IsPlacement { get { return true; } }

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
					value.AddPlacement(this);
				}
			}
		}

		/// <summary>
		/// Gets a placement location called bin
		/// </summary>
		public static Placement TrashBin { get { return new Placement("bin"); } }

		#endregion
	}
}
