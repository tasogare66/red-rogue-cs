using System;
using redroguecs;

using com.robotacid.level;
using com.robotacid.sound;
///import com.robotacid.ui.Dialog;
///import flash.system.Capabilities;

namespace com.robotacid.ui.menu {
	/**
	 * A menu for the title screen
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class TitleMenu : Menu {
		
		private GameMenu gameMenu;
		
		public MenuList newGameList;
		public MenuList optionsList;
		
		public MenuOption newGameOption;
		public MenuOption continueOption;
		public MenuOption optionsOption;
		
		public MenuOption actionRPGOption;
		public MenuOption dogmaticOption;
		
		public TitleMenu(GameMenu gameMenu) : base(Game.WIDTH, Game.HEIGHT) {
			//super(Game.WIDTH, Game.HEIGHT);
			this.gameMenu = gameMenu;
///			Boolean continuing = Boolean(UserData.gameState.player.xml);
			Boolean continuing = false;	//FIXME:	
			
			MenuList trunk = new MenuList();
			optionsList = new MenuList();
			newGameList = new MenuList();
			
			newGameOption = new MenuOption("new game", newGameList);
			newGameOption.help = "play a new game from the entrance to the dungeons of chaos.";
			continueOption = new MenuOption("continue game", null, continuing);
			continueOption.help = "resume a game you have left. the game will auto-save entering a new area. play resumes only from the entrance to a level.";
			optionsOption = new MenuOption("options", optionsList);
			optionsOption.help = "configure settings.";
			MenuOption versionOption = new MenuOption("v " + Game.versionToString(), null, false);
			versionOption.help = "current version.";
			
			actionRPGOption = new MenuOption("action rpg");
			actionRPGOption.help = "standard play mode. preferable to those who like action role playing games.";
			dogmaticOption = new MenuOption("dogmatic");
			dogmaticOption.help = "time will only move forward when you perform an action or hold down a key. preferable to those who like roguelikes.";
			
			trunk.options.push(newGameOption);
			trunk.options.push(continueOption);
			trunk.options.push(optionsOption);
			trunk.options.push(gameMenu.creditsOption);
			trunk.options.push(versionOption);
			
			// link direct to game menu objects - easier to keep consistent
			optionsList.options.push(gameMenu.soundOption);
			optionsList.options.push(gameMenu.fullScreenOption);
			optionsList.options.push(gameMenu.rngSeedOption);
			optionsList.options.push(gameMenu.resetOption);
			
			newGameList.options.push(actionRPGOption);
			newGameList.options.push(dogmaticOption);
			newGameList.selection = UserData.settings.dogmaticMode ? 1 : 0;
			
			setTrunk(trunk);
			
			// is there saved game data? start menu on continue if so
			if(continuing){
				select(1);
			}
			help.text = currentMenuList.options[selection].help;
		}
		
		override public void changeSelection(){
			
			if(currentMenuList.options.length == 0) return;
			
			MenuOption option = currentMenuList.options[selection];
			
			if(parent != null && !String.IsNullOrEmpty(option.help)){
				help.text = option.help;
			}
			
			if(option.name == "sfx"){
				gameMenu.onOffOption.state = SoundManager.sfx ? 0 : 1;
				renderMenu();
				
			} else if(option.name == "music"){
				gameMenu.onOffOption.state = SoundManager.music ? 0 : 1;
				renderMenu();
				
			} else if(option.name == "fullscreen"){
				//gameMenu.onOffOption.state = game.stage.displayState == "normal" ? 1 : 0;
				gameMenu.onOffOption.state = Game.fullscreenOn ? 0 : 1;
				renderMenu();
				
			} else if(nextMenuList != null && nextMenuList == gameMenu.sureList){
				// make sure that visiting the sure list always defaults to NO
				gameMenu.sureList.selection = GameMenu.NO;
				renderMenu();
				
			}
		}
		
		override public void executeSelection() {
			MenuOption option = currentMenuList.options[selection];
			if(currentMenuList == gameMenu.sureList && currentMenuList.selection == GameMenu.YES){
				// erasing the shared object
				if(previousMenuList.options[previousMenuList.selection] == gameMenu.resetOption){
					if(Game.dialog == null){
						Game.dialog = new Dialog(
							"reset",
							"are you sure you want to reset all of your settings? this cannot be undone.",
							//function():void{gameMenu.reset(true)},
							delegate(){ gameMenu.reset(true); },
							Dialog.emptyCallback
						);
					}
				}
			} else if(option == gameMenu.onOffOption){
				// turning off sfx
				if(previousMenuList.options[previousMenuList.selection].name == "sfx"){
					SoundManager.sfx = gameMenu.onOffOption.state == 1;
				
				// turning off music
				} else if(previousMenuList.options[previousMenuList.selection].name == "music"){
					if(SoundManager.music){
						SoundManager.turnOffMusic();
///						if(SoundManager.soundLoops["underworldMusic2"]) SoundManager.stopSound("underworldMusic2");
					} else {
						SoundManager.turnOnMusic();
						if(game.map != null && game.map.type == Map.AREA && game.map.level == Map.UNDERWORLD){
							SoundManager.fadeLoopSound("underworldMusic2");
						}
					}
					
				// toggle fullscreen
				} else if(previousMenuList.options[previousMenuList.selection].name == "fullscreen"){
					if(gameMenu.onOffOption.state == 1){
						Game.fullscreenOn = true;
#if false
						if(Capabilities.playerType == "StandAlone"){
							gameMenu.fullscreen();
						} else {
							if(Game.dialog == null){
								Game.dialog = new Dialog(
									"activate fullscreen",
									"flash's security restrictions require you to press the menu key to continue\n\nThese restrictions also limit keyboard input to cursor keys and space. Press Esc to exit fullscreen.",
									gameMenu.fullscreen
								);
							}
						}
#endif
					} else {
						Game.fullscreenOn = false;
						stage.displayState = "normal";
					
					}
				}
			} else if(option == gameMenu.steedOption){
				gameMenu.url = "http://robotacid.com";
				gameMenu.openURL();
				
			} else if(option == gameMenu.nateOption){
				gameMenu.url = "http://gallardosound.com";
				gameMenu.openURL();
			
			} else if(option == gameMenu.redRogueOption){
				gameMenu.url = "http://redrogue.net";
				gameMenu.openURL();
			
			} else if(option == continueOption){
				launchGame(false, game.dogmaticMode);
				
			} else if(option == actionRPGOption){
				if(UserData.gameState.player.xml != null){
					if(Game.dialog == null){
						Game.dialog = new Dialog(
							"new game",
							"you have a game in progress\nare you sure you want to start from level 1?",
							//function():void{launchGame(true, false);},
							delegate(){ launchGame(true, false); },
							Dialog.emptyCallback
						);
					}
				} else launchGame(true, false);
				
			} else if(option == dogmaticOption){
				if(UserData.gameState.player.xml != null){
					if(Game.dialog == null){
						Game.dialog = new Dialog(
							"new game",
							"you have a game in progress\nare you sure you want to start from level 1?",
							//function():void{launchGame(true, true);},
							delegate(){ launchGame(true, true); },
							Dialog.emptyCallback
						);
					}
				} else launchGame(true, true);
				
			} else if(option == gameMenu.copySeedOption){
				gameMenu.copyRngSeed();
				
			}
		}
		
		private void launchGame(Boolean newGame, Boolean dogmaticMode){
			game.state = Game.GAME;
			UserData.settings.dogmaticMode = game.dogmaticMode = dogmaticMode;
			game.reset(newGame);
			game.menuCarousel.deactivate();
		}
	}

}