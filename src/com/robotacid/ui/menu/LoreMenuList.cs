using System;
using redroguecs;

using com.robotacid.level;
///import com.robotacid.engine.Character;
using com.robotacid.engine;
///import com.robotacid.engine.Item;
///import com.robotacid.ui.TextBox;
using flash.display;
using Point = flash.geom.Point;
///import flash.geom.Rectangle;

namespace com.robotacid.ui.menu {
	/**
	 * Manages the Lore section of the menu, updating through use of the identify Effect as well as
	 * housing the map renderer
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class LoreMenuList : MenuList {
		
		public Game game;
		public Menu menu;
		public TextBox infoTextBox;
		
		public MenuList racesList;
		public MenuList itemsList;
		public MenuList weaponsList;
		public MenuList armourList;
///		public QuestMenuList questsList;
		
		public MenuInfo mapInfo;
		public MenuInfo logInfo;
		public MenuInfo raceInfo;
		public MenuInfo weaponInfo;
		public MenuInfo armourInfo;
		
		public MenuOption racesOption;
		public MenuOption itemsOption;
		public MenuOption armourOption;
		public MenuOption weaponsOption;
		
		public MenuOption questsOption;
		
		public LoreMenuList(TextBox infoTextBox, Menu menu, Game game) {
			//super();
			this.infoTextBox = infoTextBox;
			this.menu = menu;
			this.game = game;
			
			racesList = new MenuList();
			itemsList = new MenuList();
			weaponsList = new MenuList();
			armourList = new MenuList();
///			questsList = new QuestMenuList(menu);
///			questsList.loadFromArray(UserData.gameState.quests);
			
			mapInfo = new MenuInfo(renderMap, true);
			logInfo = new MenuInfo(renderLog);
			raceInfo = new MenuInfo(renderRace);
			weaponInfo = new MenuInfo(renderWeapon);
			armourInfo = new MenuInfo(renderArmour);
			
			MenuOption option;
			int i;
			Boolean unlocked;
#if false
			for(i = 0; i < Character.stats["names"].length; i++){
				unlocked = Boolean(UserData.settings.loreUnlocked.races[i]) || i == 0;
				option = new MenuOption(Character.stats["names"][i], raceInfo, unlocked);
				option.recordable = false;
				option.hidden = !unlocked;
				racesList.options.push(option);
			}
			for(i = 0; i < Item.stats["weapon names"].length; i++){
				unlocked = Boolean(UserData.settings.loreUnlocked.weapons[i]);
				option = new MenuOption(Item.stats["weapon names"][i], weaponInfo, unlocked);
				option.recordable = false;
				option.hidden = !unlocked;
				weaponsList.options.push(option);
			}
			for(i = 0; i < Item.stats["armour names"].length; i++){
				unlocked = Boolean(UserData.settings.loreUnlocked.armour[i]);
				option = new MenuOption(Item.stats["armour names"][i], armourInfo, unlocked);
				option.recordable = false;
				option.hidden = !unlocked;
				armourList.options.push(option);
			}
#endif
			
			MenuOption mapOption = new MenuOption("map", mapInfo);
			mapOption.recordable = false;
			MenuOption logOption = new MenuOption("log", logInfo);
			logOption.recordable = false;
			racesOption = new MenuOption("races", racesList);
			itemsOption = new MenuOption("items", itemsList);
			weaponsOption = new MenuOption("weapons", weaponsList);
			armourOption = new MenuOption("armour", armourList);
///			questsOption = new MenuOption("quests", questsList);
			
			options.push(mapOption);
			options.push(logOption);
			options.push(racesOption);
			options.push(itemsOption);
			options.push(questsOption);
			
			itemsList.options.push(weaponsOption);
			itemsList.options.push(armourOption);
			
		}
		
		/* Checks for lore corresponding to the enitity submitted and unlocks the entry if currently locked */
		public void unlockLore(Entity entity){
#if false
			MenuOption option;
			Boolean newLore = false;
			if(entity is Character){
				option = racesList.options[entity.name];
				if(!option.active){
					newLore = true;
					racesOption.visited = false;
				}
			} else if(entity is Item){
				Item item = entity as Item;
				if(item.type == Item.WEAPON){
					option = weaponsList.options[item.name];
					if(!option.active){
						newLore = true;
						weaponsOption.visited = false;
						itemsOption.visited = false;
					}
				} else if(item.type == Item.ARMOUR){
					option = armourList.options[item.name];
					if(!option.active){
						newLore = true;
						armourOption.visited = false;
						itemsOption.visited = false;
					}
				}
			}
			if(newLore){
				option.active = true;
				option.visited = false;
				option.hidden = false;
				menu.update();
				game.console.print("new lore unlocked");
			}
#endif
		}
		
		/* Locks all of the Lore (called on a hard reset) */
		public void reset() {
			// note that the rogue's lore stays unlocked
			MenuOption option;
			int i;
//			Boolean unlocked;
			for(i = 1; i < racesList.options.length; i++){
				option = racesList.options[i];
				option.hidden = true;
				option.active = false;
			}
			for(i = 0; i < weaponsList.options.length; i++){
				option = weaponsList.options[i];
				option.hidden = true;
				option.active = false;
			}
			for(i = 0; i < armourList.options.length; i++){
				option = armourList.options[i];
				option.hidden = true;
				option.active = false;
			}
		}
		
		/* Callback for mapInfo rendering */
		private void renderMap(){
			BitmapData textBuffer;
			uint col = infoTextBox.backgroundCol;
			String nameStr = Map.getName(game.map.level, game.map.type);
			if(game.map.type == Map.MAIN_DUNGEON) nameStr += " : " + game.map.level;
			if(game.map.completionTotal != 0){
				if(game.map.completionCount == 0) nameStr += "\n100%";
				else {
					int total = 100 - (((100 / game.map.completionTotal) * game.map.completionCount) >> 0);
					nameStr += "\n" + total + "%";
				}
			}
			
			infoTextBox.backgroundCol = 0x0;
			infoTextBox.align = "center";
			infoTextBox.text = nameStr;
			textBuffer = infoTextBox.bitmapData.clone();
			
			infoTextBox.backgroundCol = 0x99666666;
			infoTextBox.text = "";
///			game.miniMap.renderTo(infoTextBox.bitmapData);
			infoTextBox.bitmapData.copyPixels(textBuffer, textBuffer.rect, new Point(), null, null, true);
			
			infoTextBox.backgroundCol = col;
			infoTextBox.align = "left";
		}
		
		/* Show an extended console report */
		private void renderLog(){
			infoTextBox.wordWrap = false;
			infoTextBox.marquee = true;
			infoTextBox.text = game.console.getLog(MenuInfo.TEXT_BOX_LINES);
		}
		
		/* Callback for raceInfo rendering */
		private void renderRace(){
			int n = racesList.selection;
			String str = "";
#if false
			str += Character.stats["names"][n] + "\n\n";
			str += Character.stats["descriptions"][n] + "\n\n";
			str += "special: " + Character.stats["specials"][n] + "\n";
			str += "attack: " + Character.stats["attacks"][n] + " + " + Character.stats["attack levels"][n] + " x lvl\n";
			str += "defence: " + Character.stats["defences"][n] + " + " + Character.stats["defence levels"][n] + " x lvl\n";
			str += "health: " + Character.stats["healths"][n] + " + " + Character.stats["health levels"][n] + " x lvl\n";
			str += "damage: " + Character.stats["damages"][n] + " + " + Character.stats["damage levels"][n] + " x lvl\n";
			str += "attack speed: " + Character.stats["attack speeds"][n] + " + " + Character.stats["attack speed levels"][n] + " x lvl\n";
			str += "move speed: " + Character.stats["speeds"][n] + " + " + Character.stats["speed levels"][n] + " x lvl\n";
			str += "knockback: " + Character.stats["knockbacks"][n] + "\n";
			str += "stun: " + Character.stats["stuns"][n] + "\n";
			str += "endurance: " + Character.stats["endurances"][n] + "\n";
			str += "bravery: " + Character.stats["braveries"][n];
#endif
			infoTextBox.wordWrap = false;
			infoTextBox.marquee = true;
			infoTextBox.text = str;
		}
		
		/* Callback for weaponInfo rendering */
		private void renderWeapon(){
			int n = weaponsList.selection;
			String str = "";
#if false
			str += Item.stats["weapon names"][n] + "\n\n";
			str += Item.stats["weapon descriptions"][n] + "\n\n";
			str += "special: " + Item.stats["weapon specials"][n] + "\n";
			str += "range: ";
			int range = Item.stats["weapon ranges"][n];
			Array rangeStr = [];
			if(range & Item.MELEE) rangeStr.push("melee");
			if(range & Item.MISSILE) rangeStr.push("missile");
			if(range & Item.THROWN) rangeStr.push("thrown");
			str += rangeStr.join(",") + "\n";
			str += "damage: " + Item.stats["weapon damages"][n] + " + " + Item.stats["weapon damage levels"][n] + " x lvl\n";
			str += "attack: " + Item.stats["weapon attacks"][n] + " + " + Item.stats["weapon attack levels"][n] + " x lvl\n";
			str += "knockback: " + Item.stats["weapon knockbacks"][n] + "\n";
			str += "stun: " + Item.stats["weapon stuns"][n] + "\n";
			str += "hearts: +" + ((Item.stats["weapon butchers"][n] * 100) >> 0) + "%";
#endif
			infoTextBox.wordWrap = false;
			infoTextBox.marquee = true;
			infoTextBox.text = str;
		}
		
		/* Callback for armourInfo rendering */
		private void renderArmour(){
			int n = armourList.selection;
			String str = "";
#if false
			str += Item.stats["armour names"][n] + "\n\n";
			str += Item.stats["armour descriptions"][n] + "\n\n";
			str += "special: " + Item.stats["armour specials"][n] + "\n";
			str += "defence: " + Item.stats["armour defences"][n] + " + " + Item.stats["armour defence levels"][n] + " x lvl\n";
			str += "endurance: " + Item.stats["armour endurances"][n];
#endif
			infoTextBox.wordWrap = false;
			infoTextBox.marquee = true;
			infoTextBox.text = str;
		}
		
	}

}