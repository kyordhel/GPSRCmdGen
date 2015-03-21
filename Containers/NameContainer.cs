using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen.Containers
{
	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing names  
	/// </summary>
	[XmlRoot(ElementName = "names", Namespace = "")]
	public class NameContainer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.NameContainer"/> class.
		/// </summary>
		public NameContainer() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.NameContainer"/> class.
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
