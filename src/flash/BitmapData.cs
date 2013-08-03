using System;
using flash.geom;

namespace flash.display
{
    public class BitmapData
	{
		public int height { get; private set; }
        public Rectangle rect { get; private set; }
		public Boolean transparent { get; private set; }
		public int width { get; private set; }

		uint fillColor;

		public BitmapData(int width, int height, Boolean transparent = true, uint fillColor = 0xFFFFFFFF){
			this.width = width;
			this.height = height;
			this.transparent= transparent;
			this.fillColor = fillColor;
			this.rect = new Rectangle(0, 0, width, height);
		}

		public BitmapData clone() {
			//FIXME:	
			return new BitmapData(width, height, transparent, fillColor);
		}

        public void colorTransform(Rectangle rect, ColorTransform colorTransform) {
			//FIXME:	
        }

        public void fillRect(Rectangle rect, uint color) {
			//FIXME:	
        }

        public void copyPixels(BitmapData sourceBitmapData, Rectangle sourceRect, Point destPoint, BitmapData alphaBitmapData = null, Point alphaPoint = null, Boolean mergeAlpha = false) {
			//FIXME:	
        }
    }
}
