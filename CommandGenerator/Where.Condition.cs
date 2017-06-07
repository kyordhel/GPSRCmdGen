using System;
using System.Reflection;

namespace RoboCup.AtHome.CommandGenerator
{
    public partial class WhereParser{

		/// <summary>
		/// Represents a single condition in a where clause
		/// </summary>
		public class Condition : IEvaluable
        {
            public Condition()
            {
            }

            public string PropertyName{ get; internal set; }

            public string Operator{ get; internal set; }

            public string Value{ get; internal set; }

			public char ValueType{ get; internal set; }

            public bool Evaluate(object obj){
				if (obj == null)
					return false;
				PropertyInfo pi = obj.GetType().GetProperty(this.PropertyName);
				if (pi == null)
					return false;

				try{
					if(ValueType == 's')
						return Compare(this.Value, Convert.ToString(pi.GetValue(obj)));
					else if(ValueType == 'n')
						return Compare(Double.Parse(this.Value), (double)pi.GetValue(obj));
				}
				catch{
					return false;
				}
				return false;
            }

			private bool Compare(string a, string b){
				switch(this.Operator)
				{
					case "=": return a == b;
					case "!=": return a != b;
				}
				return false;
			}

			private bool Compare(double a, double b){
				switch(this.Operator)
				{
					case "=": return a == b;
					case "!=": return a != b;
					case ">": return a > b;
					case ">=": return a >= b;
					case "<": return a < b;
					case "<=": return a <= b;
				}
				return false;
			}

			internal static Condition Parse(string s, ref int cc){
				// A where clause starts with an identifier followed by a binary operator
				// and ends with a value. The type pattern is: io[sn]

				char type;
				Condition condition = new Condition();

				condition.PropertyName = WhereParser.ReadNext(s, ref cc, out type);
				if (type != 'i')
					return null;
				condition.Operator = WhereParser.ReadNext(s, ref cc, out type);
				if (type != 'o')
					return null;
				condition.Value = WhereParser.ReadNext(s, ref cc, out type);
				condition.ValueType = type;
				if ((type != 's') && (type != 'n'))
					return null;
				return condition;
			}
        }
    }
}
