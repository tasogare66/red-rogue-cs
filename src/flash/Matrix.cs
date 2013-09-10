namespace flash.geom
{
	public class Matrix {
		public double a, b, c, d;
		public double tx, ty;

		public Matrix(double _a = 1, double _b = 0, double _c = 0, double _d = 1, double _tx = 0, double _ty = 0) {
			this.a = _a;
			this.b = _b;
			this.c = _c;
			this.d = _d;
			this.tx = _tx;
			this.ty = _ty;
		}
	}
}

// Local Variables:
// coding: utf-8
// End: