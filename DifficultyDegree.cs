using System;

namespace GPSRCmdGen
{
	/// <summary>
	/// Represent the difficulty for a given task or something related with solving a task
	/// </summary>
	public enum DifficultyDegree
	{
		/// <summary>
		/// The difficulty degree is unkwnon
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// Performing the task is trivial
		/// </summary>
		None = 0,
		/// <summary>
		/// Solving the task requires a minimum effort
		/// A task using an element with this attribute is easy to solve
		/// </summary>
		Easy = 1,
		/// <summary>
		/// Solving the task requires a moderate effort
		/// A task using an element with this attribute is not so easy to solve
		/// </summary>
		Moderate = 3,
		/// <summary>
		/// Solving the task requires a high effort
		/// A task using an element with this attribute is hard to solve
		/// </summary>
		High = 5
	}
}

