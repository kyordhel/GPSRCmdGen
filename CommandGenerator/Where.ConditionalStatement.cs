using System;

namespace RoboCup.AtHome.CommandGenerator
{
    public partial class WhereParser{
		/// <summary>
		/// Encapsulates several conditions
		/// </summary>
		public class ConditionalStatement : IEvaluable
        {

			/// <summary>
			/// Gets the default value (left value in a binary statement)
			/// </summary>
			/// <value>A.</value>
			public IEvaluable A{ get; internal set; }

			/// <summary>
			/// Gets the operator for the conditional statement
			/// </summary>
			/// <value>The operator.</value>
			public string Operator{ get; internal set; }

			/// <summary>
			/// Gets the right value in a binary statement
			/// </summary>
			/// <value>A.</value>
			public IEvaluable B{ get; internal set; }

			/// <summary>
			/// Evaluate the conditional statement.
			/// If there is no operator returns A. If the operator is NOT returns !A.
			/// Otherwise returns the result of evaluating A op B.
			/// </summary>
			/// <param name="obj">A boolean result of evaluating the conditional statement</param>
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

			public override string ToString()
			{
				if (String.IsNullOrEmpty(Operator))
					return A.ToString();
				else if(Operator.ToLower() == "not")
					return string.Format("NOT(B)]", A);
				return string.Format("{0} {1} {2}", A, Operator, B);
			}
        }
    }
}

