using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	/// <summary>
	/// Real-wolrd object type according to the RoboCup@Home Rulebook 2015
	/// </summary>
	[Serializable]
	public enum GPSRObjectType
	{
		/// <summary>
		/// The object is well known
		/// (Regular and there are no perceptible
		/// differences between two of the kind,
		/// such as two coke cans of the same brand and model).
		/// </summary>
		[XmlEnum("known")]
		Known = 0,
		/// <summary>
		/// Some object features are known
		/// (Irregular with perceptible differences between two of the kind,
		/// such as two apples or bannanas).
		/// </summary>
		[XmlEnum("alike")]
		Alike = 1,
		/// <summary>
		/// No object features are known, thus, it can not be identified
		/// </summary>
		[XmlEnum("unknown")]
		Unknown = -1
	}

	/// <summary>
	/// Represents an object in the real world according to the RoboCup@Home Rulebook 2015
	/// </summary>
	[Serializable]
	public class GPSRObject : INameable, ITiered
	{
		#region Variables

		/// <summary>
		/// Stores the category to which the object belongs
		/// </summary>
		private Category category;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.GPSRObject"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public GPSRObject():this("object"){
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.GPSRObject"/> class.
		/// </summary>
		/// <param name="name">The name of the object</param>
		public GPSRObject(string name) : 
		this(name, GPSRObjectType.Known, DifficultyDegree.Unknown){}

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.GPSRObject"/> class.
		/// </summary>
		/// <param name="name">The name of the object</param>
		/// <param name="type">The type of the object</param>
		/// <param name="tier">The difficulty degree for GRASPING the object</param>
		public GPSRObject(string name, GPSRObjectType type, DifficultyDegree tier){
			this.Name = name;
			this.Tier = tier;
			this.Type = type;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the object
		/// </summary>
		[XmlAttribute("name")]
		public string Name{get;set;}

		/// <summary>
		/// Gets or sets the type of the object.
		/// </summary>
		[XmlAttribute("type"), DefaultValue(GPSRObjectType.Known)]
		public GPSRObjectType Type{ get; set; }

		/// <summary>
		/// Gets or sets the  difficulty degree (tier) for GRASPING the gesture
		/// </summary>
		[XmlAttribute("difficulty"), DefaultValue(DifficultyDegree.Unknown)]
		public DifficultyDegree Tier{ get; set; }

		/// <summary>
		/// Gets or sets the category to which the object belongs
		/// </summary>
		[XmlIgnore]
		public Category Category{
			get{ return this.category; }
			set{
				if (this.category == value)
					return;
				this.category = value;
				if (value != null) {
					value.AddObject (this);
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="GPSRCmdGen.GPSRObject"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="GPSRCmdGen.GPSRObject"/>.</returns>
		public override string ToString()
		{
			return String.Format("{0} ({1}, {2})", this.Name, this.Type, this.Tier);
		}

		#endregion
	}
}

