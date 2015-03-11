using System;

namespace GPSRCmdGen
{
	class Program
	{
		Generator gen;

		public Program(){

		}

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

		private string GetTask(char opc)
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

		private void PrintTask(string task)
		{
			if (String.IsNullOrEmpty (task))
				return;
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			string pad = String.Empty.PadRight (Console.BufferWidth - 1, '=');
			Console.WriteLine (pad);
			Console.WriteLine();
			Console.WriteLine(task.PadRight(4));
			Console.WriteLine();
			Console.WriteLine (pad);
			Console.ForegroundColor = pc;
			Console.WriteLine();
		}

		public void Run()
		{
			char opc = '\0';
			Setup();
			do
			{
				opc = GetOption();
				string task = GetTask(opc);
				PrintTask(task);
			}
			while(opc != 'q');
		}

		void Setup ()
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
			gen.LoadGestures ();
			Console.Write ("Loading grammars...");
			gen.LoadGrammars ();
			//Console.Write ("Loading actions...");
			//gen.LoadActions ();
			gen.ValidateLocations ();
			Console.WriteLine ();
			Console.WriteLine ();
		}

		public static void Main (string[] args)
		{
			// ExampleFilesGenerator.GenerateExampleFiles ();
			new Program().Run ();
		}
	}
}
