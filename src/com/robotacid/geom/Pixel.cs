namespace com.robotacid.geom {
	
	/**
	 * ...
	 * @author Aaron Steed, robotacid.com
	 */
	public class Pixel {
		
		public int x;
		public int y;
		
		public Pixel(int x = 0, int y = 0) {
			this.x = x;
			this.y = y;
		}
		/* Manhattan distance */
		public int mDist(Pixel p) {
			return (p.x < x ? x - p.x : p.x - x) + (p.y < y ? y - p.y : p.y - y);
		}
		public string toString() {
			return "(" + x + "," + y + ")";
		}
		public Pixel copy() {
			return new Pixel(x, y);
		}
		
	}
	
}