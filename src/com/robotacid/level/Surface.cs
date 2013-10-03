using System;

using Pixel = com.robotacid.geom.Pixel;
using com.robotacid.phys;
using flash.display;
using flash;

namespace com.robotacid.level {
	
	/**
	 * Describes a position on the map that has a supporting surface below it
	 * 
	 * The properties value describes the surface being stood on if a Collider is at map position x,y
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class Surface : Pixel {
		
		public static Vector< Vector<Surface> > map;
		public static Vector<Surface> surfaces;
		public static BitmapData fragmentationMap;
		public static uint entranceCol;
		
		public int properties;
		public Room room;
		public Boolean nearEntrance;
		
		public Surface(int x = 0, int y = 0, int properties = 0) : base(x, y) {
			//super(x, y);
			this.properties = properties;
			nearEntrance = false;
		}
		
		public static void initMap(int width, int height){
			int r, c, i;
			map = new Vector< Vector<Surface> >();
			for(r = 0; r < height; r++){
				map.push(new Vector<Surface>());
				for(c = 0; c < width; c++){
					map[r].push(null);
				}
			}
			surfaces = new Vector<Surface>();
			fragmentationMap = null;
			entranceCol = 0x0;
		}
		
		public static void removeSurface(int x, int y){
			if(map[y][x] != null){
				int n;
				Surface surface = map[y][x];
				map[y][x] = null;
				n = surfaces.IndexOf(surface);
				if(n > -1) surfaces.splice(n, 1);
				if(surface.room != null){
					n = surface.room.surfaces.IndexOf(surface);
					if(n > -1) surface.room.surfaces.splice(n, 1);
				}
			}
		}
		
		public static Surface getClosestSurface(int x, int y){
			return null;
		}
		
		/* Diagnositic illustration of the AI graph for the map */
		public static void draw(Graphics gfx, double scale, Pixel topLeft, Pixel bottomRight){
			int r, c, i;
			Surface surface;
			for(r = topLeft.y; r <= bottomRight.y; r++){
				for(c = topLeft.x; c <= bottomRight.x; c++){
					if(map[r][c] != null){
						surface = map[r][c];
						gfx.moveTo(surface.x * scale, (surface.y + 1) * scale);
						gfx.lineTo((surface.x + 1) * scale, (surface.y + 1) * scale);
						
						if(surface.properties == (Collider.SOLID | Collider.WALL)){
							gfx.moveTo(surface.x * scale, 2 + (surface.y + 1) * scale);
							gfx.lineTo((surface.x + 1) * scale, 2 + (surface.y + 1) * scale);
						}
						
						if(surface.room != null){
							gfx.moveTo(surface.x * scale, 2 + (surface.y + 1) * scale);
							gfx.lineTo(surface.room.x * scale, surface.room.y * scale);
						}
					}
				}
			}
		}
		
	}

}