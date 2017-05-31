using System;
using System.Collections.Generic;

namespace RoboCup.AtHome.CommandGenerator
{
	/// <summary>
	/// Helper class that contains extension methods
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Shuffles the list using the Fisher-Yates shuffle
		/// </summary>
		/// <param name="list">The list to be shuffled</param>
		/// <param name="rnd">a random number generator</param>
		/// <typeparam name="T">The data type of the list.</typeparam>
		public static void Shuffle<T>(this IList<T> list, Random rnd)  
		{   
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rnd.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}

		/// <summary>
		/// Retrieves and removes the last element in the list 
		/// </summary>
		/// <param name="list">The list from which the last element will be extracted</param>\
		/// <typeparam name="T">The data type of the list.</typeparam>
		public static T PopLast<T>(this IList<T> list)
		{
			T item = list [list.Count - 1];
			list.RemoveAt (list.Count - 1);
			return item;
		}
	}
}

