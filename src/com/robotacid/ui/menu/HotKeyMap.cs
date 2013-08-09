﻿using System;

using com.robotacid.ui;
using flash;

namespace com.robotacid.ui.menu {
	/**
	 * This maps out a behaviour for a hot key
	 *
	 * A hot key runs down a specific path to activate a menu option
	 *
	 * However - as I decided to make the topology of the menu dynamic, it became apparent that the hot
	 * keys needed to seek alternative paths to similar goals when the current route became blocked
	 *
	 * Thus when a hot key encounters a blocked path, or the path no longer appears to operate how it used
	 * to, then it will seek out MenuOptions in the current list that bear the same "context"
	 *
	 * Consider setting up a key to consume health potions - you would want this key to remain in that
	 * "context" whether you have potions to spare or not
	 *
	 * @author Aaron Steed, robotacid.com
	 */
	public class HotKeyMap{
		
		public Menu menu;
		public Boolean active;
		public int key;
		public Vector<int> selectionBranch;
		public Vector<MenuOption> optionBranch;
		
		public int length;
		
		public HotKeyMap(int key, Menu menu) {
			this.menu = menu;
			this.key = key;
			active = false;
		}
		
		/* Clears all lists and prepares for a new recording
		 *
		 * If XML is fed in as a parameter, this object tries to construct a map based
		 * on the contents of the XML - it is assumed that XML came from the toXML() method
		 * of this object */
		public void init(XML xml = null){
			selectionBranch = new Vector<int>();
			optionBranch = new Vector<MenuOption>();
			length = 0;
#if false
			if(xml){
				for each(var branch:XML in xml.branch){
					selectionBranch.push(int(branch.@selection));
					// for each optionBranch step, a fake MenuOption is made with the right
					// name and context. And so hopefully the course correction will find the
					// right option and execute
					optionBranch.push(new MenuOption(branch.@name));
					optionBranch[optionBranch.length - 1].context = branch.@context;
					length++;
				}
			}
#endif
		}
		
		
		public void push(MenuOption option, int selection){
			optionBranch.push(option);
			selectionBranch.push(selection);
			length++;
		}
		
		public void pop(int steps = 1){
			while(steps-- > 0){
				optionBranch.pop();
				selectionBranch.pop();
				if(--length <= 0) break;
			}
		}
		
		public void execute(){
			// first we need to walk back up the menu to the trunk before we can set off
			// down the hot key route
			//trace("hot keyed");
			while(menu.branch.length > 1) menu.stepLeft();
			
			int j;
			
			for(int i = 0; i < length; i++){
				
				// now here of course is where we verify our course of action
				// we know where we want to go, but are we headed down the right path?
				// if not then this map will modify itself first to match name and then to match context
				
				// option index correction
				if(
					selectionBranch[i] > menu.currentMenuList.options.length - 1 ||
					menu.currentMenuList.options[menu.currentMenuList.selection] != optionBranch[i]
				){
					for(j = 0; j < menu.currentMenuList.options.length; j++){
						if(menu.currentMenuList.options[j] == optionBranch[i]){
							selectionBranch[i] = j;
							menu.select(j);
							break;
						}
					}
				}
				
				// option inactive - search for similar path
				if(!optionBranch[i].active){
					
					// get the actual name of this option
					String name = optionBranch[i].name;
					if(optionBranch[i] is MenuOptionStack) name = (optionBranch[i] as MenuOptionStack).singleName;
					
					for(j = 0; j < menu.currentMenuList.options.length; j++){
						// search for the same name
						if(
							menu.currentMenuList.options[j].name == name ||
							(
								menu.currentMenuList.options[j] is MenuOptionStack &&
								(menu.currentMenuList.options[j] as MenuOptionStack).singleName == name
							)
						){
							optionBranch[i] = menu.currentMenuList.options[j];
							selectionBranch[i] = j;
							menu.select(j);
							break;
						}
					}
					// name search failed - search for context match
					if(j == menu.currentMenuList.options.length && optionBranch[i].context != null){
						for(j = 0; j < menu.currentMenuList.options.length; j++){
							// search for the same name
							if(menu.currentMenuList.options[j].context == optionBranch[i].context){
								optionBranch[i] = menu.currentMenuList.options[j];
								selectionBranch[i] = j;
								menu.select(j);
								break;
							}
						}
					}
					// all searches blank, abort request
					if(j == menu.currentMenuList.options.length) return;
				}
				
				menu.select(selectionBranch[i]);
				menu.stepRight();
			}
		}
		
#if false
		public function toXML():XML{
			var xml:XML = <hotKey />;
			for(var i:int = 0; i < length; i++){
				var branchNode:XML = <branch />;
				branchNode.@selection = selectionBranch[i];
				branchNode.@name = (optionBranch[i] is ToggleMenuOption) ? (optionBranch[i] as ToggleMenuOption).names[0] : optionBranch[i].name;
				branchNode.@context = optionBranch[i].context;
				xml.appendChild(branchNode);
			}
			return xml;
		}
#endif
		
		public static Vector<String> getOptionsHotKeyed(MenuList list, Vector<HotKeyMap> hotKeyMaps){
			int i, j;
			HotKeyMap hotKeyMap;
			MenuOption hotKeySelectionOption;
			MenuOption option;
			Vector<String> strs = new Vector<String>();
			String str;
			for(i = 0; i < list.options.length; i++){
				str = "";
				option = list.options[i];
				for(j = 0; j < hotKeyMaps.length; j++){
					hotKeyMap = hotKeyMaps[j];
					// do an object, then index, then name comparison
					if(hotKeyMap != null){
						hotKeySelectionOption = hotKeyMap.optionBranch[hotKeyMap.optionBranch.length - 1];
						if(
							option == hotKeySelectionOption ||
							(
								hotKeyMap.selectionBranch[hotKeyMap.selectionBranch.length - 1] == i && (
									option.name == hotKeySelectionOption.name || (
										option is ToggleMenuOption &&
										(option as ToggleMenuOption).names[0] == hotKeySelectionOption.name
									)
								)
							)
						){
							str = "(" + Key.keyString((uint)Key.custom[Menu.HOT_KEY_OFFSET + j]) + ")";
							break;
						}
					}
				}
				strs.push(str);
			}
			return strs;
		}
	}

}