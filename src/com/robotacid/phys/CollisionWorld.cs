using System.Collections.Generic;

using App;

///import flash.display.Graphics;
///import flash.geom.Rectangle;

namespace com.robotacid.phys{
	
	/**
	 * Management object for platformer collisions - this makes installing a physics engine as simple as dragging
	 * and dropping com.robotacid.phys
	 *
	 * @author Aaron Steed, robotacid.com
	 */
	public class CollisionWorld{
		
		public double scale;
		public double invScale;
		public Rectangle bounds;
		public List<Collider> colliders;
///		public var forces:Vector.<Force>;
		//public var map:Vector.<Vector.<int>>;
		public int[][] map;
		public int width;
		public int height;
		
///		private var i:int;
		
///		public var debug:Graphics;
		
		
		/* Beyond a certain point, floating point math starts to fail because binary can't represent it.
		 * This causes errors in testing areas for Colliders - so we ignore values beyond a number of digits */
		public const double INTERVAL_TOLERANCE = 0.00000001d;
		
		public CollisionWorld(int width, int height, double scale){
			this.width = width;
			this.height = height;
			this.scale = scale;
			this.invScale = 1.0 / scale;
			//map = new Vector.<Vector.<int>>();
			map = new int[height][];
			int r, c;
			for(r = 0; r < height; r++){
				//map[r] = new Vector.<int>();
				map[r] = new int[width];
				for(c = 0; c < width; c++){
					map[r][c] = 0;
				}
			}
			bounds = new Rectangle(0, 0, width * scale, height * scale);
			colliders = new List<Collider>();
//			forces = new Vector.<Force>();
		}
		
#if false
		public function main():void{
			var collider:Collider, force:Force;
			for(i = forces.length - 1; i > -1; i--){
				force = forces[i];
				if(force.active) force.main();
				else forces.splice(i, 1);
			}
			for(i = 0; i < colliders.length; i++){
				collider = colliders[i];
				collider.pressure = 0;
				collider.upContact = collider.rightContact = collider.downContact = collider.leftContact = null;
			}
			for(i = 0; i < colliders.length; i++){
				collider = colliders[i];
				if(collider.awake) collider.main();
			}
			if(debug){
				for(i = 0; i < colliders.length; i++){
					collider = colliders[i];
					collider.draw(debug);
				}
			}
			
			for(i = colliders.length - 1; i > -1; i--){
				collider = colliders[i];
				if(collider.crushed){
					if(Boolean(collider.crushCallback)) collider.crushCallback();
					else removeCollider(collider);
				}
			}
		}
		
		/* Creates a new Collider in the simulation
		 *
		 * properties is set to the SOLID constant value - the compiler is refusing to compile a reference to the property */
		public function addCollider(x:Number, y:Number, width:Number, height:Number, properties:int = 15, ignoreProperties:int = 0, state:int = 0):Collider{
			throw new Error();
			// force the collider to be in the bounds of the map
			if(x < bounds.x) x = bounds.x;
			if(y < bounds.y) y = bounds.y;
			if(x + width > bounds.x + bounds.width) x = (bounds.x + bounds.width) - width;
			if(y + height > bounds.y + bounds.height) y = (bounds.y + bounds.height) - height;
			var collider:Collider = new Collider(x, y, width, height, this, properties, ignoreProperties, state);
			colliders.push(collider);
			
			if(collider.properties & Collider.MISSILE) throw new Error("");
			
			return collider;
		}
		
		/* Removes a position on "map" and divorces any colliders that have adopted the position as a parent */
		public function removeMapPosition(mapX:int, mapY:int):void{
			map[mapY][mapX] = 0;
			var i:int, collider:Collider;
			for(i = 0; i < colliders.length; i++){
				collider = colliders[i];
				if(collider.mapCollider && collider.parent == collider.mapCollider && collider.mapCollider.x == mapX * scale && collider.mapCollider.y == mapY * scale){
					collider.divorce();
				}
			}
		}
#endif
		
		/* Removes a Collider from the simulation */
		public void removeCollider(Collider collider){
			collider.divorce();
			//colliders.splice(colliders.indexOf(collider), 1);
			colliders.Remove ( collider );
			collider.world = null;
		}
		
		/* Brings a previously removed Collider back into the world */
		public void restoreCollider(Collider collider){
			colliders.Add(collider);
			collider.world = this;
		}
		
		/* Return a Collider that contains the coord x,y */
		public Collider getColliderAt(double x, double y){
			int i;
			Collider collider;
			for(i = 0; i < colliders.Count; i++){
				collider = colliders[i];
				// change priority x tests to y tests for a tall game
				if(x >= collider.x && x < collider.x + collider.width && y >= collider.y && y < collider.y + collider.height){
					return collider;
				}
			}
			return null;
		}
		
		/* Return all the Colliders that touch the rectangle "area", this method ignores the map
		 *
		 * -1 is equivalent to 0xFFFFFFFF, thus "properties" by default returns all objects */
		public List<Collider> getCollidersIn(Rectangle area, Collider ignore = null, int properties = -1, int ignoreProperties = 0){
			int i;
			Collider collider;
			List<Collider> result = new List<Collider>();
			for(i = 0; i < colliders.Count; i++){
				
				collider = colliders[i];
				
				// floating point error causes a lot of false positives, so that's
				// why I'm using a tolerance value to ignore those drifting values
				// at the end of the Number datatype
				if(
					collider != ignore &&
					((collider.properties & properties) != 0) &&
					!((collider.properties & ignoreProperties) != 0) &&
					collider.x + collider.width - INTERVAL_TOLERANCE > area.x &&
					area.x + area.width - INTERVAL_TOLERANCE > collider.x &&
					collider.y + collider.height - INTERVAL_TOLERANCE > area.y &&
					area.y + area.height - INTERVAL_TOLERANCE > collider.y
				){
					result.Add(collider);
				}
			}
			return result;
		}
		
#if false
		public function addForce(collider:Collider, x:Number = 0, y:Number = 0, dampingX:Number = 0.5, dampingY:Number = 0.5):Force{
			var force:Force = new Force(collider, x, y, dampingX, dampingY);
			forces.push(force);
			return force;
		}
#endif
	}
}