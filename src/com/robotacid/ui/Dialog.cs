using System;
using redroguecs;

using com.robotacid.ui.menu;
///import flash.display.Bitmap;
///import flash.display.BitmapData;
using flash.display;
///import flash.display.Stage;
using flash.events;
///import flash.events.KeyboardEvent;
///import flash.events.MouseEvent;
using Rectangle = flash.geom.Rectangle;
using flash.ui;

namespace com.robotacid.ui {
	/**
	 * A pop up dialog
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class Dialog : Sprite{
		
		private Boolean active;
		private Action okayCallback;
		private Action cancelCallback;
		private TextBox okayTextBox;
		private TextBox cancelTextBox;
		private Sprite okayButton;
		private Sprite cancelButton;
		private int previousGameState;
		
		public static Game game;
		
		public const double WIDTH = 220d;
		public const uint ROLL_OUT_COL = 0xFF000000;
		public const uint ROLL_OVER_COL = 0xFF555555;
		
		public Dialog(String titleStr, String text, Action okayCallback = null, Action cancelCallback = null) {
			this.okayCallback = okayCallback;
			this.cancelCallback = cancelCallback;
			active = true;
			alpha = 0;
			x = Game.WIDTH * 0.5;
			y = Game.HEIGHT * 0.5;
			
			// create background and text
			TextBox textBox = new TextBox(WIDTH, 12, 0x0, 0x0);
			textBox.align = "center";
			textBox.alignVert = "center";
			textBox.text = text;
			// resize TextBox to match text
			textBox.setSize((int)WIDTH, 2 + textBox.lines.length * textBox.lineSpacing);
			textBox.x = -(int)(textBox.width * 0.5);
			textBox.y = -(int)(textBox.height * 0.5);
			Bitmap background = new Bitmap(new BitmapData((int)WIDTH, (int)textBox.height + 48, true, 0xFF999999));
			background.bitmapData.fillRect(new Rectangle(1, 1, background.width - 2, background.height - 2), 0xFF111111);
			background.x = -(int)(background.width * 0.5);
			background.y = -(int)(background.height * 0.5);
			addChild(background);
			addChild(textBox);
			
			// create title
			TextBox titleBox = new TextBox(WIDTH - 20, 12, ROLL_OUT_COL);
			titleBox.align = "center";
			titleBox.text = titleStr;
			titleBox.y = background.y + 2;
			titleBox.x = -(int)(titleBox.width * 0.5);
			addChild(titleBox);
			
			// buttons:
			// there is always an okay button
			okayButton = new Sprite();
			okayTextBox = new TextBox(Menu.LIST_WIDTH, 12, ROLL_OUT_COL);
			okayTextBox.align = "center";
			okayButton.addChild(okayTextBox);
			addChild(okayButton);
///			okayButton.addEventListener(MouseEvent.CLICK, okay, false, 0, true);
///			okayButton.addEventListener(MouseEvent.ROLL_OVER, okayOver, false, 0, true);
///			okayButton.addEventListener(MouseEvent.ROLL_OUT, okayOut, false, 0, true);
			game.stage.addEventListener(KeyboardEvent.KEY_DOWN, (Action1<KeyboardEvent>)okay);
			
			//if(!Boolean(cancelCallback)){
			if( cancelCallback == null ){
				// create singular okay button
				okayTextBox.text = "press menu key";
				okayButton.y = background.y + background.height - (okayTextBox.height + 2);
				okayButton.x = -(int)(okayTextBox.width * 0.5);
				
			} else {
				// create two buttons
				cancelButton = new Sprite();
				cancelTextBox = new TextBox(Menu.LIST_WIDTH, 12, ROLL_OUT_COL);
				cancelTextBox.align = "center";
				cancelButton.addChild(cancelTextBox);
				addChild(cancelButton);
///				cancelButton.addEventListener(MouseEvent.CLICK, cancel, false, 0, true);
///				cancelButton.addEventListener(MouseEvent.ROLL_OVER, cancelOver, false, 0, true);
///				cancelButton.addEventListener(MouseEvent.ROLL_OUT, cancelOut, false, 0, true);
				game.stage.addEventListener(KeyboardEvent.KEY_DOWN, (Action1<KeyboardEvent>)cancel);
				
				okayTextBox.text = "right to accept";
				cancelTextBox.text = "left to cancel";
				
				okayButton.y = background.y + background.height - (okayTextBox.height + 2);
				okayButton.x = 1;
				cancelButton.y = background.y + background.height - (cancelTextBox.height + 2);
				cancelButton.x = -(cancelButton.width + 1);
			}
			
			// launch dialog
			addEventListener(Event.ENTER_FRAME, (Action1<Event>)onEnterFrame, false, 0, true);
			game.addChild(this);
			Key.lockOut = true;
			previousGameState = game.state;
			game.state = Game.DIALOG;
		}
		
		private void onEnterFrame(Event e){
			if(active){
				if(alpha < 1) alpha += 0.1;
			} else {
				if(alpha > 0){
					alpha -= 0.1;
				} else {
					removeEventListener(Event.ENTER_FRAME, (Action1<Event>)onEnterFrame);
					if(parent != null) parent.removeChild(this);
				}
			}
		}
		
		private void okay(Event e){
			if(!active) return;
			if(e is KeyboardEvent){
				// we've locked out keys so we have to go for the Key class' internals
				//if(!Boolean(cancelCallback)){
				if( cancelCallback != null ){
					if((e as KeyboardEvent).keyCode != Key.custom[Game.MENU_KEY]) return;
				} else {
					if(!((e as KeyboardEvent).keyCode == Key.custom[Game.RIGHT_KEY] || (e as KeyboardEvent).keyCode == Keyboard.RIGHT)) return;
				}
			}
			active = false;
			Key.lockOut = false;
			game.stage.removeEventListener(KeyboardEvent.KEY_DOWN, (Action1<KeyboardEvent>)okay);
			game.state = previousGameState;
			//if(Boolean(okayCallback)) okayCallback();
			if(okayCallback != null) okayCallback();
			Game.dialog = null;
		}
		
		private void cancel(Event e){
			if(!active) return;
			if(e is KeyboardEvent){
				// we've locked out keys so we have to go for the Key class' internals
				if(!((e as KeyboardEvent).keyCode == Key.custom[Game.LEFT_KEY] || (e as KeyboardEvent).keyCode == Keyboard.LEFT)) return;
			}
			active = false;
			Key.lockOut = false;
			game.stage.removeEventListener(KeyboardEvent.KEY_DOWN, (Action1<KeyboardEvent>)okay);
			game.stage.removeEventListener(KeyboardEvent.KEY_DOWN, (Action1<KeyboardEvent>)cancel);
			game.state = previousGameState;
			cancelCallback();
			Game.dialog = null;
		}
		
		private void okayOver(MouseEvent e){
			okayTextBox.backgroundCol = ROLL_OVER_COL;
			okayTextBox.draw();
		}
		
		private void okayOut(MouseEvent e){
			okayTextBox.backgroundCol = ROLL_OUT_COL;
			okayTextBox.draw();
		}
		
		private void cancelOver(MouseEvent e){
			cancelTextBox.backgroundCol = ROLL_OVER_COL;
			cancelTextBox.draw();
		}
		
		private void cancelOut(MouseEvent e){
			cancelTextBox.backgroundCol = ROLL_OUT_COL;
			cancelTextBox.draw();
		}
		
		/* Fills in for all the empty callbacks fed to the Dialog */
		public static void emptyCallback(){
			//
		}
	}

}