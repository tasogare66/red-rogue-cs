using flash.display;
using Matrix = flash.geom.Matrix;

namespace com.robotacid.gfx {
	
	/**
	 * Wrapper for a Bitmap created to capture a DisplayObject
	 *
	 * @author Aaron Steed, robotacid.com
	 */
	public class CaptureBitmap : Bitmap {
		
		public CaptureBitmap(BitmapData bitmapData = null)
			: base((bitmapData != null) ? bitmapData : new BitmapData(1, 1, true, 0x0)) {
			//super(bitmapData ? bitmapData : new BitmapData(1, 1, true, 0x0));
		}
		
		public void capture(DisplayObject target, Matrix matrix = null, int width = 0, int height = 0){
			if(width == 0 || height == 0){
				if(bitmapData.width != target.width || bitmapData.height != target.height){
					bitmapData = new BitmapData((int)target.width, (int)target.height, bitmapData.transparent, 0x0);
				}
			} else {
				bitmapData = new BitmapData(width, height, bitmapData.transparent, 0x0);
			}
			bitmapData.draw(target, matrix);
			x = target.x;
			y = target.y;
		}
		
	}

}