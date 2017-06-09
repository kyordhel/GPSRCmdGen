using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using RoboCup.AtHome.CommandGenerator.ReplaceableTypes;
using Object = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.Object;

namespace RoboCup.AtHome.CommandGenerator
{
	public class ObjectManager : IEnumerable<Category>
	{
		private Dictionary<string, Category> categories;

		private ObjectManager ()
		{
			this.categories = new Dictionary<string, Category>();
		}

		public Location GetCategoryLocation (Category category)
		{
			if ((category == null) || !this.categories.ContainsKey(category.Name))
				return null;
			return this.categories[category.Name].DefaultLocation;
		}

		internal List<Category> Categories{ get{ return new List<Category>(this.categories.Values);} }

		/// <summary>
		/// Gets a lists with all the objects in the container. This is an O(n) operation
		/// </summary>
		public List<Object> Objects
		{
			get
			{
				List<Object> objects = new List<Object>(100);
				foreach (Category c in this.categories.Values)
				{
					foreach (Object o in c.Objects)
						objects.Add(o);
				}
				return objects;
			}
		}

		/// <summary>
		/// Returns the number of GPSRObjects in the collection
		/// </summary>
		public int CategoryCount { get { return this.categories.Count; } }

		#region ICollection implementation

		public void Add (Category item)
		{
			if (item == null)
				return;
			if (!this.categories.ContainsKey (item.Name)) {
				this.categories.Add (item.Name, item);
				return;
			}
			Category category = this.categories[item.Name];
			foreach (Object o in item.Objects) {
				if (category.Contains (o.Name))
					continue;
				o.Category = item;
				category.AddObject (o);
			}
		}

		public void Add (Object item)
		{
			if (item == null)
				return;
			if (item.Category == null)
				throw new ArgumentException ("Cannot add objects without a category");
			if (!this.categories.ContainsKey(item.Category.Name))
				this.Add(item.Category);
			else this.categories[item.Category.Name].AddObject(item);
		}

		public void Clear ()
		{
			this.categories.Clear ();
		}

		public bool Contains (Category item)
		{
			if (item == null) return false;
			return this.categories.ContainsKey (item.Name);
		}

		public bool Contains (Location item)
		{
			if (item == null) return false;
			foreach (Category c in this.categories.Values)
			{
				if (c.LocationString == item.Name)
					return true;
			}
			return false;
		}

		public bool Contains (Object item)
		{
			foreach (Category c in this.categories.Values)
			{
				if (c.Contains(item.Name))
					return true;
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (Category c in this.categories.Values){
				sb.Append(c.Name);
				sb.Append(" (");
				sb.Append(c.ObjectCount);
				sb.Append("), ");
			}
			if (sb.Length > 2) sb.Length -= 2;
			return sb.ToString();
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator<Category> System.Collections.Generic.IEnumerable<Category>.GetEnumerator ()
		{
			return this.categories.Values.GetEnumerator ();
		}

		/*

		IEnumerator<GPSRObject> System.Collections.Generic.IEnumerable<GPSRObject>.GetEnumerator ()
		{
			return this.objects.GetEnumerator ();
		}

		IEnumerator<Location> System.Collections.Generic.IEnumerable<Location>.GetEnumerator ()
		{
			List<Location> l = new List<Location> (this.categories.Count);
			for (int i = 0; i < this.categories.Count; ++i)
				l.Add (this.categories [i].DefaultLocation);
			return l.GetEnumerator ();
		}
		
		*/

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return this.categories.Values.GetEnumerator ();
		}
		#endregion

		#region Singleton

		private static ObjectManager instance;
		static ObjectManager() { instance = new ObjectManager(); }

		public static ObjectManager Instance { get { return instance; } }

		#endregion
	}
}

