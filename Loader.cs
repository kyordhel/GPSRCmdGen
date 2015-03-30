using System;
using System.IO;

namespace GPSRCmdGen
{

	/// <summary>
	/// Helper class for loading data
	/// </summary>
	public static class Loader
	{
		#region Variables

		/// <summary>
		/// Stores the path of the executable file
		/// </summary>
		private static readonly string exePath;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes the <see cref="GPSRCmdGen.Loader"/> class.
		/// </summary>
		static Loader ()
		{
			Loader.exePath = AppDomain.CurrentDomain.BaseDirectory;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the path of the executable file
		/// </summary>
		public static string ExePath{get {return Loader.exePath;} }

		#endregion

		#region Methods

		/// <summary>
		/// Gets a full path for the given filename using the executable
		/// file path as base path path.
		/// </summary>
		/// <param name="fileName">The name of the file to combine into a path</param>
		/// <returns>A full path for the given fileName.</returns>
		public static string GetPath(string fileName){
			return Path.Combine (Loader.exePath, fileName);
		}

		/// <summary>
		/// Gets a full path for the given filename using the executable
		/// file path as base path path and a subdirectory.
		/// </summary>
		/// <param name="subdir">The name of the subdirectory that will contain the file</param>
		/// <param name="fileName">The name of the file to combine into a path</param>
		/// <returns>A full path for the given fileName.</returns>
		public static string GetPath(string subdir, string fileName){
			return Path.Combine (Path.Combine(Loader.exePath, subdir), fileName);
		}

		#endregion
	}
}

