using System;


namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Implements a virtual task of custom value for grammatical and replacement purposes
	/// </summary>
	public class NamedTaskElement : INameable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.NamedTaskElement"/> class.
		/// </summary>
		public NamedTaskElement () { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.NamedTaskElement"/> class.
		/// </summary>
		public NamedTaskElement (string name) { this.Name = name; }

		/// <summary>
		/// Returns the previously defined string
		/// </summary>
		public string Name { get; set; }
	}
}

