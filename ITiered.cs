using System;

namespace GPSRCmdGen
{
	/// <summary>
	/// Represents an object which has an asociated difficulty degree (tier)
	/// </summary>
	public interface ITiered
	{
		/// <summary>
		/// Gets the difficulty degree (tier) of the object
		/// </summary>
		DifficultyDegree Tier{ get; }
	}
}

