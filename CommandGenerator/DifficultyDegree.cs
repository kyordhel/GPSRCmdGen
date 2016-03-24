using System;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Represent the difficulty for a given task or something related with solving a task
	/// </summary>
	public enum DifficultyDegree
	{
		/// <summary>
		/// The difficulty degree is unkwnon
		/// </summary>
		[XmlEnum("unknown")]
		Unknown = -1,
		/// <summary>
		/// Performing the task is trivial
		/// </summary>
		[XmlEnum("none")]
		None = 0,
		/// <summary>
		/// Solving the task requires a minimum effort
		/// A task using an element with this attribute is easy to solve
		/// </summary>
		[XmlEnum("easy")]
		Easy = 1,
		/// <summary>
		/// Solving the task requires a moderate effort
		/// A task using an element with this attribute is not so easy to solve
		/// </summary>
		[XmlEnum("moderate")]
		Moderate = 3,
		/// <summary>
		/// Solving the task requires a high effort
		/// A task using an element with this attribute is hard to solve
		/// </summary>
		[XmlEnum("high")]
		High = 5
	}
}

