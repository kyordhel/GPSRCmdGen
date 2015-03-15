using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	[Serializable]
	public class PredefindedQuestion : INameable, ITiered, IMetadatable
	{
		public PredefindedQuestion() : this("tell your name", "[varies]", DifficultyDegree.Easy) { }

		public PredefindedQuestion(string question) : this(question, "[varies]", DifficultyDegree.Easy) { }

		public PredefindedQuestion(string question, string answer) : this(question, answer, DifficultyDegree.Easy) { }

		public PredefindedQuestion(string question, string answer, DifficultyDegree tier)
		{
			this.Question = question;
			this.Answer = answer;
			this.Tier = tier;
		}

		[XmlElement("q")]
		public string Question { get; set; }

		string INameable.Name { get { return "question"; } }

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

		[XmlElement("a")]
		public string Answer { get; set; }

		[XmlAttribute("difficulty"), DefaultValue(DifficultyDegree.Easy)]
		public DifficultyDegree Tier{ get; set; }

		public override string ToString()
		{
			return String.Format("{0} ({1})", this.Question, this.Tier);
		}
	}
}

