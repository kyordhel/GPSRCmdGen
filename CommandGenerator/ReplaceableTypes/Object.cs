using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	/// <summary>
	/// Real-wolrd object type according to the RoboCup@Home Rulebook 2015
	/// </summary>
	[Serializable]
	public enum ObjectType
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
		/// Special objects for special tests such as bags
		/// </summary>
		[XmlEnum("special")]
		Special = 2,
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
	public class Object : INameable, ITiered, IDescribable
	{
		#region Variables

		/// <summary>
		/// Stores the category to which the object belongs
		/// </summary>
		private Category category;

		/// <summary>
		/// Stores the properties that describe the object.
		/// </summary>
		private Dictionary<string, string> properties;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.GPSRObject"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes only</remarks>
		public Object():this("object"){
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.GPSRObject"/> class.
		/// </summary>
		/// <param name="name">The name of the object</param>
		public Object(string name) : 
		this(name, ObjectType.Known, DifficultyDegree.Unknown){}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.GPSRObject"/> class.
		/// </summary>
		/// <param name="name">The name of the object</param>
		/// <param name="type">The type of the object</param>
		/// <param name="tier">The difficulty degree for GRASPING the object</param>
		public Object(string name, ObjectType type, DifficultyDegree tier){
			this.Name = name;
			this.Tier = tier;
			this.Type = type;
			this.properties = new Dictionary<string, string>();
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
		[XmlAttribute("type"), DefaultValue(ObjectType.Known)]
		public ObjectType Type{ get; set; }

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

		/// <summary>
		/// Gets the properties that describe the object.
		/// </summary>
		/// <value>Dictionary of properties of the object.</value>
		[XmlIgnore]
		public Dictionary<string, string> Properties{ get { return this.properties; } }

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether this instance has a property with the specified propertyName.
		/// </summary>
		/// <returns><c>true</c> if this instance has property with the specified propertyName; otherwise, <c>false</c>.</returns>
		/// <param name="propertyName">Property name.</param>
		public bool HasProperty(string propertyName){ return this.Properties.ContainsKey(propertyName); }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.GPSRObject"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.GPSRObject"/>.</returns>
		public override string ToString()
		{
			return String.Format("{0} ({1}, {2})", this.Name, this.Type, this.Tier);
		}

		#endregion
	}
}

