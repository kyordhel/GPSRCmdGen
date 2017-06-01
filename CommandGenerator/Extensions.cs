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
		public static TSource PopLast<TSource>(this IList<TSource> source)
		{
			if (source == null)
				throw new ArgumentNullException ("Source is null.");
			if (source.Count < 1)
				throw new InvalidOperationException ("The source sequence is empty.");
			TSource item = source[source.Count - 1];
			source.RemoveAt (source.Count - 1);
			return item;
		}

		public static TSource PopFirst<TSource>(this IList<TSource> source, Func<TSource, bool> predicate)
		{
			if ((source == null) || (predicate == null))
				throw new ArgumentNullException ("Source or predicate is null.");
			if (source.Count < 1)
				throw new InvalidOperationException ("The source sequence is empty.");
			for (int i = 0; i < source.Count; ++i) {
				if (predicate (source [i])) {
					TSource item = source [i];
					source.RemoveAt(i);
					return item;
				}
			}
			throw new InvalidOperationException ("No element satisfies the condition in predicate");
		}
	}
}

