using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RoboCup.AtHome.CommandGenerator;
using RoboCup.AtHome.CommandGenerator.Containers;
using RoboCup.AtHome.CommandGenerator.ReplaceableTypes;

namespace RoboCup.AtHome.SPRTest
{
	/// <summary>
	/// Generates Random Sentences for the GPSR test
	/// </summary>
	public class QuestionsGenerator : RoboCup.AtHome.CommandGenerator.Generator
	{
		#region Variables

		private Random rnd = new Random(DateTime.Now.Millisecond);

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of QuestionsGenerator
		/// </summary>
		public QuestionsGenerator() : base() {
			Quiet = true;
		}

		#endregion

		#region Properties

		#endregion

		#region Methods

		public List<Task> GetQuestions(){
			/**
			* It would be quicker with simple if's but I rather use this weird solution 
			* in case the distribution changes.
			*/
			int aq = rnd.Next(1,3);
			int cq = rnd.Next(1,3);
			int oq = 4 - (aq+cq);

			List<Task> tasks = new List<Task>();
			tasks.Add(GetPredefinedQuestion());
			while(aq-- > 0)
				tasks.Add(GetArenaQuestion());
			while(cq-- > 0)
				tasks.Add(GetCrowdQuestion());
			while(oq-- > 0)
				tasks.Add(GetObjectQuestion());
			Shuffle(tasks);
			return tasks;
		}

		public Task GetArenaQuestion(){
			return GenerateTask("ArenaQuestions", DifficultyDegree.High);
		}

		public Task GetCrowdQuestion(){
			return GenerateTask("CrowdQuestions", DifficultyDegree.High);
		}

		public Task GetObjectQuestion(){
			return GenerateTask("ObjectQuestions", DifficultyDegree.High);
		}

		public Task GetPredefinedQuestion(){
			return GenerateTask("PredefinedQuestions", DifficultyDegree.High);
		}

		private void Shuffle<T>(IList<T> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				--n;  
				int k = rnd.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}

		#endregion

		#region Load Methods

		/// <summary>
		/// Loads the set of gestures from disk. If no gestures file is found, 
		/// the default set is loaded from Factory
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
				this.allGrammars = Loader.LoadGrammars("spr_grammars");
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
			/*
			try {
				this.allNames = Loader.Load<NameContainer> (Loader.GetPath("Names.xml")).Names;
				Green("Done!");
			} catch {
				this.allNames = Loader.LoadXmlString<NameContainer> (Resources.Names).Names;
				Err ("Failed! Default Names loaded");
			}
			*/
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

