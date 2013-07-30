using redroguecs;

///import com.robotacid.ai.Brain;
///import com.robotacid.level.Map;
///import com.robotacid.gfx.BlitRect;
///import com.robotacid.gfx.BlitSprite;
///import com.robotacid.gfx.Renderer;
///import com.robotacid.engine.Character;
///import com.robotacid.engine.Entity;
///import com.robotacid.phys.Collider;
///import com.robotacid.util.array.getParams;
///import com.robotacid.util.array.protectedSplitArray;
///import com.robotacid.util.clips.startClips;
///import flash.display.BitmapData;
///import flash.display.DisplayObject;
///import flash.display.Sprite;
///import flash.display.MovieClip;
///import flash.filters.DropShadowFilter;
///import flash.geom.Point;
///import flash.geom.Rectangle;

namespace com.robotacid.engine {
	
	/**
	* Converts indices into MapObjects and their derivatives
	*
	* More complex indices have data attached to them in parenthesis ()
	*
	* @author Aaron Steed, robotacid.com
	*/
	public class MapTileConverter {
		
		// NOTE TO MAINTAINER
		//
		// These can't be made static due to some completely illegal references I'm getting away
		// with in ID_TO_GRAPHIC
		public Game game;
///		public var renderer:Renderer;
///		public var mapTileManager:MapTileManager;
		
///		private var item:*;
		private int n;
///		private var mc:DisplayObject;
		
		public bool forced;
///		public var data:Array;
///		public string params;
		private int i;
		private int j;
		private int id;
		private int dir;
///		private var array:Array;
///		private var tile:*;
		
		private const int UP = 1;
		private const int RIGHT = 2;
		private const int DOWN = 4;
		private const int LEFT = 8;
		
		public const int WALL = 1;
		public const int EMPTY = 0;
		
///		public static const IN_PARENTHESIS:RegExp =/(?<=\().*(?=\))/;
		
		// these are just here to stop me writing a load of magic numbers down when map generating
		// awkwardly enough, at work I just use the strings of classes - but I'm gonna just stay
		// old school with this one and have a smidgin more speed from not raping getQualifiedClassName
		
		public const int LADDER = 13;
		public const int LADDER_TOP = 14;
		public const int LEDGE = 15;
		public const int LEDGE_SINGLE = 16;
		public const int LEDGE_MIDDLE = 17;
		public const int LEDGE_START_LEFT = 18;
		public const int LEDGE_START_RIGHT = 19;
		public const int LEDGE_END_LEFT = 20;
		public const int LEDGE_END_RIGHT = 21;
		public const int LEDGE_START_LEFT_END = 22;
		public const int LEDGE_START_RIGHT_END = 23;
		
		public const int LADDER_LEDGE = 24;
		public const int LADDER_LEDGE_SINGLE = 25;
		public const int LADDER_LEDGE_MIDDLE = 26;
		public const int LADDER_LEDGE_START_LEFT = 27;
		public const int LADDER_LEDGE_START_RIGHT = 28;
		public const int LADDER_LEDGE_END_LEFT = 29;
		public const int LADDER_LEDGE_END_RIGHT = 30;
		public const int LADDER_LEDGE_START_LEFT_END = 31;
		public const int LADDER_LEDGE_START_RIGHT_END = 32;
		
		public const int LADDER_TOP_LEDGE = 33;
		public const int LADDER_TOP_LEDGE_SINGLE = 34;
		public const int LADDER_TOP_LEDGE_MIDDLE = 35;
		public const int LADDER_TOP_LEDGE_START_LEFT = 36;
		public const int LADDER_TOP_LEDGE_START_RIGHT = 37;
		public const int LADDER_TOP_LEDGE_END_LEFT = 38;
		public const int LADDER_TOP_LEDGE_END_RIGHT = 39;
		public const int LADDER_TOP_LEDGE_START_LEFT_END = 40;
		public const int LADDER_TOP_LEDGE_START_RIGHT_END = 41;
		
		public const int PIPE_CORNER_RIGHT_DOWN = 42;
		public const int PIPE_HORIZ1 = 43;
		public const int PIPE_CROSS = 44;
		public const int PIPE_T_LEFT_DOWN_RIGHT = 45;
		public const int PIPE_T_UP_RIGHT_DOWN = 46;
		public const int PIPE_HORIZ2 = 47;
		public const int PIPE_CORNER_LEFT_UP = 48;
		public const int PIPE_VERT1 = 49;
		public const int PIPE_T_LEFT_UP_DOWN = 50;
		public const int PIPE_VERT2 = 51;
		public const int PIPE_T_RIGHT_UP_LEFT = 52;
		public const int PIPE_CORNER_LEFT_DOWN = 53;
		public const int PIPE_CORNER_UP_RIGHT = 54;
		
		public const int STAIRS_UP = 58;
		public const int STAIRS_DOWN = 59;
		public const int HEAL_STONE = 60;
		public const int GRIND_STONE = 61;
		
		public const int RAT = 62;
		public const int SPIDER = 63;
		public const int BAT = 64;
		public const int COG = 65;
		public const int COG_RAT = 66;
		public const int COG_SPIDER = 67;
		public const int COG_BAT = 68;
		
		public const int PILLAR_BOTTOM = 69;
		public const int PILLAR_MID1 = 70;
		public const int PILLAR_MID2 = 71;
		public const int PILLAR_TOP = 72;
		public const int PILLAR_SINGLE1 = 73;
		public const int PILLAR_SINGLE2 = 74;
		public const int CHAIN_MID = 75;
		public const int CHAIN_BOTTOM = 76;
		public const int CHAIN_TOP = 77;
		public const int RECESS = 78;
		public const int OUTLET = 79;
		public const int DRAIN = 80;
		public const int STALAGMITE1 = 81;
		public const int STALAGMITE2 = 82;
		public const int STALAGMITE3 = 83;
		public const int STALAGMITE4 = 84;
		public const int STALAGTITE1 = 85;
		public const int STALAGTITE2 = 86;
		public const int STALAGTITE3 = 87;
		public const int STALAGTITE4 = 88;
		public const int CRACK1 = 89;
		public const int CRACK2 = 90;
		public const int CRACK3 = 91;
		public const int SKULL = 92;
		public const int GROWTH1 = 93;
		public const int GROWTH2 = 94;
		public const int GROWTH3 = 95;
		public const int GROWTH4 = 96;
		public const int GROWTH5 = 97;
		public const int GROWTH6 = 98;
		public const int GROWTH7 = 99;
		public const int GROWTH8 = 100;
		public const int GROWTH9 = 101;
		public const int GROWTH10 = 102;
		public const int GROWTH11 = 103;
		public const int GROWTH12 = 104;
		public const int STAIRS_UP_GFX = 105;
		public const int STAIRS_DOWN_GFX = 106;
		
#if false
		// These references are technically illegal. Game.game doesn't even exist yet, but some how the
		// compiler is letting the issue slide so long as I don't static reference Game
		public static var ID_TO_GRAPHIC:Array = [
			"",						// 0
			new BlitRect(0, 0, Game.SCALE, Game.SCALE, 0xFF000000),// wall
			,
			,
			,//test collider
			,// 5
			,
			,
			,
			new BlitSprite(new Game.game.library.BackB1),
			new BlitSprite(new Game.game.library.BackB2),// 10
			new BlitSprite(new Game.game.library.BackB3),
			new BlitSprite(new Game.game.library.BackB4),
			new BlitSprite(new Game.game.library.LadderB),
			new BlitSprite(new Game.game.library.LadderTopB),
			new BlitSprite(new LedgeMC9),// 15
			new BlitSprite(new LedgeMC4),
			new BlitSprite(new LedgeMC1),
			new BlitSprite(new LedgeMC6),
			new BlitSprite(new LedgeMC8),
			new BlitSprite(new LedgeMC2),//20
			new BlitSprite(new LedgeMC3),
			new BlitSprite(new LedgeMC5),
			new BlitSprite(new LedgeMC7),
			// ladder ledge combos - LadderB is painted over these next 9
			new BlitSprite(new LedgeMC9),
			new BlitSprite(new LedgeMC4),// 25
			new BlitSprite(new LedgeMC1),
			new BlitSprite(new LedgeMC6),
			new BlitSprite(new LedgeMC8),
			new BlitSprite(new LedgeMC2),
			new BlitSprite(new LedgeMC3),// 30
			new BlitSprite(new LedgeMC5),
			new BlitSprite(new LedgeMC7),
			// ladder top ledge combos - LadderTopB is painted over these next 9
			new BlitSprite(new LedgeMC9),
			new BlitSprite(new LedgeMC4),
			new BlitSprite(new LedgeMC1),// 35
			new BlitSprite(new LedgeMC6),
			new BlitSprite(new LedgeMC8),
			new BlitSprite(new LedgeMC2),
			new BlitSprite(new LedgeMC3),
			new BlitSprite(new LedgeMC5),// 40
			new BlitSprite(new LedgeMC7),
			new BlitSprite(new Game.game.library.PipeB1),
			new BlitSprite(new Game.game.library.PipeB2),
			new BlitSprite(new Game.game.library.PipeB3),
			new BlitSprite(new Game.game.library.PipeB4),// 45
			new BlitSprite(new Game.game.library.PipeB5),
			new BlitSprite(new Game.game.library.PipeB6),
			new BlitSprite(new Game.game.library.PipeB7),
			new BlitSprite(new Game.game.library.PipeB8),
			new BlitSprite(new Game.game.library.PipeB9),// 50
			new BlitSprite(new Game.game.library.PipeB10),
			new BlitSprite(new Game.game.library.PipeB11),
			new BlitSprite(new Game.game.library.PipeB12),
			new BlitSprite(new Game.game.library.PipeB13),
			,//55
			,
			,
			StairsUpMC,
			StairsDownMC,
			Sprite,//60
			Sprite,
			RatMC,
			SpiderMC,
			BatMC,
			CogMC,//65
			CogMC,
			CogMC,
			CogMC,
			,
			,//70
			,
			,
			,
			,
			,//75
			,
			,
			new BlitSprite(new RecessDecorMC),
			new BlitSprite(new OutletDecorMC),
			new BlitSprite(new DrainDecorMC),//80
			,
			,
			,
			,
			,//85
			,
			,
			,
			,
			,//90
			,
			new BlitSprite(new SkullDecorMC),
			,
			,
			,//95
			,
			,
			,
			,
			,//100
			,
			,
			,
			,
			new BlitSprite(new StairsUpMC),
			new BlitSprite(new StairsDownMC)
		];
		
		public function MapTileConverter(r:MapTileManager, game:Game, renderer:Renderer) {
			this.mapTileManager = r;
			this.game = game;
			this.renderer = renderer;
			
		}
#endif
		
		public static bool preProcessed = false;
		
		/* Do any preprocessing needed on the BlitSprites */
		public static void init(){
			if(preProcessed) return;
#if false
			int i;
			Point point = new Point();
			for(i = 15; i <= 41; i++){
				ID_TO_GRAPHIC[i].resize(0, 0, 16, 16);
				(ID_TO_GRAPHIC[i].data as BitmapData).applyFilter(ID_TO_GRAPHIC[i].data, ID_TO_GRAPHIC[i].rect, point, new DropShadowFilter(1, 90, 0, 0.3, 0, 3, 1));
			}
			for(i = 24; i <= 32; i++){
				ID_TO_GRAPHIC[i].add(ID_TO_GRAPHIC[LADDER]);
			}
			for(i = 33; i <= 41; i++){
				ID_TO_GRAPHIC[i].add(ID_TO_GRAPHIC[LADDER_TOP]);
			}
			// create background graphics
			var mc:MovieClip;
			mc = new PillarDecorMC();
			for(i = 0; i < mc.totalFrames; i++){
				mc.gotoAndStop(i + 1);
				ID_TO_GRAPHIC[PILLAR_BOTTOM + i] = new BlitSprite(mc);
			}
			mc = new ChainDecorMC();
			for(i = 0; i < mc.totalFrames; i++){
				mc.gotoAndStop(i + 1);
				ID_TO_GRAPHIC[CHAIN_MID + i] = new BlitSprite(mc);
			}
			mc = new StalagmiteDecorMC();
			for(i = 0; i < mc.totalFrames; i++){
				mc.gotoAndStop(i + 1);
				ID_TO_GRAPHIC[STALAGMITE1 + i] = new BlitSprite(mc);
			}
			mc = new CrackDecorMC();
			for(i = 0; i < mc.totalFrames; i++){
				mc.gotoAndStop(i + 1);
				ID_TO_GRAPHIC[CRACK1 + i] = new BlitSprite(mc);
			}
			mc = new CrackDecorMC();
			for(i = 0; i < mc.totalFrames; i++){
				mc.gotoAndStop(i + 1);
				ID_TO_GRAPHIC[CRACK1 + i] = new BlitSprite(mc);
			}
			mc = new GrowthDecorMC();
			for(i = 0; i < mc.totalFrames; i++){
				mc.gotoAndStop(i + 1);
				ID_TO_GRAPHIC[GROWTH1 + i] = new BlitSprite(mc);
			}
#endif
			preProcessed = true;
		}
		
#if false
		/* Converts a number in one of the map layers into a MapObject or a MovieClip or Sprite
		 *
		 * When createTile finds an array of information to convert it will return a stacked array
		 */
		public function createTile(x:int, y:int):*{
			if(!mapTileManager.map[y]){
				trace("out of bounds y "+y+" "+mapTileManager.height);
			}
			if(!mapTileManager.map[y][x]) return null;
			
			if(mapTileManager.map[y][x] is Array){
				array = mapTileManager.map[y][x];
				tile = [];
				var temp:*;
				for(i = 0; i < array.length; i++){
					// some tiles convert without returning data, they manage converting themselves
					// back into map indices on their own - we don't give these to the renderer
					temp = convertIndicesToObjects(x, y, array[i]);
					if(temp) tile.push(temp);
				}
				if(tile.length == 0) tile = null;
			} else {
				tile = convertIndicesToObjects(x, y, mapTileManager.map[y][x])
			}
			// clear map position - the object is now roaming in the engine
			if(!mapTileManager.bitmapLayer) mapTileManager.map[y][x] = null;
			return tile;
			
		}
		
		public function convertIndicesToObjects(x:int, y:int, obj:*):*{
			if(obj is Entity){
				if(obj.addToEntities) game.entities.push(obj);
				obj.active = true;
				if(obj is Chest || obj is Altar){
					game.items.push(obj);
				} else if(obj is Portal){
					game.portals.push(obj);
				} else if(obj is Torch){
					obj.mapInit();
					game.torches.push(obj);
				} else if(obj is ColliderEntity){
					game.world.restoreCollider(obj.collider);
					if(obj is Character){
						obj.restoreEffects();
						if(obj is Monster){
							Brain.monsterCharacters.push(obj);
							if(!obj.mapInitialised) obj.mapInit();
						} else if(obj is MinionClone){
							Brain.playerCharacters.push(obj);
						}
					} else if(obj is Item){
						game.items.push(obj);
					} else if(obj is ChaosWall){
						game.chaosWalls.push(obj);
					}
				}
				
				if(!obj.free) return obj;
			}
			//trace(r.mapArray[y][x]);
			
			id = int(obj);
			if (!obj || id == 0) return null;
			
			// is this id a Blit object?
			if(mapTileManager.bitmapLayer){
				return ID_TO_GRAPHIC[id];
			}
			n = x + y * mapTileManager.width;
			// generate MovieClip
			if(id > 0 && ID_TO_GRAPHIC[id]){
				mc = new ID_TO_GRAPHIC[id];
			}
			if(mc != null){
				mc.x = x * mapTileManager.scale;
				mc.y = y * mapTileManager.scale;
			}
			
			// objects defined by index and created on the fly
			
			if(id == STAIRS_UP){
				// stairs up
				item = new Portal(mc, new Rectangle(x * Game.SCALE, y * Game.SCALE, Game.SCALE, Game.SCALE), Portal.STAIRS, game.map.level - 1, game.map.level == 1 ? Map.AREA : Map.MAIN_DUNGEON);
				if(Map.isPortalToPreviousLevel(x, y, Portal.STAIRS, item.targetLevel, item.targetType)) game.entrance = item;
			} else if(id == STAIRS_DOWN){
				// stairs down
				if(game.map.level == Map.OVERWORLD && game.map.type == Map.AREA){
					mc = new OverworldStairsMC();
					mc.x = x * mapTileManager.scale;
					mc.y = y * mapTileManager.scale;
				}
				item = new Portal(mc, new Rectangle(x * Game.SCALE, y * Game.SCALE, Game.SCALE, Game.SCALE), Portal.STAIRS, game.map.level + 1, Map.MAIN_DUNGEON);
				if(Map.isPortalToPreviousLevel(x, y, Portal.STAIRS, item.targetLevel, item.targetType)) game.entrance = item;
			} else if(id == HEAL_STONE){
				item = new Stone(x * Game.SCALE, y * Game.SCALE, Stone.HEAL);
			} else if(id == GRIND_STONE){
				item = new Stone(x * Game.SCALE, y * Game.SCALE, Stone.GRIND);
			} else if(id == RAT){
				item = new Critter(mc, (x + 0.5) * Game.SCALE, (y + 1) * Game.SCALE, Critter.RAT);
			} else if(id == SPIDER){
				item = new Critter(mc, (x + 0.5) * Game.SCALE, (y + 0.5) * Game.SCALE, Critter.SPIDER);
			} else if(id == BAT){
				item = new Critter(mc, (x + 0.5) * Game.SCALE, y * Game.SCALE, Critter.BAT);
			} else if(id == COG){
				item = new Critter(mc, (x + 0.5) * Game.SCALE, (y + 0.5) * Game.SCALE, Critter.COG);
			} else if(id == COG_RAT){
				item = new Critter(mc, (x + 0.5) * Game.SCALE, (y + 1) * Game.SCALE, Critter.COG | Critter.RAT);
			} else if(id == COG_SPIDER){
				item = new Critter(mc, (x + 0.5) * Game.SCALE, (y + 0.5) * Game.SCALE, Critter.COG | Critter.SPIDER);
			} else if(id == COG_BAT){
				item = new Critter(mc, (x + 0.5) * Game.SCALE, y * Game.SCALE, Critter.COG | Critter.BAT);
			}
			
			// just gfx?
			else {
				item = new Entity(mc);
			}
			
			if(item != null){
				item.mapX = item.initX = x;
				item.mapY = item.initY = y;
				item.mapZ = mapTileManager.currentLayer;
				item.tileId = mapTileManager.map[y][x];
				if(item is ColliderEntity){
					game.world.restoreCollider(item.collider);
				}
				if(!item.free){
					return item;
				}
			}
			return null;
		}
		
		/* Get block properties for a location */
		public static function getMapProperties(n:*):int {
			// map location has parameters
			if(!(n >= 0 || n <= 0) && n is String) {
				n = n.match(/\d+/)[0];
			}
			
			if(n == LADDER) return Collider.LADDER;
			if(n == LADDER_TOP) return 0;
			if(n >= 15 && n <= 23) return Collider.UP | Collider.LEDGE;
			if(n >= 33 && n <= 41) return Collider.UP | Collider.LEDGE;
			if(n >= 24 && n <= 32) return Collider.LADDER | Collider.LEDGE | Collider.UP;
			if(n > 0) return Collider.SOLID | Collider.WALL;
			
			return 0;
		}
#endif
		
		/* Get a tile index for a pipe graphic based on directions the pipes are supposed to lead out of a tile */
		public static int getPipeTileIndex(int dirs) {
			if(dirs == (UP | RIGHT)) return PIPE_CORNER_UP_RIGHT;
			else if(dirs == (UP | LEFT)) return PIPE_CORNER_LEFT_UP;
			else if(dirs == (DOWN | LEFT)) return PIPE_CORNER_LEFT_DOWN;
			else if(dirs == (DOWN | RIGHT)) return PIPE_CORNER_RIGHT_DOWN;
			else if(dirs == (UP | RIGHT | LEFT)) return PIPE_T_RIGHT_UP_LEFT;
			else if(dirs == (DOWN | RIGHT | LEFT)) return PIPE_T_LEFT_DOWN_RIGHT;
			else if(dirs == (UP | RIGHT | DOWN)) return PIPE_T_UP_RIGHT_DOWN;
			else if(dirs == (DOWN | UP | LEFT)) return PIPE_T_LEFT_UP_DOWN;
			else if(dirs == (DOWN | UP)) return Game.game.random.coinFlip() ? PIPE_VERT1 : PIPE_VERT2;
			else if(dirs == (LEFT | RIGHT)) return Game.game.random.coinFlip() ? PIPE_HORIZ1 : PIPE_HORIZ2;
			else if(dirs == (DOWN | UP | LEFT | RIGHT)) return PIPE_CROSS;
			return 0;
		}
		
	}
	
}