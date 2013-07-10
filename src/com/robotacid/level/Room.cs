using System.Collections.Generic;

using com.robotacid.geom;

namespace com.robotacid.level {
	
	/**
	 * Needed a pixel version of a Rect for making rooms
	 * 
	 * Also need the capacity to theme rooms and identify the other rooms they are connected to
	 * for pacing
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class Room {
		
		public bool start;
		public int gridNum;
		public int x;
		public int y;
		public int width;
		public int height;
		public int id;
		public int num;
		public List<Room> siblings;
		public List<Pixel> doors;
///		public List<Surface> surfaces;
		
		public static int roomCount = 0;
		
		public Room(int x = 0, int y = 0, int width = 0, int height = 0, int id = 0) {
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.id = id;
			start = false;
			siblings = new List<Room>();
			doors = new List<Pixel>();
///			surfaces = new List<Surface>();
			num = roomCount++;
		}
		public bool touchesDoors(Pixel p) {
			for(int i = 0; i < doors.Count; i++){
				if(p.y == doors[i].y && (p.x == doors[i].x - 1 || p.x == doors[i].x + 1)) return true;
				if(p.x == doors[i].x && (p.y == doors[i].y - 1 || p.y == doors[i].y + 1)) return true;
			}
			return false;
		}
		/* Do two Rooms touch? */
		public bool touches(Room b) {
			return !(this.x > b.x + b.width || this.x + this.width < b.x || this.y > b.y + b.height || this.y + this.height < b.y);
		}
		/* Do two Rooms intersect? */
		public bool intersects(Room b) {
			return !(this.x > b.x + (b.width - 1) || this.x + (this.width - 1) < b.x || this.y > b.y + (b.height - 1) || this.y + (this.height - 1) < b.y);
		}
		/* Is this point inside the Room */
		public bool contains(int x, int y) {
			return x >= this.x && y >= this.y && x < this.x + width && y < this.y + height;
		}
	}
	
}