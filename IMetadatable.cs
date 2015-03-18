using System;

namespace GPSRCmdGen
{
	/// <summary>
	/// Represents an object which contains a set of strings as metadata
	/// </summary>
	public interface IMetadatable : INameable
	{
		string[] Metadata { get; }
	}
}
