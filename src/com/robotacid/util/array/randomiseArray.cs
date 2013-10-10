using com.robotacid.util;
using flash;

namespace com.robotacid.util {
class array {
	/* Does just what it says */
	public static void randomiseArray<T>(Array<T> a, XorRandom random){
		//for(var x:*, j:int, i:int = a.length; i; j = random.rangeInt(i), x = a[--i], a[i] = a[j], a[j] = x){}
		T x;
		int j;
		for(int i = a.length; i != 0; ){
			j = random.rangeInt(i);
			x = a[--i];
			a[i] = a[j];
			a[j] = x;
		}
	}
}

}