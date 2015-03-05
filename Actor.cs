using System;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	[Serializable]
	public class Actor
	{
		#region Constructors

		public Actor ()
		{
			this.Type = "Unknown";
		}

		public Actor (string type)
		{
			this.Type = type;
		}

		public Actor (string type, string subtype)
		{
			this.Type = type;
			this.SubType = subtype;
		}

		#endregion

		#region Properties

		public string Type{ get; set; }
		public string SubType{ get; set; }

		#endregion

		#region Methods

		public override string ToString ()
		{
			return string.Format ("[Actor: Type={0}, SubType={1}]", Type, SubType);
		}

		#endregion

		#region Static Members

		public static Actor Crowd{ get { return new Actor ("Object"); } }
		public static Actor Location{ get { return new Actor ("Location"); } }
		public static Actor LocationPlacement{ get { return new Actor ("Location", "Placement"); } }
		public static Actor Object{ get { return new Actor ("Object"); } }
		public static Actor ObjectKnown{ get { return new Actor ("Object", "Known"); } }
		public static Actor ObjectAlike{ get { return new Actor ("Object", "Alike"); } }
		public static Actor ObjectUnknown{ get { return new Actor ("Object", "Unknown"); } }
		public static Actor Operator{ get { return new Actor ("Person", "Any"); } }
		public static Actor Person{ get { return new Actor ("Person"); } }


		#endregion
	}
}

