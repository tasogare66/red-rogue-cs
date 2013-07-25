using System.Collections.Generic;

using redroguecs;

///import com.robotacid.gfx.Renderer;
///import flash.display.DisplayObject;
///import flash.display.DisplayObjectContainer;
///import flash.geom.Matrix;
///import flash.geom.Point;
///import flash.geom.Rectangle;

namespace com.robotacid.engine {
	
	/**
	 * Base game object
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class Entity {
		
		public static Game game;
///		public static var renderer:Renderer;
		
///		public var gfx:DisplayObject;
		public bool active;
		public bool addToEntities;
		public bool callMain;
		
		public int name;
		public int light;
		public List<uint> lightCols;
		public string tileId;
		public bool free = false;
		public int mapX, mapY, mapZ;
		public int initX, initY;
		
		// these are debug tools for differentiating between objects and their instantiation order
		public static int entityCount = 0;
		public int entityNum;
		
///		public static var matrix:Matrix = new Matrix();
		
		public const double SCALE = Game.SCALE;
		public const double INV_SCALE = Game.INV_SCALE;
		
///		public Entity(gfx:DisplayObject, bool free = false, bool addToEntities = true) {
		public Entity(bool free = false, bool addToEntities = true) {
///			this.gfx = gfx;
			this.free = free;
			this.addToEntities = addToEntities;
			active = true;
			callMain = false;
			light = 0;
			entityNum = entityCount++;
			if(addToEntities) game.entities.Add(this);
		}
		
		public void main() {
			
		}
		
		/* Called to make this object visible */
		public void render() {
#if false
			matrix = gfx.transform.matrix;
			matrix.tx -= renderer.bitmap.x;
			matrix.ty -= renderer.bitmap.y;
			renderer.bitmapData.draw(gfx, matrix, gfx.transform.colorTransform);
#endif
		}
		
		public void unpause() {
			
		}
		
		/* Remove from play and convert back into a map number.
		 * Free roaming encounters will want to pin themselves in a new locale
		 */
		public virtual void remove() {
			if(active){
				active = false;
#if false
				// if there is already content on the id map, then we convert that content into an array
				if(game.mapTileManager.mapLayers[mapZ][mapY][mapX]){
					if(game.mapTileManager.mapLayers[mapZ][mapY][mapX] is Array){
						game.mapTileManager.mapLayers[mapZ][mapY][mapX].push(this);
					} else {
						game.mapTileManager.mapLayers[mapZ][mapY][mapX] = [game.mapTileManager.mapLayers[mapZ][mapY][mapX], this];
					}
				} else game.mapTileManager.mapLayers[mapZ][mapY][mapX] = this;
#endif
			}
		}
		
		public string nameToString() {
			return "none";
		}
		
#if false
		public function toXML():XML{
			return <entity />;
		}
#endif
		
	}

}