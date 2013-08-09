using System.Collections.Generic;

using flash;

namespace com.robotacid.ui.menu {
	
	/**
	 * A wrapper for a list of MenuOptions that stores the current selected
	 * option. The selection value is vital for figuring out the route taken
	 * to the selected option.
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class MenuList {
		
		/* This variable can be used to store all the references that lead to this MenuList
		 * this is not a default owing to how cumbersome this could be to manage */
		public Vector<MenuOption> pointers;
		
		public Vector<MenuOption> options;
		public int selection;
		public bool accessible;
		
		public MenuList(Vector<MenuOption> options = null) {
			if( options != null ) this.options = options;
			else this.options = new Vector<MenuOption>();
			accessible = true;
			selection = 0;
		}
		
		public string optionsToString(string separator = "\n", Vector<string> hotKeyMapStrings = null) {
			string str = "";
			MenuOption option;
			for(int i = 0; i < options.length; i++){
				option = options[i];
				str += option.hidden ? "" : option.name;
				if( hotKeyMapStrings != null && i < hotKeyMapStrings.length && hotKeyMapStrings[i] != "" ){
					str += " " + hotKeyMapStrings[i];
				}
				if(i < options.length - 1) str += separator;
			}
			return str;
		}
		
		/* Change all options within to point to a given target */
		public void changeTargets(MenuList target) {
			for(int i = 0; i < options.length; i++){
				options[i].target = target;
			}
		}
		
	}
	
}