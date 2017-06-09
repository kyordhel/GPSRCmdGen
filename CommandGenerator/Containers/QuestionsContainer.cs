using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using PredefinedQuestion = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.PredefinedQuestion;

namespace RoboCup.AtHome.CommandGenerator.Containers
{
	/// <summary>
	/// Helper class. Implements a container for (de)serlaizing categories
	/// </summary>
	[XmlRoot(ElementName = "questions", Namespace = "")]
	public class QuestionsContainer
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.QuestionsContainer"/> class.
		/// </summary>
		public QuestionsContainer() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoboCup.AtHome.CommandGenerator.QuestionsContainer"/> class.
		/// </summary>
		/// <param name="questions">List of questions.</param>
		public QuestionsContainer(List<PredefinedQuestion> questions) { this.Questions = questions; }

		/// <summary>
		/// Gets or sets the list of questions.
		/// </summary>
		/// <value>The questions.</value>
		[XmlElement("question")]
		public List<PredefinedQuestion> Questions { get; set; }
	}
}
