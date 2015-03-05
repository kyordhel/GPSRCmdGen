using System;
using System.Collections.Generic;

namespace GPSRCmdGen
{
	public class Generator
	{
		private List<Action> allActions;
		private List<Location> allLocations;
		private List<Name> allNames;
		private List<GPSRObject> allObjects;


		public Generator ()
		{
			this.allActions = new List<Action> ();
			this.allLocations = new List<Location> ();
			this.allNames = new List<Name> ();
			this.allObjects = new List<GPSRObject> ();
		}

		public object GetTask (char opc)
		{
			throw new NotImplementedException ();
		}

		#region Load Methods

		public void LoadActions ()
		{
			try{
				this.allActions = Loader.Load<Action> ("Actions.xml");
				Green("Done!");
			}
			catch{
				this.allActions = Factory.GetDefaultActions ();
				Err ("Failed! Default Actions loaded");
			}
		}

		public void LoadLocations ()
		{
			try {
				this.allLocations = Loader.Load<Location> ("Locations.xml");
				Green("Done!");
			} catch {
				this.allLocations = Factory.GetDefaultLocations ();
				Err ("Failed! Default Locations loaded");
			}
		}

		public void LoadNames ()
		{
			try {
				this.allNames = Loader.Load<Name> ("Names.xml");
				Green("Done!");
			} catch {
				this.allNames = Factory.GetDefaultNames ();
				Err ("Failed! Default Names loaded");
			}
		}

		public void LoadObjects ()
		{
			try {
				this.allObjects = Loader.Load<GPSRObject> ("Objects.xml");
				Green("Done!");
			} catch {
				this.allObjects = Factory.GetDefaultObjects ();
				Err ("Failed! Default Objects loaded");
			}
		}

		#endregion

		#region Static Methods

		private static void Err(Exception ex){
			Err (null, ex);
		}

		private static void Err(string message){
			Err(message, null);
		}

		private static void Err(string message, Exception ex){
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			if(!String.IsNullOrEmpty(message))
				Console.WriteLine (message);
			if(ex != null)
				Console.WriteLine ("Exception {0}:", ex.Message);
			Console.ForegroundColor = pc;
		}

		private static void Green(string message){
			ConsoleColor pc = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine (message);
			Console.ForegroundColor = pc;
		}


		#endregion
	}
}

