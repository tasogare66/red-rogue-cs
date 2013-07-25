using redroguecs;

using com.robotacid.phys;
///import flash.display.DisplayObject;
///import flash.geom.Rectangle;

namespace com.robotacid.engine {
	
	/**
	 * Base Entity for all objects that use Colliders
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class ColliderEntity : Entity {
		
		public Collider collider;
		
///		public ColliderEntity(gfx:DisplayObject, bool addToEntities = true) {
		public ColliderEntity(bool addToEntities = true) : base(true, addToEntities) {
			//super(gfx, true, addToEntities);
			
		}
		
		/* Initialises the collider for this Entity */
		public void createCollider(double x, double y, int properties, int ignoreProperties, int state = 0, bool positionByBase = true){
#if false
			var bounds:Rectangle = gfx.getBounds(gfx);
			if(positionByBase){
				collider = new Collider(x - bounds.width * 0.5, y - bounds.height, bounds.width, bounds.height, Game.SCALE, properties, ignoreProperties, state);
			} else {
				collider = new Collider(x + bounds.left, y + bounds.top, bounds.width, bounds.height, Game.SCALE, properties, ignoreProperties, state);
			}
			collider.userData = this;
#endif
			mapX = (int)((collider.x + collider.width * 0.5) * Game.INV_SCALE);
			mapY = (int)((collider.y + collider.height * 0.5) * Game.INV_SCALE);
		}
		
		override public void remove() {
			if(collider.world != null) collider.world.removeCollider(collider);
			base.remove();
		}
		
	}

}