using System;
using System.IO;
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
			int xmlFilesCnt = System.IO.Directory.GetFiles(Loader.ExePath, "*.xml", System.IO.SearchOption.TopDirectoryOnly).Length;
			if ((xmlFilesCnt < 4) || !System.IO.Directory.Exists(Loader.GetPath("eegpsr_grammars")))
				ExampleFilesGenerator.GenerateExampleFiles();
		}

		/// <summary>
		/// Displays the list of available options
		/// </summary>
		protected void DisplayMenu()
		{
			ConsoleColor c = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("EEGPSR Command Generator 2018 Beta");
			Console.WriteLine("====================================");
			Console.Out.Flush();
			Console.ForegroundColor = c;
			Console.WriteLine();
			Console.WriteLine("Normal commands");
			Console.WriteLine("    1.  Category I   - Three at once");
			Console.WriteLine("    2.  Category II  - People");
			Console.WriteLine("    3.  Category III - Objects");
			Console.WriteLine();
			Console.WriteLine("Commands with Incomplete information");
			Console.WriteLine("    4.  Category I   - Three at once");
			Console.WriteLine("    5.  Category II  - People");
			Console.WriteLine("    6.  Category III - Objects");
			Console.WriteLine();
			Console.WriteLine("Commands with Erroneous information");
			Console.WriteLine("    7.  Category I   - Three at once");
			Console.WriteLine("    8.  Category II  - People");
			Console.WriteLine("    9.  Category III - Objects");
			Console.WriteLine();
		}

		/// <summary>
		/// Request the user to choose an option for random task generation.
		/// </summary>
		/// <returns>The user's option.</returns>
		protected override char GetOption()
		{
			int opcMin = 1;
			int opcMax = 9;
			ConsoleKeyInfo k;
			Console.WriteLine("Press Esc to quit, q for QR Code, t for type in a QR, c to clear.");
			Console.Write("Choose an option: ");
			k = Console.ReadKey(true);
			if (k.Key == ConsoleKey.Escape)
				return '\0';
			Console.WriteLine(k.KeyChar);
			return k.KeyChar;
		}

		private Task GetTask(char category)
		{
			switch (category)
			{
				case '1': return gen.GenerateTask("Category I - Three at once");
				case '2': return gen.GenerateTask("Category II - People");
				case '3': return gen.GenerateTask("Category III - Objects");
				case '4': return gen.GenerateTask("Category I - Three at once (with incomplete information)");
				case '5': return gen.GenerateTask("Category II - People (with incomplete information)");
				case '6': return gen.GenerateTask("Category III - Objects (with incomplete information)");
				case '7': return gen.GenerateTask("Category I - Three at once (with erroneous information)");
				case '8': return gen.GenerateTask("Category II - People (with erroneous information)");
				case '9': return gen.GenerateTask("Category III - Objects (with erroneous information)");
				default: return null;
			}
		}

		/// <summary>
		/// Starts the user input loop
		/// </summary>
		public override void Run()
		{
			Task task = null;
			char opc = '\0';
			Setup();
			DisplayMenu();
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
		protected override void RunOption(char opc, ref Task task)
		{
			switch (opc)
			{
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					task = GetTask(opc);
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
					Console.Clear();
					DisplayMenu();
					break;
			}

			if(task == null) return;
			Console.WriteLine("Choosen option {0}", opc);
			PrintTask(task);
		}

		/// <summary>
		/// Initializes the random task Generator and loads data from lists and storage
		/// </summary>
		protected override void Setup()
		{
			this.gen = new EEGPSRGenerator();

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine("EEGPSR Command Generator 2018 Beta");
			Console.WriteLine();
			base.LoadData();
			Console.WriteLine();
			Console.WriteLine();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		[STAThread]
		public static void Main(string[] args)
		{
			InitializePath();
			if (args.Length == 0)
			{
				new Program().Run();
				return;
			}
			ParseArgs(args);

		}

		/// <summary>
		/// Parses the arguments.
		/// </summary>
		/// <param name="args">Arguments given to the application.</param>
		private static void ParseArgs(string[] args)
		{

			int category;
			Program p = new Program();

			p.Setup();
			for (int i = 0; i < args.Length; ++i)
			{
				if (Int32.TryParse(args[i], out category) && (category > 0) && (category < 7))
				{
					Task t = null;
					p.RunOption((char)(category + '0'), ref t);
					continue;
				}

				if (args[i] == "--bulk")
					DoBulk(p, args, ref i);
			}
		}

		private static void DoBulk(Program p, string[] args, ref int i)
		{
			int dCount;
			if ((args.Length < (i + 2)) || !Int32.TryParse(args[++i], out dCount) || (dCount < 1))
				dCount = 100;

			Console.WriteLine("Generating {0} examples in bulk mode for 9 categories", dCount);
			try
			{
				for (char category = '1'; category <= '9'; ++category)
				{
					Console.WriteLine("Generating {0} examples for category {1}", dCount, category);
					BulkExamples(p, category, dCount);
				}
			}
			catch (Exception ex) { Console.WriteLine(ex.Message); }
		}

		private static void BulkExamples(Program p, char category, int count)
		{
			string oDir = String.Format("EEGPSR Cat{0} Examples", category);
			if(!Directory.Exists(oDir))
			Directory.CreateDirectory(oDir);
			string oFile = Path.Combine(oDir, String.Format("{0}.txt", oDir));
			using (StreamWriter writer = new StreamWriter(oFile, false, System.Text.Encoding.UTF8)){
				for(int i = 1; i <= count; ++i){
					Task task = p.GetTask(category);
					if (task == null) continue;
					string sTask = task.ToString().Trim();
					if (sTask.Length < 1) continue;
					sTask = sTask.Substring(0, 1).ToUpper() + sTask.Substring(1);

					WriteTaskToFile(writer, task, sTask, i);
					GenerateTaskQR(sTask, i, oDir);
				}
			}
		}

		private static void WriteTaskToFile(StreamWriter writer, Task task, string sTask, int i)
		{
			string pad = String.Empty.PadRight(79, '#');
			writer.WriteLine(pad);
			writer.WriteLine("#");
			writer.WriteLine("# Example {0}", i);
			writer.WriteLine("#");
			writer.WriteLine(pad);
			writer.WriteLine();
			writer.WriteLine(sTask);
			writer.WriteLine();
			List<string> remarks = new List<string>();
			foreach (Token token in task.Tokens)
			{
				if (token.Metadata.Count < 1)
					continue;
				if (String.IsNullOrEmpty(token.Name))
					remarks.AddRange(token.Metadata);
				else
				{
					writer.WriteLine("{0}", token.Name);
					foreach (string md in token.Metadata)
						writer.WriteLine("\t{0}", md);
				}
			}
			if (remarks.Count > 0)
			{
				writer.WriteLine("Remarks");
				foreach (string r in remarks)
					writer.WriteLine("\t{0}", r);
			}
			writer.WriteLine();
		}

		private static void GenerateTaskQR(string task, int i, string oDir)
		{
			string oFile;
			System.Drawing.Image qr = CommandGenerator.GUI.QRDialog.GenerateQRBitmap(task, 500);
			oFile = Path.Combine(oDir, String.Format("Example{0} - {1}.png", i.ToString().PadLeft(3, '0'), task));
			if (File.Exists(oFile)) File.Delete(oFile);
			qr.Save(oFile, System.Drawing.Imaging.ImageFormat.Png);
		}
	}
}
