using App;

namespace com.robotacid.util {
	/**
	 * XorShift random number generator
	 *
	 * Adapted from: http://www.calypso88.com/?p=524
	 *
	 * I've inlined the algorithm repeatedly because it actually runs faster than Math.random()
	 * so we might as well keep the speed boost on all calls to this object
	 *
	 * @author Aaron Steed, robotacid.com
	 */
	public class XorRandom {
		
		static readonly uint uint_MAX_VALUE = 4294967295;
		public static readonly double MAX_RATIO = 1d / (double)uint_MAX_VALUE;
		public uint r;
		public uint seed;
		
		public XorRandom(uint seed = 0) {
			
			//seed = 1195675104;
			
			if(seed != 0){
				r = seed;
			} else {
				r = seedFromDate();
			}
			this.seed = r;
		}
		
		/* Get a seed using a Date object */
		public static uint seedFromDate() {
			//uint r = (new Date().time % uint.MAX_VALUE) as uint;
			uint r = (uint)unchecked( System.DateTime.Now.Ticks.GetHashCode() );
			// once in a blue moon we can roll a zero from sourcing the seed from the Date
			//if(r == 0) r = Math.random() * MAX_RATIO;
			if(r == 0) r = (uint)(Rand.Math_random() * uint_MAX_VALUE);
			return r;
		}
		
		/* Returns a number from 0 - 1 */
		public double value() {
			r ^= r << 21;
			r ^= r >> 35;
			r ^= r << 4;
			return r * MAX_RATIO;
		}
		
		public double range(double n) {
			r ^= r << 21;
			r ^= r >> 35;
			r ^= r << 4;
			return r * MAX_RATIO * n;
		}
		
		public int rangeInt(double n) {
			r ^= r << 21;
			r ^= r >> 35;
			r ^= r << 4;
			return (int)(r * MAX_RATIO * n);
		}
		
		public bool coinFlip() {
			r ^= r << 21;
			r ^= r >> 35;
			r ^= r << 4;
			return r * MAX_RATIO < 0.5;
		}
	}

}