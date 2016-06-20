using System;
using System.Collections.Generic;
using System.Threading;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Base class for program control logic
	/// </summary>
	public abstract class BaseProgram
	{
		protected abstract Generator Gen { get; }

		/// <summary>
		/// Gets user text from console input and displays it in a QR code
		/// </summary>
		protected void DisplayQRText()
		{
			Console.WriteLine("Write text for QR code and press INTRO.");
			Console.Write("QR Text: ");
			ShowQRDialog(Console.ReadLine());
		}

		/// <summary>
		/// Request the user to choose an option for random task generation.
		/// </summary>
		/// <returns>The user's option.</returns>
		protected virtual char GetOption(int opcMin, int opcMax)
		{
			ConsoleKeyInfo k;
			Console.WriteLine("Press Esc to quit, q for QR Code, t for type in a QR, c to clear.");
			Console.Write("Enter category {0} to {1}: ", opcMin, opcMax);
			do
			{
				k = Console.ReadKey(true);
			} while ((k.Key != ConsoleKey.Escape) && (k.KeyChar != 'q') && (k.KeyChar != 't') && (k.KeyChar != 'c') && ((k.KeyChar < ('0' + opcMin)) || (k.KeyChar > ('0' + opcMax) )));
			if (k.Key == ConsoleKey.Escape)
				return '\0';
			Console.WriteLine(k.KeyChar);
			return k.KeyChar;
		}

		/// <summary>
		/// Request the user to choose an option for random task generation.
		/// </summary>
		/// <returns>The user's option.</returns>
		protected abstract char GetOption();

		/// <summary>
		/// Loads data from lists and storage
		/// </summary>
		protected void LoadData()
		{
			Console.Write("Loading objects...");
			Gen.LoadObjects();
			Console.Write("Loading names...");
			Gen.LoadNames();
			Console.Write("Loading locations...");
			Gen.LoadLocations();
			Console.Write("Loading gestures...");
			Gen.LoadGestures();
			Console.Write("Loading predefined questions...");
			Gen.LoadQuestions();
			Console.Write("Loading grammars...");
			Gen.LoadGrammars();
			Gen.ValidateLocations();
		}

		/// <summary>
		/// Prints a task including metadata into the output stream.
		/// </summary>
		/// <param name="task">The task to be print</param>
		protected virtual void PrintTask(Task task)
		{
			if (task == null)
				return;
			string sTask = task.ToString().Trim();
			if (sTask.Length < 1)
				return;

			// switch Console color to white, backuping the previous one
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			// Prints a === line
			string pad = String.Empty.PadRight(Console.BufferWidth - 1, '=');
			Console.WriteLine(pad);
			Console.WriteLine();

			// Prints task string and metadata
			sTask = sTask.Substring(0, 1).ToUpper() + sTask.Substring(1);
			do
			{
				// sTask = sTask.PadRight(4);
				int cut = sTask.Length;
				if (cut >= Console.BufferWidth)
					cut = sTask.LastIndexOf(' ', Console.BufferWidth-1);
				Console.WriteLine(sTask.Substring(0, cut));
				sTask = sTask.Substring(cut).Trim();
			} while (!String.IsNullOrEmpty(sTask));
			PrintTaskMetadata(task);
			Console.WriteLine();
			// Prints another line
			Console.WriteLine(pad);
			// Restores Console color
			Console.ForegroundColor = pc;
			Console.WriteLine();
		}

		/// <summary>
		/// Prints the task metadata.
		/// </summary>
		/// <param name="task">The task object containing metadata to print.</param>
		protected void PrintTaskMetadata(Task task)
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
		protected void PrintMetadata(Token token, List<string> remarks)
		{
			if (token.Metadata.Count < 1) return;
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
		protected void PrintRemarks(List<string> remarks)
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
		public virtual void Run()
		{
			Task task = null;
			char opc = '\0';
			Setup();
			do
			{
				opc = GetOption();
				RunOption(opc, ref task);
			}
			while (opc != '\0');
		}

		/// <summary>
		/// Executes the user's option
		/// </summary>
		/// <param name="opc">User option (category).</param>
		protected abstract void RunOption(char opc, ref Task task);

		/// <summary>
		/// When overriden in a derived class initializes the random task Generator and loads data from lists and storage
		/// </summary>
		protected abstract void Setup();

		/// <summary>
		/// Creates and displays a QR dialog window with the given text
		/// </summary>
		/// <param name="text">Thext to show in the QR code</param>
		public static void ShowQRDialog(string text)
		{
			Thread thread = new Thread(new ThreadStart( () => {
				RoboCup.AtHome.CommandGenerator.GUI.QRDialog.OpenQRWindow(text);
				System.Windows.Forms.Application.Run();
			} ));
			thread.IsBackground = true;
			thread.Start();
		}
	}
}
