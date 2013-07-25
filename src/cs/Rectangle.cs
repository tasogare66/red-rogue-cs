using Sce.PlayStation.Core;

namespace App
{
	public class Rectangle {
		public Sce.PlayStation.Core.Rectangle rectangle;

		public Rectangle() {
			rectangle = new Sce.PlayStation.Core.Rectangle();
		}

		public Rectangle( double ix, double iy, double iw, double ih ){
			rectangle = new Sce.PlayStation.Core.Rectangle( (float)ix, (float)iy, (float)iw, (float)ih );
		}

		public double x {
			set { this.rectangle.X = (float)value; }
			get { return this.rectangle.X; }
		}
		public double y {
			set { this.rectangle.Y = (float)value; }
			get { return this.rectangle.Y; }
		}

		public double width {
			set { this.rectangle.Width = (float)value; }
			get { return this.rectangle.Width; }
		}
		public double height {
			set { this.rectangle.Height = (float)value; }
			get { return this.rectangle.Height; }
		}

		public virtual string toString() {
			//(x=1, y=2, w=3, h=4)
			return "(x=" + this.rectangle.X + ", y=" + this.rectangle.Y + ", w=" + this.rectangle.Width + ", h=" + this.rectangle.Height + ")";
		}
	}
}
