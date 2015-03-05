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
			Console.WriteLine("Press q for quit.");
			Console.Write("Enter category 1, 2, or 3: ");
			do {
				k = Console.ReadKey (true);
			} while((k.KeyChar != 'q') && ((k.KeyChar < '1') || (k.KeyChar > '3')));
			Console.WriteLine (k.KeyChar);
			return k.KeyChar;
		}

		private void PrintTask(char opc)
		{
			Console.WriteLine("Choosen category {0}", opc);
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine (gen.GetTask (opc));
			Console.ForegroundColor = pc;
			Console.WriteLine();
			Console.WriteLine();
		}

		public void Run()
		{
			char opc = '\0';
			Setup();
			do
			{
				opc = GetOption();
				if((opc >= '1') && (opc <= '3'))
					PrintTask(opc);
			}
			while(opc != 'q');
		}

		void Setup ()
		{
			this.gen = new Generator ();

			Console.WriteLine ("GPSR Generator 0.1 Beta");
			Console.WriteLine ();
			Console.Write ("Loading objects...");
			gen.LoadObjects();
			Console.Write ("Loading names...");
			gen.LoadNames ();
			Console.Write ("Loading locations...");
			gen.LoadLocations ();
			Console.Write ("Loading actions...");
			gen.LoadActions ();

			Console.WriteLine ();
			Console.WriteLine ();
		}

		public static void Main (string[] args)
		{
			Grammar g = Grammar.FromString (Grammar.g);
			new Program().Run ();
		}
	}
}
