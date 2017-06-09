using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	/// <summary>
	/// Represents a gesture performed by a human such as pointing, waving or rising arms
	/// in accordance with the RoboCup@Home Rulebook 2015
	/// </summary>
	public class Gesture : INameable, ITiered
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Gesture"/> class.
		/// </summary>
		/// <remarks>Intended fo serialiazation purposes only</remarks>
		public Gesture () : this("none", DifficultyDegree.Easy){ }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Gesture"/> class.
		/// </summary>
		/// <param name="name">The name of the gesture</param>
		public Gesture (string name) : this(name, DifficultyDegree.Easy){ }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Gesture"/> class.
		/// </summary>
		/// <param name="name">The name of the gesture</param>
		/// <param name="tier">The difficulty degree for DETECTING the gesture</param>
		public Gesture (string name, DifficultyDegree tier){
			this.Name = name;
			this.Tier = tier;
		}

		/// <summary>
		/// Gets or sets the name of the gesture
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the  difficulty degree (tier) for DETECTING the gesture
		/// </summary>
		[XmlAttribute("difficulty"), DefaultValue(DifficultyDegree.Unknown)]
		public DifficultyDegree Tier{ get; set; }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Gesture"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Gesture"/>.</returns>
		public override string ToString()
		{
			return String.Format("{0} ({1})", this.Name, this.Tier);
		}
	}
}

