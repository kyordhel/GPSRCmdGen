using System;


namespace GPSRCmdGen
{
	/// <summary>
	/// Implements a virtual task value with no data
	/// </summary>
	public class HiddenTaskElement : INameable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GPSRCmdGen.HiddenTaskElement"/> class.
		/// </summary>
		public HiddenTaskElement () { }

		/// <summary>
		/// Returns an empty string
		/// </summary>
		public string Name { get { return String.Empty; } }
	}
}

