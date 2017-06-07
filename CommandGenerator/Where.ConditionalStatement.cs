using System;

namespace RoboCup.AtHome.CommandGenerator
{
    public partial class WhereParser{
		/// <summary>
		/// Encapsulates several conditions
		/// </summary>
		public class ConditionalStatement : IEvaluable
        {
            public ConditionalStatement()
            {
				
            }

			public IEvaluable A{ get; internal set; }

			public string Operator{ get; internal set; }

			public IEvaluable B{ get; internal set; }


            public bool Evaluate(object obj){
				if (String.IsNullOrEmpty(this.Operator))
					return A != null ? A.Evaluate(obj) : false;
				
				switch (this.Operator.ToLower())
				{
					case "and":
						return A.Evaluate(obj) && B.Evaluate(obj);

					case "or":
						return A.Evaluate(obj) || B.Evaluate(obj);

					case "xor":
						return A.Evaluate(obj) ^ B.Evaluate(obj);

					case "not":
						return !A.Evaluate(obj);
				}

				throw new NotSupportedException(String.Format("Operator '{0}' is not supported"));
				//return false;
            }
        }
    }
}

