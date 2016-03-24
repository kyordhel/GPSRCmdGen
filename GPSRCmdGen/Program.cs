using System;
using System.Collections.Generic;
using RoboCup.AtHome.CommandGenerator;

namespace RoboCup.AtHome.GPSRCmdGen
{
	/// <summary>
	/// Contains the program control logic
	/// </summary>
	public class Program : BaseProgram
	{

		/// <summary>
		/// Checks if at least one of the required files are present. If not, initializes the 
		/// directory with example files
		/// </summary>
		public static void InitializePath()
		{
			int xmlFilesCnt = System.IO.Directory.GetFiles (Loader.ExePath, "*.xml", System.IO.SearchOption.TopDirectoryOnly).Length;
			if ((xmlFilesCnt < 1) && !System.IO.Directory.Exists (Loader.GetPath("grammars")))
				ExampleFilesGenerator.GenerateExampleFiles ();
		}

		/// <summary>
		/// Initializes the random task Generator and loads data from lists and storage
		/// </summary>
		protected override void Setup()
		{
			this.gen = new GPSRGenerator ();

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine ("GPSR Generator 2016 Beta");
			Console.WriteLine ();
			base.LoadData();
			Console.WriteLine ();
			Console.WriteLine ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			InitializePath ();
			if (args.Length == 0) {
				new Program ().Run ();
				return;
			}
			ParseArgs (args);

		}

		/// <summary>
		/// Parses the arguments.
		/// </summary>
		/// <param name="args">Arguments given to the application.</param>
		private static void ParseArgs (string[] args)
		{
			int category;
			DifficultyDegree tier;
			Program p = new Program ();

			p.Setup ();
			foreach (string arg in args) {
				if (!Int32.TryParse (arg, out category) || (category < 1) || (category > 3)) {
					Console.WriteLine ("Invalid category input {0}", arg);
					continue;
				}
				switch (category) {
					case 1: tier = DifficultyDegree.Easy; break;
					case 2: tier = DifficultyDegree.Moderate; break;
					case 3: tier = DifficultyDegree.High; break;
					default: return;
				}
				p.PrintTask(p.gen.GenerateTask(tier));
			}
		}
	}
}
