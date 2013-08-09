using System;
using flash;

namespace com.robotacid.ui.menu {
	
	/**
	 * A MenuOption that can be flipped between a number of states
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class ToggleMenuOption : MenuOption{
		
		private int _state;
		public Array<String> names;
		
		public ToggleMenuOption(Array<String> names, MenuList next = null, Boolean active = true)
		: base(names[0], next, active) {
			this.names = names;
			//super(names[0], next, active);
			_state = 0;
			
		}
		
		public int state {
			get {
				return _state;
			}

			set {
				_state = value;
				name = names[ value ];
			}
		}
		
	}

}