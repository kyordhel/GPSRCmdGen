using System;
using System.Collections.Generic;
using RoboCup.AtHome.CommandGenerator;

namespace RoboCup.AtHome.EEGPSRCmdGen
{
	/// <summary>
	/// Contains the program control logic
	/// </summary>
	public class Program : BaseProgram
	{
		/// <summary>
		/// Random Task generator
		/// </summary>
		protected EEGPSRGenerator gen;

		protected override Generator Gen
		{
			get { return this.gen; }
		}

		public Program()
		{
			gen = new EEGPSRGenerator();
		}

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
		/// Request the user to choose an option for random task generation.
		/// </summary>
		/// <returns>The user's option.</returns>
		protected override char GetOption()
		{
			return base.GetOption(1, 6);
		}

		/// <summary>
		/// Executes the user's option
		/// </summary>
		/// <param name="opc">User option (category).</param>
		protected override void RunOption(char opc, ref Task task)
		{
			switch (opc)
			{
				case '1': task = gen.GenerateTask("Cat1 - Advanced Manipulation");
					break;
				case '2': task = gen.GenerateTask("Cat2 - Advanced Object Recognition");
					break;
				case '3': task = gen.GenerateTask("Cat3 - HRI and Incomplete Commands");
					break;
				case '4': task = gen.GenerateTask("Cat4 - Memory and Awareness");
					break;
				case '5': task = gen.GenerateTask("Cat5 - People Recognition and Navigation");
					break;
				case '6': task = gen.GenerateTask("Cat6 - Simple Tasks");
					break;

				case 'c':
					Console.Clear();
					return;

				case 't':
					DisplayQRText();
					return;

				case 'q':
					if (task == null)
					{
						Generator.Warn("Generate a task first");
						return;
					}

					ShowQRDialog(task.ToString());
					return;

				default:
					break;
			}

			Console.WriteLine("Choosen category {0}", opc);
			PrintTask(task);
		}

		/// <summary>
		/// Initializes the random task Generator and loads data from lists and storage
		/// </summary>
		protected override void Setup ()
		{
			this.gen = new EEGPSRGenerator ();

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine ("EEGPSR Generator 2016 Beta");
			Console.WriteLine ();
			base.LoadData();
			Console.WriteLine ();
			Console.WriteLine ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		[STAThread]
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
					case 1: tier = DifficultyDegree.High; break;
					case 2: tier = DifficultyDegree.High; break;
					case 3: tier = DifficultyDegree.High; break;
					default: return;
				}
				p.PrintTask(p.gen.GenerateTask(tier));
			}
		}
	}
}
