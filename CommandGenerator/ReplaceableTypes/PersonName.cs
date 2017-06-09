using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	/// <summary>
	/// Represents a human gender
	/// </summary>
	public enum Gender{Female, Male}

	/// <summary>
	/// Represents a person name according to the RoboCup@Home Rulebook 2015
	/// </summary>
	[Serializable, XmlRoot("name")]
	public class PersonName : INameable
	{
		#region Variables

		/// <summary>
		/// Stores a person's name.
		/// </summary>
		private string name;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Name"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes</remarks>
		public PersonName () : this(String.Empty, Gender.Female){}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Name"/> class.
		/// </summary>
		/// <param name="value">The person's name</param>
		public PersonName (string value) : this(value, Gender.Female){}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Name"/> class.
		/// </summary>
		/// <param name="value">The person's name</param>
		/// <param name="gender">The person's gender</param>
		public PersonName (string value, Gender gender){ this.name = value; this.Gender = gender;}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the person's name this instance represents.
		/// </summary>
		[XmlText]
		public string Name
		{
			get{ return this.name; }
			set{ this.name = value; }
		}

		/// <summary>
		/// Gets the gender of the stored name.
		/// </summary>
		[XmlAttribute("gender"), DefaultValue(Gender.Female)]
		public Gender Gender{ get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Name"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Name"/>.</returns>
		public override string ToString ()
		{
			return this.Name;
		}

		/// <summary>
		/// Implicitly converts a string to a female person's name
		/// </summary>
		/// <param name="s">String to cast/param>
		public static implicit operator PersonName(string s){
			return new PersonName (s);
		}

		#endregion
	}
}

