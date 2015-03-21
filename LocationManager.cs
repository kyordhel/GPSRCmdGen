using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GPSRCmdGen
{
	public class LocationManager : ICollection<Room>, ICollection<Beacon>, ICollection<Placement>, IEnumerable<Location>
	{
		private List<Room> rooms;
		private List<Beacon> beacons;
		private List<Placement> placements;

		public LocationManager()
		{
			this.rooms = new List<Room>();
			this.beacons = new List<Beacon>();
			this.placements = new List<Placement>();
		}

		public Room GetRoomOfLocation(string locationName)
		{
			for (int i = 0; i < this.placements.Count; ++i)
			{
				if (locationName == this.placements[i].Name)
					return this.placements[i].Room;
			}
			for (int i = 0; i < this.beacons.Count; ++i)
			{
				if (locationName == this.beacons[i].Name)
					return this.beacons[i].Room;
			}
			return null;
		}

		[XmlArray("rooms")]
		[XmlArrayItem("room")]
		internal List<Room> Rooms
		{
			get { return this.rooms; }
		}

		[XmlIgnore]
		public List<Beacon> Beacons { get { return this.beacons; } }

		[XmlIgnore]
		public List<Placement> Placements { get { return this.placements; } }

		#region ICollection implementation

		public void Add(Room item)
		{
			if (item == null)
				return;
			if (!this.rooms.Contains(item))
			{
				this.rooms.Add(item);
			}
			foreach (Beacon b in item.Beacons)
			{
				if (this.beacons.Contains(b))
					continue;
				this.beacons.Add(b);
				// Fix room
				b.Room = item;
			}
			foreach (Placement o in item.Placements)
			{
				if (this.placements.Contains(o))
					continue;
				this.placements.Add(o);
				// Fix room
				o.Room = item;
			}
		}

		public void Add(Beacon item)
		{
			if (item == null)
				return;
			if (item.Room == null)
				throw new ArgumentException("Cannot add beacons without a room");
			if (!this.rooms.Contains(item.Room))
				this.Add(item.Room);
			else if (!this.beacons.Contains(item))
				this.beacons.Add(item);
		}

		public void Add(Placement item)
		{
			if (item == null)
				return;
			if (item.Room == null)
				throw new ArgumentException("Cannot add placements without a room");
			if (!this.rooms.Contains(item.Room))
				this.Add(item.Room);
			else if (!this.placements.Contains(item))
				this.placements.Add(item);
		}

		public void Clear()
		{
			this.placements.Clear();
			this.beacons.Clear();
			this.rooms.Clear();
		}

		public bool Contains(Room item)
		{
			return this.rooms.Contains(item);
		}

		public bool Contains(Beacon item)
		{
			return this.beacons.Contains(item);
		}

		public bool Contains(Placement item)
		{
			return this.placements.Contains(item);
		}

		public void CopyTo(Room[] array, int arrayIndex)
		{
			this.rooms.CopyTo(array, arrayIndex);
		}

		public void CopyTo(Beacon[] array, int arrayIndex)
		{
			this.beacons.CopyTo(array, arrayIndex);
		}

		public void CopyTo(Placement[] array, int arrayIndex)
		{
			this.placements.CopyTo(array, arrayIndex);
		}

		public bool Remove(Room item)
		{
			if (this.rooms.Remove(item))
			{
				foreach (Beacon b in item.Beacons)
					this.beacons.Remove(b);
				foreach (Placement p in item.Placements)
					this.placements.Remove(p);
				return true;
			}
			return false;
		}

		public bool Remove(Beacon item)
		{
			if (item == null)
				return false;
			if (this.beacons.Remove(item))
			{
				this.rooms.Remove(item.Room);
				return true;
			}
			return false;
		}

		public bool Remove(Placement item)
		{
			if (item == null)
				return false;
			if (this.placements.Remove(item))
			{
				this.rooms.Remove(item.Room);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the number of rooms in the collection
		/// </summary>
		public int Count { get { return rooms.Count; } }

		public bool IsReadOnly { get { return false; } }

		#endregion

		#region IEnumerable implementation

		IEnumerator<Room> System.Collections.Generic.IEnumerable<Room>.GetEnumerator()
		{
			return this.rooms.GetEnumerator();
		}

		IEnumerator<Beacon> System.Collections.Generic.IEnumerable<Beacon>.GetEnumerator()
		{
			return this.beacons.GetEnumerator();
		}

		IEnumerator<Placement> System.Collections.Generic.IEnumerable<Placement>.GetEnumerator()
		{
			return this.placements.GetEnumerator();
		}

		IEnumerator<Location> System.Collections.Generic.IEnumerable<Location>.GetEnumerator()
		{
			List<Location> l = new List<Location>(this.rooms.Count + this.beacons.Count + this.placements.Count);
			foreach (Room r in this.rooms)
			{
				l.Add(r);
				foreach (Location b in r.Beacons)
					l.Add(b);
				foreach (Location p in r.Placements)
					l.Add(p);
			}
			return l.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.rooms.GetEnumerator();
		}

		#endregion
	}
}

