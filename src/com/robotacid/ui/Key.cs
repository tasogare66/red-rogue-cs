/**
* Keyboard input utility to overcome Adobe's marvellous security restrictions
* Outstanding bug with keys sticking that occurs once in a blue moon - still haven't caught it yet
*
* @author Aaron Steed, robotacid.com
* @version 1.1
*
* Adapted from http://www.kirupa.com/forum/showthread.php?p=2098269
*
* Comments from original follow:
*
* The Key class recreates functionality of
* Key.isDown of ActionScript 1 and 2. Before using
* Key.isDown, you first need to initialize the
* Key class with a reference to the stage using
* its Key.initialize() method. For key
* codes use the flash.ui.Keyboard class.
*
* Usage:
* Key.initialize(stage);
* if (Key.isDown(Keyboard.LEFT)) {
*    // Left key is being pressed
* }
*/

using System;

using flash.display;
using flash.events;
///import flash.events.KeyboardEvent;
using flash.ui;
using flash;
using redroguecs;

namespace com.robotacid.ui {
	
	public class Key {
		public static Boolean initialized = false;	// marks whether or not the class has been initialized
		//private static Array<Boolean> keysDown = new Array<Boolean>();	// stores key codes of all keys pressed
		private static Boolean[] keysDown = new Boolean[flash.ui.Keyboard.KEYMAX];
		public static Array<int> custom;				// list of customised keys
///		public static var reserved:Array = [];	// list of reserved keys
		public static Boolean lockOut = false;		// used to brick the Key class
		public static Stage stage;
		public static int keysPressed = 0;
		public const int NUMBER_0 = 48;
		public const int NUMBER_1 = 49;
		public const int NUMBER_2 = 50;
		public const int NUMBER_3 = 51;
		public const int NUMBER_4 = 52;
		public const int NUMBER_5 = 53;
		public const int NUMBER_6 = 54;
		public const int NUMBER_7 = 55;
		public const int NUMBER_8 = 56;
		public const int NUMBER_9 = 57;
		public const int A = 65;
		public const int B = 66;
		public const int C = 67;
		public const int D = 68;
		public const int E = 69;
		public const int F = 70;
		public const int G = 71;
		public const int H = 72;
		public const int I = 73;
		public const int J = 74;
		public const int K = 75;
		public const int L = 76;
		public const int M = 77;
		public const int N = 78;
		public const int O = 79;
		public const int P = 80;
		public const int Q = 81;
		public const int R = 82;
		public const int S = 83;
		public const int T = 84;
		public const int U = 85;
		public const int V = 86;
		public const int W = 87;
		public const int X = 88;
		public const int Y = 89;
		public const int Z = 90;
		
		public static Array<int> keyLog = new Array<int>();
		public static string keyLogString = "";
		public const int KEY_LOG_LENGTH = 10;
		
		public static int hotKeyTotal = 0;
		
///		public static const KONAMI_CODE:String = [Keyboard.UP, Keyboard.UP, Keyboard.DOWN, Keyboard.DOWN, Keyboard.LEFT, Keyboard.RIGHT, Keyboard.LEFT, Keyboard.RIGHT, B, A].toString();
		
		public Key() {
		}
        /*
        * Initializes the key class creating assigning event
        * handlers to capture necessary key events from the stage
		*
		* optional customKeys is an array of key codes referring to
		* user definable keys
        */
        public static void init(Stage _stage) {
            if (!initialized) {
                stage = _stage;
				
				// assign listeners for key presses and deactivation of the player
				stage.addEventListener(KeyboardEvent.KEY_DOWN, keyPressed);
				stage.addEventListener(KeyboardEvent.KEY_UP, keyReleased);
                stage.addEventListener(Event.DEACTIVATE, (Action1<Event>)clearKeys);
				
				// init key logger
				for(int i = 0; i < KEY_LOG_LENGTH; i++) keyLog.push(0);
				keyLogString = keyLog.ToString();
				
                // mark initialization as true so redundant
                // calls do not reassign the event handlers
                initialized = true;
            }
        }
		
        /**
        * Returns true or false if the key represented by the
        * custom key index is being pressed
        */
        public static Boolean customDown(int index) {
            //return !lockOut && custom != null && (keysDown[custom[index]]);
			return !lockOut && custom != null && (isDown(custom[index]));
        }
		
        /**
        * Returns true or false if the key represented by the
        * keyCode passed is being pressed
        */
        public static Boolean isDown(int keyCode) {
            //return !lockOut && (keysDown[keyCode]);
			return !lockOut && ( GameFrameworkRedRogueCs.Instance.convertKeyDown(keyCode) );
        }
		
		/* Tests whether a pattern of key codes matches the recent key log
		 * patterns are given as strings to skip laborious trawling through arrays of numbers */
		public static Boolean matchLog(String pattern){
			if(pattern.Length > keyLogString.Length) return false;
			return keyLogString.Substring(keyLogString.Length - pattern.Length) == pattern;
		}
		
        /**
        * Event handler for capturing keys being pressed
        */
        private static void keyPressed(KeyboardEvent _event) {
            // create a property in keysDown with the name of the keyCode
			//if(!Boolean(keysDown[_event.keyCode])) keysPressed++;
			if(!(keysDown[_event.keyCode])) keysPressed++;
			keysDown[_event.keyCode] = true;
			
			keyLog.shift();
			keyLog.push(0);	// 追加tasogare66
			keyLog[KEY_LOG_LENGTH - 1] = _event.keyCode;
			keyLogString = keyLog.ToString();
        }
		
        /**
        * Event handler for capturing keys being released
        */
        private static void keyReleased(KeyboardEvent _event) {
            keysDown[_event.keyCode] = false;
			if(keysPressed > 0) keysPressed--;
			else {
				// the keyboard layout may have changed, clear the buffer to repair damage
				clearKeys();
			}
        }
		
        /**
        * Event handler for Flash Player deactivation
        */
        public static void clearKeys(Event _event = null) {
            // clear all keys in keysDown since the player cannot
            // detect keys being pressed or released when not focused
            //keysDown = [];
			Array.Clear(keysDown, 0, keysDown.Length);
			keysPressed = 0;
        }
		
		/*
		 * Return a string representing a key pressed
		 * a 3 letter string is returned for special characters
		 *
		 */
		public static String keyString(uint keyCode){
			switch(keyCode){
				case Keyboard.BACKSPACE:
					return "bsp";
				case Keyboard.CAPS_LOCK:
					return "cpl";
				case Keyboard.CONTROL:
					return "ctr";
				case Keyboard.DELETE:
					return "del";
				case Keyboard.DOWN:
					return "dwn";
				case Keyboard.END:
					return "end";
				case Keyboard.ENTER:
					return "ent";
				case Keyboard.ESCAPE:
					return "esc";
				case Keyboard.HOME:
					return "hom";
				case Keyboard.INSERT:
					return "ins";
				case Keyboard.LEFT:
					return "lft";
				case Keyboard.PAGE_DOWN:
					return "pgd";
				case Keyboard.PAGE_UP:
					return "pgu";
				case Keyboard.RIGHT:
					return "rgt";
				case Keyboard.SHIFT:
					return "sht";
				case Keyboard.SPACE:
					return "spc";
				case Keyboard.TAB:
					return "tab";
				case Keyboard.UP:
					return "up";
				case 186:
					return ":";
				case 188:
					return ".";
				case 190:
					return ",";
				case 191:
					return "?";
				case 109:
					return "n -";
				case 107:
					return "n +";
				case 187:
					return "+";
				case 189:
					return "-";
				case 222:
					return "'";
				default:
					if(keyCode >= 96 && keyCode <= 105){
						//return "n "+String.fromCharCode(keyCode-48);
						return "n "+new String((Char)(keyCode-48), 1);
					} else {
						//return String.fromCharCode(keyCode);
						return new String((Char)(keyCode), 1);
					}
			}
			//return "";
		}
	}
}
