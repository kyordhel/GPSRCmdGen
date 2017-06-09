using System;
using System.Reflection;
using IDescribable = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.IDescribable;

namespace RoboCup.AtHome.CommandGenerator
{
    public partial class WhereParser{

		/// <summary>
		/// Represents a single condition in a where clause
		/// </summary>
		public class Condition : IEvaluable
        {

			#region Properties

            public string PropertyName{ get; internal set; }

            public string Operator{ get; internal set; }

            public string Value{ get; internal set; }

			public char ValueType{ get; internal set; }

			#endregion

			#region Methods

            public bool Evaluate(object obj){
				if (obj == null)
					return false;

				object value = this.GetPropertyValue(obj);
				try{
					switch(this.ValueType){
						case '0': return CompareNull(value);
						case 'B': return CompareBoolean(value);
						case 's': return (value == null) ? false : Compare(this.Value, value);
						case 'n': return (value == null) ? false : Compare(Double.Parse(this.Value), (double)value);
					}
				}
				catch{
					return false;
				}
				return false;
			}

			private object GetPropertyValue(object obj)
			{
				if (obj == null)
					return false;
				IDescribable dObj = obj as IDescribable;
				PropertyInfo pi = obj.GetType().GetProperty(this.PropertyName);

				return (pi != null) ? pi.GetValue(obj) : GetPropertyValue(dObj);
			}

			private object GetPropertyValue(IDescribable obj){
				if ((obj == null) || !obj.HasProperty(this.PropertyName))
					return null;

				switch (this.ValueType)
				{
					case 'n': return Double.Parse(obj.Properties[this.PropertyName]);
					case 'B': return Boolean.Parse(obj.Properties[this.PropertyName]);
					case 's':
					default: return obj.Properties[this.PropertyName];
				}
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

			private bool Compare(string a, string b){
				switch(this.Operator)
				{
					case "=": return a == b;
					case "!=": return a != b;
				}
				return false;
			}

			private bool Compare(string a, object b){
				if (b is INameable)
					return Compare(a, ((INameable)b).Name);
				return Compare(a, Convert.ToString(b));
			}

			private bool CompareBoolean(object value){
				bool b = (value == null) ? false : (bool)value;
				bool a = Boolean.Parse(this.Value);
				switch(this.Operator)
				{
					case "=": return a == b;
					case "!=": return a != b;
				}
				return false;
			}

			private bool CompareNull(object value){
				switch(this.Operator)
				{
					case "=": return value == null;
					case "!=": return value != null;
				}
				return false;
			}

			public override string ToString()
			{
				return string.Format("{0} {1} {2}", PropertyName, Operator, Value);
			}

			#endregion

			#region Static members

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
				if(type.IsAnyOf('0', 'B', 'n', 's'))
					return condition;
				return null;
			}

			#endregion
        }
    }
}
