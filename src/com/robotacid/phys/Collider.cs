using System;
using System.Collections.Generic;

using App;

///import com.robotacid.engine.Missile;
///import flash.display.Graphics;
///import flash.geom.Rectangle;

namespace com.robotacid.phys {
	
	/**
	 * A crate-like collision object.
	 *
	 * Movement is separated into X axis and Y axis separately for speed and sanity
	 *
	 * Collisions are handled recursively, allowing the Collider to push queues of Colliders.
	 *
	 * The Collider has several states to reflect how it may need to be handled.
	 *
	 * The stackCallback is for assigning to a function that signals a Collider has hit the floor.
	 *
	 * The crushCallback is for assigning to a function that signals a Collider has been crushed. A crush
	 * callback must call CollisionWorld.removeCollider as it is assumed the callback will want access to
	 * the world before the collider is destroyed
	 *
	 * @author Aaron Steed, robotacid.com
	 */
	public class Collider : Rectangle {
		
		public CollisionWorld world;
		public Collider parent;
		public Collider mapCollider;
		public List<Collider> children;
///		public var userData:*;
///		public var stackCallback:Function;
///		public var crushCallback:Function;
///		public var stompCallback:Function;
		public Collider upContact;
		public Collider rightContact;
		public Collider downContact;
		public Collider leftContact;
		
		public int state;
		public int properties;
		public int ignoreProperties;
		public int stompProperties;
		public double vx;
		public double vy;
		public double gravity;
		public double dampingX;
		public double dampingY;
		public double pushDamping;
		public int pressure;
		public bool crushed;
		public int awake;
		public bool boundsPressure;
		public bool stackable;
		
		/* Establishes a minimum movement policy */
		public const double MOVEMENT_TOLERANCE = 0.0001d;
		
		/* Used for compensate for floating point value drift */
		public const double INTERVAL_TOLERANCE = CollisionWorld.INTERVAL_TOLERANCE;
		
		/* Echoing Box2D, colliders sleep when inactive to prevent method calls that aren't needed */
		public static int AWAKE_DELAY = 3;
		
		protected static Collider tempCollider;
		
		public const double DEFAULT_GRAVITY = 0.8d;
		public const double DEFAULT_DAMPING_X = 0.45d;
		public const double DEFAULT_DAMPING_Y = 0.99d;
		public const double DEFAULT_PUSH_DAMPING = 1d;
		
		// states
		public const int FALL = 0;
		public const int STACK = 1;
		public const int HOVER = 2;
		public const int MAP_COLLIDER = 3;
		
		/* No block here */
		public const int EMPTY = 0;
		
		// properties 0 to 3 are the sides of a Rectangle
		public const int UP = 1 << 0;
		public const int RIGHT = 1 << 1;
		public const int DOWN = 1 << 2;
		public const int LEFT = 1 << 3;
		// equivalent to (UP | RIGHT | LEFT | DOWN) the compiler won't allow calculated constants as default params
		public const int SOLID = 15;
		
		/* A Collider that doesn't move */
		public const int STATIC = 1 << 4;
		/* A Collider that can break */
		public const int BREAKABLE = 1 << 5;
		/* A free moving crate style Collider - for puzzles */
		public const int FREE = 1 << 6;
		/* A Collider that moves on its own */
		public const int MOVING = 1 << 7;
		/* This Collider is the collision space of a monster */
		public const int MONSTER = 1 << 8;
		/* This Collider is the collision space of the player */
		public const int PLAYER = 1 << 9;
		/* A Collider whose upper edge resists colliders moving down but not in any other direction */
		public const int LEDGE = 1 << 10;
		/* Dungeon walls */
		public const int WALL = 1 << 11;
		/* This Collider is either a monster or the player */
		public const int CHARACTER = 1 << 12;
		/* This Collider is a decapitated head */
		public const int HEAD = 1 << 13;
		/* This is an area that is a ladder */
		public const int LADDER = 1 << 14;
		/* This Collider is a slave of the player */
		public const int MINION = 1 << 15;
		/* This Collider is an animation for the decapitation of Characters */
		public const int CORPSE = 1 << 16;
		/* This Collider is a projectile */
		public const int MISSILE = 1 << 17;
		/* This Collider is a collectable item */
		public const int ITEM = 1 << 18;
		/* This Collider is a wall that can be attacked */
		public const int STONE = 1 << 19;
		/* This Collider is a wall that moves randomly */
		public const int CHAOS = 1 << 20;
		/* This Collider is a missile of the player team */
		public const int PLAYER_MISSILE = 1 << 21;
		/* This Collider is a missile of the monster team */
		public const int MONSTER_MISSILE = 1 << 22;
		/* This Collider is a horror creature */
		public const int HORROR = 1 << 23;
		/* This Collider is a wall on the edge of the map */
		public const int MAP_EDGE = 1 << 24;
		/* This Collider is a barrier that can be raised */
		public const int GATE = 1 << 25;
		/* This Collider is the end-game boss */
		public const int BALROG = 1 << 26;
		
		public Collider(double x = 0, double y = 0, double width = 0, double height = 0, double scale = 0, int properties = SOLID, int ignoreProperties = 0, int state = 0) : base(x, y, width, height) {
			//super(x, y, width, height);
			this.properties = properties;
			this.ignoreProperties = ignoreProperties;
			this.state = state;
			
			vx = vy = 0;
			gravity = DEFAULT_GRAVITY;
			dampingX = DEFAULT_DAMPING_X;
			dampingY = DEFAULT_DAMPING_Y;
			pushDamping = DEFAULT_PUSH_DAMPING;
			awake = AWAKE_DELAY;
			boundsPressure = false;
			stackable = true;
			
			children = new List<Collider>();
			
			if(state != MAP_COLLIDER){
				// create a dummy surface for interacting with the map
				mapCollider = new Collider(0, 0, scale, scale, scale, SOLID, 0, MAP_COLLIDER);
			}
		}
		
		public void main(){
			if(state == STACK || state == FALL){
				
				vx *= dampingX;
				if((vx > 0 ? vx : -vx) > MOVEMENT_TOLERANCE) moveX(vx);
				
				// check for ignoring parent
				if(parent != null && (parent.properties & ignoreProperties) != 0){
					parent.removeChild(this);
				}
				
				//if(!parent || vy < -MOVEMENT_TOLERANCE){
				if(parent == null || vy < -MOVEMENT_TOLERANCE){
					vy = vy * dampingY + gravity;
					if((vy > 0 ? vy : -vy) > MOVEMENT_TOLERANCE) moveY(vy);
				} else if(vy > MOVEMENT_TOLERANCE){
					vy = 0;
				}
				
				if(parent != null){
					if(state != STACK){
						state = STACK;
///						if(Boolean(stackCallback)) stackCallback();
					}
					
				} else if(state != FALL){
					state = FALL;
				}
				
			} else if(state == HOVER){
				
				vx *= dampingX;
				vy *= dampingY;
				if((vx > 0 ? vx : -vx) > MOVEMENT_TOLERANCE) moveX(vx);
				if((vy > 0 ? vy : -vy) > MOVEMENT_TOLERANCE) moveY(vy);
				
			} else if(state == MAP_COLLIDER){
				
				if((vx > 0 ? vx : -vx) > MOVEMENT_TOLERANCE) moveX(vx);
				if((vy > 0 ? vy : -vy) > MOVEMENT_TOLERANCE) moveY(vy);
			}
			
			// will put the collider to sleep if it doesn't move
			if((vx > 0 ? vx : -vx) < MOVEMENT_TOLERANCE && (vy > 0 ? vy : -vy) < MOVEMENT_TOLERANCE && (awake != 0)) awake--;
		}
		
		public void drag(double vx, double vy){
			moveX(vx);
			moveY(vy);
		}
		
#if false
		/* =================================================================
		 * Sorting callbacks for colliding with objects in the correct order
		 * =================================================================
		 */
		public static function sortLeftWards(a:Collider, b:Collider):Number{
			if(a.x < b.x) return -1;
			else if(a.x > b.x) return 1;
			return 0;
		}
		
		public static function sortRightWards(a:Collider, b:Collider):Number{
			if(a.x > b.x) return -1;
			else if(a.x < b.x) return 1;
			return 0;
		}
		
		public static function sortTopWards(a:Collider, b:Collider):Number{
			if(a.y < b.y) return -1;
			else if(a.y > b.y) return 1;
			return 0;
		}
		
		public static function sortBottomWards(a:Collider, b:Collider):Number{
			if(a.y > b.y) return -1;
			else if(a.y < b.y) return 1;
			return 0;
		}
#endif
		
		/* add a child Collider to this Collider - it will move when this collider moves */
		public void addChild(Collider collider){
			collider.parent = this;
			collider.vy = 0;
			// optimisation:
			// children must be ordered leftwards so their parent can
			// move them with out them colliding into each other
			if(children.Count != 0){
				if(children.Count == 1){
					if(collider.x < children[0].x){
						//children.unshift(collider);
						children.Insert(0, collider);
					} else {
						//children.push(collider);
						children.Add(collider);
					}
				} else {
					//children.push(collider);
					children.Add(collider);
///					children.sort(sortLeftWards);
//FIXME:	
				}
			} else {
				children[0] = collider;
			}
		}
		
		/* remove a child collider from children */
		public void removeChild(Collider collider){
			collider.parent = null;
			//children.splice(children.indexOf(collider), 1);
			children.Remove(collider);
			collider.awake = AWAKE_DELAY;
		}
		
		/* Get rid of children and parent - used to remove the collider from the game and clear current interaction */
		public void divorce(){
			if(parent != null){
				parent.removeChild(this);
				vy = 0;
			}
			Collider collider;
			for(int i = 0; i < children.Count; i++){
				collider = children[i];
				collider.parent = null;
				collider.vy = 0;
				collider.awake = AWAKE_DELAY;
			}
			pressure = 0;
			//children.length = 0;
			children.Clear();
			awake = AWAKE_DELAY;
		}
		
		/* Creates a parent Collider out of thin air for this Collider - there are edge cases where this is desirable */
		public void createParent(int properties){
			if(parent != null) parent.removeChild(this);
			mapCollider.x = x - width * 0.5;
			mapCollider.y = world.bounds.y + world.bounds.height;
			mapCollider.properties = properties;
			mapCollider.addChild(this);
		}
		
		public double moveX(double vx, Collider source = null){
			if((vx > 0 ? vx : -vx) < MOVEMENT_TOLERANCE) return 0;
			int i;
			List<Collider> obstacles;
			Collider collider;
			double obstacleShouldMove;
			double obstacleActuallyMoved;
			int mapX;
			int mapY;
			double n;
			int minX;
			int minY;
			int maxX;
			int maxY;
			int property;
			double tempDamping;
			if(vx > 0){
				
				// =============================================================================
				// collision with map:
				if(state != MAP_COLLIDER){
					// inline Math.ceil on X axis
					n = (x + width + vx - INTERVAL_TOLERANCE) * world.invScale;
					//maxX = n != n >> 0 ? (n >> 0) + 1 : n >> 0;
					maxX = (int)Math.Ceiling( n );
					maxY = (int)((y + height - INTERVAL_TOLERANCE) * world.invScale);
					n = (x + width - INTERVAL_TOLERANCE) * world.invScale;
					//minX = n != n >> 0 ? (n >> 0) + 1 : n >> 0;
					minX = (int)Math.Ceiling( n );
					if(minX >= world.width) minX = world.width - 1;
					if(maxX >= world.width) maxX = world.width - 1;
					minY = (int)((y + INTERVAL_TOLERANCE) * world.invScale);
					
					//scanForwards:
					for(mapX = minX; mapX <= maxX; mapX++){
						for(mapY = minY; mapY <= maxY; mapY++){
							property = world.map[mapY][mapX];
							if(mapX * world.scale < (x + width - INTERVAL_TOLERANCE) + vx && (property & LEFT) != 0 && !((property & ignoreProperties) != 0)){
								vx -= (x + width + vx) - mapX * world.scale;
								if(this.vx > 0) this.vx = 0;
								pressure |= RIGHT;
								//break scanForwards;
								goto scanForwards;
							}
						}
					}
					scanForwards: ;
				}
				
				// =============================================================================
				// collision with other Colliders:
				// check there's still velocity to justify a check
				if((vx > 0 ? vx : -vx) > MOVEMENT_TOLERANCE){
					obstacles = world.getCollidersIn(new Rectangle(x + width, y, vx, height), this, LEFT, ignoreProperties);
					// small optimisation here - sorting needs to be avoided
#if false
					if(obstacles.Count > 2 ) obstacles.sort(sortLeftWards);
					else if(obstacles.Count == 2){
						if(obstacles[0].x > obstacles[1].x){
							tempCollider = obstacles[0];
							obstacles[0] = obstacles[1];
							obstacles[1] = tempCollider;
						}
					}
#endif
					for(i = 0; i < obstacles.Count; i++){
						collider = obstacles[i];
						// because the vx may get altered over this loop, we need to still check for overlap
						if(collider.x < x + width + vx){
							// bypass colliders we're already inside and platform vs platform
							if(collider.x > x + width - INTERVAL_TOLERANCE && !(state == MAP_COLLIDER && collider.state == MAP_COLLIDER)){
								
								obstacleShouldMove = (x + width + vx) - collider.x;
								if(collider.state == MAP_COLLIDER) obstacleActuallyMoved = 0;
								else if(collider.pushDamping == 1 || state == MAP_COLLIDER) obstacleActuallyMoved = collider.moveX(obstacleShouldMove, this);
								else obstacleActuallyMoved = collider.moveX(obstacleShouldMove * collider.pushDamping, this);
								
								if(collider.state != MAP_COLLIDER){
									collider.pressure |= LEFT;
									if(state != MAP_COLLIDER){
										collider.leftContact = this;
										rightContact = collider;
									}
								}
								
								if(state != MAP_COLLIDER){
									if(obstacleActuallyMoved < obstacleShouldMove){
										vx -= obstacleShouldMove - obstacleActuallyMoved;
										// kill energy when recursively hitting bounds
										if(collider.vx == 0 && this.vx > 0) this.vx = 0;
									}
									pressure |= RIGHT;
								} else {
									if(obstacleActuallyMoved < obstacleShouldMove){
										//collider.crushed = true;
									}
								}
							}
						} else break;
					}
				}
				
				// =============================================================================
				// collision with bounds:
				if(state != MAP_COLLIDER && x + width + vx > world.bounds.x + world.bounds.width){
					vx -= (x + width + vx) - (world.bounds.x + world.bounds.width);
					this.vx = 0;
					if(boundsPressure) pressure |= RIGHT;
				}
			} else if(vx < 0){
				
				// =============================================================================
				// collision with map:
				if(state != MAP_COLLIDER){
					// inline Math.floor on X axis
					n = (x + vx) * world.invScale - 1;
					//maxX = n << 0;
					maxX = (int)n;
					maxY = (int)((y + height - INTERVAL_TOLERANCE) * world.invScale);
					n = x * world.invScale - 1;
					//minX = n << 0;
					minX = (int)n;
					if(minX < 0) minX = 0;
					if(maxX < 0) maxX = 0;
					minY = (int)((y + INTERVAL_TOLERANCE) * world.invScale);
					
					//scanBackwards:
					for(mapX = minX; mapX >= maxX; mapX--){
						for(mapY = minY; mapY <= maxY; mapY++){
							property = world.map[mapY][mapX];
							if((mapX + 1) * world.scale - INTERVAL_TOLERANCE > x + vx && (property & RIGHT) != 0 && !((property & ignoreProperties) != 0)){
								vx -= (x + vx) - (mapX + 1) * world.scale;
								if(this.vx < 0) this.vx = 0;
								pressure |= LEFT;
								//break scanBackwards;
								goto scanBackwards;
							}
						}
					}
					scanBackwards: ;
				}
				
				// =============================================================================
				// collision with other Colliders:
				// check there's still velocity to justify a check
				if((vx > 0 ? vx : -vx) > MOVEMENT_TOLERANCE){
					obstacles = world.getCollidersIn(new Rectangle(x + vx, y, -vx, height), this, RIGHT, ignoreProperties);
#if false
					// small optimisation here - sorting needs to be avoided
					if(obstacles.Count > 2 ) obstacles.sort(sortRightWards);
					else if(obstacles.length == 2){
						if(obstacles[0].x < obstacles[1].x){
							tempCollider = obstacles[0];
							obstacles[0] = obstacles[1];
							obstacles[1] = tempCollider;
						}
					}
#endif
					for(i = 0; i < obstacles.Count; i++){
						collider = obstacles[i];
						// because the vx may get altered over this loop, we need to still check for overlap
						if(collider.x + collider.width > x + vx){
							// bypass colliders we're already inside and platform vs platform
							if(collider.x + collider.width - INTERVAL_TOLERANCE < x && !(state == MAP_COLLIDER && collider.state == MAP_COLLIDER)){
								
								obstacleShouldMove = (x + vx) - (collider.x + collider.width);
								if(collider.state == MAP_COLLIDER) obstacleActuallyMoved = 0;
								else if(collider.pushDamping == 1 || state == MAP_COLLIDER) obstacleActuallyMoved = collider.moveX(obstacleShouldMove, this);
								else obstacleActuallyMoved = collider.moveX(obstacleShouldMove * collider.pushDamping, this);
								
								if(collider.state != MAP_COLLIDER){
									collider.pressure |= RIGHT;
									if(state != MAP_COLLIDER){
										collider.rightContact = this;
										leftContact = collider;
									}
								}
								
								if(state != MAP_COLLIDER){
									if(obstacleActuallyMoved > obstacleShouldMove){
										vx += obstacleActuallyMoved - obstacleShouldMove;
										// kill energy when recursively hitting bounds
										if(collider.vx == 0 && this.vx < 0) this.vx = 0;
									}
									pressure |= LEFT;
								} else {
									if(obstacleActuallyMoved > obstacleShouldMove){
										//collider.crushed = true;
									}
								}
							}
						} else break;
					}
				}
				
				// =============================================================================
				// collision with bounds:
				if(state != MAP_COLLIDER && x + vx < world.bounds.x){
					vx += world.bounds.x - (x + vx);
					this.vx = 0;
					if(boundsPressure) pressure |= LEFT;
				}
			}
			x += vx;
			
			// if the collider has a parent, check it is still sitting on it
			if(parent != null && (x + width <= parent.x || x >= parent.x + parent.width)){
				parent.removeChild(this);
			}
			// if the collider has children, move them
			if(children.Count > 0){
				if(vx > 0){
					for(i = children.Count - 1; i > -1; i--){
						collider = children[i];
						collider.moveX(vx);
					}
				} else if(vx < 0){
					for(i = 0; i < children.Count; i++){
						collider = children[i];
						collider.moveX(vx);
					}
				}
			}
			awake = AWAKE_DELAY;
			tempCollider = null;
			return vx;
		}
		
		public double moveY(double vy, Collider source = null){
			if((vy > 0 ? vy : -vy) < MOVEMENT_TOLERANCE) return 0;
			int i, j;
			List<Collider> obstacles;
			List<Collider> stompees;
			Collider collider;
			double obstacleShouldMove;
			double obstacleActuallyMoved;
			int mapX;
			int mapY;
			double n;
			int minX;
			int minY;
			int maxX;
			int maxY;
			int property;
			if(vy > 0){
				
				// =============================================================================
				// collision with map:
				if(state != MAP_COLLIDER){
					// inline Math.ceil on Y axis
					n = (y + height + vy - INTERVAL_TOLERANCE) * world.invScale;
					//maxY = n != n >> 0 ? (n >> 0) + 1 : n >> 0;
					maxY = (int)Math.Ceiling( n );
					maxX = (int)((x + width - INTERVAL_TOLERANCE) * world.invScale);
					n = (y + height - INTERVAL_TOLERANCE) * world.invScale;
					//minY = n != n >> 0 ? (n >> 0) + 1 : n >> 0;
					minY = (int)Math.Ceiling( n );
					if(minY >= world.height) minY = world.height - 1;
					if(maxY >= world.height) maxY = world.height - 1;
					minX = (int)((x + INTERVAL_TOLERANCE) * world.invScale);
					
					//scanForwards:
					for(mapY = minY; mapY <= maxY; mapY++){
						for(mapX = minX; mapX <= maxX; mapX++){
							property = world.map[mapY][mapX];
							if(mapY * world.scale < y + height + vy && (property & UP) != 0 && !((property & ignoreProperties) != 0)){
								vy -= (y + height + vy) - mapY * world.scale;
								if(this.vy > 0) this.vy = 0;
								pressure |= DOWN;
								// create a dummy collider surface to stand on
								if(stackable && parent != mapCollider){
									if(parent != null) parent.removeChild(this);
									mapCollider.x = mapX * world.scale;
									mapCollider.y = mapY * world.scale;
									mapCollider.properties = property;
									mapCollider.addChild(this);
								}
								//break scanForwards;
								goto scanForwards;
							}
						}
					}
					scanForwards: ;
				}
				
				// =============================================================================
				// collision with other Colliders:
				// check there's still velocity to justify a check
				if((vy > 0 ? vy : -vy) > MOVEMENT_TOLERANCE){
					obstacles = world.getCollidersIn(new Rectangle(x, y + height, width, vy), this, UP, ignoreProperties);
					// small optimisation here - sorting needs to be avoided
#if false
					if(obstacles.Count > 2 ) obstacles.sort(sortTopWards);
					else if(obstacles.length == 2){
						if(obstacles[0].y > obstacles[1].y){
							tempCollider = obstacles[0];
							obstacles[0] = obstacles[1];
							obstacles[1] = tempCollider;
						}
					}
#endif
					for(i = 0; i < obstacles.Count; i++){
						collider = obstacles[i];
						// because the vy may get altered over this loop, we need to still check for overlap
						if(collider.y < y + height + vy){
							// bypass colliders we're already inside and platform vs platform
							if(collider.y > y + height - INTERVAL_TOLERANCE && !(state == MAP_COLLIDER && collider.state == MAP_COLLIDER)){
								
								obstacleShouldMove = (y + height + vy) - collider.y;
								if(collider.state == MAP_COLLIDER) obstacleActuallyMoved = 0;
								else if(collider.pushDamping == 1 || state == MAP_COLLIDER) obstacleActuallyMoved = collider.moveY(obstacleShouldMove, this);
								else obstacleActuallyMoved = collider.moveY(obstacleShouldMove * collider.pushDamping, this);
								
								if(collider.state != MAP_COLLIDER){
									collider.pressure |= UP;
									if(state != MAP_COLLIDER){
										collider.upContact = this;
										downContact = collider;
									}
								}
								
								if(state != MAP_COLLIDER){
									
#if false
									// ==========================================================================
									// STOMP LOGIC
									// this is specific to Red Rogue, used by Characters to perform their stomp-stun attack
									if(stompProperties && Boolean(collider.stompCallback)){
										
										n = collider.x + collider.width * 0.5;
										if(n < x + width * 0.5){
											collider.moveX(x - (collider.x + collider.width + MOVEMENT_TOLERANCE));
										} else {
											collider.moveX((x + width + MOVEMENT_TOLERANCE) - collider.x);
										}
										// if the collider is not STACKed, kick it downwards
										if(collider.state != Collider.STACK){
											collider.moveY(obstacleShouldMove, this);
											collider.state = Collider.FALL;
										}
										// scan, the stomp may not have pushed the collider free - it is doomed
										//stompees = world.getCollidersIn(new Rectangle(x, y + vy, width, height), this, stompProperties);
										//for(j = 0; j < stompees.length; j++){
											//tempCollider = stompees[j];
											//if(tempCollider == collider) collider.crushed = true;
										//}
										 //stomp callback only when the victim's center is under our base - be generous
										//if(!collider.crushed && (n < x || n > x + width - INTERVAL_TOLERANCE)) collider.stompCallback(this);
										collider.stompCallback(this);
										divorce();
										
									} else {
										if(obstacleActuallyMoved < obstacleShouldMove){
											vy -= obstacleShouldMove - obstacleActuallyMoved;
											// kill energy when recursively hitting bounds
											if(collider.vy == 0 && this.vy > 0) this.vy = 0;
										}
										pressure |= DOWN;
									
										// make this Collider a child of the obstacle
										if((stackable && collider.stackable) && collider != parent){
											if(parent) parent.removeChild(this);
											collider.addChild(this);
										}
									}
#endif
								} else {
									if(obstacleActuallyMoved < obstacleShouldMove){
										//collider.crushed = true;
									}
								}
							}
						} else break;
					}
				}
				
				// =============================================================================
				// collision with bounds:
				if(state != MAP_COLLIDER && y + height + vy > world.bounds.y + world.bounds.height){
					vy -= (y + height + vy) - (world.bounds.y + world.bounds.height);
					this.vy = 0;
					if(boundsPressure) pressure |= DOWN;
				}
			} else if(vy < 0){
				
				// =============================================================================
				// collision with map:
				if(state != MAP_COLLIDER){
					// inline Math.floor on X axis
					n = (y + vy) * world.invScale - 1;
					//maxY = n << 0;
					maxY = (int)n;
					maxX = (int)((x + width - INTERVAL_TOLERANCE) * world.invScale);
					n = y * world.invScale - 1;
					//minY = n << 0;
					minY = (int)n;
					if(minY < 0) minY = 0;
					if(maxY < 0) maxY = 0;
					minX = (int)((x + INTERVAL_TOLERANCE) * world.invScale);
					
					//scanBackwards:
					for(mapY = minY; mapY >= maxY; mapY--){
						for(mapX = minX; mapX <= maxX; mapX++){
							property = world.map[mapY][mapX];
							if((mapY + 1) * world.scale > y + vy && (property & DOWN) != 0 && !((property & ignoreProperties) != 0)){
								vy -= (y + vy) - ((mapY + 1) * world.scale);
								if(this.vy < 0) this.vy = 0;
								pressure |= UP;
								//break scanBackwards;
								goto scanBackwards;
							}
						}
					}
					scanBackwards: ;
				}
				
				// =============================================================================
				// collision with other Colliders:
				// check there's still velocity to justify a check
				if((vy > 0 ? vy : -vy) > MOVEMENT_TOLERANCE){
					obstacles = world.getCollidersIn(new Rectangle(x, y + vy, width, -vy), this, DOWN, ignoreProperties);
					// small optimisation here - sorting needs to be avoided
#if false
					if(obstacles.length > 2 ) obstacles.sort(sortBottomWards);
					else if(obstacles.length == 2){
						if(obstacles[0].y < obstacles[1].y){
							tempCollider = obstacles[0];
							obstacles[0] = obstacles[1];
							obstacles[1] = tempCollider;
						}
					}
#endif
					for(i = 0; i < obstacles.Count; i++){
						collider = obstacles[i];
						// because the vy may get altered over this loop, we need to still check for overlap
						if(collider.y + collider.height > y + vy){
							// bypass colliders we're already inside and platform vs platform
							if(collider.y + collider.height - INTERVAL_TOLERANCE < y && !(state == MAP_COLLIDER && collider.state == MAP_COLLIDER)){
								
								obstacleShouldMove = (y + vy) - (collider.y + collider.height);
								if(collider.state == MAP_COLLIDER) obstacleActuallyMoved = 0;
								else if(collider.pushDamping == 1 || state == MAP_COLLIDER) obstacleActuallyMoved = collider.moveY(obstacleShouldMove, this);
								else obstacleActuallyMoved = collider.moveY(obstacleShouldMove * collider.pushDamping, this);
								
								if(collider.state != MAP_COLLIDER){
									collider.pressure |= DOWN;
									if(state != MAP_COLLIDER){
										collider.downContact = this;
										upContact = collider;
									}
								}
								
								if(state != MAP_COLLIDER){
									if(obstacleActuallyMoved > obstacleShouldMove){
										vy += obstacleActuallyMoved - obstacleShouldMove;
										// kill energy when recursively hitting bounds
										if(collider.vy == 0 && this.vy < 0) this.vy = 0;
									}
									pressure |= UP;
								} else {
									if(obstacleActuallyMoved > obstacleShouldMove){
										//collider.crushed = true;
									}
								}
								// make the obstacle a child of this Collider
								if((stackable && collider.stackable) && collider.state != MAP_COLLIDER && collider.parent != this && collider.pushDamping > 0){
									if(collider.parent != null) collider.parent.removeChild(collider);
									addChild(collider);
								}
							}
						} else break;
					}
				}
				
				// =============================================================================
				// collision with bounds:
				if(state != MAP_COLLIDER && y + vy < world.bounds.y){
					vy += world.bounds.y - (y + vy);
					this.vy = 0;
					if(boundsPressure) pressure |= UP;
				}
			}
			y += vy;
			
			// move children - ie: blocks stacked on top of this Collider
			// stacked children should not be moved when travelling up - this Collider is already taking care of that
			// by pushing them, climbing children on the other hand must be moved
			if(vy > 0){
				for(i = 0; i < children.Count; i++){
					collider = children[i];
					collider.moveY(vy);
				}
			// if there is a parent, is it still below?
			} else if(vy < 0){
				if(parent != null && parent != source && parent.y > y + height + INTERVAL_TOLERANCE){
					parent.removeChild(this);
				}
				for(i = 0; i < children.Count; i++){
					collider = children[i];
					if(collider.state == HOVER){
						children[i].moveY(vy);
					}
				}
			}
			awake = AWAKE_DELAY;
			tempCollider = null;
			return vy;
		}
		
		/* Return a recent contact */
		public Collider getContact() {
			if(upContact != null) return upContact;
			else if(rightContact != null) return rightContact;
			else if(downContact != null) return downContact;
			else if(leftContact != null) return leftContact;
			return null;
		}
		
#if false
		/* Pushes a collider out of any map surfaces it overlaps - used to resolve changing a collider's shape */
		public function resolveMapInsertion(world:CollisionWorld = null):void{
			world = world || this.world;
			if(!world) return;
			
			var mapX:int, mapY:int;
			
			mapY = (y + height * 0.5) * world.invScale;
			
			mapX = x * world.invScale;
			if((world.map[mapY][mapX] & RIGHT) && x >= (mapX + 0.5) * world.scale) x = (mapX + 1) * world.scale;
			
			mapX = (x + width - INTERVAL_TOLERANCE) * world.invScale;
			if((world.map[mapY][mapX] & LEFT) && x + width - INTERVAL_TOLERANCE <= (mapX + 0.5) * world.scale) x = mapX * world.scale-width;
			
			mapX = (x + width * 0.5) * world.invScale;
			
			mapY = y * world.invScale;
			if((world.map[mapY][mapX] & DOWN) && y >= (mapY + 0.5) * world.scale) y = (mapY + 1) * world.scale;
			
			mapY = (y + height - INTERVAL_TOLERANCE) * world.invScale;
			if((world.map[mapY][mapX] & UP) && y + height - INTERVAL_TOLERANCE <= (mapY + 0.5) * world.scale) y = mapY * world.scale-height;
			
		}
		
		/* Draw debug diagram */
		public function draw(gfx:Graphics):void{
			gfx.lineStyle(1, 0x33AA66);
			gfx.drawRect(x, y, width, height);
			if(awake){
				gfx.drawRect(x + width * 0.4, y + height * 0.4, width * 0.2, height * 0.2);
			}
			if(parent != null){
				gfx.moveTo(x + width * 0.5, y + height * 0.5);
				gfx.lineTo(parent.x + parent.width * 0.5, parent.y + parent.height * 0.5);
			}
			if(state == STACK){
				gfx.drawCircle(x + width * 0.5, y + height - height * 0.25, Math.min(width, height) * 0.25);
			} else if(state == FALL){
				gfx.drawCircle(x + width * 0.5, y + height * 0.5, Math.min(width, height) * 0.25);
			}
			if(pressure){
				if(pressure & UP){
					gfx.moveTo(x + width * 0.2, y + height * 0.2);
					gfx.lineTo(x + width * 0.8, y + height * 0.2);
				}
				if(pressure & RIGHT){
					gfx.moveTo(x + width * 0.8, y + height * 0.2);
					gfx.lineTo(x + width * 0.8, y + height * 0.8);
				}
				if(pressure & DOWN){
					gfx.moveTo(x + width * 0.2, y + height * 0.8);
					gfx.lineTo(x + width * 0.8, y + height * 0.8);
				}
				if(pressure & LEFT){
					gfx.moveTo(x + width * 0.2, y + height * 0.2);
					gfx.lineTo(x + width * 0.2, y + height * 0.8);
				}
			}
		}
#endif

		override public string toString() {
			return "(x:"+x+" y:"+y+" width:"+width+" height:"+height+" type:"+propertiesToString(properties)+")";
		}
		
		/* Returns all properties of this block as a string */
		public static string propertiesToString(int type) {
			if (type == EMPTY) return "EMPTY";
			int n;
			string s = "";
			for (int i = 0; i < 12; i++){
				n = type & (1 << i);
				if (s == "UP|RIGHT|DOWN|LEFT|") s = "SOLID|";
				if (n == UP) s += "UP|";
				else if (n == RIGHT) s += "RIGHT|";
				else if (n == DOWN) s += "DOWN|";
				else if (n == LEFT) s += "LEFT|";
				else if (n == STATIC) s += "STATIC|";
				else if (n == BREAKABLE) s += "BREAKABLE|";
				else if (n == FREE) s += "FREE|";
				else if (n == MOVING) s += "MOVING|";
				else if (n == MONSTER) s += "MONSTER|";
				else if (n == PLAYER) s += "PLAYER|";
				else if (n == LEDGE) s += "LEDGE|";
				else if (n == WALL) s += "WALL|";
				else if (n == CHARACTER) s += "CHARACTER|";
			}
			return s.Substring(0, s.Length - 1);
		}
	}
}