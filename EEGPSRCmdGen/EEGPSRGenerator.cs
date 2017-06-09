using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RoboCup.AtHome.CommandGenerator;
using RoboCup.AtHome.CommandGenerator.Containers;
using RoboCup.AtHome.CommandGenerator.ReplaceableTypes;

namespace RoboCup.AtHome.EEGPSRCmdGen
{
	/// <summary>
	/// Generates Random Sentences for the EEGPSR test
	/// </summary>
	public class EEGPSRGenerator : RoboCup.AtHome.CommandGenerator.Generator
	{
		#region Variables

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of EEGPSRGenerator
		/// </summary>
		public EEGPSRGenerator() : base() {}

		#endregion

		#region Properties

		#endregion

		#region Load Methods

		/// <summary>
		/// Randomly generates a task with the given grammar
		/// </summary>
		/// <param name="grammarName">The name of the grammar to generate the task</param>
		/// <param name="tier">The maximum difficulty degree allowed to produce the task</param>
		/// <returns></returns>
		public Task GenerateTask(string grammarName)
		{
			return base.GenerateTask(grammarName, DifficultyDegree.High);
		}

		/// <summary>
		/// Loads the set of gestures from disk. If no gestures file is found, 
		/// the default set is loaded from Resources
		/// </summary>
		public override void LoadGestures()
		{
			try {
				this.allGestures = Loader.Load<GestureContainer> (Loader.GetPath("Gestures.xml")).Gestures;
				Green("Done!");
			} catch {
				this.allGestures = Loader.LoadXmlString<GestureContainer> (Resources.Gestures).Gestures;
				Err ("Failed! Default Gestures loaded");
			}
		}

		/// <summary>
		/// Loads the grammars from disk. If no grammars are found, the application is
		/// terminated.
		/// </summary>
		public override void LoadGrammars()
		{
			try {
				this.allGrammars = Loader.LoadGrammars("eegpsr_grammars");
				Green("Done!");
			} catch {

				Err ("Failed! Application terminated");
				Environment.Exit (0);
			}
		}

		/// <summary>
		/// Loads the set of locations from disk. If no locations file is found, 
		/// the default set is loaded from Resources
		/// </summary>
		public override void LoadLocations ()
		{
			try {
			this.allLocations = Loader.LoadLocations (Loader.GetPath("Locations.xml"));
				Green("Done!");
			} catch {
				List<Room> defaultRooms = Loader.LoadXmlString<RoomContainer> (Resources.Locations).Rooms;
				foreach (Room room in defaultRooms)
					this.allLocations.Add(room); 
				Err ("Failed! Default Locations loaded");
			}
		}

		/// <summary>
		/// Loads the set of names from disk. If no names file is found, 
		/// the default set is loaded from Resources
		/// </summary>
		public override void LoadNames()
		{
			try {
				this.allNames = Loader.Load<NameContainer> (Loader.GetPath("Names.xml")).Names;
				Green("Done!");
			} catch {
				this.allNames = Loader.LoadXmlString<NameContainer> (Resources.Names).Names;
				Err ("Failed! Default Names loaded");
			}
		}

		/// <summary>
		/// Loads the set of objects and categories from disk. If no objects file is found, 
		/// the default set is loaded from Resources
		/// </summary>
		public override void LoadObjects()
		{
			try {
				this.allObjects = Loader.LoadObjects(Loader.GetPath("Objects.xml"));
				Green("Done!");
			} catch {
				List<Category> defaultCategories = Loader.LoadXmlString<CategoryContainer> (Resources.Objects).Categories;
				foreach (Category category in defaultCategories)
					this.allObjects.Add(category);
				Err ("Failed! Default Objects loaded");
			}
		}

		/// <summary>
		/// Loads the set of questions from disk. If no questions file is found, 
		/// the default set is loaded from Resources
		/// </summary>
		public override void LoadQuestions()
		{
			try
			{
				this.allQuestions = Loader.Load<QuestionsContainer>(Loader.GetPath("Questions.xml")).Questions;
				Green("Done!");
			}
			catch
			{
				this.allQuestions = Loader.LoadXmlString<QuestionsContainer> (Resources.Questions).Questions;
				Err("Failed! Default Objects loaded");
			}
		}

		#endregion

		#region Static Methods

		#endregion
	}
}

