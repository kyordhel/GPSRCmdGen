using System;

namespace RoboCup.AtHome.CommandGenerator
{
	public interface IWildcard
	{
		/// <summary>
		/// Gets the keycode associated to each wildcard group unique replacements
		/// </summary>
		string Keycode{ get; }
	}
}

