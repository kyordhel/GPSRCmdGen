using System;

namespace GPSRCmdGen
{
	public class Token
	{
		private string key;
		private INameable value;

		public Token (string key) : this(key, null){}

		public Token (string key, INameable value)
		{
			this.key = key;
			this.value = value;
		}

		public string Key { get { return this.key; } }
		public INameable Value { get { return value; } }

		public string StringValue { get { return value == null ? this.key : this.value.Name; } }

		public override string ToString()
		{
			if(value == null)
				return this.key;
			return String.Format ("{0} => {1}", this.key, this.value.Name);
		}
	}
}

