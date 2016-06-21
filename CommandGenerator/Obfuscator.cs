using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Implements a generic INameable object for obfuscation
	/// </summary>
	public class Obfuscator : INameable
	{
		public readonly string name;

		public Obfuscator(string name)
		{
			this.name = name;
		}

		#region INameable Members

		public string Name
		{
			get { return name; }
		}

		#endregion
	}
}
