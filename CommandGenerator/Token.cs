using System;
using System.Collections.Generic;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Represents a Token: an element of a Task
	/// A token is a substring extracted from a taks prototype string after analysis,
	/// which may correspond to either a literal string or a wildcard (value != null).
	/// </summary>
	public class Token : IMetadatable
	{
		#region Variables

		/// <summary>
		/// Stores the original substring in the taks prototype string.
		/// </summary>
		private string key;
		/// <summary>
		/// Stores the replacement object for the wildcard represented by this
		/// Token in the taks prototype string.
		/// </summary>
		private INameable value;

		/// <summary>
		/// Stores he metadata contained in this Token, fetched from both,
		/// taks prototype string (metadata stored in grammar) and the
		/// metadata asociated to the Token's value.
		/// </summary>
		private List<string> metadata;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Token"/> class.
		/// </summary>
		/// <param name="key">The original substring in the taks prototype string.</param>
		public Token (string key) : this(key, null, null){}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Token"/> class.
		/// </summary>
		/// <param name="key">The original substring in the taks prototype string.</param>
		/// <param name="value">The replacement object for the wildcard represented by this Token.</param>
		public Token (string key, INameable value) : this(key, value, null){}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.Token"/> class.
		/// </summary>
		/// <param name="key">The original substring in the taks prototype string.</param>
		/// <param name="value">The replacement object for the wildcard represented by this Token.</param>
		/// <param name="metadata">Additional metadata to add (e.g. from the grammar
		/// or the taks prototype string).</param>
		public Token (string key, INameable value, IEnumerable<string> metadata)
		{
			this.key = key;
			this.value = value;
			this.metadata = new List<string>();
			IMetadatable imvalue = value as IMetadatable;
			if(imvalue != null)
				this.metadata.AddRange (imvalue.Metadata);
			if(metadata != null)
				this.metadata.AddRange(metadata);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the original substring in the taks prototype string.
		/// </summary>
		public string Key { get { return this.key; } }

		/// <summary>
		/// Gets the replacement object for the wildcard represented by this
		/// Token in the taks prototype string, if the token is a wildcard.
		/// If the Token is a literal string, it returns null.
		/// </summary>
		public INameable Value { get { return value; } }

		/// <summary>
		/// Gets a value indicating if the Token is a wildcard
		/// </summary>
		public bool IsWildcard{ get { return this.value != null; } }

		/// <summary>
		/// Gets the INameable.Name. of the Token.
		/// When Value property is not null, it returns the name of the Toklen's value.
		/// When Value property is null, it returns the Token's key, 
		/// </summary>
		public string Name { get { return value == null ? this.key : this.value.Name; } }

		/// <summary>
		/// Gets the metadata contained in this Token, fetched from both,
		/// taks prototype string (metadata stored in grammar) and the
		/// metadata asociated to the Token's value.
		/// </summary>
		/// <value>The metadata.</value>
		public List<string> Metadata { get { return this.metadata; } }

		/// <summary>
		/// Gets the metadata contained in this Token, fetched from both,
		/// taks prototype string (metadata stored in grammar) and the
		/// metadata asociated to the Token's value.
		/// </summary>
		/// <value>The metadata.</value>
		string[] IMetadatable.Metadata { get { return this.metadata.ToArray(); } }

		#endregion

		#region Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Token"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.Token"/>.</returns>
		public override string ToString()
		{
			if(value == null)
				return this.key;
			return String.Format ("{0} => {1}", this.key, this.value.Name);
		}

		#endregion
	}
}

