﻿using System;

///import com.robotacid.gfx.BlitClip;
///import com.robotacid.ui.TextBox;
using flash.display;
///import flash.display.BitmapData;
using flash.events;
///import flash.external.ExternalInterface;
///import flash.geom.Point;
///import flash.geom.Rectangle;

namespace com.robotacid.ui {
	
	/**
	 * Used to print out events that occur in the game, it simulates the look of a TextBox.
	 * 
	 * Scrolls new lines of text in by buffering them to images and then scrolling them in
	 * and shifting the image of the old messages out
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class Console : Bitmap{
		
		public int targetScrollDir;
		public String log;
		public int logLines;
		
#if false
		private var lineBuffer:Vector.<BitmapData>;
		private var lineWidthBuffer:Vector.<Number>;
		private var border:BitmapData;
		private var textBox:TextBox;
		private var insertionPoint:BlitClip;
		private var insertionPointPos:Point;
		private var insertionPointRect:Rectangle;
		private var scrollPoint:Point;
		private var point:Point;
		private var scrollSpeed:Number;
		private var scrollDir:int;
		private var scrolling:Boolean;
		private var insertionPointFrame:int;
#endif
		
		public const int LINES = 3;
		public const double HEIGHT = 35;
		public const uint BACKGROUND_COL = 0xFF111111;
		public const uint BORDER_COL = 0xFF999999;
		public const double SCROLL_SPEED_MAX = 4;
		public const double LINE_SPACING = 11;
		public const double SCROLL_UP_STOP_Y = HEIGHT - (LINE_SPACING + 2);
		
		public Console() {
#if false
			super(new BitmapData(Game.WIDTH, HEIGHT, true, BACKGROUND_COL));
			lineBuffer = new Vector.<BitmapData>();
			lineWidthBuffer = new Vector.<Number>();
			point = new Point();
			border = bitmapData.clone();
			border.fillRect(bitmapData.rect, BORDER_COL);
			border.fillRect(new Rectangle(1, 1, bitmapData.width - 2, bitmapData.height - 2), 0x0);
			textBox = new TextBox(Game.WIDTH, LINE_SPACING + 1, BACKGROUND_COL, BACKGROUND_COL);
			textBox.wordWrap = false;
			insertionPoint = Game.renderer.insertionPointBlit;
			insertionPointRect = insertionPoint.rect.clone();
			log = "";
			logLines = 0;
			
			// by default, console text scrolls in upwards
			scrollDir = targetScrollDir = UserData.settings.consoleScrollDir;
			insertionPointPos = new Point();
			scrollPoint = new Point();
			if(scrollDir == 1){
				scrollPoint.y = 0;
				insertionPointPos.y = 3;
			} else if(scrollDir == -1){
				scrollPoint.y = SCROLL_UP_STOP_Y;
				insertionPointPos.y = SCROLL_UP_STOP_Y + 3;
			}
			
			addEventListener(Event.ENTER_FRAME, main, false, 0, true);
#endif
		}
		
		public void main(Event e = null){
#if flase
			if(scrollDir == 1){
				if(scrollPoint.y < 0){
					scrollSpeed = SCROLL_SPEED_MAX;
					if( -scrollPoint.y < SCROLL_SPEED_MAX) scrollSpeed = -scrollPoint.y;
					bitmapData.scroll(0, scrollSpeed);
					scrollPoint.y += scrollSpeed;
					bitmapData.copyPixels(lineBuffer[lineBuffer.length - 1], textBox.bitmapData.rect, scrollPoint);
					bitmapData.copyPixels(border, border.rect, point, null, null, true);
					
					if(scrollPoint.y == 0){
						lineBuffer.pop();
						lineWidthBuffer.pop();
						if(lineBuffer.length){
							scrollPoint.y = -LINE_SPACING;
							insertionPointPos.x = lineWidthBuffer[lineWidthBuffer.length - 1];
						}
						else scrolling = false;
					}
				}
			} else if(scrollDir == -1){
				if(scrollPoint.y > SCROLL_UP_STOP_Y){
					scrollSpeed = -SCROLL_SPEED_MAX;
					if(scrollPoint.y < SCROLL_UP_STOP_Y + SCROLL_SPEED_MAX) scrollSpeed = SCROLL_UP_STOP_Y - scrollPoint.y;
					bitmapData.scroll(0, scrollSpeed);
					scrollPoint.y += scrollSpeed;
					bitmapData.copyPixels(lineBuffer[lineBuffer.length - 1], textBox.bitmapData.rect, scrollPoint);
					bitmapData.copyPixels(border, border.rect, point, null, null, true);
					
					if(scrollPoint.y == SCROLL_UP_STOP_Y){
						lineBuffer.pop();
						lineWidthBuffer.pop();
						if(lineBuffer.length){
							scrollPoint.y = SCROLL_UP_STOP_Y + LINE_SPACING;
							insertionPointPos.x = lineWidthBuffer[lineWidthBuffer.length - 1];
						}
						else scrolling = false;
					}
				}
			}
			if(!scrolling){
				// animate a glowing cursor after the last entry
				insertionPoint.x = insertionPointRect.x = insertionPointPos.x;
				insertionPoint.y = insertionPointRect.y = insertionPointPos.y;
				bitmapData.fillRect(insertionPointRect, BACKGROUND_COL);
				insertionPoint.render(bitmapData, insertionPointFrame++);
				if(insertionPointFrame >= insertionPoint.totalFrames) insertionPointFrame = 0;
				
				// wait until scrolling has stopped before switching the direction of text
				if(targetScrollDir != scrollDir){
					scrollDir = targetScrollDir;
					if(scrollDir == 1){
						scrollPoint.y = 0;
						insertionPointPos.y = 3;
					} else if(scrollDir == -1){
						scrollPoint.y = SCROLL_UP_STOP_Y;
						insertionPointPos.y = SCROLL_UP_STOP_Y + 3;
					}
					print("console scroll direction changed");
				}
			}
#endif
		}
		
		/* Adds a new image of a line of text to the buffer */
		public void print(String str){
#if false
			// catch multiple lines here, split and recurse
			str = str.toUpperCase();
			if(str.indexOf("\n") > -1){
				var printList:Array = str.split("\n");
				while(printList.length) print(printList.shift());
				return;
			}
			textBox.text = str;
			lineBuffer.unshift(textBox.bitmapData.clone());
			lineWidthBuffer.unshift(textBox.lineWidths[0] + textBox.tracking + 2);
			if(scrollDir == 1){
				if(scrollPoint.y == 0) scrollPoint.y = -LINE_SPACING * scrollDir;
			} else if(scrollDir == -1){
				if(scrollPoint.y == SCROLL_UP_STOP_Y) scrollPoint.y = SCROLL_UP_STOP_Y + LINE_SPACING;
			}
			if(!scrolling){
				insertionPointRect.x = insertionPoint.x;
				insertionPointRect.y = insertionPoint.y;
				insertionPointFrame = 0;
				bitmapData.fillRect(insertionPointRect, BACKGROUND_COL);
			}
			insertionPointPos.x = lineWidthBuffer[lineWidthBuffer.length - 1];
			scrolling = true;
			if(Game.allowScriptAccess){
				ExternalInterface.call("printToLog", str);
			}
			log += str + "\n";
			logLines++;
#endif
		}
		
		/* Return the last "lines" number of prints to the log */
		public String getLog(int lines){
#if false
			if(log.length == 0) return "";
			var list:Array = [];
			// wind back from end of log
			var end:int = log.length - 1;
			var start:int;
			do{
				start = log.lastIndexOf("\n", end - 1);
				if(scrollDir == -1) list.unshift(log.substring(start + 1, end));
				else list.push(log.substring(start + 1, end));
				end = start;
			} while(start > -1 && --lines);
			return list.join("\n");
#endif
			return "";	//FIXME:	
		}
		
		/* Changes the scrolling behaviour of the console */
		public void toggleScrollDir(){
			if(targetScrollDir == -1) targetScrollDir = 1;
			else targetScrollDir = -1;
		}
		
	}

}