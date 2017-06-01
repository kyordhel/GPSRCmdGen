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

		/// <summary>
		/// Returns and removes the first element in a sequence that satisfies a specified condition.
		/// </summary>
		/// <returns>The first element in the sequence that passes the test in the specified predicate function.</returns>
		/// <param name="source">An IList<T> to return and remove an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
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

		/// <summary>
		/// Determines whether the specified String object has the same value of any of an array of other string objects.
		/// </summary>
		/// <returns><c>true</c> if the value of the source parameter is equal to the value of any of the strings contained in the others parameter; otherwise, <c>false</c>.</returns>
		/// <param name="source">The first string to compare, or null</param>
		/// <param name="others">The set of strings to compare, or null.</param>
		public static bool IsAnyOf(this string source, params string[] others){
			foreach (string o in others)
<<<<<<< c5276dab489b25422713f7eb8487acc2826e9733
				if (String.Equals (source, o))
=======
				if (String.Equals (s, o))
>>>>>>> More extension methods
					return true;
			return false;
		}

		/// <summary>
		/// Determines whether the specified String object has the same value of any of an array of other string objects. A parameter specifies the culture, case, and sort rules used in the comparison.
		/// </summary>
		/// <returns><c>true</c> if the value of the source parameter is equal to the value of any of the strings contained in the others parameter; otherwise, <c>false</c>.</returns>
		/// <param name="source">The first string to compare, or null</param>
		/// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison. </param>
		/// <param name="others">The set of strings to compare, or null.</param>
		public static bool IsAnyOf(this string source, StringComparison comparisonType, params string[] others){
			foreach (string o in others)
<<<<<<< c5276dab489b25422713f7eb8487acc2826e9733
				if (String.Equals (source, o, comparisonType))
=======
				if (String.Equals (s, o, comparisonType))
>>>>>>> More extension methods
					return true;
			return false;
		}
	}
}

