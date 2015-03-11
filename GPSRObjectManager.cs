using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	public class GPSRObjectManager : ICollection<Category>, ICollection<GPSRObject>, IEnumerable<Location>
	{
		private List<Category> categories;
		private List<GPSRObject> objects;

		public GPSRObjectManager ()
		{
			this.categories = new List<Category> ();
			this.objects = new List<GPSRObject> ();
		}

		Location GetCategoryLocation (Category category)
		{
			for (int i = 0; i < this.categories.Count; ++i) {
				if (category.Name == this.categories [i].Name)
					return this.categories [i].DefaultLocation;
			}
			return null;
		}

		[XmlArray("categories")]
		[XmlArrayItem("category")]
		internal List<Category> Categories{
			get{ return this.categories;}
		}

		[XmlIgnore]
		public List<GPSRObject> Objects { get{return this.objects;} }

		#region ICollection implementation

		public void Add (Category item)
		{
			if (item == null)
				return;
			if (!this.categories.Contains (item)) {
				this.categories.Add (item);
			}
			foreach (GPSRObject o in item.Objects) {
				if (this.objects.Contains (o))
					continue;
				this.objects.Add (o);
				// Fix category
				o.Category = item;
			}
		}

		public void Add (GPSRObject item)
		{
			if (item == null)
				return;
			if (item.Category == null)
				throw new ArgumentException ("Cannot add objects without a category");
			if (!this.categories.Contains (item.Category))
				this.Add (item.Category);
			else if (!this.objects.Contains (item)) {
				if (item.Category.DefaultLocation != this.GetCategoryLocation (item.Category))
					throw new Exception ("Category location mismatch");
				this.objects.Add (item);
			}
		}

		public void Clear ()
		{
			this.objects.Clear ();
			this.categories.Clear ();
		}

		public bool Contains (Category item)
		{
			return this.categories.Contains (item);
		}

		public bool Contains (Location item)
		{
			for (int i = 0; i < this.categories.Count; ++i) {
				if (item == this.categories [i].DefaultLocation)
					return true;
			}
			return false;
		}

		public bool Contains (GPSRObject item)
		{
			return this.objects.Contains (item);
		}

		public void CopyTo (Category[] array, int arrayIndex)
		{
			this.categories.CopyTo (array, arrayIndex);
		}

		public void CopyTo (GPSRObject[] array, int arrayIndex)
		{
			this.objects.CopyTo (array, arrayIndex);
		}

		public bool Remove (Category item)
		{
			if (this.categories.Remove (item)) {
				foreach (GPSRObject o in item.Objects)
					this.objects.Remove (o);
				return true;
			}
			return false;
		}

		public bool Remove (GPSRObject item)
		{
			if (item == null)
				return false;
			if (this.objects.Remove (item)) {
				this.categories.Remove (item.Category);
				return true;
			}
			return false;
		}

		public bool Remove (Location item)
		{
			throw new ArgumentException ("Locations can not be removed directly from this collection");
		}

		/// <summary>
		/// Returns the number of GPSRObjects in the collection
		/// </summary>
		public int Count { get { return objects.Count; } }

		public bool IsReadOnly { get { return false; } }

		#endregion

		#region IEnumerable implementation

		IEnumerator<Category> System.Collections.Generic.IEnumerable<Category>.GetEnumerator ()
		{
			return this.categories.GetEnumerator ();
		}

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

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return this.objects.GetEnumerator ();
		}

		#endregion
	}
}

