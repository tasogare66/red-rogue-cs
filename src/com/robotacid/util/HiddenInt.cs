using App;

namespace com.robotacid.util {
	
	/**
	* A wrapper for ints to protect them from tools like CheatEngine
	*
	* @author Aaron Steed, robotacid.com
	*/
	public class HiddenInt {
		
		private int _value;
		private int r;
		
		public HiddenInt(int start_value = 0){
			r = (int)(Rand.Math_random()*2000000)-1000000;
			_value = start_value ^ r;
		}
		// Getter setters for value
		public int value {
			set {
				r = (int)(Rand.Math_random()*2000000)-1000000;
				_value = value ^ r;
			}
			get {
				return _value ^ r;
			}
		}
	}
	
}