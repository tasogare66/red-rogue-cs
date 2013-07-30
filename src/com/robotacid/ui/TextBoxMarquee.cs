namespace com.robotacid.ui {
	/**
	 * Rocks text back and forth in a limited space
	 * 
	 * @author Aaron Steed, robotacid.com
	 */
	public class TextBoxMarquee {
		
		public int offset;
		private int minOffset;
		private int count;
		private int dir;
		
		public const int SPEED = 1;
		public const int ROCK_DELAY = 30;
		
		public TextBoxMarquee(int minOffset) {
			this.minOffset = minOffset;
			this.offset = 0;
			count = ROCK_DELAY;
			dir = -1;
		}
		
		public void main(){
			if(count != 0){
				count--;
			} else {
				if(dir < 0){
					offset -= SPEED;
					if(offset <= minOffset){
						offset = minOffset;
						count = ROCK_DELAY;
						dir = 1;
					}
				} else if(dir > 0){
					offset += SPEED * 2;
					if(offset >= 0){
						offset = 0;
						count = ROCK_DELAY;
						dir = -1;
					}
				}
			}
		}
		
	}

}