using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using PersonName = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.PersonName;

namespace RoboCup.AtHome.CommandGenerator.Containers
{
	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing names  
	/// </summary>
	[XmlRoot(ElementName = "names", Namespace = "")]
	public class NameContainer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.NameContainer"/> class.
		/// </summary>
		public NameContainer() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.NameContainer"/> class.
		/// </summary>
		/// <param name="names">List of names.</param>
		public NameContainer(List<PersonName> names) { this.Names = names; }

		/// <summary>
		/// Gets or sets the list of names.
		/// </summary>
		[XmlElement("name")]
		public List<PersonName> Names { get; set; }
	}
}
