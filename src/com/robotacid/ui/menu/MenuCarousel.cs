using System.Collections.Generic;
using flash.display;

namespace com.robotacid.ui.menu {
	
	/**
	 * Handles changeover between menus
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class MenuCarousel : Sprite {
		
		public List<Menu> menus;
		public Menu currentMenu;
		
		public bool active;
		
		public MenuCarousel() {
			menus = new List<Menu>();
			active = false;
		}
		
		public void addMenu(Menu menu) {
			menus.Add(menu);
			menu.carousel = this;
		}
		
		public void setCurrentMenu(Menu menu) {
			if(currentMenu == menu) return;
///			if(currentMenu && currentMenu.parent){
///				currentMenu.deactivate();
///				menu.activate();
///			}
			currentMenu = menu;
		}
		
		public void activate() {
			active = true;
///			currentMenu.activate();
		}
		
		public void deactivate() {
			active = false;
///			currentMenu.deactivate();
		}
	}

}