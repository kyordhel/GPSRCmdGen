using System;
using System.Collections.Generic;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	/// <summary>
	/// Represents an object which can be described by means of its properties.
	/// All properties are stored in a Dictionary<string, string> publicly accessible
	/// </summary>
    public interface IDescribable
    {
		/// <summary>
		/// Gets the properties that describe the object.
		/// </summary>
		/// <value>Dictionary of properties of the object.</value>
		Dictionary<string, string> Properties{ get; }

		/// <summary>
		/// Determines whether this instance has a property with the specified propertyName.
		/// </summary>
		/// <returns><c>true</c> if this instance has property with the specified propertyName; otherwise, <c>false</c>.</returns>
		/// <param name="propertyName">Property name.</param>
		bool HasProperty(string propertyName);
    }
}

