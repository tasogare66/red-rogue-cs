using System;
using redroguecs;

using flash.display;

namespace com.robotacid.ui {
	/**
	 * A simple fade to segue between scenes with optional text inbetween
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class Transition : Sprite{
		
		public Boolean active;
		public Action changeOverCallback;
		public Action completeCallback;
		public int dir;
		public Boolean forceComplete;
		
		private TextBox textBox;
		private int textCount;
		
		public const double FADE_STEP = 1.0 / 10;
		public const int TEXT_DELAY = 60;
		
		public Transition() {
			active = false;
			dir = 0;
			textBox = new TextBox(100, 28, 0xFF000000, 0xFFAA0000);
			textBox.x = Game.WIDTH * 0.5 - textBox.width * 0.5;
			textBox.y = Game.HEIGHT * 0.5 - textBox.height * 0.5;
			textBox.alignVert = "center";
			textBox.align = "center";
			textBox.visible = false;
			addChild(textBox);
		}
		
		public void main(){
			// fade in text and delay
			if(alpha == 1 && textBox.visible){
				if(textCount != 0){
					if(textBox.alpha < 1){
						textBox.alpha += FADE_STEP;
						if(textBox.alpha >= 1) textBox.alpha = 1;
					} else {
						textCount--;
					}
				} else {
					if(textBox.alpha > 0){
						textBox.alpha -= FADE_STEP;
						if(textBox.alpha <= 0){
							textBox.alpha = 0;
							textBox.visible = false;
						}
					}
				}
			// fade in, callback, fade out, callback
			} else {
				if(dir > 0){
					alpha += FADE_STEP;
					if(alpha >= 1){
						alpha = 1;
						dir = -1;
						changeOverCallback();
					}
				} else if(dir < 0){
					alpha -= FADE_STEP;
					if(alpha <= 0){
						dir = 0;
						alpha = 0;
						active = false;
						graphics.clear();
						changeOverCallback = null;
						completeCallback = null;
						if(completeCallback != null) completeCallback();
					}
				}
			}
		}
		
		/* Initiate a transition */
		public void init(Action changeOverCallback, Action completeCallback = null, String text = "", Boolean skipToBlack = false, Boolean forceComplete = false){
			this.changeOverCallback = changeOverCallback;
			this.completeCallback = completeCallback;
			this.forceComplete = forceComplete;
			active = true;
			graphics.beginFill(0);
			graphics.drawRect(0, 0, Game.WIDTH, Game.HEIGHT);
			graphics.endFill();
			if(skipToBlack){
				dir = -1;
				alpha = 1;
				changeOverCallback();
			} else {
				dir = 1;
				alpha = 0;
			}
			if(text != ""){
				textCount = TEXT_DELAY;
				textBox.text = text;
				textBox.visible = true;
				textBox.alpha = 0;
			}
		}
		
	}

}