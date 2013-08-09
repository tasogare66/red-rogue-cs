using System;

namespace com.robotacid.ui.menu {
	/**
	 * An option that allows a user to enter data
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class MenuInputList : MenuList{
		
		public MenuOption option;
///		public var charsAllowed:RegExp;
		public int charLimit;
		public Boolean newLineFinish;
///		public var inputCallback:Function;
		public String promptName;
		public String input;
		public Boolean done;
		
		private Boolean firstInput;

#if false
		public MenuInputList(name:String, charsAllowed:RegExp, charLimit:int, inputCallback:Function, newLineFinish:Boolean = true) {
			option = new MenuOption(name);
			super(Vector.<MenuOption>([option]));
			this.charsAllowed = charsAllowed;
			this.charLimit = charLimit;
			this.inputCallback = inputCallback;
			this.newLineFinish = newLineFinish;
			promptName = "enter value";
			input = "";
			option.recordable = false;
			firstInput = false;
		}
#endif
		
		public void begin(){
			option.name = promptName;
			input = "";
			done = false;
		}
		
		public void addChar(String _char){
#if false
			if(_char.search(charsAllowed) > -1){
				input += _char;
				option.name = input;
				if(input.Length >= charLimit){
					finish();
				}
			}
#endif
		}
		
		public void removeChar(){
			if(input.Length > 0){
				input = input.Substring(0, input.Length - 1);
				option.name = input;
			}
		}
		
		public void finish(){
///			inputCallback(this);
			done = true;
		}
		
	}

}