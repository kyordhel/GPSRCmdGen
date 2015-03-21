using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen.Containers
{
	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing categories
	/// </summary>
	[XmlRoot(ElementName = "categories", Namespace = "")]
	public class CategoryContainer
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.CategoryContainer"/> class.
		/// </summary>
		public CategoryContainer() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.CategoryContainer"/> class.
		/// </summary>
		/// <param name="categories">List of categories.</param>
		public CategoryContainer(List<Category> categories) { this.Categories = categories; }

		/// <summary>
		/// Gets or sets the list of categories.
		/// </summary>
		[XmlElement("category")]
		public List<Category> Categories { get; set; }
	}
}
