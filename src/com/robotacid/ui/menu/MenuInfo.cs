using System;

namespace com.robotacid.ui.menu {
	/**
	 * Instead of a list of options an image or text is shown as the next list
	 * 
	 * A callback is given to the object to render the info
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class MenuInfo : MenuList {
		
		public Action renderCallback;
		public Boolean update;
		
		public const int TEXT_BOX_LINES = 15;
		
		public MenuInfo(Action renderCallback, Boolean update = false) {
			//super();
			this.renderCallback = renderCallback;
			this.update = update;
			//accessible = false;
		}
		
	}

}