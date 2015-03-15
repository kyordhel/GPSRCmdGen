using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPSRCmdGen
{
	public interface IMetadatable : INameable
	{
		string[] Metadata { get; }
	}
}
