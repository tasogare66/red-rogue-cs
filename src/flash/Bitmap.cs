using System;

using flash.display;


namespace flash.display
{
	public class Bitmap : DisplayObject
	{
		public BitmapData bitmapData { get; set; }

		public double x {
			set { this.bitmapData.x = value; }
			get { return this.bitmapData.x; }
		}
		public double y {
			set { this.bitmapData.y = value; }
			get { return this.bitmapData.y; }
		}
		public double width {
			get { return this.bitmapData.width; }
		}
		public double height {
			get { return this.bitmapData.height; }
		}
		public double scaleX {
			set { this.bitmapData.scaleX = value; }
			get { return this.bitmapData.scaleX; }
		}
		public double scaleY {
			set { this.bitmapData.scaleY = value; }
			get { return this.bitmapData.scaleY; }
		}

		public Bitmap(BitmapData bitmapData = null, String pixelSnapping = "auto", Boolean smoothing = false)
		{
			this.bitmapData = bitmapData;
		}

		// 追加、ファイル名指定
		public Bitmap(String fileName) : this( new BitmapData(fileName) )
		{
		}

		~Bitmap()
		{
			this.bitmapData.dispose();
		}

		public override void RenderToOffScreen()
		{
			if( bitmapData != null ){
				bitmapData.RenderToOffScreen();
			}
		}

		public override void Render ()
		{
			base.Render();

			if( bitmapData != null ){
				bitmapData.Render();
			}
		}
	}
}
