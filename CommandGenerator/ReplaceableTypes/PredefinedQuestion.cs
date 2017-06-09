using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace RoboCup.AtHome.CommandGenerator.ReplaceableTypes
{
	/// <summary>
	/// Represents a Question from the set of Predefined Questions
	/// according to the RoboCup@Home Rulebook 2015
	/// </summary>
	[Serializable]
	public class PredefinedQuestion : INameable, ITiered, IMetadatable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.PredefindedQuestion"/> class.
		/// </summary>
		/// <remarks>Intended for serialization purposes</remarks>
		public PredefinedQuestion() : this("tell your name", "[varies]", DifficultyDegree.Easy) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.PredefindedQuestion"/> class.
		/// </summary>
		/// <param name="question">The question for which there is no an unique answer</param>
		public PredefinedQuestion(string question) : this(question, "[varies]", DifficultyDegree.Easy) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.PredefindedQuestion"/> class.
		/// </summary>
		/// <param name="question">The question</param>
		/// <param name="answer">The answer for the question</param>
		public PredefinedQuestion(string question, string answer) : this(question, answer, DifficultyDegree.Easy) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.PredefindedQuestion"/> class.
		/// </summary>
		/// <param name="question">The question</param>
		/// <param name="answer">The answer for the question</param>
		/// <param name="tier">The  difficulty degree (tier) for UNDERSTANDING the question</param>
		public PredefinedQuestion(string question, string answer, DifficultyDegree tier)
		{
			this.Question = question;
			this.Answer = answer;
			this.Tier = tier;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the question string
		/// </summary>
		[XmlElement("q")]
		public string Question { get; set; }

		/// <summary>
		/// Returns the <c>"question"</c> string
		/// </summary>
		[XmlIgnore]
		string INameable.Name { get { return "question"; } }

		/// <summary>
		/// Returns a set of two strings, the firs containing the question,
		/// and the second one containing the answer
		/// </summary>
		[XmlIgnore]
		string[] IMetadatable.Metadata
		{
			get
			{
				return new String[]{
				String.Format("Q: {0}", this.Question),
				String.Format("A: {0}", this.Answer)
			};
			}
		}

		/// <summary>
		/// Gets or sets the answer string
		/// </summary>
		[XmlElement("a")]
		public string Answer { get; set; }

		/// <summary>
		/// Gets or sets the  difficulty degree (tier) for UNDERSTANDING the question
		/// </summary>
		[XmlAttribute("difficulty"), DefaultValue(DifficultyDegree.Easy)]
		public DifficultyDegree Tier{ get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.PredefindedQuestion"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="RoboCup.AtHome.CommandGenerator.PredefindedQuestion"/>.</returns>
		public override string ToString()
		{
			return String.Format("{0} ({1})", this.Question, this.Tier);
		}

		#endregion
	}
}

