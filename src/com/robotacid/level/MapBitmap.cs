﻿using System;
using System.Collections.Generic;
using redroguecs;
using App;

using com.robotacid.geom;
using com.robotacid.phys;
using com.robotacid.util;
using flash.display;
///import flash.display.BitmapData;
///import flash.display.Graphics;
using Rectangle = flash.geom.Rectangle;
using Point = flash.geom.Point;
using flash;

namespace com.robotacid.level {
	
	/**
	 * This class creates the level layout as a bitmap
	 *
	 * This gives us the advantage of testing connectivity using the floodFill method and it
	 * means in the event of debugging, it's a simple task to see what state the level is in
	 *
	 * @author Aaron Steed, robotacid.com
	 */
	public class MapBitmap : Bitmap{
		
		public static Game game;
		
		public int level;
		public int type;
		public int zone;
		public int size;
		
		public int pitTraps;
		
		public Vector<Room> rooms;
		public Vector<Pixel> gates;
		
		public Room leftSecretRoom;
		public Room rightSecretRoom;
		public double leftSecretWidth;
		public double rightSecretWidth;
		
		public Rectangle adjustedMapRect;
		
		// temp variables
		private int i, j, k, n, r, c, dir;
		private Node node;
		private Room room;
		
		// pacing variables - these keep a consistent scale for the level
		public int vertPace;
		public int horizPace;
		
		public static Array<Pixel> directions;
		
		public const int MIN_ROOM_WIDTH = 4;
		public const int MIN_ROOM_HEIGHT = 3;
		
		public const int LEDGE_LENGTH = 4;
		public const int LADDER_TREE_HEIGHT = 6;
		public const double LEDGINESS = 0.4d;
		public const double LADDERINESS = 0.1d;
		
		// All features in the level are represented by colours
		public const uint PATH = 0xFFFFFF88;
		public const uint NODE = 0xFFFF00FF;
		public const uint WALL = 0xFFFF0000;
		public const uint EMPTY = 0xFFFFFF00;
		public const uint DIGGING = 0xFF00FF00;
		public const uint TUNNELLING = 0xFF00CC00;
		public const uint LADDER = 0xFF0000FF;
		public const uint LEDGE = 0xFF00FF00;
		public const uint LADDER_LEDGE = 0xFF00FFFF;
		public const uint GATE = 0xFFF0F0F0;
		public const uint PIT = 0xFFFFFFFF;
		public const uint SECRET = 0xFFCCCCCC;
		
		public const uint CONNECTIVITY_TEST = 0x99660000;
		
		// types
		public const int MAIN_DUNGEON = Map.MAIN_DUNGEON;
		public const int ITEM_DUNGEON = Map.ITEM_DUNGEON;
		public const int AREA = Map.AREA;
		
		// zones
		public const int DUNGEONS = Map.DUNGEONS;
		public const int SEWERS = Map.SEWERS;
		public const int CAVES = Map.CAVES;
		public const int CHAOS = Map.CHAOS;
		
		public const int UP = 1;
		public const int RIGHT = 1 << 1;
		public const int DOWN = 1 << 2;
		public const int LEFT = 1 << 3;
		
		public const int MAX_SIZE = 5;
		public const int MIN_SIZE = 3;
		
		public const double SECRET_FREQ = 0.5d;
		
		public const int OVERWORLD_WIDTH = 25;
		public const int OVERWORLD_HEIGHT = 13;
		
		public const int UNDERWORLD_WIDTH = 20;
		public const int UNDERWORLD_HEIGHT = 13;
		
		public static readonly Array<int> ZONE_HORIZ_PACE = new Array<int>{
			3, 4, 2, 3
		};
		public static readonly Array<int> ZONE_VERT_PACE = new Array<int>{
			2, 1, 2, 1
		};
		
		/* Adobe don't provide this as a constant for some reason */
		public const int MAXIMUM_PIXELS = 16769025;
		
		public MapBitmap(int level, int type, int zone = 0) : base(null, "auto", false) {
			
			this.level = level;
			this.type = type;
			this.zone = zone;
			
#if false
			// directions has to be reinitialised to ensure seed consistency
			directions = new Array<Pixel>{new Pixel(0, -1), new Pixel(1, 0), new Pixel(0, 1), new Pixel( -1, 0)};
			
//			BitmapData bitmapData;
			
			if(type == AREA){
				if(level == Map.OVERWORLD) bitmapData = createOverworld();
				else if(level == Map.UNDERWORLD) bitmapData = createUnderworld();
				
			} else if(type == MAIN_DUNGEON || type == ITEM_DUNGEON){
				
				size = level;
				
				if(size > MIN_SIZE){
					// the level parameter seems to fail at dishing out cool levels at about 8
					// and beyond 5 the dungeons get crazily big and empty
					// so I'm capping it at 5
					size = size > MAX_SIZE ? MAX_SIZE : size;
					// but we also get some cool levels earlier, so let's randomise
					size = (int)(1 + Map.random.range(size));
					size = size < MIN_SIZE ? MIN_SIZE : size;
				}
				
				// create pacing standard for this level
				horizPace = (int)Math.Ceiling(size * 0.5) * ZONE_HORIZ_PACE[zone];
				vertPace = (int)Math.Ceiling(size * 0.5) * ZONE_VERT_PACE[zone];
				
				bitmapData = createRoomsAndTunnels();
				
				// view map generation debug
				//var temp:Bitmap = new Bitmap(bitmapData.clone());
				//temp.scaleX = temp.scaleY = 2;
				//Game.game.addChild(temp);
			}
			
//			super(bitmapData, "auto", false);
			
			if(type == MAIN_DUNGEON || type == ITEM_DUNGEON){
				while(!createRoutes()){}
				createPits();
				createSecrets();
				createGates();
				
			}
			createSurfaces();
#endif
		}
		
		/* Creates the base map for the overworld area */
		public BitmapData createOverworld(){
			BitmapData overworldMap = new BitmapData(OVERWORLD_WIDTH, OVERWORLD_HEIGHT, true, WALL);
			overworldMap.fillRect(new Rectangle(1, 1, overworldMap.width - 2, overworldMap.height - 2), EMPTY);
			adjustedMapRect = new Rectangle(0, 0, overworldMap.width * Game.SCALE, overworldMap.height * Game.SCALE);
			return overworldMap;
		}
		
		/* Creates the base map for the underworld area */
		public BitmapData createUnderworld(){
			BitmapData underworldMap = new BitmapData(UNDERWORLD_WIDTH, UNDERWORLD_HEIGHT, true, WALL);
			underworldMap.fillRect(new Rectangle(1, 1, underworldMap.width - 2, underworldMap.height - 2), EMPTY);
			adjustedMapRect = new Rectangle(0, 0, underworldMap.width * Game.SCALE, underworldMap.height * Game.SCALE);
			return underworldMap;
		}
		
		/* This plots the size, number of rooms and how those rooms are connected */
		public BitmapData createRoomsAndTunnels(){
			
			// sometimes the generator will fail - then we recreate the level
			
			Boolean goodLevel = true;
			BitmapData data;
			do{
				goodLevel = true;
			
				// create a list of rooms, then randomly assign a sibling
				rooms = new Vector<Room>();
				int totalRooms = -MIN_SIZE + 4 * size + Map.random.rangeInt(4 * size);
				if(totalRooms < 5) totalRooms = 5;
				for(i = 0; i < totalRooms; i++){
					rooms.push(new Room());
				}
				int pick;
				for(i = 0; i < rooms.length; i++){
					do{
						pick = (int)Map.random.range(rooms.length);
					} while(pick == i);
					rooms[i].siblings.push(rooms[pick]);
				}
				// add more siblings in sewers
				if(zone == SEWERS){
					for(i = 0; i < rooms.length; i++){
						for(j = 0; j < Map.random.rangeInt(3); j++){
							do{
								pick = (int)Map.random.range(rooms.length);
							} while(pick == i);
							rooms[i].siblings.push(rooms[pick]);
						}
					}
				}
				
				// now the hard part, positioning and then connecting them:
				
				// size each room and get a ball park size for each cell in the grid
				
				int cellHeight = 0;
				int cellWidth = 0;
				
				for(i = 0; i < rooms.length; i++){
					rooms[i].width = MIN_ROOM_WIDTH + Map.random.rangeInt(horizPace);
					if(rooms[i].width > cellWidth) cellWidth = rooms[i].width;
					rooms[i].height = MIN_ROOM_HEIGHT + Map.random.rangeInt(vertPace);
					if(rooms[i].height > cellHeight) cellHeight = rooms[i].height;
				}
				
				cellHeight += 2;
				cellWidth += 2;
				
				// get a grid size for the cells
				// we basically create a minimum of 2 or a maximum of half the number of cells either way
				int gridHeight = (int)(2 + Map.random.range((rooms.length / 2) - 2));
				int gridWidth = (int)Math.Ceiling((1.0 * rooms.length) / gridHeight);
				
				// let's assign grid numbers
				Array<int> nums = new Array<int>();
				for(i = 0; i < gridHeight * gridWidth; i++) nums.push(i);
				for(i = 0; i < rooms.length; i++){
					int n = (int)Map.random.range(nums.length);
					rooms[i].gridNum = nums[n];
					nums.splice(n, 1);
				}
				
				// we use a bitmap for digging, we can use floodFill to verify connectivity later:
				data = new BitmapData(gridWidth * cellWidth, gridHeight * cellHeight, true, WALL);
				
				// place the rooms in their cells
				for(i = 0; i < rooms.length; i++){
					room = rooms[i];
					room.x = (room.gridNum % gridWidth) * cellWidth;
					room.y = (room.gridNum / gridWidth);
					room.y *= cellHeight;
					// random offset the rooms
					room.x += (int)(1 + Map.random.range((cellWidth - 1) - room.width));
					room.y += (int)(1 + Map.random.range((cellHeight - 1) - room.height));
					// draw the room:
					data.fillRect(new Rectangle(
						room.x, room.y, room.width, room.height
					), DIGGING);
					
					// round rooms
					if(zone == CAVES){
						data.fillRect(new Rectangle(
							(room.x + room.width * 0.5) - (room.height * 0.5),
							(room.y + room.height * 0.5) - (room.width * 0.5),
							room.height,
							room.width
						), DIGGING);
					
					// T and + shaped rooms
					} else if(zone == CHAOS){
						data.fillRect(new Rectangle(
							room.x + Map.random.range(room.width - room.height),
							room.y - Map.random.range(room.width - room.height),
							room.height,
							room.width
						), DIGGING);
					}
				}
				
				// now we have to connect the rooms to their siblings
				
				int side;
				for(i = 0; i < rooms.length; i++){
					for(j = 0; j < rooms[i].siblings.length; j++){
						// pick an exit
						Pixel exit = new Pixel();
						room = rooms[i];
						do{
							side = 1 << Map.random.rangeInt(4);
							if((side & UP) != 0){
								exit.x = room.x + Map.random.rangeInt(room.width);
								exit.y = room.y;
							} else if((side & RIGHT) != 0){
								exit.x = room.x + room.width - 1;
								exit.y = room.y + Map.random.rangeInt(room.height);
							} else if((side & DOWN) != 0){
								exit.x = room.x + Map.random.rangeInt(room.width);
								exit.y = room.y + room.height - 1;
							} else if((side & LEFT) != 0){
								exit.x = room.x;
								exit.y = room.y + Map.random.rangeInt(room.height);
							}
						} while(room.touchesDoors(exit) || onEdge(exit, data.width, data.height));
						room.doors.push(exit);
						// now pick an entrance
						Pixel entrance = new Pixel();
						room = rooms[i].siblings[j];
						do{
							side = 1 << Map.random.rangeInt(4);
							if((side & UP) != 0){
								entrance.x = room.x + Map.random.rangeInt(room.width);
								entrance.y = room.y - 1;
							} else if((side & RIGHT) != 0){
								entrance.x = room.x + room.width;
								entrance.y = room.y + Map.random.rangeInt(room.height);
							} else if((side & DOWN) != 0){
								entrance.x = room.x + Map.random.rangeInt(room.width);
								entrance.y = room.y + room.height;
							} else if((side & LEFT) != 0){
								entrance.x = room.x - 1;
								entrance.y = room.y + Map.random.rangeInt(room.height);
							}
						} while(room.touchesDoors(entrance) || onEdge(entrance, data.width, data.height));
						room.doors.push(entrance);
						
						// try getting there, favour pre-existing routes
						
						Vector<Pixel> neighbours = new Vector<Pixel>(4, true);
						// this randomisation keeps the search from being weighted
						array.randomiseArray(directions, Map.random);
						int m = 0;
						do{
							data.setPixel32(exit.x, exit.y, TUNNELLING);
							neighbours[0] = new Pixel(exit.x + directions[0].x, exit.y + directions[0].y);
							neighbours[1] = new Pixel(exit.x + directions[1].x, exit.y + directions[1].y);
							neighbours[2] = new Pixel(exit.x + directions[2].x, exit.y + directions[2].y);
							neighbours[3] = new Pixel(exit.x + directions[3].x, exit.y + directions[3].y);
							int best = int.MaxValue;
							Pixel choice = null;
							for(k = 0; k < neighbours.length; k++){
								int dist = entrance.mDist(neighbours[k]);
								if(data.getPixel32(neighbours[k].x, neighbours[k].y) != DIGGING){
									dist+=3;
								}
								if(dist < best){
									best = dist;
									choice = neighbours[k];
								}
							}
							if(choice != null){
								exit.x = choice.x;
								exit.y = choice.y;
							} else {
								break;
							}
							// it does not pay to dig forever
							m++;
							if(m > 200){
								break;
							}
						} while(exit.x != entrance.x || exit.y != entrance.y);
						
						data.setPixel32(exit.x, exit.y, TUNNELLING);
						data.floodFill(exit.x, exit.y, DIGGING);
					}
				}
				
				// create random crags on caves levels
				if(zone == CAVES){
					for(i = 0; i < (data.width + data.height) * 3; i++){
						c = 1 + Map.random.rangeInt(data.width - 1);
						r = 1 + Map.random.rangeInt(data.height - 1);
						if(data.getPixel32(c, r) == WALL && (
							data.getPixel32(c - 1, r) == DIGGING ||
							data.getPixel32(c + 1, r) == DIGGING ||
							data.getPixel32(c, r + 1) == DIGGING
						)){
							data.setPixel32(c, r, DIGGING);
						}
					}
				}
				
				// did the room generation create two separate networks?
				data.floodFill(rooms[0].x, rooms[0].y, EMPTY);
				
				for(i = 0; i < rooms.length; i++){
					if(data.getPixel32(rooms[i].x, rooms[i].y) != EMPTY){
						App.Util.trace("failed room connection");
						goodLevel = false;
					}
				}
			
			} while(!goodLevel);
			
			// remove floor nubs (pointless 1 square pits in the floor)
			// they confuse enemies and the route planner puts ladders in them leading to nothing but a one square pit
			Vector<uint> pixels = data.getVector(data.rect);
			for(i = data.width; i < pixels.length - data.width; i++){
				if(pixels[i] == EMPTY && pixels[i - 1] == WALL && pixels[i + 1] == WALL && pixels[i + data.width] == WALL){
					pixels[i] = WALL;
				}
			}
			data.setVector(data.rect, pixels);
			
			// trim the map, we may have a large portion of unused rock
			Rectangle mapBounds = data.getColorBoundsRect(0xFFFFFFFF, EMPTY);
			// the rooms will have to be moved!
			int moveX = (int)(1 - mapBounds.x);
			int moveY = (int)(1 - mapBounds.y);
			for(i = 0; i < rooms.length; i++){
				rooms[i].x += moveX;
				rooms[i].y += moveY;
			}
			BitmapData trimmedData = new BitmapData((int)(mapBounds.width + 2), (int)(mapBounds.height + 2), true, 0xFFFF0000);
			
			trimmedData.copyPixels(data, mapBounds, new Point(1, 1));
			
			return trimmedData;
		}
		
		/* Given a network of rooms and tunnels, a platform game character requires ledges and
		 * ladders to navigate that network and be able to visit every corner of it
		 * 
		 * The route planner does a final check for connectivity, if this fails then route planner aborts, returning false */
		public Boolean createRoutes(){
			
			// count all the cliffs that are in the level. A cliff is an L-shaped area where the
			// character can fall off
			
			// cliffs form the basis for the graph we will generate to create a route around the map
			// every cliff is two nodes (the ledge where you step off, and the bottom) in the graph
			
			// first we get the bitmap as a vector, this makes iterating over it faster
			
			Vector<uint> pixels = bitmapData.getVector(bitmapData.rect);
			int mapWidth = bitmapData.width;
			Vector<int> cliffs = new Vector<int>();
			
			// there is a border around the map - we don't need to count it
			for(i = mapWidth; i < pixels.length - mapWidth; i++){
				c = i % mapWidth;
				if(c > 0 && c < mapWidth - 1){
					if(pixels[i] != WALL && pixels[i - mapWidth] != WALL){
						// we have a gap, now we look for the cliffs
						if(pixels[i - 1] == WALL && pixels[i - 1 - mapWidth] != WALL){
							cliffs.push(i - mapWidth);
							pixels[i - mapWidth] = NODE;
						} else if(pixels[i + 1] == WALL && pixels[(i + 1) - mapWidth] != WALL){
							cliffs.push(i - mapWidth);
							pixels[i - mapWidth] = NODE;
						}
					}
					// we can use this sweep to mark out horizontal paths as we go, saving time
					if(pixels[i] == EMPTY && pixels[i + mapWidth] == WALL){
						pixels[i] = PATH;
					}
				}
			}
			
			// we've marked out the top nodes, now we'll get the bottom nodes and mark out paths as we do
			for(i = 0; i < cliffs.length; i++){
				n = cliffs[i] + mapWidth;
				while(pixels[n] != NODE && pixels[n] != WALL){
					if(pixels[n] != NODE) pixels[n] = pixels[n + mapWidth] == WALL ? NODE : PATH;
					n += mapWidth;
				}
			}
			
			// here's comes the graph to reduce the number of routes and give us a more
			// tricky level to navigate
			
			Vector<Node> graph = new Vector<Node>();
			
			// create an empty grid so that locating graph nodes is fast
			Vector< Vector<Node> > graphGrid = new Vector< Vector<Node> >(bitmapData.height, true);
			for(r = 0; r < bitmapData.height; r++){
				graphGrid[r] = new Vector<Node>(mapWidth, true);
			}
			for(i = mapWidth; i < pixels.length - mapWidth; i++){
				if(pixels[i] == NODE){
					c = i % mapWidth;
					r = i / mapWidth;
					node = new Node(c, r);
					graph.push(node);
					graphGrid[r][c] = node;
				}
			}
			// now search for connections
			Vector<int> dirs = new Vector<int>(4, true);
			dirs[0] = 1;
			dirs[1] = -1;
			dirs[3] = -mapWidth;
			dirs[2] = mapWidth;
			
			int pos;
			
			for(i = 0; i < graph.length; i++){
				
				// only 4 directions to look in
				pos = graph[i].x + graph[i].y * mapWidth;
				
				for(j = 0; j < dirs.length; j++){
					if(pixels[pos + dirs[j]] == PATH || pixels[pos + dirs[j]] == NODE){
						n = pos + dirs[j];
						do{
							if(pixels[n] == NODE){
								c = n % mapWidth;
								r = n / mapWidth;
								graph[i].connections.push(graphGrid[r][c]);
								graph[i].connectionsActive.push(false);
								graphGrid[r][c].connections.push(graph[i]);
								graphGrid[r][c].connectionsActive.push(false);
								// mark out where drops are - it can lead to broken maps
								if(j == 2){
									graph[i].drop = true;
								}
								break;
							}
							n += dirs[j];
						} while(pixels[n] == PATH || pixels[n] == NODE);
					}
				}
			}
			
			// so lets visit all points on the graph now and mark our good connections
			// whilst we're at it we'll delete our node pixels to clean up
			
			Vector<Node> visitedNodes = new Vector<Node>();
			node = graph[Map.random.rangeInt(graph.length)];
			node.visited = true;
			visitedNodes.push(node);
			pixels[node.x + node.y * mapWidth] = PATH;
			
			i = 0;
			while(i < graph.length){
				search:
				for(i = 0; i < visitedNodes.length; i++){
					for(j = 0; j < visitedNodes[i].connections.length; j++){
						if(!visitedNodes[i].connectionsActive[j] && !visitedNodes[i].connections[j].visited){
							visitedNodes[i].connectionsActive[j] = true;
							visitedNodes[i].connections[j].visited = true;
							pixels[visitedNodes[i].connections[j].x + visitedNodes[i].connections[j].y * mapWidth] = PATH;
							visitedNodes[i].connections[j].connectionsActive[visitedNodes[i].connections[j].connections.IndexOf(visitedNodes[i])] = true;
							visitedNodes.push(visitedNodes[i].connections[j]);
							//break search;
							goto search;
						}
					}
				}
			}
			
			// we load the pixels back in to flood out the paths
			bitmapData.setVector(bitmapData.rect, pixels);
			bitmapData.floodFill(graph[0].x, graph[0].y, EMPTY);
			pixels = bitmapData.getVector(bitmapData.rect);
			
			// DEBUG! ---------------------------------------------------------------
			// did I fuck up?
			// let's draw it and find out
			/*var gfx:Graphics = Game.debug.graphics;
			gfx.lineStyle(2, 0);
			for(i = 0; i < graph.length; i++){
				for(j = 0; j < graph[i].connections.length; j++){
					if(graph[i].connectionsActive[j]){
						gfx.moveTo((graph[i].x + 0.5) * Game.SCALE, (graph[i].y + 0.5) * Game.SCALE);
						gfx.lineTo((graph[i].connections[j].x + 0.5) * Game.SCALE, (graph[i].connections[j].y + 0.5) * Game.SCALE);
					}
				}
			}*/
			
			// Now lets put in ladders and ledges
			
			int dest;
			
			// wherever there's a vertical node connection, we slap a ladder in there
			// except that for now we mark out the tops of ladders, and cap a ledge in where a ladder
			// would normally stop and drop the player into open space
			for(i = 0; i < graph.length; i++){
				for(j = 0; j < graph[i].connections.length; j++){
					if(graph[i].connectionsActive[j]){
						if(graph[i].connections[j].x == graph[i].x){
							n = graph[i].x + graph[i].y * mapWidth;
							dest = graph[i].connections[j].x + graph[i].connections[j].y * mapWidth;
							// xor swap so we get end the job with a ladder at the top
							if(dest > n){
								n ^= dest;
								dest ^= n;
								n ^= dest;
							}
							if(pixels[n + mapWidth] != WALL){
								pixels[n + mapWidth] = LEDGE;
							}
							pixels[dest + mapWidth] = LADDER_LEDGE;
						}
						// after we query a connection, we deactivate at both ends, we don't need it anymore
						graph[i].connectionsActive[j] = false;
						graph[i].connections[j].connectionsActive[graph[i].connections[j].connections.IndexOf(graph[i])] = false;
					}
				}
				// also we slap a ledge in under any drop node (a node with a downwards link) - they can cause broken maps
				if(graph[i].drop){
					if(pixels[graph[i].x + (graph[i].y + 1) * mapWidth] == EMPTY){
						pixels[graph[i].x + (graph[i].y + 1) * mapWidth] = LEDGE;
					}
				}
			}
			
			// sprinkle some extra ladders in
			for(i = mapWidth; i < pixels.length - mapWidth; i++){
				if(pixels[i] == EMPTY && pixels[i - mapWidth] == EMPTY){
					if(pixels[i - 1] == WALL && pixels[i + 1] == EMPTY && Map.random.value() < LADDERINESS){
						pixels[i] = LADDER_LEDGE;
					} else if(pixels[i + 1] == WALL && pixels[i - 1] == EMPTY && Map.random.value() < LADDERINESS){
						pixels[i] = LADDER_LEDGE;
					}
				}
			}
			
			// all of our ladders are slap-bang next to walls, how about we see if we can move them into the
			// room a little and extend the ledge that reaches to them
			
			// note to self - a ledge with a wall on top of it looks fucking stupid
			
			// all of our ladders are slap-bang next to walls, how about we see if we can move them into the
			// room a little and extend the ledge that reaches to them
			
			// note to self - a ledge with a wall on top of it looks fucking stupid
			
			for(i = mapWidth; i < pixels.length - mapWidth; i++){
				if(pixels[i] == LADDER_LEDGE){
					if(pixels[i - 1] == EMPTY){
						// pull out the ladder ledge
						j = i;
						for(n = (int)Map.random.range(LEDGE_LENGTH); n != 0; n--){
							if(pixels[j - 1] == EMPTY && pixels[j - 1 - mapWidth] != WALL){
								pixels[j] = LEDGE;
								pixels[j - 1] = LADDER_LEDGE;
							} else break;
							j--;
						}
						// add a bit of extension past it
						for(n = (int)Map.random.range(LEDGE_LENGTH); n != 0; n--){
							if(pixels[j - 1] == EMPTY && pixels[j - 1 - mapWidth] != WALL){
								pixels[j - 1] = LEDGE;
							} else break;
							j--;
						}
					} else if(pixels[i + 1] == EMPTY){
						// push out the ladder ledge
						for(n = (int)Map.random.range(LEDGE_LENGTH); n != 0; n--){
							if(pixels[i + 1] == EMPTY && pixels[i + 1 - mapWidth] != WALL){
								pixels[i] = LEDGE;
								pixels[i + 1] = LADDER_LEDGE;
							} else break;
							i++;
						}
						// add a bit of extension past it
						j = i;
						for(n = (int)Map.random.range(LEDGE_LENGTH); n != 0; n--){
							if(pixels[j + 1] == EMPTY && pixels[j + 1 - mapWidth] != WALL){
								pixels[j + 1] = LEDGE;
							} else break;
							j++;
						}
					}
				}
			}
			
			// cast ladders down from all the ladder ledges
			for(i = mapWidth; i < pixels.length - mapWidth; i++){
				if(pixels[i] == LADDER_LEDGE){
					n = i + mapWidth;
					while(pixels[n] == EMPTY){
						pixels[n] = LADDER;
						n += mapWidth;
					}
				}
			}
			
			// create some extra ladders in a tree like fashion (finding a wall base to root in a grow up out of)
			int cast;
			for(i = mapWidth; i < pixels.length - mapWidth; i++){
				if(pixels[i] == EMPTY && pixels[i - mapWidth] == EMPTY && pixels[i + mapWidth] == WALL && Map.random.value() < LADDERINESS){
					n = (int)(1 + Map.random.range(LADDER_TREE_HEIGHT));
					cast = i;
					while(n-- > 0){
						if(pixels[cast] == EMPTY && pixels[cast - mapWidth] == EMPTY){
							pixels[cast] = LADDER_LEDGE;
							if(pixels[cast + mapWidth] == LADDER_LEDGE){
								pixels[cast + mapWidth] = LADDER;
							}
							cast -= mapWidth;
						} else break;
					}
					// this method tends to create solo ledges on top of ladders that look pretty bad
					// so we randomly add a bit of ledge to the sides at the top
					Array<int> available = new Array<int>();
					cast += mapWidth;
					if(pixels[cast - 1] == EMPTY && pixels[cast - 1 - mapWidth] != WALL){
						available.push(cast - 1);
					}
					if(pixels[cast + 1] == EMPTY && pixels[cast + 1 - mapWidth] != WALL){
						available.push(cast + 1);
					}
					if(available.length > 0){
						pixels[available[Map.random.rangeInt(available.length)]] = LEDGE;
					}
				}
			}
			
			// sprinkle a few more ledges in
			for(i = mapWidth; i < pixels.length - mapWidth; i++){
				if(pixels[i] == LADDER && Map.random.value() < LEDGINESS){
					if(pixels[i - 1] == EMPTY){
						j = i;
						for(n = (int)Map.random.range(LEDGE_LENGTH); n != 0; n--){
							if(pixels[j - 1] == EMPTY && pixels[j - 1 - mapWidth] != WALL){
								pixels[j - 1] = LEDGE;
							} else if(pixels[j - 1] == LADDER){
								pixels[j - 1] = LADDER_LEDGE;
							}
							else break;
							j--;
							pixels[i] = LADDER_LEDGE;
						}
					}
					if(pixels[i + 1] == EMPTY){
						j = i;
						for(n = (int)Map.random.value() * LEDGE_LENGTH; n != 0; n--){
							if(pixels[j + 1] == EMPTY && pixels[j + 1 - mapWidth] != WALL){
								pixels[j + 1] = LEDGE;
							} else if(pixels[j + 1] == LADDER){
								pixels[j + 1] = LADDER_LEDGE;
							} else break;
							j++;
							pixels[i] = LADDER_LEDGE;
						}
					}
				}
			}
			
			// It is possible for the route planning to go awry.
			// Double check that the level has full connectivity
			BitmapData connectionBitmapData = new BitmapData(bitmapData.width * 2, bitmapData.height * 2);
			connectionBitmapData.fillRect(connectionBitmapData.rect, 0xFFFFFFFF);
			Vector<uint> connectionPixels = connectionBitmapData.getVector(connectionBitmapData.rect);
			int connectionMapWidth = mapWidth * 2;
			Pixel floodSeed = null;
			for(i = mapWidth; i < pixels.length - mapWidth; i++){
				if(
					pixels[i] != WALL &&
					(
						pixels[i + mapWidth] == LEDGE ||
						pixels[i + mapWidth] == WALL ||
						pixels[i + mapWidth] == LADDER_LEDGE
					)
				){
					c = i % mapWidth;
					r = i / mapWidth;
					j = c * 2 + r * 2 * connectionMapWidth;
					connectionPixels[j + connectionMapWidth] = CONNECTIVITY_TEST;
					connectionPixels[j + 1 + connectionMapWidth] = CONNECTIVITY_TEST;
					if(floodSeed == null) floodSeed = new Pixel(c * 2, 1 + r * 2);
				}
				if(pixels[i] == LADDER || pixels[i] == LADDER_LEDGE){
					c = i % mapWidth;
					r = i / mapWidth;
					j = c * 2 + r * 2 * connectionMapWidth;
					connectionPixels[j] = CONNECTIVITY_TEST;
					connectionPixels[j + connectionMapWidth] = CONNECTIVITY_TEST;
					if(floodSeed == null) floodSeed = new Pixel(c * 2, r * 2);
				}
			}
			connectionBitmapData.setVector(connectionBitmapData.rect, connectionPixels);
			connectionBitmapData.floodFill(floodSeed.x, floodSeed.y, 0xFF000000);
			//var connectionBitmap:Bitmap = new Bitmap(connectionBitmapData);
			//Game.game.addChild(connectionBitmap);
			if(connectionBitmapData.getColorBoundsRect(0xFFFFFFFF, CONNECTIVITY_TEST).width <= 0){
				App.Util.trace("failed route planning");
				return false;
			}
			bitmapData.setVector(bitmapData.rect, pixels);
			return true;
		}
		
		/* This finds suitable locations for pits and digs them, leaving a trap marker for the Map class
		 * to turn into a Trap Entity */
		public void createPits(){
			
			pitTraps = 0;
			int totalPits = (int)(game.content.getTraps(level, type) * 0.5);
			if(totalPits == 0) return;
			
			int mapWidth = bitmapData.width;
			Vector<int> pits = new Vector<int>();
			Vector<uint> pixels = bitmapData.getVector(bitmapData.rect);
			
			// any bottom corner can serve as a location for a pit, as there always seems to be a
			// route around
			
			// - just added a mod to the pits so they cap with a ledge above them after triggering
			for(i = mapWidth; i < pixels.length - mapWidth * 2; i++){
				if(pixels[i] != WALL && pixels[i] != LADDER && pixels[i] != LADDER_LEDGE && pixels[i + mapWidth] == WALL){// && (pixels[i - 1] == WALL || pixels[i + 1] == WALL)){
					// we need to verify the pit can tunnel down to a new area
					for(j = i + mapWidth * 2; j < pixels.length - mapWidth * 2; j += mapWidth){
						if(pixels[j] == EMPTY){
							pits.push(i + mapWidth);
							break;
						}
					}
				}
			}
			
			// we have a selection of locations, it remains to select from them and dig
			while(totalPits > 0 && pits.length > 0){
				int r = (int)Map.random.range(pits.length);
				pixels[pits[r]] = PIT;
				for(i = pits[r] + mapWidth; i < pixels.length - mapWidth * 2; i += mapWidth){
					if(pixels[i] == EMPTY){
						break;
					} else {
						pixels[i] = EMPTY;
					}
				}
				pits.splice(r, 1);
				totalPits--;
				pitTraps++;
			}
			
			bitmapData.setVector(bitmapData.rect, pixels);
		}
		
		/* This creates extra rooms on the edges of the map, hidden by a destructible wall */
		public void createSecrets(){
			
			int totalSecrets = game.content.getSecrets(level, type);
			
			adjustedMapRect = new Rectangle(0, 0, bitmapData.width * Game.SCALE, bitmapData.height * Game.SCALE);
			
			if(totalSecrets == 0) return;
			
			int mapWidth = bitmapData.width;
			Vector<int> secretsLeft = new Vector<int>();
			Vector<int> secretsRight = new Vector<int>();
			Vector<uint> pixels = bitmapData.getVector(bitmapData.rect);
			
			// we check the edges of the map for locations where a secret room would be acceptable
			
			// to avoid any overlap and need to recalculate routes, only one secret per side maximum
			
			Room room;
			int pos, posY, corridorLength;
			BitmapData temp;
			int ladderPos, ledgePos;
			
			// left edge:
			for(i = mapWidth; i < pixels.length - mapWidth; i += mapWidth){
				if(pixels[i+1] != WALL && pixels[i+1+mapWidth] != LADDER && pixels[i+1+mapWidth] != EMPTY && pixels[i+1+mapWidth] != PIT){
					secretsLeft.push(i);
				}
			}
			
			if(secretsLeft.length > 0 && Map.random.value() < SECRET_FREQ){
				pos = secretsLeft[Map.random.rangeInt(secretsLeft.length)];
				posY = pos / mapWidth;
				room = new Room();
				room.width = MIN_ROOM_WIDTH + Map.random.rangeInt(horizPace * 0.5);
				room.height = MIN_ROOM_HEIGHT + Map.random.rangeInt(vertPace * 0.5);
				if(room.height > bitmapData.height - 2) room.height = bitmapData.height - 2;
				corridorLength = 1 + Map.random.rangeInt(5);
				// recreate bitmap
				temp = new BitmapData(1 + mapWidth + corridorLength + room.width, bitmapData.height, true, WALL);
				temp.copyPixels(bitmapData, bitmapData.rect, new Point(1 + room.width + corridorLength, 0));
				bitmapData = temp;
				pixels = bitmapData.getVector(bitmapData.rect);
				pixels[1 + room.width + corridorLength + posY * bitmapData.width] = SECRET;
				for(i = 0; i < corridorLength; i++){
					pixels[-i + room.width + corridorLength + posY * bitmapData.width] = EMPTY;
				}
				room.x = 1;
				room.y = posY - Map.random.rangeInt(room.height);
				if(room.y + room.height > bitmapData.height - 1) room.y = bitmapData.height - 1 - room.height;
				if(room.y < 1) room.y = 1;
				bitmapData.setVector(bitmapData.rect, pixels);
				bitmapData.fillRect(new Rectangle(1, room.y, room.width, room.height), EMPTY);
				// enable access:
				// I like the idea of the secret rooms being spartan in function
				// they should feel like a cubby hole
				pixels = bitmapData.getVector(bitmapData.rect);
				if(posY < room.y + room.height - 1){
					ladderPos = 1 + Map.random.rangeInt(room.width) + (posY + 1) * bitmapData.width;
					pixels[ladderPos] = LADDER_LEDGE;
					ledgePos = ladderPos + 1;
					while(pixels[ledgePos] != WALL){
						pixels[ledgePos] = LEDGE;
						ledgePos++;
					}
					ledgePos = ladderPos - 1;
					while(pixels[ledgePos] != WALL && Map.random.coinFlip()){
						pixels[ledgePos] = LEDGE;
						ledgePos--;
					}
					ladderPos += bitmapData.width;
					while(pixels[ladderPos] != WALL){
						pixels[ladderPos] = LADDER;
						ladderPos += bitmapData.width;
					}
				}
				bitmapData.setVector(bitmapData.rect, pixels);
				leftSecretRoom = room;
				leftSecretWidth = (1 + room.width + corridorLength) * Game.SCALE;
				adjustedMapRect.x += leftSecretWidth;
				for(i = 0; i < rooms.length; i++){
					rooms[i].x += 1 + room.width + corridorLength;
				}
				mapWidth = bitmapData.width;
				totalSecrets--;
			}
			
			if(totalSecrets == 0) return;
			
			// right edge:
			for(i = mapWidth + mapWidth - 1; i < pixels.length - mapWidth; i += mapWidth){
				if(pixels[i-1] != WALL && pixels[i-1+mapWidth] != LADDER && pixels[i-1+mapWidth] != EMPTY && pixels[i-1+mapWidth] != PIT){
					secretsRight.push(i);
				}
			}
			
			if(secretsRight.length > 0 && Map.random.value() < SECRET_FREQ){
				pos = secretsRight[Map.random.rangeInt(secretsRight.length)];
				posY = pos / mapWidth;
				room = new Room();
				room.width = MIN_ROOM_WIDTH + Map.random.rangeInt(horizPace * 0.5);
				room.height = MIN_ROOM_HEIGHT + Map.random.rangeInt(vertPace * 0.5);
				if(room.height > bitmapData.height - 2) room.height = bitmapData.height - 2;
				corridorLength = 1 + Map.random.rangeInt(5);
				// recreate bitmap
				temp = new BitmapData(1 + bitmapData.width + corridorLength + room.width, bitmapData.height, true, WALL);
				temp.copyPixels(bitmapData, bitmapData.rect, new Point());
				bitmapData = temp;
				pixels = bitmapData.getVector(bitmapData.rect);
				pixels[mapWidth - 1 + posY * bitmapData.width] = SECRET;
				for(i = 0; i < corridorLength; i++){
					pixels[mapWidth + i + posY * bitmapData.width] = EMPTY;
				}
				room.x = mapWidth + corridorLength;
				room.y = posY - Map.random.rangeInt(room.height);
				if(room.y + room.height > bitmapData.height - 1) room.y = bitmapData.height - 1 - room.height;
				if(room.y < 1) room.y = 1;
				bitmapData.setVector(bitmapData.rect, pixels);
				bitmapData.fillRect(new Rectangle(room.x, room.y, room.width, room.height), EMPTY);
				// enable access:
				// I like the idea of the secret rooms being spartan in function
				// they should feel like a cubby hole
				pixels = bitmapData.getVector(bitmapData.rect);
				if(posY < room.y + room.height - 1){
					ladderPos = room.x + Map.random.rangeInt(room.width) + (posY + 1) * bitmapData.width;
					pixels[ladderPos] = LADDER_LEDGE;
					ledgePos = ladderPos - 1;
					while(pixels[ledgePos] != WALL){
						pixels[ledgePos] = LEDGE;
						ledgePos--;
					}
					ledgePos = ladderPos + 1;
					while(pixels[ledgePos] != WALL && Map.random.coinFlip()){
						pixels[ledgePos] = LEDGE;
						ledgePos++;
					}
					ladderPos += bitmapData.width;
					while(pixels[ladderPos] != WALL){
						pixels[ladderPos] = LADDER;
						ladderPos += bitmapData.width;
					}
				}
				bitmapData.setVector(bitmapData.rect, pixels);
				rightSecretWidth = (1 + room.width + corridorLength) * Game.SCALE;
				rightSecretRoom = room;
				mapWidth = bitmapData.width;
			}
		}
		
		/* Create obstructions the player has to pass by force other means */
		public void createGates(){
			gates = new Vector<Pixel>();
			
			// skip creating gates on level 1 - too early for new mechanics
			if(level == 1) return;
			
			// find sites suitable for gates
			
			Vector<uint> mapPixels = bitmapData.getVector(bitmapData.rect);
			int range = (int)((this.height - 1) * this.width - 1);
			int width = bitmapData.width;
			int height = bitmapData.height;
			int minX = (int)(2 + adjustedMapRect.x * Game.INV_SCALE);
			int maxX = (int)(-2 + (adjustedMapRect.x + adjustedMapRect.width) * Game.INV_SCALE);
			int x;
			Boolean passageFound = false;
			int startPassage = 0;
			
			// iterate through all pixels and find locations suitable
			for(i = width + 1; i < range; i++){
				x = i % width;
				// locate a passage 
				if(
					x > minX && x < maxX &&
					mapPixels[i - (width + 1)] == WALL &&
					mapPixels[i - (width - 1)] == WALL &&
					mapPixels[i - width] == WALL &&
					mapPixels[i + (width + 1)] == WALL &&
					mapPixels[i + (width - 1)] == WALL &&
					mapPixels[i + width] == WALL &&
					mapPixels[i - 1] == EMPTY &&
					mapPixels[i + 1] == EMPTY
				){
					if(!passageFound){
						startPassage = i;
						passageFound = true;
					}
				} else {
					if(passageFound){
						// mark a site halfway along the passage
						c = ((int)( (((i - 1) - startPassage) * 0.5) ) + startPassage) % width;
						r = i / width;
						gates.push(new Pixel(c, r));
						passageFound = false;
					}
				}
			}
			
			//trace("gate total", gates.length);
		}
		
		/* Mark where all usable sections of floor are on a level */
		public void createSurfaces(){
			
			// initial sweep to get all usable surfaces in the level
			Surface surface;
			int properties;
			int width = bitmapData.width;
			int height = bitmapData.height;
			Surface.initMap(width, height);
			Vector<uint> mapPixels = bitmapData.getVector(bitmapData.rect);
			for(i = width; i < mapPixels.length - width; i++){
				if(
					mapPixels[i] != WALL &&
					mapPixels[i] != PIT &&
					mapPixels[i] != SECRET &&
					mapPixels[i] != GATE
				){
					c = i % width;
					r = i / width;
					properties = 0;
					if(mapPixels[i + width] == LEDGE) properties = Collider.UP | Collider.LEDGE;
					else if(mapPixels[i + width] == LADDER_LEDGE) properties = Collider.UP | Collider.LEDGE | Collider.LADDER;
					else if(mapPixels[i + width] == WALL) properties = Collider.SOLID | Collider.WALL;
					if(mapPixels[i] == LADDER || mapPixels[i] == LADDER_LEDGE) properties |= Collider.LADDER;
					if(properties != 0){
						surface = new Surface(c, r, properties);
						Surface.map[r][c] = surface;
						Surface.surfaces.push(surface);
					}
				}
			}
			// now find which of those surfaces are in a room
			// all surfaces in a room are found by casting down from the top of the room - catching areas dug out below the room
			if(rooms != null){
				
				Vector<Room> roomList = rooms.slice();
				if(leftSecretRoom != null) roomList.push(leftSecretRoom);
				if(rightSecretRoom != null) roomList.push(rightSecretRoom);
				roomList.Sort(Map.sortRoomsTopWards);
				
				for(i = 0; i < roomList.length; i++){
					room = roomList[i];
					for(c = room.x; c < room.x + room.width; c++){
						r = room.y;
						n = c + r * width;
						while(n < mapPixels.length - width && mapPixels[n] != WALL && mapPixels[n] != PIT && mapPixels[n] != SECRET && mapPixels[n] != GATE){
							if(Surface.map[r][c] != null){
								surface = Surface.map[r][c];
								// due to casting, a room may have already claimed a surface
								// logically the bottom-most room must be the true owner
								if(surface.room != null){
									surface.room.surfaces.splice(surface.room.surfaces.IndexOf(surface), 1);
								}
								room.surfaces.push(surface);
								surface.room = room;
							}
							r++;
							n += width;
						}
					}
				}
			}
		}
		
		/* is this pixel sitting on the edge of the map? it will likely cause me trouble if it is... */
		public static bool onEdge(Pixel pixel, int width, int height) {
			return pixel.x<= 0 || pixel.x >= width-1 || pixel.y <= 0 || pixel.y >= height-1;
		}
	}

}