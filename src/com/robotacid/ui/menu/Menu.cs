﻿using System;
using System.Collections.Generic;

using redroguecs;

using com.robotacid.gfx;
///import com.robotacid.gfx.CaptureBitmap;
///import com.robotacid.gfx.DebrisFX;
using com.robotacid.sound;
///import com.robotacid.ui.Key;
///import com.robotacid.ui.TextBox;
///import flash.display.Bitmap;
///import flash.display.BitmapData;
///import flash.display.DisplayObjectContainer;
///import flash.display.MovieClip;
///import flash.display.Shape;
using flash.display;
using flash.events;
using flash.geom;
using Matrix = flash.geom.Matrix;
///import flash.geom.Point;
///import flash.geom.Rectangle;
///import flash.text.TextLineMetrics;
using flash.ui;
using flash;

namespace com.robotacid.ui.menu {
	
	/**
	 * This is a key based menu system, designed to allow the player to maintain
	 * preferences and player inventory / skills from one place. The ability to
	 * modify key inputs and setting up of "hot keys" that activate user defined
	 * menu options has been implemented.
	 *
	 * To use: Define MenuLists and MenuOptions. MenuOptions can be pointers to MenuLists
	 * or when they are not, an attempt to traverse right from them will fire an event.
	 *
	 * There are two methods to override:
	 *
	 * executeSelection: The user has traversed right until they have reached a MenuOption that does
	 * not point to a MenuList. Steping right from that option fires the execute method.
	 * Listening to this event gives the programmer the opportunity to examine the "branch"
	 * property of the Menu and see what the user selected. An appropriate method can then
	 * be called.
	 *
	 * changeSelection: Every time the menu is moved, change is called. The programmer may want to
	 * emit a noise for this, or deactivate/reactivate options based on where the user has
	 * walked to. Bear in mind that MenuOptions are already capable of deactivating other
	 * options when stepped forward through (this is to prevent infinite menu walks, whilst
	 * allowing a recursive path to define hot keys).
	 *
	 * @author Aaron Steed, robotacid.com
	 */
	public class Menu : Sprite {
		
		public static Game game;
		
		// gfx
		public MenuCarousel carousel;
		public Sprite textHolder;
		public TextBox help;
		public Bitmap selectionWindow;
		public Bitmap selectionWindowTaperPrevious;
		public Bitmap selectionWindowTaperNext;
		public TextBox previousTextBox;
		public TextBox currentTextBox;
		public TextBox nextTextBox;
		public TextBox infoTextBox;
		public CaptureBitmap capture;
		public Bitmap selectionCopyBitmap;
		public TextBox selectText;
		
		public int selection;
		public bool hideChangeSelection;
		
		public Vector<MenuList> branch;
		public string branchStringCurrentOption;
		public string branchStringHistory;
		public string branchStringSeparator = "/";
		
		public Vector<HotKeyMap> hotKeyMaps;
		public HotKeyMap hotKeyMapRecord;
		public MenuOption changeKeysOption;
		public MenuOption hotKeyOption;
		public MenuOption inputOption;
		
		public MenuList previousMenuList;
		public MenuList currentMenuList;
		public MenuList nextMenuList;
		
		public static MenuList keyChanger;
		
		// display area that the menu takes up
		public double _width;
		public double _height;
		
		public Shape maskShape;
		
		// animation and key states - move delay is consistent through all menus
		public bool keyLock;
		public static int moveDelay = 4;
		
		private Vector<int> dirStack;
		private int dir;
		private int moveCount;
		private int moveReset;
		private double vx;
		private double vy;
		private double previousAlphaStep;
		private double currentAlphaStep;
		private double nextAlphaStep;
		private double captureAlphaStep;
		private double helpVy;
		private double alphaStep;
		private int keysDown;
		private Vector<Boolean> hotKeyDown;
		private int keysLocked;
		private int keysHeldCount;
		private int stackCount;
///		private var movementMovieClips:Vector.<MovieClip>;
		private int movementGuideCount;
		private bool animatingSelection;
		private int notVistitedColFrame;
		private bool inputKeyPressed;
		
		public static readonly double LIST_WIDTH = 100d;
		public static readonly double LINE_SPACING = 11d;
		public static readonly double SELECTION_WINDOW_TAPER_WIDTH = 50d;
		public static readonly double INFO_TEXT_BOX_WIDTH = 110d;
		public static readonly double SIDE_ALPHAS = 0.7d;
		public static readonly uint SELECTION_WINDOW_COL = 0xFFEEEEEE;
		public static readonly int KEYS_HELD_DELAY = 5;
		public static readonly int MOVEMENT_GUIDE_DELAY = 30;
		public static readonly ColorTransform DISABLED_COL = new ColorTransform(1, 1, 1, 1, -100, -100, -100);
		public static readonly uint BACKGROUND_COL = 0x66111111;
		public static readonly uint BORDER_COL = 0xFF999999;
		
		public static Vector<ColorTransform> NOT_VISITED_COLS;
		
		// game key properties
		public static readonly int UP_KEY = 0;
		public static readonly int DOWN_KEY = 1;
		public static readonly int LEFT_KEY = 2;
		public static readonly int RIGHT_KEY = 3;
		public static readonly int MENU_KEY = 4;
		
		// infoTextBox states
		public static readonly int HIDDEN = 0;
		public static readonly int PREVIEW = 1;
		public static readonly int EXPANDED = 2;
		
		public static readonly int HOT_KEY_OFFSET = 5;
		
		public static readonly int UP = 1;
		public static readonly int RIGHT = 2;
		public static readonly int DOWN = 4;
		public static readonly int LEFT = 8;
		
		public static readonly int UP_MOVE = 0;
		public static readonly int RIGHT_MOVE = 1;
		public static readonly int DOWN_MOVE = 2;
		public static readonly int LEFT_MOVE = 3;
		
		public Menu(double width, double height, MenuList trunk = null){
			_width = width;
			_height = height;
			
			int i;
			dirStack = new Vector<int>();
			dir = 0;
			vx = vy = 0;
			moveCount = 0;
			moveReset = moveDelay;
			keysHeldCount = KEYS_HELD_DELAY;
			movementGuideCount = MOVEMENT_GUIDE_DELAY;
			hideChangeSelection = false;
			animatingSelection = false;
			
			// initialise NOT_VISITED_COLS
			initNotVisitedCols();
			
			// initialise the branch recorders - these will help examine the history of
			// menu usage
			branch = new Vector<MenuList>();
			hotKeyMaps = new Vector<HotKeyMap>();
			hotKeyDown = new Vector<Boolean>();
			for(i = 0; i < Key.hotKeyTotal; i++){
				hotKeyMaps.push(null);
				//hotKeyDown[i] = false;
				hotKeyDown.push(false);
			}
			
			// create a mask to contain the menu
			maskShape = new Shape();
			addChild(maskShape);
			maskShape.graphics.beginFill(0xFF0000);
			maskShape.graphics.drawRect(0, 0, _width, _height);
			maskShape.graphics.endFill();
			mask = maskShape;
			
			help = new TextBox(320, 36, BACKGROUND_COL, BORDER_COL);
			
			// create TextBoxes to render the current state of the menu
			textHolder = new Sprite();
			addChild(textHolder);
			textHolder.x = (int)(-LIST_WIDTH * 0.5 + _width * 0.5);
			textHolder.y = (int)((LINE_SPACING * 3) + (_height - (LINE_SPACING * 3)) * 0.5 - LINE_SPACING * 0.5);
			
			previousTextBox = new TextBox(LIST_WIDTH, 1 + LINE_SPACING + TextBox.BORDER_ALLOWANCE * 2, BACKGROUND_COL, BORDER_COL);
			previousTextBox.alpha = 0.7;
			previousTextBox.wordWrap = false;
			previousTextBox.marquee = true;
			currentTextBox = new TextBox(LIST_WIDTH, 1 + LINE_SPACING + TextBox.BORDER_ALLOWANCE * 2, BACKGROUND_COL, BORDER_COL);
			currentTextBox.wordWrap = false;
			currentTextBox.marquee = true;
			nextTextBox = new TextBox(LIST_WIDTH, 1 + LINE_SPACING + TextBox.BORDER_ALLOWANCE * 2, BACKGROUND_COL, BORDER_COL);
			nextTextBox.alpha = 0.7;
			nextTextBox.wordWrap = false;
			nextTextBox.marquee = true;
			infoTextBox = new TextBox(INFO_TEXT_BOX_WIDTH, 169, BACKGROUND_COL, BORDER_COL);
			infoTextBox.wordWrap = false;
			infoTextBox.marquee = true;
			infoTextBox.visible = false;
			capture = new CaptureBitmap();
			capture.visible = false;
			
			previousTextBox.x = -LIST_WIDTH;
			nextTextBox.x = LIST_WIDTH;
			infoTextBox.x = LIST_WIDTH;
			infoTextBox.y = -77;
			
			textHolder.addChild(previousTextBox);
			textHolder.addChild(currentTextBox);
			textHolder.addChild(nextTextBox);
			textHolder.addChild(infoTextBox);
			textHolder.addChild(capture);
			
			previousTextBox.visible = false;
			nextTextBox.visible = false;
			
			// the selection window shows what option we are currently on
			selectionWindow = new Bitmap(new BitmapData((int)LIST_WIDTH, (int)LINE_SPACING));
			selectionCopyBitmap = new Bitmap(selectionWindow.bitmapData.clone());
			selectionCopyBitmap.visible = false;
			selectionWindow.x = -selectionWindow.width * 0.5 + _width * 0.5;
			selectionWindow.y = 1 + (int)((LINE_SPACING * 3) + (_height - (LINE_SPACING * 3)) * 0.5 - LINE_SPACING * 0.5 - TextBox.BORDER_ALLOWANCE) >> 0;
			selectionCopyBitmap.x = selectionWindow.x;
			selectionCopyBitmap.y = selectionWindow.y;
			selectionWindowTaperNext = new Bitmap(new BitmapData((int)SELECTION_WINDOW_TAPER_WIDTH, (int)LINE_SPACING, true, 0x0));
			selectionWindowTaperNext.x = selectionWindow.x + selectionWindow.width;
			selectionWindowTaperNext.y = selectionWindow.y;
			selectionWindowTaperPrevious = new Bitmap(new BitmapData((int)SELECTION_WINDOW_TAPER_WIDTH, (int)LINE_SPACING, true, 0x0));
			selectionWindowTaperPrevious.x = selectionWindow.x - selectionWindowTaperPrevious.width;
			selectionWindowTaperPrevious.y = selectionWindow.y;
			drawSelectionWindow();
			addChild(selectionCopyBitmap);
			addChild(selectionWindow);
			addChild(selectionWindowTaperNext);
			addChild(selectionWindowTaperPrevious);
			
			// selection prompt
			selectText = new TextBox(LIST_WIDTH, 1 + LINE_SPACING + TextBox.BORDER_ALLOWANCE * 2, 0x0, 0x0);
			selectText.text = "select";
			selectText.x = textHolder.x + nextTextBox.x;
			selectText.y = -(TextBox.BORDER_ALLOWANCE + 1) + textHolder.y + nextTextBox.y;
			selectText.alpha = 0;
			addChild(selectText);
			// movement arrows illustate where we can progress on the menu
#if false
			movementMovieClips = new Vector<MovieClip>(4, true);
			for(i = 0; i < movementMovieClips.length; i++){
				movementMovieClips[i] = new MenuArrowMC();
				addChild(movementMovieClips[i]);
				movementMovieClips[i].visible = false;
			}
			movementMovieClips[UP_MOVE].x = (selectionWindow.x + selectionWindow.width * 0.5) >> 0;
			movementMovieClips[UP_MOVE].y = selectionWindow.y;
			movementMovieClips[RIGHT_MOVE].x = selectionWindow.x + selectionWindow.width;
			movementMovieClips[RIGHT_MOVE].y = (selectionWindow.y + selectionWindow.height * 0.5) >> 0;
			movementMovieClips[RIGHT_MOVE].rotation = 90;
			movementMovieClips[DOWN_MOVE].x = 1 + (selectionWindow.x + selectionWindow.width * 0.5) >> 0;
			movementMovieClips[DOWN_MOVE].y = selectionWindow.y + selectionWindow.height;
			movementMovieClips[DOWN_MOVE].rotation = 180;
			movementMovieClips[LEFT_MOVE].x = selectionWindow.x;
			movementMovieClips[LEFT_MOVE].y = 1 + (selectionWindow.y + selectionWindow.height * 0.5) >> 0;
			movementMovieClips[LEFT_MOVE].rotation = -90;
#endif
			
			addChild(help);
			
			if(trunk != null) setTrunk(trunk);
		}
		
		/* Overridden to perform actions the menu selects */
		public virtual void executeSelection(){
			
		}
		
		/* Overridden to change states of options as selections change */
		public virtual void changeSelection(){
			if(currentMenuList.options.length == 0) return;
			MenuOption option = currentMenuList.options[selection];
			if(parent != null && !String.IsNullOrEmpty(option.help)){
				help.text = option.help;
			}
		}
		
		/* The trunk is MenuList 0. All options and lists branch outwards like a tree
		 * from this list. Calling this method also moves the menu to the trunk and
		 * renders the current state
		 */
		public void setTrunk(MenuList menuList){
			currentMenuList = menuList;
			previousMenuList = null;
			branch = new Vector<MenuList>();
			branch.push(menuList);
			branchStringHistory = "";
			moveReset = moveDelay;
			keysHeldCount = KEYS_HELD_DELAY;
			keyLock = true;
			update();
		}
		
		/* Returns a string representation of the current menu history.
		 * Use this to debug the menu and to quickly identify traversed menu paths
		 */
		public String branchString(){
			return branchStringHistory + (branchStringHistory.Length > 0 ? branchStringSeparator : "") + branchStringCurrentOption;
		}
		
		/* Sets the current MenuList selection, re-renders the menu and calls
		 * changeSelection()
		 */
		public void select(int n){
			selection = n;
			currentMenuList.selection = n;
			if(currentMenuList.options.length > 0){
				branchStringCurrentOption = currentMenuList.options[n].name;
				if(currentMenuList.options[n].target != null){
					nextMenuList = currentMenuList.options[n].target;
				} else {
					nextMenuList = null;
				}
			} else {
				nextMenuList = null;
			}
			if(parent != null) renderMenu();
			if(!hideChangeSelection) changeSelection();
		}
		
		/* Either walks forward to the MenuList pointed to by the current option
		 * or when there is no MenuList pointed to, fires the executeSelection method and
		 * jumps the menu back to the trunk.
		 */
		public void stepRight(){
			if(
				!((nextMenuList != null) && !nextMenuList.accessible) && (
					((hotKeyMapRecord != null) && currentMenuList.options[selection].recordable) ||
					((hotKeyMapRecord == null) && currentMenuList.options[selection].active)
				)
			){
				// walk forward
				if(nextMenuList != null){
					// recording?
					if(hotKeyMapRecord != null){
						hotKeyMapRecord.push(currentMenuList.options[currentMenuList.selection], currentMenuList.selection);
					}
					
					// hot key? initialise a HotKeyMap
					if(currentMenuList.options[currentMenuList.selection].hotKeyOption){
						hotKeyMapRecord = new HotKeyMap(currentMenuList.selection, this);
						hotKeyMapRecord.init();
					}
					
					branchStringHistory += (branchStringHistory.Length > 0 ? branchStringSeparator : "") + currentMenuList.options[selection].name;
					
					branch.push(nextMenuList);
					
					previousMenuList = currentMenuList;
					currentMenuList = nextMenuList;
					if(currentMenuList == keyChanger) keyLock = true;
					if(currentMenuList is MenuInputList) (currentMenuList as MenuInputList).begin();
					update();
					
				// nothing to walk forward to - call the executeSelection
				} else {
					dirStack.length = 0;
					// if the Menu is recording a path for a hot key, then we store that
					// hot key here:
					if(hotKeyMapRecord != null){
						hotKeyMapRecord.push(currentMenuList.options[currentMenuList.selection], currentMenuList.selection);
						hotKeyMaps[hotKeyMapRecord.key] = hotKeyMapRecord;
						hotKeyMapRecord = null;
						// because recording a hot key involves deactivating recursive MenuOptions
						// we have to return to the trunk by foot.
						hideChangeSelection = true;
						while(branch.length > 1) stepLeft();
						hideChangeSelection = false;
					} else {
						executeSelection();
						
						int selectionStep = currentMenuList.options[currentMenuList.selection].selectionStep;
						if(selectionStep == MenuOption.TRUNK) setTrunk(branch[0]);
						else {
							if(selectionStep == MenuOption.EXIT_MENU){
								setTrunk(branch[0]);
								if(Game.game.state == Game.MENU){
									Game.game.pauseGame();
								}
							} else {
								// inspect the branch list for empty menu lists that will crash the walk back
								// upon finding one we drop out to the root menu
								for(int i = branch.length - 1; i > -1; i--){
									if(branch[i].options.length == 0){
										setTrunk(branch[0]);
										return;
									}
								}
								// the step back and forth shakes out select events and updates the labels
								while(selectionStep-- > 0){
									stepLeft();
								}
								stepRight();
							}
						}
					}
				}
			}
		}
		
		/* Walk back to the previous MenuList. MenuLists and MenuOptions have no memory
		 * of their forebears, so the branch history and previousMenuList is used
		 */
		public void stepLeft(){
			if(previousMenuList != null){
				// are we recording?
				if(hotKeyMapRecord != null){
					hotKeyMapRecord.pop();
					if(hotKeyMapRecord.length < 0){
						hotKeyMapRecord = null;
					}
				}
				branch.pop();
				if(branch.length > 1){
					//branchStringHistory = branchStringHistory.substr(0, branchStringHistory.lastIndexOf(branchStringSeparator));
					branchStringHistory = branchStringHistory.Substring(0, branchStringHistory.LastIndexOf(branchStringSeparator));
				} else {
					branchStringHistory = "";
				}
				nextMenuList = currentMenuList;
				currentMenuList = previousMenuList;
				previousMenuList = branch.length > 1 ? branch[branch.length - 2] : null;
				
				update();
			}
		}
		
		/* This renders the current menu state, though it is better to use select()
		 * as it will also update the branchString property and update
		 * what MenuList leads from the currently selected MenuOption.
		 */
		public void renderMenu(){
			currentTextBox.visible = true;
			selectionWindow.visible = true;
			if(previousMenuList != null){
				previousTextBox.setSize((int)LIST_WIDTH, (int)(LINE_SPACING * previousMenuList.options.length + TextBox.BORDER_ALLOWANCE));
				previousTextBox.text = previousMenuList.optionsToString("\n", HotKeyMap.getOptionsHotKeyed(previousMenuList, hotKeyMaps));
				previousTextBox.y = -previousMenuList.selection * LINE_SPACING - TextBox.BORDER_ALLOWANCE;
				setLineCols(previousMenuList, previousTextBox);
				previousTextBox.visible = true;
				selectionWindowTaperPrevious.visible = true;
			} else {
				previousTextBox.visible = false;
				selectionWindowTaperPrevious.visible = false;
			}
			if(currentMenuList != null){
				if(currentMenuList == keyChanger){
					keyChanger.options[0].name = "press a key";
				}
				currentTextBox.setSize((int)LIST_WIDTH, (int)(LINE_SPACING * currentMenuList.options.length + TextBox.BORDER_ALLOWANCE));
				currentTextBox.text = currentMenuList.optionsToString("\n", HotKeyMap.getOptionsHotKeyed(currentMenuList, hotKeyMaps));
				currentTextBox.y = -currentMenuList.selection * LINE_SPACING - TextBox.BORDER_ALLOWANCE;
				setLineCols(currentMenuList, currentTextBox);
			}
			if(currentMenuList.options.length == 0){
				// assuming this is a MenuInfo being expanded
				if(currentMenuList is MenuInfo){
					(currentMenuList as MenuInfo).renderCallback();
					infoTextBox.visible = true;
					selectionWindowTaperNext.visible = false;
					nextTextBox.visible = false;
					currentTextBox.visible = false;
					selectionWindow.visible = false;
				}
				
			} else if(currentMenuList.options[selection].active && (nextMenuList != null)){
				if(nextMenuList is MenuInfo){
					(nextMenuList as MenuInfo).renderCallback();
					infoTextBox.visible = true;
					selectionWindowTaperNext.visible = false;
					nextTextBox.visible = false;
				} else {
					if(nextMenuList == keyChanger){
						keyChanger.options[0].name = Key.keyString((uint)Key.custom[selection]);
					}
					nextTextBox.setSize((int)LIST_WIDTH, (int)(LINE_SPACING * nextMenuList.options.length + TextBox.BORDER_ALLOWANCE));
					nextTextBox.text = nextMenuList.optionsToString("\n", HotKeyMap.getOptionsHotKeyed(nextMenuList, hotKeyMaps));
					nextTextBox.y = -nextMenuList.selection * LINE_SPACING - TextBox.BORDER_ALLOWANCE;
					setLineCols(nextMenuList, nextTextBox);
					nextTextBox.visible = true;
					selectionWindowTaperNext.visible = true;
					infoTextBox.visible = false;
				}
			} else {
				infoTextBox.visible = false;
				nextTextBox.visible = false;
				selectionWindowTaperNext.visible = false;
			}
		}
		
		/* Updates the rendering of coloured MenuOptions (disabled and not-visited) */
		public void setLineCols(MenuList menuList, TextBox textBox){
			MenuOption menuOption;
			for(int i = 0; i < menuList.options.length; i++){
				menuOption = menuList.options[i];
				// disabled
				if(
					!(
						((hotKeyMapRecord != null) && menuOption.recordable) ||
						((hotKeyMapRecord == null) && menuOption.active)
					)
				) textBox.setLineCol(i, DISABLED_COL);
				// not visited
				if(!menuOption.visited){
					textBox.setLineCol(i, NOT_VISITED_COLS[notVistitedColFrame]);
				}
			}
		}
		
		private void animateUp(){
			//if((dir & UP) && moveReset > 2) moveReset--;
			
			// capture the next menu
			capture.capture(nextTextBox);
			capture.visible = nextTextBox.visible;
			capture.alpha = nextTextBox.alpha;
			nextTextBox.alpha = 0;
			infoTextBox.alpha = 0;
			nextAlphaStep = SIDE_ALPHAS / moveReset;
			captureAlphaStep = -SIDE_ALPHAS / moveReset;
			
			double currentY = currentTextBox.y;
			select(selection - 1);
			vy = (currentTextBox.y - currentY) / moveReset;
			currentTextBox.y = currentY;
			
			SoundManager.playSound("step");
			
			dir |= UP;
			dir &= ~DOWN;
		}
		
		private void animateDown(){
			//if((dir & DOWN) && moveReset > 2) moveReset--;
			
			// capture the next menu
			capture.capture(nextTextBox);
			capture.visible = nextTextBox.visible;
			capture.alpha = nextTextBox.alpha;
			nextTextBox.alpha = 0;
			infoTextBox.alpha = 0;
			nextAlphaStep = SIDE_ALPHAS / moveReset;
			captureAlphaStep = -SIDE_ALPHAS / moveReset;
			
			double currentY = currentTextBox.y;
			select(selection + 1);
			vy = (currentTextBox.y - currentY) / moveReset;
			currentTextBox.y = currentY;
			
			SoundManager.playSound("step");
			
			dir |= DOWN;
			dir &= ~UP;
		}
		
		private void animateRight(){
			if(nextMenuList != null){
				if(nextMenuList.accessible){
					// capture the previous menu
					capture.capture(previousTextBox);
					capture.visible = previousTextBox.visible;
					capture.alpha = previousTextBox.alpha;
					stepRight();
					
					previousTextBox.x += LIST_WIDTH;
					currentTextBox.x += LIST_WIDTH;
					nextTextBox.x += LIST_WIDTH;
					if(nextMenuList != null){
						infoTextBox.x += LIST_WIDTH;
						infoTextBox.alpha = 0;
					}
					previousTextBox.alpha = 1;
					currentTextBox.alpha = SIDE_ALPHAS;
					nextTextBox.alpha = 0;
					
					previousAlphaStep = (SIDE_ALPHAS - 1.0) / moveReset;
					currentAlphaStep = (1.0 - SIDE_ALPHAS) / moveReset;
					nextAlphaStep = 1.0 / moveReset;
					captureAlphaStep = -SIDE_ALPHAS / moveReset;
					vx = -LIST_WIDTH / moveReset;
			
					SoundManager.playSound("miss", 0.2);
				}
				
			// initialise and launch menu selection animation
			} else {
				// capture an image of the current menu state
				capture.capture(textHolder, new Matrix(1, 0, 0, 1, textHolder.x, textHolder.y), (int)_width, (int)_height);
				capture.x = -textHolder.x;
				capture.y = -textHolder.y;
				capture.alpha = 1;
				capture.visible = true;
				
				// copy, brighten, then erase the text of the selected item
				Rectangle selectionWindowRect = new Rectangle(selectionWindow.x, selectionWindow.y, selectionWindow.width, selectionWindow.height);
				selectionCopyBitmap.bitmapData.copyPixels(capture.bitmapData, selectionWindowRect, new Point());
				capture.bitmapData.fillRect(selectionWindowRect, 0x0);
				selectionCopyBitmap.visible = true;
				
				stepRight();
				// hide the advanced menu
				previousTextBox.alpha = 0;
				currentTextBox.alpha = 0;
				nextTextBox.alpha = 0;
			
				moveReset = moveDelay * 3;
				moveCount = moveReset;
				
				previousAlphaStep = SIDE_ALPHAS / moveReset;
				currentAlphaStep = 1.0 / moveReset;
				nextAlphaStep = SIDE_ALPHAS / moveReset;
				captureAlphaStep = -1.0 / moveReset;
				vx = -LIST_WIDTH / moveReset;
				
				animatingSelection = true;
				selectionWindowTaperNext.visible = false;
			
				SoundManager.playSound("click");
			}
			
			dir |= RIGHT;
			dir &= ~LEFT;
		}
		
		private void animateLeft(){
			// capture the next menu
			if(infoTextBox.visible){
				capture.capture(infoTextBox);
				capture.visible = infoTextBox.visible;
				capture.alpha = 1;
			} else {
				capture.capture(nextTextBox);
				capture.visible = nextTextBox.visible;
				capture.alpha = previousTextBox.alpha;
			}
			stepLeft();
			
			previousTextBox.x -= LIST_WIDTH;
			currentTextBox.x -= LIST_WIDTH;
			nextTextBox.x -= LIST_WIDTH;
			previousTextBox.alpha = 0;
			currentTextBox.alpha = SIDE_ALPHAS;
			nextTextBox.alpha = 1;
			infoTextBox.alpha = 1;
			
			previousAlphaStep = 1.0 / moveReset;
			currentAlphaStep = (1.0 - SIDE_ALPHAS) / moveReset;
			nextAlphaStep = (SIDE_ALPHAS - 1.0) / moveReset;
			captureAlphaStep = -SIDE_ALPHAS / moveReset;
			vx = LIST_WIDTH / moveReset;
			
			SoundManager.playSound("miss", 0.2);
			
			dir |= LEFT;
			dir &= ~RIGHT;
		}
		
		/* We listen for key input here, the keyLock property is used to stop the menu
		 * endlessly firing the same selection */
		public void main(Event e = null){
			int i, j, newKey;
			MenuInputList inputList;
			MenuInfo menuInfo = null;
			
			// handle menu opening animation
			if(help.y < 0){
				help.y += helpVy;
				if(help.y >= 0) help.y = 0;
			}
			if(alpha < 1){
				alpha += alphaStep;
				if(alpha >= 1) alpha = 1;
			}
			
			// if we are listening for input:
			if(currentMenuList is MenuInputList){
				inputList = currentMenuList as MenuInputList;
				if(Key.keysPressed != 0){
					if(!inputKeyPressed){
						newKey = Key.keyLog[Key.KEY_LOG_LENGTH - 1];
						if(inputList.newLineFinish && newKey == Keyboard.ENTER) inputList.finish();
						else if(newKey == Keyboard.DELETE || newKey == Keyboard.BACKSPACE) inputList.removeChar();
						else inputList.addChar(Key.keyString((uint)newKey));
						inputKeyPressed = true;
						renderMenu();
					}
				} else {
					inputKeyPressed = false;
				}
				keyLock = true;
				if(inputList.done) stepLeft();
			}
			// if the keyChanger is active, listen for keys to change the current key set
			if(!keyLock && currentMenuList == keyChanger){
				if(Key.keysPressed != 0){
					// ignore cursor keys - some idiot will try to brick the menu for themselves
					if(!(Key.isDown(Keyboard.UP) || Key.isDown(Keyboard.RIGHT) || Key.isDown(Keyboard.DOWN) || Key.isDown(Keyboard.LEFT))){
						newKey = Key.keyLog[Key.KEY_LOG_LENGTH - 1];
						Key.custom[previousMenuList.selection] = newKey;
						// change the menu names of the affected keys
						changeKeyName(changeKeysOption.target.options[previousMenuList.selection], newKey);
						if(previousMenuList.selection >= HOT_KEY_OFFSET){
							changeKeyName(hotKeyOption.target.options[previousMenuList.selection - HOT_KEY_OFFSET], newKey);
						}
						keyLock = true;
						stepLeft();
					} else {
						game.console.print("that would potentially break the menu");
					}
				}
			}
			// track hot keys so they can instantly perform menu actions
			// hot keys are accessible only when the keyLock is off or the menu is hidden
			if((parent != null && !keyLock) || parent == null){
				for(i = 0; i < Key.hotKeyTotal; i++){
					if(hotKeyMaps[i] != null){
						if(Key.customDown(HOT_KEY_OFFSET + i)){
							if(!hotKeyDown[i]){
								hotKeyMaps[i].execute();
								keyLock = true;
								hotKeyDown[i] = true;
								break;
							}
						} else if(hotKeyDown[i]){
							hotKeyDown[i] = false;
						}
					}
				}
			}
			// load key inputs into a single variable
			int lastKeysDown = keysDown;
			keysDown = 0;
			if(Key.keysPressed != 0){
				// bypass reading keys if the menu is not on the display list
				if(parent != null){
					if(!keyLock){
						
						if(
							((!game.multiplayer && Key.isDown(Keyboard.UP)) || Key.customDown(Game.UP_KEY)) &&
							!((!game.multiplayer && Key.isDown(Keyboard.DOWN)) || Key.customDown(Game.DOWN_KEY))
						){
							keysDown |= UP;
							keysDown &= ~DOWN;
						} else {
							keysLocked &= ~UP;
						}
						if (
							((!game.multiplayer && Key.isDown(Keyboard.DOWN)) || Key.customDown(Game.DOWN_KEY)) &&
							!((!game.multiplayer && Key.isDown(Keyboard.UP)) || Key.customDown(Game.UP_KEY))
						){
							keysDown |= DOWN;
							keysDown &= ~UP;
						} else {
							keysLocked &= ~DOWN;
						}
						if(
							((!game.multiplayer && Key.isDown(Keyboard.LEFT)) || Key.customDown(Game.LEFT_KEY)) &&
							!((!game.multiplayer && Key.isDown(Keyboard.RIGHT)) || Key.customDown(Game.RIGHT_KEY))
						){
							keysDown |= LEFT;
							keysDown &= ~RIGHT;
						} else {
							keysLocked &= ~LEFT;
						}
						if(
							((!game.multiplayer && Key.isDown(Keyboard.RIGHT)) || Key.customDown(Game.RIGHT_KEY)) &&
							!((!game.multiplayer && Key.isDown(Keyboard.LEFT)) || Key.customDown(Game.LEFT_KEY))
						){
							keysDown |= RIGHT;
							keysDown &= ~LEFT;
						} else {
							keysLocked &= ~RIGHT;
						}
					}
				} else {
					keyLock = true;
				}
				if(lastKeysDown != 0 & keysDown != 0){
					if(keysHeldCount != 0) keysHeldCount--;
				}
			} else {
				keyLock = false;
				keysLocked = 0;
				lastKeysDown = 0;
				moveReset = moveDelay;
				keysHeldCount = KEYS_HELD_DELAY;
			}
			// load directions in - keys are locked out of new input unless held down till
			// keysHeldCount reaches zero - then fast browsing is activated
			if(keysDown != 0){
				if(!(keysDown != 0 & keysLocked != 0)){
					dirStack.push(keysDown);
					keysLocked |= keysDown;
				} else if(keysHeldCount == 0 && moveCount == 0){
					dirStack.push(keysDown);
				}
			}
				
			// animate marquees and movement guides
			if(parent != null){
				if(dir == 0 && dirStack.length == 0){
					if(previousTextBox.visible){
						previousTextBox.updateMarquee();
						setLineCols(previousMenuList, previousTextBox);
					}
					if(currentTextBox.visible){
						currentTextBox.updateMarquee();
						setLineCols(currentMenuList, currentTextBox);
					}
					if(nextTextBox.visible){
						nextTextBox.updateMarquee();
						setLineCols(nextMenuList, nextTextBox);
					}
					if(infoTextBox.visible){
						//menuInfo = (nextMenuList as MenuInfo) || (currentMenuList as MenuInfo);
						menuInfo = (nextMenuList as MenuInfo);
						if( menuInfo == null ) menuInfo = (currentMenuList as MenuInfo);
						if(menuInfo.update){
							menuInfo.renderCallback();
						} else {
							infoTextBox.updateMarquee();
						}
					}
					// update the visited glow frame
					notVistitedColFrame++;
					if(notVistitedColFrame >= NOT_VISITED_COLS.length) notVistitedColFrame = 0;
					
					if(movementGuideCount != 0){
						if(currentMenuList != keyChanger && !(currentMenuList is MenuInputList)) movementGuideCount--;
						if(movementGuideCount == 0){
///							for(i = 0; i < movementMovieClips.length; i++) movementMovieClips[i].gotoAndPlay(1);
						}
					} else {
///						movementMovieClips[UP_MOVE].visible = currentMenuList.selection > 0;
///						movementMovieClips[DOWN_MOVE].visible = currentMenuList.selection < currentMenuList.options.length - 1;
///						movementMovieClips[RIGHT_MOVE].visible = currentMenuList.options.length && currentMenuList.options[selection].active && !(nextMenuList && !nextMenuList.accessible);
///						movementMovieClips[LEFT_MOVE].visible = Boolean(previousMenuList);
					}
					if(
						currentMenuList.options.length != 0 &&
						currentMenuList.options[selection].active &&
						nextMenuList == null &&
						!(currentMenuList is MenuInputList) &&
						selectText.alpha < 1
					){
						selectText.alpha += 0.1;
					}
				} else {
					movementGuideCount = MOVEMENT_GUIDE_DELAY;
///					for(i = 0; i < movementMovieClips.length; i++) movementMovieClips[i].visible = false;
					selectText.alpha = 0;
				}
			}
			// check if there are directions loaded into the dirStack
			if(dir == 0){
				do{
					if(dirStack.length != 0){
						dir = dirStack.shift();
						if((dir & UP) != 0 && selection > 0) animateUp();
						else if((dir & DOWN) != 0 && selection < currentMenuList.options.length - 1) animateDown();
						else if(
							(dir & RIGHT) != 0 &&
							(
								currentMenuList.options.length != 0 &&
								!(nextMenuList != null && !nextMenuList.accessible) && (
									(hotKeyMapRecord != null && currentMenuList.options[selection].recordable) ||
									(hotKeyMapRecord == null && currentMenuList.options[selection].active)
								)
							)
						){
							animateRight();
						}
						else if((dir & LEFT) != 0 && previousMenuList != null) animateLeft();
						else {
							// illegal move
							dir = 0;
						}
						// mark option visited
						if(currentMenuList.options.length != 0) currentMenuList.options[currentMenuList.selection].visited = true;
						
					} else break;
				} while(dir == 0);
				if(dir != 0) moveCount = moveReset;
				
			// animate the menu
			} else {
				// selection animation
				if(animatingSelection){
					capture.alpha += captureAlphaStep;
					capture.x += vx;
					if(capture.alpha <= 0){
						selectionCopyBitmap.visible = false;
						animatingSelection = false;
						moveReset = moveDelay;
						vx = 0;
						// flush the direction stack again to avoid leaping off selecting things after the anim
						dirStack.length = 0;
					}
				} else {
					// browsing animation
					if(moveCount != 0){
						if((dir & (UP | DOWN)) != 0){
							currentTextBox.y += vy;
							nextTextBox.alpha += nextAlphaStep;
							infoTextBox.alpha += nextAlphaStep;
							capture.alpha += captureAlphaStep;
						}
						if((dir & (RIGHT | LEFT)) != 0){
							previousTextBox.x += vx;
							currentTextBox.x += vx;
							nextTextBox.x += vx;
							infoTextBox.x += vx;
							capture.x += vx;
							previousTextBox.alpha += previousAlphaStep;
							currentTextBox.alpha += currentAlphaStep;
							nextTextBox.alpha += nextAlphaStep;
							infoTextBox.alpha += nextAlphaStep;
							capture.alpha += captureAlphaStep;
							
							if(infoTextBox.visible){
								//menuInfo = (currentMenuList as MenuInfo) || (nextMenuList as MenuInfo);
								menuInfo = (currentMenuList as MenuInfo);
								if( menuInfo == null ) menuInfo = (nextMenuList as MenuInfo);
								if(menuInfo == currentMenuList || (menuInfo == nextMenuList && (dir & LEFT) != 0)){
									infoTextBox.setSize((int)(infoTextBox.width - vx), (int)infoTextBox.height);
								}
							}
						}
						
						moveCount--;
						// animation over
						if(moveCount == 0){
							// force everything into its right place to flush floating point errors
							currentTextBox.x = 0;
							previousTextBox.x = -LIST_WIDTH;
							nextTextBox.x = LIST_WIDTH;
							infoTextBox.x = (currentMenuList is MenuInfo) ? 0 : LIST_WIDTH;
							currentTextBox.alpha = 1;
							previousTextBox.alpha = nextTextBox.alpha = SIDE_ALPHAS;
							infoTextBox.alpha = 1;
							capture.alpha = 0;
							dir = 0;
							// reduce the animation time when holding down keys for fast browsing
							if(keysDown != 0 && keysHeldCount == 0){
								if(moveReset > 1) moveReset--;
							} else {
								moveReset = moveDelay;
							}
							if(menuInfo != null){
								if(currentMenuList == menuInfo){
									infoTextBox.setSize((int)(INFO_TEXT_BOX_WIDTH + LIST_WIDTH), (int)infoTextBox.height);
								} else if(nextMenuList == menuInfo){
									infoTextBox.setSize((int)INFO_TEXT_BOX_WIDTH, (int)infoTextBox.height);
								}
							}
							update();
						}
					}
				}
			}
		}
		
		/* Update the bitmapdata for the selection window */
		public void drawSelectionWindow(){
#if false
			selectionWindow.bitmapData.fillRect(
				new Rectangle(
					selectionWindow.bitmapData.rect.x,
					selectionWindow.bitmapData.rect.y,
					selectionWindow.bitmapData.rect.width,
					selectionWindow.bitmapData.rect.height
				), SELECTION_WINDOW_COL);
			selectionWindow.bitmapData.fillRect(
				new Rectangle(
					selectionWindow.bitmapData.rect.x + 1,
					selectionWindow.bitmapData.rect.y + 1,
					selectionWindow.bitmapData.rect.width - 2,
					selectionWindow.bitmapData.rect.height- 2
				), 0x0);
				var step:int = 255 / SELECTION_WINDOW_TAPER_WIDTH;
			for(var c:uint = SELECTION_WINDOW_COL, n:int = 0; n < SELECTION_WINDOW_TAPER_WIDTH; c -= 0x01000000 * step, n++){
				selectionWindowTaperNext.bitmapData.setPixel32(n, 0, c);
				selectionWindowTaperNext.bitmapData.setPixel32(n, selectionWindow.height-1, c);
				selectionWindowTaperPrevious.bitmapData.setPixel32(SELECTION_WINDOW_TAPER_WIDTH - n, 0, c);
				selectionWindowTaperPrevious.bitmapData.setPixel32(SELECTION_WINDOW_TAPER_WIDTH - n, selectionWindow.height - 1, c);
			}
#endif
		}
		
		/* Short hand for calling select(currentMenuList.selection) - and more obvious */
		public void update(){
			select(currentMenuList.selection);
		}
		
		/* Called when opening the menu */
		public void activate(){
			update();
			help.y = -help.height;
			helpVy = help.height / (moveDelay * 2);
			alpha = 0;
			alphaStep = 1.0 / moveDelay;
			renderMenu();
			carousel.addChild(this);
		}
		
		/* Called when closing the menu */
		public void deactivate(){
			if(parent != null) parent.removeChild(this);
		}
		
		/* Inits a MenuOption that leads to a MenuList offering the ability to redefine keys. */
		public void initChangeKeysMenuOption(){
			MenuOption keyChangerOption = new MenuOption("no key data");
			Vector<MenuOption> keyChangerOptions = new Vector<MenuOption>();
			keyChangerOptions.push(keyChangerOption);
			keyChanger = new MenuList(keyChangerOptions);
			
			Vector<MenuOption> changeKeysMenuOptions = new Vector<MenuOption>();
			changeKeysMenuOptions.push(new MenuOption("up:" + Key.keyString((uint)Key.custom[0]), keyChanger));
			changeKeysMenuOptions.push(new MenuOption("down:" + Key.keyString((uint)Key.custom[1]), keyChanger));
			changeKeysMenuOptions.push(new MenuOption("left:" + Key.keyString((uint)Key.custom[2]), keyChanger));
			changeKeysMenuOptions.push(new MenuOption("right:" + Key.keyString((uint)Key.custom[3]), keyChanger));
			changeKeysMenuOptions.push(new MenuOption("menu:" + Key.keyString((uint)Key.custom[4]), keyChanger));
			
			MenuList changeKeysMenuList = new MenuList(changeKeysMenuOptions);
			
			for(int i = 0; i < Key.hotKeyTotal; i++){
				changeKeysMenuList.options.push(new MenuOption("hot key:" + Key.keyString((uint)Key.custom[HOT_KEY_OFFSET + i]), keyChanger));
			}
			changeKeysOption = new MenuOption("change keys", changeKeysMenuList);
			changeKeysOption.recordable = false;
		}
		
		/* Returns a MenuOption that leads to a MenuList offering the ability to create "hot keys"
		 *
		 * Recording a hot key involves walking from the trunk out and firing a executeSelection
		 * method. This will not fire the method, but store the selections made and bind
		 * them to that hot key. Pressing the hot key will then walk the menu back to the trunk
		 * and out to the selected option for that hot key.
		 *
		 * This feature requires submitting the trunk MenuList and a MenuOption that can be
		 * deactivated to prevent the user from walking out to the hot key menu again or
		 * doing another menu activity like redefining keys. I will expand the deactivation
		 * reference to an array at a later date.
		 */
		public void initHotKeyMenuOption(MenuList trunk){
			MenuList hotKeyMenuList = new MenuList();
			MenuOption option;
			hotKeyOption = new MenuOption("set hot key");
			hotKeyOption.recordable = false;
			for(int i = 0; i < Key.hotKeyTotal; i++){
				option = new MenuOption("");
				option.name = "hot key:" + Key.keyString((uint)Key.custom[HOT_KEY_OFFSET + i]);
				option.help = "pressing right on the menu will begin recording. execute a menu option to bind it to this key. this will not actually execute the option";
				option.target = trunk;
				option.hotKeyOption = true;
				hotKeyMenuList.options.push(option);
			}
			hotKeyOption.target = hotKeyMenuList;
		}
		
		/* Initialise the glow on non visited options */
		private void initNotVisitedCols(){
			NOT_VISITED_COLS = new Vector<ColorTransform>();
			double colSteps = 30;
			double step = Math.PI / colSteps;
			double colMax = 100;
			ColorTransform colTransform;
			for(int i = 0; i < colSteps; i++){
				colTransform = new ColorTransform(1, 1, 1, 1, colMax * Math.Sin(step * i), colMax * Math.Sin(step * i), colMax * Math.Sin(step * i), colMax * Math.Sin(step * i));
				NOT_VISITED_COLS.push(colTransform);
			}
		}
		
		/* Changes the name of a menu option associated with a key to a given keyCode */
		private void changeKeyName(MenuOption option, int newKey){
			String str = option.name.Substring(0, option.name.IndexOf(":") + 1) + Key.keyString((uint)newKey);
			option.name = str;
		}
		
		/* Checks the branch array for the presence of a MenuList */
		public bool listInBranch(MenuList list) {
			for(int i = 0; i < branch.Count; i++){
				if(branch[i] == list){
					return true;
				}
			}
			return false;
		}
		
	}

}