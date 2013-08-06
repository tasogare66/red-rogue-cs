using System;

using flash.display;


namespace flash.display
{
	public class Bitmap : DisplayObject
	{
		public BitmapData bitmapData { get; set; }

		public double x {
			set { this.bitmapData.x = (float)value; }
			get { return this.bitmapData.x; }
		}
		public double y {
			set { this.bitmapData.y = (float)value; }
			get { return this.bitmapData.y; }
		}
		public double width {
			get { return this.bitmapData.width; }
		}
		public double height {
			get { return this.bitmapData.height; }
		}


		public Bitmap(BitmapData bitmapData = null, String pixelSnapping = "auto", Boolean smoothing = false)
		{
			this.bitmapData = bitmapData;
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
			if( this.Status == ActorStatus.Action && bitmapData != null ){
				bitmapData.Render();
			}

			base.Render ();
		}
	}
}
