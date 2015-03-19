using System;
using System.Collections.Generic;

namespace GPSRCmdGen
{
	/// <summary>
	/// Contains the program control logic
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Random Task generator
		/// </summary>
		Generator gen;

		/// <summary>
		/// Request the user to choose an option for random task generation.
		/// </summary>
		/// <returns>The user's option.</returns>
		private char GetOption(){
			ConsoleKeyInfo k;
			Console.WriteLine("Press q to quit, c to clear.");
			Console.Write("Enter category 1, 2, or 3: ");
			do {
				k = Console.ReadKey (true);
			} while((k.KeyChar != 'q') && (k.KeyChar != 'c') && ((k.KeyChar < '1') || (k.KeyChar > '3')));
			Console.WriteLine (k.KeyChar);
			return k.KeyChar;
		}

		/// <summary>
		/// Gets a randomly generated task based on user input.
		/// </summary>
		/// <returns>A randonly generated task.</returns>
		/// <param name="opc">User option (category).</param>
		private Task GetTask(char opc)
		{
			DifficultyDegree tier = DifficultyDegree.Unknown;
			switch (opc) {
				case '1': tier = DifficultyDegree.Easy; break;
				case '2': tier = DifficultyDegree.Moderate; break;
				case '3': tier = DifficultyDegree.High; break;
				case 'c': Console.Clear (); return null;
				default: return null;
			}

			Console.WriteLine("Choosen category {0}", opc);
			return gen.GenerateTask (tier);
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
		/// Prints a task including metadata into the output stream.
		/// </summary>
		/// <param name="task">The task to be print</param>
		private void PrintTask(Task task)
		{
			if (task == null)
				return;
			// switch Console color to white, backuping the previous one
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			// Prints a === line
			string pad = String.Empty.PadRight (Console.BufferWidth - 1, '=');
			Console.WriteLine (pad);
			Console.WriteLine();
			// Prints task string and metadata
			Console.WriteLine(task.ToString().PadRight(4));
			PrintTaskMetadata(task);
			Console.WriteLine();
			// Prints another line
			Console.WriteLine (pad);
			// Restores Console color
			Console.ForegroundColor = pc;
			Console.WriteLine();
		}

		/// <summary>
		/// Prints the task metadata.
		/// </summary>
		/// <param name="task">The task object containing metadata to print.</param>
		private void PrintTaskMetadata(Task task)
		{
			Console.WriteLine();
			List<string> remarks = new List<string>();
			// Print named metadata
			foreach (Token token in task.Tokens)
				PrintMetadata(token, remarks);
			PrintRemarks(remarks);
		}

		/// <summary>
		/// Prints the metadata of the given Token
		/// </summary>
		/// <param name="token">The token onject containing the metadata to print</param>
		/// <param name="remarks">A list to store all metadata whose token has no name</param>
		private void PrintMetadata(Token token, List<string> remarks)
		{
			if (token.Metadata.Length < 1) return;
			// Store remarks for later
			if (String.IsNullOrEmpty(token.Name))
			{
				remarks.AddRange(token.Metadata);
				return;
			}
			// Print current token metadata
			Console.WriteLine("{0}", token.Name);
			foreach (string md in token.Metadata)
				Console.WriteLine("\t{0}", md);
		}

		/// <summary>
		/// Prints remaining metadata stored in the remarks list
		/// </summary>
		/// <param name="remarks">List of remarks strings</param>
		private static void PrintRemarks(List<string> remarks)
		{
			if (remarks.Count > 0)
			{
				Console.WriteLine("remarks");
				foreach (string r in remarks)
					Console.WriteLine("\t{0}", r);
			}
		}

		/// <summary>
		/// Starts the user input loop
		/// </summary>
		public void Run()
		{
			char opc = '\0';
			Setup();
			do
			{
				opc = GetOption();
				Task task = GetTask(opc);
				PrintTask(task);
			}
			while(opc != 'q');
		}

		/// <summary>
		/// Initializes the random task Generator and loads data from lists and storage
		/// </summary>
		private void Setup ()
		{
			this.gen = new Generator ();

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine ("GPSR Generator 0.1 Beta");
			Console.WriteLine ();
			Console.Write ("Loading objects...");
			gen.LoadObjects();
			Console.Write ("Loading names...");
			gen.LoadNames ();
			Console.Write ("Loading locations...");
			gen.LoadLocations ();
			Console.Write ("Loading gestures...");
			gen.LoadGestures();
			Console.Write("Loading predefined questions...");
			gen.LoadQuestions();
			Console.Write ("Loading grammars...");
			gen.LoadGrammars ();
			gen.ValidateLocations ();
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
