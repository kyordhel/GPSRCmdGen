using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Room = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.Room;
using Location = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.Location;
using SpecificLocation = RoboCup.AtHome.CommandGenerator.ReplaceableTypes.SpecificLocation;


namespace RoboCup.AtHome.CommandGenerator
{
	public class LocationManager : IEnumerable<Location> //, ICollection<Room>, ICollection<SpecificLocation>, 
	{
		private Dictionary<string, Room> rooms;

		private LocationManager()
		{
			this.rooms = new Dictionary<string, Room>();
		}

		internal List<Room> Rooms { get { return new List<Room>(this.rooms.Values); } }

		/// <summary>
		/// Returns the number of rooms in the collection
		/// </summary>
		public int RoomCount { get { return rooms.Count; } }

		#region ICollection implementation

		public void Add(Room item)
		{
			if (item == null)
				return;
			if (!this.rooms.ContainsKey(item.Name))
			{
				this.rooms.Add(item.Name, item);
				return;
			}
			Room room = this.rooms[item.Name];
			foreach (SpecificLocation location in item.Locations)
			{
				if (room.Contains(location.Name))
					continue;
				location.Room = room;
				room.AddLocation(location);
			}
		}

		public void Add(SpecificLocation item)
		{
			if (item == null)
				return;
			if (item.Room == null)
				throw new ArgumentException("Cannot add locations without a room");
			if (!this.rooms.ContainsKey(item.Room.Name))
				this.Add(item.Room);
			else this.rooms[item.Room.Name].AddLocation(item);
		}

		public void Clear()
		{
			foreach (Room room in rooms.Values)
				room.Clear();
			this.rooms.Clear();
		}

		public bool Contains(Room item)
		{
			if (item == null) return false;
			return this.rooms.ContainsKey(item.Name);
		}

		public bool Contains(SpecificLocation item)
		{
			if (item == null) return false;
			foreach (Room room in rooms.Values)
			{
				if (room.Contains(item.Name))
					return true;
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (Room r in this.rooms.Values)
			{
				sb.Append(r.Name);
				sb.Append(" (");
				sb.Append(r.LocationCount);
				sb.Append("), ");
			}
			if (sb.Length > 2) sb.Length -= 2;
			return sb.ToString();
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator<Location> System.Collections.Generic.IEnumerable<Location>.GetEnumerator()
		{
			List<Location> l = new List<Location>(100);
			foreach (Room r in this.rooms.Values)
			{
				l.Add(r);
				foreach (Location b in r.Locations)
					l.Add(b);
				foreach (Location p in r.Locations)
					l.Add(p);
			}
			return l.GetEnumerator();
		}

		/*

		IEnumerator<Room> System.Collections.Generic.IEnumerable<Room>.GetEnumerator()
		{
			return this.rooms.GetEnumerator();
		}

		IEnumerator<SpecificLocation> System.Collections.Generic.IEnumerable<SpecificLocation>.GetEnumerator()
		{
			return this.locations.GetEnumerator();
		}

		*/

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.rooms.GetEnumerator();
		}

		#endregion

		#region Singleton

		private static LocationManager instance;
		static LocationManager() { instance = new LocationManager(); }

		public static LocationManager Instance { get { return instance; } }

		#endregion
	}
}

