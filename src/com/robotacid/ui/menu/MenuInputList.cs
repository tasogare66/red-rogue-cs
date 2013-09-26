using System;
using System.Text.RegularExpressions;
using flash;

namespace com.robotacid.ui.menu {
	/**
	 * An option that allows a user to enter data
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class MenuInputList : MenuList{
		
		public MenuOption option;
		public Regex charsAllowed;
		public int charLimit;
		public Boolean newLineFinish;
		//public Action1<MenuList> inputCallback;
		public Action1<MenuInputList> inputCallback;
		public String promptName;
		public String input;
		public Boolean done;
		
//		private Boolean firstInput;

		// MEMO:引数Action1<MenuList>のcallbackは必要か?
		//public MenuInputList(String name, Regex charsAllowed, int charLimit, Action1<MenuList> inputCallback, Boolean newLineFinish = true) {
		public MenuInputList(String name, Regex charsAllowed, int charLimit, Action1<MenuInputList> inputCallback, Boolean newLineFinish = true) {
			option = new MenuOption(name);
			//super(Vector.<MenuOption>([option]));
			this.options.push( option );
			this.charsAllowed = charsAllowed;
			this.charLimit = charLimit;
			this.inputCallback = inputCallback;
			this.newLineFinish = newLineFinish;
			promptName = "enter value";
			input = "";
			option.recordable = false;
//			firstInput = false;
		}
		
		public void begin(){
			option.name = promptName;
			input = "";
			done = false;
		}
		
		public void addChar(String _char){
			//if(_char.search(charsAllowed) > -1){
			if( charsAllowed.IsMatch(_char) ){
				input += _char;
				option.name = input;
				if(input.Length >= charLimit){
					finish();
				}
			}
		}
		
		public void removeChar(){
			if(input.Length > 0){
				input = input.Substring(0, input.Length - 1);
				option.name = input;
			}
		}
		
		public void finish(){
			inputCallback(this);
			done = true;
		}
		
	}

}