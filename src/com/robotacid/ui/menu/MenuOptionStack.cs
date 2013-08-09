using System;

namespace com.robotacid.ui.menu {
	
	/**
	 * A way of making a MenuOption appear to be multiple MenuOptions in the same option
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class MenuOptionStack : MenuOption{
		
		private int _total;
		
		public String singleName;
		
		public MenuOptionStack(String name, MenuList next = null, Boolean active = true) : base(name, next, active) {
			//super(name, next, active);
			singleName = name;
			_total = 1;
		}
		
		public int total {
			get {
				return _total;
			}

			set {
				_total = value;
				name = (_total > 1 ? _total + " x " : "") + singleName;
			}
		}
		
		/* Updates the stacked name */
		public void updateName(){
			total = _total;
		}
	}

}