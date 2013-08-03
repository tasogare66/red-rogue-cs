using System;
using System.Collections.Generic;

using flash.display;
///	import flash.display.BitmapData;
///	import flash.geom.ColorTransform;
///	import flash.geom.Point;
using flash.geom;
///	import flash.utils.getQualifiedClassName;
using flash;

namespace com.robotacid.ui {
	
	/**
	 * Custom bitmap font
	 *
	 * @author Aaron Steed, robotacid.com
	 */
	public class TextBox : Bitmap {
		
#if false
		[Embed(source = "../../../assets/font/a.png")] public static var A:Class;
		[Embed(source = "../../../assets/font/b.png")] public static var B:Class;
		[Embed(source = "../../../assets/font/c.png")] public static var C:Class;
		[Embed(source = "../../../assets/font/d.png")] public static var D:Class;
		[Embed(source = "../../../assets/font/e.png")] public static var E:Class;
		[Embed(source = "../../../assets/font/f.png")] public static var F:Class;
		[Embed(source = "../../../assets/font/g.png")] public static var G:Class;
		[Embed(source = "../../../assets/font/h.png")] public static var H:Class;
		[Embed(source = "../../../assets/font/i.png")] public static var I:Class;
		[Embed(source = "../../../assets/font/j.png")] public static var J:Class;
		[Embed(source = "../../../assets/font/k.png")] public static var K:Class;
		[Embed(source = "../../../assets/font/l.png")] public static var L:Class;
		[Embed(source = "../../../assets/font/m.png")] public static var M:Class;
		[Embed(source = "../../../assets/font/n.png")] public static var N:Class;
		[Embed(source = "../../../assets/font/o.png")] public static var O:Class;
		[Embed(source = "../../../assets/font/p.png")] public static var P:Class;
		[Embed(source = "../../../assets/font/q.png")] public static var Q:Class;
		[Embed(source = "../../../assets/font/r.png")] public static var R:Class;
		[Embed(source = "../../../assets/font/s.png")] public static var S:Class;
		[Embed(source = "../../../assets/font/t.png")] public static var T:Class;
		[Embed(source = "../../../assets/font/u.png")] public static var U:Class;
		[Embed(source = "../../../assets/font/v.png")] public static var V:Class;
		[Embed(source = "../../../assets/font/w.png")] public static var W:Class;
		[Embed(source = "../../../assets/font/x.png")] public static var X:Class;
		[Embed(source = "../../../assets/font/y.png")] public static var Y:Class;
		[Embed(source = "../../../assets/font/z.png")] public static var Z:Class;
		[Embed(source = "../../../assets/font/0.png")] public static var NUMBER_0:Class;
		[Embed(source = "../../../assets/font/1.png")] public static var NUMBER_1:Class;
		[Embed(source = "../../../assets/font/2.png")] public static var NUMBER_2:Class;
		[Embed(source = "../../../assets/font/3.png")] public static var NUMBER_3:Class;
		[Embed(source = "../../../assets/font/4.png")] public static var NUMBER_4:Class;
		[Embed(source = "../../../assets/font/5.png")] public static var NUMBER_5:Class;
		[Embed(source = "../../../assets/font/6.png")] public static var NUMBER_6:Class;
		[Embed(source = "../../../assets/font/7.png")] public static var NUMBER_7:Class;
		[Embed(source = "../../../assets/font/8.png")] public static var NUMBER_8:Class;
		[Embed(source = "../../../assets/font/9.png")] public static var NUMBER_9:Class;
		[Embed(source = "../../../assets/font/APOSTROPHE.png")] public static var APOSTROPHE:Class;
		[Embed(source = "../../../assets/font/BACKSLASH.png")] public static var BACKSLASH:Class;
		[Embed(source = "../../../assets/font/COLON.png")] public static var COLON:Class;
		[Embed(source = "../../../assets/font/COMMA.png")] public static var COMMA:Class;
		[Embed(source = "../../../assets/font/EQUALS.png")] public static var EQUALS:Class;
		[Embed(source = "../../../assets/font/EXCLAMATION.png")] public static var EXCLAMATION:Class;
		[Embed(source = "../../../assets/font/FOWARDSLASH.png")] public static var FORWARDSLASH:Class;
		[Embed(source = "../../../assets/font/HYPHEN.png")] public static var HYPHEN:Class;
		[Embed(source = "../../../assets/font/LEFT_BRACKET.png")] public static var LEFT_BRACKET:Class;
		[Embed(source = "../../../assets/font/PLUS.png")] public static var PLUS:Class;
		[Embed(source = "../../../assets/font/QUESTION.png")] public static var QUESTION:Class;
		[Embed(source = "../../../assets/font/RIGHT_BRACKET.png")] public static var RIGHT_BRACKET:Class;
		[Embed(source = "../../../assets/font/SEMICOLON.png")] public static var SEMICOLON:Class;
		[Embed(source = "../../../assets/font/STOP.png")] public static var STOP:Class;
		[Embed(source = "../../../assets/font/AT.png")] public static var AT:Class;
		[Embed(source = "../../../assets/font/UNDERSCORE.png")] public static var UNDERSCORE:Class;
		[Embed(source = "../../../assets/font/PERCENT.png")] public static var PERCENT:Class;
		[Embed(source = "../../../assets/font/ASTERISK.png")] public static var ASTERISK:Class;
		[Embed(source = "../../../assets/font/QUOTES.png")] public static var QUOTES:Class;
#endif
		
///		public static const CHARACTER_CLASSES:Array = [A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, NUMBER_0, NUMBER_1, NUMBER_2, NUMBER_3, NUMBER_4, NUMBER_5, NUMBER_6, NUMBER_7, NUMBER_8, NUMBER_9, APOSTROPHE, BACKSLASH, COLON, COMMA, EQUALS, EXCLAMATION, FORWARDSLASH, HYPHEN, LEFT_BRACKET, PLUS, QUESTION, RIGHT_BRACKET, SEMICOLON, STOP, AT, UNDERSCORE, PERCENT, ASTERISK, QUOTES];
		
		public static Dictionary<String, BitmapData> characters;
		
		public Array < Array<BitmapData> > lines;	// a 2D array of all the bitmapDatas used, in lines
		public Array<int> lineWidths;				// the width of each line of text (used for alignment)
		public Array< Array<String> > textLines;	// a 2D array of the characters used (used for fetching offset and kerning data)
		public int tracking;						// tracking: the spacing between letters
		public string align;						// align: whether the text is centered, left or right aligned
		public string alignVert;					// align_vert: vertical alignment of the text
		public int lineSpacing;						// line_spacing: distance between each line of copy
		public bool wordWrap;						// turns wordWrap on and off
		public bool marquee;						// sets up marquee scroll for lines that exceed the width (wordWrap must be false)
		public uint backgroundCol;
		public uint borderCol;
		public double backgroundAlpha;
		public int leading;
		
		protected uint _colorInt;					// the actual uint of the color being applied
		protected ColorTransform _color;			// a color transform object that is applied to the whole TextBox
		
		protected int whitespaceLength;				// the distance a whitespace takes up
		
		protected int _width;
		protected int _height;
		protected String _text;
		protected Rectangle borderRect;
		protected Rectangle boundsRect;
		protected Rectangle maskRect;
		protected Vector<TextBoxMarquee> marqueeLines;
		
		public const int BORDER_ALLOWANCE = 2;
		
		public TextBox(double _width, double _height, uint backgroundCol = 0xFF111111, uint borderCol = 0xFF999999)
		: base(new BitmapData((int)_width, (int)_height, true, 0x0), "auto", false) {
			this._width = (int)_width;
			this._height = (int)_height;
			this.backgroundCol = backgroundCol;
			this.borderCol = borderCol;
			align = "left";
			alignVert = "top";
			_colorInt = 0xFFFFFF;
			wordWrap = true;
			marquee = false;
			tracking = 2;
			leading = 1;
			whitespaceLength = 4;
			lineSpacing = 11;
			_text = "";
			
			lines = new Array < Array<BitmapData> >();
			
			borderRect = new Rectangle(1, 1, _width - 2, _height - 2);
			boundsRect = new Rectangle(2, 2, _width - 4, _height - 4);
			maskRect = new Rectangle(0, 0, 1, 1);
			//super(new BitmapData(_width, _height, true, 0x0), "auto", false);
			drawBorder();
		}
		
		/* This must be called before any TextBox is created so the bitmaps can be extracted from the
		 * imported assets */
		public static void init(){
			characters = new Dictionary<String, BitmapData>();
#if false
			var textBitmap:Bitmap;
			var className:String;
			var characterName:String;
			var col:ColorTransform = new ColorTransform(1, 1, 1, 1, -8, -8, -8);
			for(var i:int = 0; i < CHARACTER_CLASSES.length; i++){
				textBitmap = new CHARACTER_CLASSES[i]();
				className = getQualifiedClassName(CHARACTER_CLASSES[i]);
				if(className.indexOf("NUMBER_") > -1) characterName = className.substr(className.indexOf("NUMBER_") + 7);
				else characterName = className.substr(className.indexOf("TextBox_") + 8);
				// taking a little of the edge off the whiteness of the text
				textBitmap.bitmapData.colorTransform(textBitmap.bitmapData.rect, col);
				characters[characterName] = textBitmap.bitmapData;
			}
#endif
		}
		
		public string text {
			set {
				_text = value;
				updateText();
				draw();
			}

			get {
				return _text;
			}
		}
		
		// color
		public uint color {
			get {
				return _colorInt;
			}
			set {
				_colorInt = value;
				if(value == 0xFFFFFF) {
					_color = null;
				} else {
					_color = new ColorTransform(
						((value >> 16) % 256) / 255,
						((value >> 8) % 256) / 255,
						(value % 256) / 255
					);
				}
				if(_color != null) transform.colorTransform = _color;
			}
		}
		
		public void setSize(int width, int height){
			_width = width;
			_height = height;
			borderRect = new Rectangle(1, 1, _width - 2, _height - 2);
			boundsRect = new Rectangle(2, 2, _width - 4, _height - 4);
			bitmapData = new BitmapData(width, height, true, 0x0);
			updateText();
			draw();
		}
		
		/* Calculates an array of BitmapDatas needed to render the text */
		protected void updateText(){
			
			// we create an array called lines that holds references to all of the
			// bitmapDatas needed and structure it like the text
			
			// the lines property is public so it can be used to ticker text
			lines = new Array< Array<BitmapData> >();
			lineWidths = new Array<int>();
			textLines = new Array< Array<String> >();
			
			var currentLine = new Array<BitmapData>();
			var currentTextLine = new Array<String>();	//FIXME:char?	
			int wordBeginning = 0;
			int currentLineWidth = 0;
			int completeWordsWidth = 0;
			int wordWidth = 0;
			var newLine = new Array<BitmapData>();
			var newTextLine = new Array<String>();
			String c;
			
			if(_text == null) _text = "";
			
			String upperCaseText = _text.ToUpper();
			
			for(int i = 0; i < upperCaseText.Length; i++){
				
				//c = upperCaseText.charAt(i);
				c = upperCaseText[i].ToString();
				
				// next we swap the special characters for descriptive strings
				if(c == " ") c = "SPACE";
				else if(c == ".") c = "STOP";
				else if(c == "?") c = "QUESTION";
				else if(c == ",") c = "COMMA";
				else if(c == "!") c = "EXCLAMATION";
				else if(c == "\\") c = "BACKSLASH";
				else if(c == "/") c = "FORWARDSLASH";
				else if(c == "=") c = "EQUALS";
				else if(c == "+") c = "PLUS";
				else if(c == "(") c = "LEFT_BRACKET";
				else if(c == ")") c = "RIGHT_BRACKET";
				else if(c == "-") c = "HYPHEN";
				else if(c == "\"") c = "QUOTES";
				else if(c == ":") c = "COLON";
				else if(c == "£") c = "POUND";
				else if(c == "_") c = "UNDERSCORE";
				else if(c == "'") c = "APOSTROPHE";
				else if(c == "@") c = "AT";
				else if(c == "&") c = "AMPERSAND";
				else if(c == "$") c = "DOLLAR";
				else if(c == "*") c = "ASTERISK";
				else if(c == ";") c = "SEMICOLON";
				else if(c == "%") c = "PERCENT";
				else if(c == "~") c = "TILDE";
				else if(c == "{") c = "LEFT_BRACE";
				else if(c == "}") c = "RIGHT_BRACE";
				else if(c == "@") c = "AT";
				else if(c == "_") c = "UNDERSCORE";
				else if(c == "%") c = "PERCENT";
				else if(c == "*") c = "ASTERISK";
				else if(c == "\"") c = "QUOTES";
				
				// new line characters
				if(c == "\n" || c == "\r" || c == "|"){
					lines.push(currentLine);
					textLines.push(currentTextLine);
					lineWidths.push(currentLineWidth);
					currentLineWidth = 0;
					completeWordsWidth = 0;
					wordBeginning = 0;
					wordWidth = 0;
					currentLine = new Array<BitmapData>();
					currentTextLine = new Array<String>();
					continue;
				}
				
				// push a character into the array
				//if(characters[c] != null){
				if( characters.ContainsKey(c) ){
					// check we're in the middle of a word - spaces are null
					if(currentLine.length > 0 && currentLine[currentLine.length -1] != null){
						currentLineWidth += tracking;
						wordWidth += tracking;
					}
					wordWidth += characters[c].width;
					currentLineWidth += characters[c].width;
					currentLine.push(characters[c]);
					currentTextLine.push(c);
				
				// the character is a SPACE or unrecognised and will be treated as a SPACE
				} else {
					if(currentLine.length > 0 && currentLine[currentLine.length - 1] != null){
						completeWordsWidth = currentLineWidth;
					}
					currentLineWidth += whitespaceLength;
					currentLine.push(null);
					currentTextLine.push(null);
					wordBeginning = currentLine.length;
					wordWidth = 0;
				}
				
				// if the length of the current line exceeds the width, we splice it into the next line
				// effecting word wrap
				
				if(currentLineWidth > _width - (BORDER_ALLOWANCE * 2) && wordWrap){
					// in the case where the word is larger than the text field we take back the last character
					// and jump to a new line with it
					if(wordBeginning == 0 && currentLine[currentLine.length - 1] != null){
						currentLineWidth -= tracking + currentLine[currentLine.length - 1].width;
						// now we take back the offending last character
						BitmapData lastBitmapData = currentLine.pop();
						String lastChar = currentTextLine.pop();
						
						lines.push(currentLine);
						textLines.push(currentTextLine);
						lineWidths.push(currentLineWidth);
						
						currentLineWidth = lastBitmapData.width;
						completeWordsWidth = 0;
						wordBeginning = 0;
						wordWidth = lastBitmapData.width;
						//currentLine = [lastBitmapData];
						currentLine = new Array<BitmapData>();
						currentLine.push(lastBitmapData);
						//currentTextLine = [lastChar];
						currentTextLine = new Array<String>();
						currentTextLine.push(lastChar);
						continue;
					}
					
					newLine = currentLine.splice(wordBeginning, (uint)(currentLine.length - wordBeginning));
					newTextLine = currentTextLine.splice(wordBeginning, (uint)(currentTextLine.length - wordBeginning));
					lines.push(currentLine);
					textLines.push(currentTextLine);
					lineWidths.push(completeWordsWidth);
					completeWordsWidth = 0;
					wordBeginning = 0;
					currentLine = newLine;
					currentTextLine = newTextLine;
					currentLineWidth = wordWidth;
				}
			}
			// save the last line
			lines.push(currentLine);
			textLines.push(currentTextLine);
			lineWidths.push(currentLineWidth);
			
			// set up marquees (if active)
			if(!wordWrap && marquee){
				marqueeLines = new Vector<TextBoxMarquee>();
				int offset;
				for(int i = 0; i < lineWidths.length; i++){
					offset = (_width - BORDER_ALLOWANCE * 2) - lineWidths[i];
					marqueeLines[i] = offset < 0 ? new TextBoxMarquee(offset) : null;
				}
			}
		}
		
		/* Render */
		public void draw(){
			
			drawBorder();
			
			int i, j;
			Point point = new Point();
			int x;
			int y = BORDER_ALLOWANCE;
			int alignX = 0;
			int alignY = 0;
			BitmapData _char;
			//Point offset;
			int wordBeginning = 0;
			int linesHeight = lineSpacing * lines.length;
			
			for(i = 0; i < lines.length; i++, point.y += lineSpacing){
				x = BORDER_ALLOWANCE;
				
				if(marquee){
					if(marqueeLines[i] != null) x += marqueeLines[i].offset;
				}
				
				wordBeginning = 0;
				for(j = 0; j < lines[i].length; j++){
					_char = lines[i][j];
					
					// alignment to bitmap
					if(align == "left"){
						alignX = 0;
					} else if(align == "center"){
						alignX = (int)(_width * 0.5 - (lineWidths[i] * 0.5 + BORDER_ALLOWANCE));
					} else if(align == "right"){
						alignX = (int)(_width - lineWidths[i]);
					}
					if(alignVert == "top"){
						alignY = 0;
					} else if(alignVert == "center"){
						alignY = (int)(_height * 0.5 - linesHeight * 0.5);
					} else if(alignVert == "bottom"){
						alignY = _height - linesHeight;
					}
					
					// print to bitmapdata
					if(_char != null){
						if(j > wordBeginning){
							x += tracking;
						}
						point.x = alignX + x;
						point.y = alignY + y + leading;
						// mask characters that are outside the boundsRect
						if(
							point.x < boundsRect.x ||
							point.y < boundsRect.y ||
							point.x + _char.rect.width >= boundsRect.x + boundsRect.width ||
							point.y + _char.rect.height >= boundsRect.y + boundsRect.height
						){
							// are they even in the bounds rect?
							if(
								point.x + _char.rect.width > boundsRect.x &&
								boundsRect.x + boundsRect.width > point.x &&
								point.y + _char.rect.height > boundsRect.y &&
								boundsRect.y + boundsRect.height > point.y
							){
								// going to make a glib assumption that the TextBox won't be smaller than a single character
								maskRect.x = point.x >= boundsRect.x ? 0 : point.x - boundsRect.x;
								maskRect.y = point.y >= boundsRect.y ? 0 : point.y - boundsRect.y;
								maskRect.width = point.x + _char.rect.width <= boundsRect.x + boundsRect.width ? _char.rect.width : (boundsRect.x + boundsRect.width) - point.x;
								maskRect.height = point.y + _char.rect.height <= boundsRect.y + boundsRect.height ? _char.rect.height : (boundsRect.y + boundsRect.height) - point.y;
								if(point.x < boundsRect.x){
									maskRect.x = boundsRect.x - point.x;
									point.x = boundsRect.x;
								}
								if(point.y < boundsRect.y){
									maskRect.y = boundsRect.y - point.y;
									point.y = boundsRect.y;
								}
								bitmapData.copyPixels(_char, maskRect, point, null, null, true);
							}
						} else {
							bitmapData.copyPixels(_char, _char.rect, point, null, null, true);
						}
						x += _char.width;
					} else {
						x += whitespaceLength;
						wordBeginning = j + 1;
					}
				}
				y += lineSpacing;
			}
			
			if(_color != null) transform.colorTransform = _color;
		}
		
		/* Get a list of rectangles describing character positions for performing transforms */
		public Vector<Rectangle> getCharRects(){
			
			Vector<Rectangle> rects = new Vector<Rectangle>();
			Rectangle rect = new Rectangle();
			int i, j;
			int x;
			int y = BORDER_ALLOWANCE;
			int alignX = 0;
			int alignY = 0;
			BitmapData _char;
			//Point offset;
			int wordBeginning = 0;
			int linesHeight = lineSpacing * lines.length;
			
			for(i = 0; i < lines.length; i++, rect.y += lineSpacing){
				x = BORDER_ALLOWANCE;
				
				if(marquee){
					if(marqueeLines[i] != null) x += marqueeLines[i].offset;
				}
				
				wordBeginning = 0;
				for(j = 0; j < lines[i].length; j++){
					_char = lines[i][j];
					
					// alignment to bitmap
					if(align == "left"){
						alignX = 0;
					} else if(align == "center"){
						alignX = (int)(_width * 0.5 - (lineWidths[i] * 0.5 + BORDER_ALLOWANCE));
					} else if(align == "right"){
						alignX = (int)(_width - lineWidths[i]);
					}
					if(alignVert == "top"){
						alignY = 0;
					} else if(alignVert == "center"){
						alignY = (int)(_height * 0.5 - linesHeight * 0.5);
					} else if(alignVert == "bottom"){
						alignY = _height - linesHeight;
					}
					
					// print to bitmapdata
					if(_char != null){
						if(j > wordBeginning){
							x += tracking;
						}
						rect.x = alignX + x;
						rect.y = alignY + y + leading;
						// mask characters that are outside the boundsRect
						if(
							rect.x < boundsRect.x ||
							rect.y < boundsRect.y ||
							rect.x + _char.rect.width >= boundsRect.x + boundsRect.width ||
							rect.y + _char.rect.height >= boundsRect.y + boundsRect.height
						){
							// are they even in the bounds rect?
							if(
								rect.x + _char.rect.width > boundsRect.x &&
								boundsRect.x + boundsRect.width > rect.x &&
								rect.y + _char.rect.height > boundsRect.y &&
								boundsRect.y + boundsRect.height > rect.y
							){
								// going to make a glib assumption that the TextBox won't be smaller than a single character
								maskRect.x = rect.x >= boundsRect.x ? 0 : rect.x - boundsRect.x;
								maskRect.y = rect.y >= boundsRect.y ? 0 : rect.y - boundsRect.y;
								maskRect.width = rect.x + _char.rect.width <= boundsRect.x + boundsRect.width ? _char.rect.width : (boundsRect.x + boundsRect.width) - rect.x;
								maskRect.height = rect.y + _char.rect.height <= boundsRect.y + boundsRect.height ? _char.rect.height : (boundsRect.y + boundsRect.height) - rect.y;
								if(rect.x < boundsRect.x){
									maskRect.x = boundsRect.x - rect.x;
									rect.x = boundsRect.x;
								}
								if(rect.y < boundsRect.y){
									maskRect.y = boundsRect.y - rect.y;
									rect.y = boundsRect.y;
								}
								rects.push(maskRect.clone());
							}
						} else {
							rect.width = _char.width;
							rect.height = _char.height;
							rects.push(rect.clone());
						}
						x += _char.width;
					} else {
						x += whitespaceLength;
						wordBeginning = j + 1;
					}
				}
				y += lineSpacing;
			}
			
			return rects;
		}
		
		/* Move rectangles of pixels and applies colorTransforms, the transforms are applied sequentially */
		public void applyTranformRects(Vector<Rectangle> sources, Vector<Point> destinations, Vector<ColorTransform> colorTransforms = null){
			Rectangle source;
			Point destination;
			ColorTransform colorTransform;
			BitmapData buffer = bitmapData.clone();
			drawBorder();
			for(int i = 0; i < sources.length; i++){
				source = sources[i];
				destination = destinations[i];
				if(colorTransforms != null){
					colorTransform = colorTransforms[i];
					buffer.colorTransform(source, colorTransform);
				}
				bitmapData.copyPixels(buffer, source, destination, null, null, true);
			}
		}
		
		/* Update lines that have been assigned TextBoxMarquees */
		public void updateMarquee(){
			TextBoxMarquee marquee;
			for(int i = 0; i < marqueeLines.length; i++){
				marquee = marqueeLines[i];
				if(marquee != null) marquee.main();
			}
			draw();
		}
		
		/* Reset the offsets on the TextBoxMarquees */
		public void resetMarquee(){
			TextBoxMarquee marquee;
			for(int i = 0; i < marqueeLines.length; i++){
				marquee = marqueeLines[i];
				if(marquee != null) marquee.offset = 0;
			}
			draw();
		}
		
		/* Applies a ColorTransform to a line of text */
		public void setLineCol(int n, ColorTransform col){
			Rectangle disableRect = new Rectangle(BORDER_ALLOWANCE, BORDER_ALLOWANCE + n * lineSpacing, _width - BORDER_ALLOWANCE * 2, lineSpacing - 1);
			bitmapData.colorTransform(disableRect, col);
		}
		
		public void drawBorder(){
			bitmapData.fillRect(bitmapData.rect, borderCol);
			bitmapData.fillRect(borderRect, backgroundCol);
		}
		
	}

}

// Local Variables:
// coding: utf-8
// End: