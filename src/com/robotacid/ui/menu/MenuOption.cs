namespace com.robotacid.ui.menu {
	/**
	 * A pointer to a MenuList, or a label in a MenuList that when stepped forward
	 * through will activate a Menu's SELECT event
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class MenuOption {
		
		public string name;
		public bool active;
		public MenuList target;
		public bool visited;
		public int selectionStep;	// controls where the menu ends up after selection
		public string help;
		public bool recordable;		// set to false to prevent a hot key recording of this option
		public bool hidden;
		
		// A reference to an object that this option affects
///		public var userData:*;
		
		// hot key maps need to find paths of similar context when the original path is removed
		public string context;
		public bool hotKeyOption;
		
		// selection steps
		public static readonly int EXIT_MENU = -1;
		public static readonly int TRUNK = 0;
		// any value above 0 is the number of steps to move back from selection
		// a value of 1 would leave the menu where it was
		
		public MenuOption(string name, MenuList target = null, bool active = true) {
			this.name = name;
			this.target = target;
			this.active = active;
			recordable = true;
			visited = true;
			hotKeyOption = false;
			hidden = false;
			selectionStep = TRUNK;
		}
		
	}

}