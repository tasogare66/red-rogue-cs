using flash.display;

namespace redroguecs
{
	public class InstructionsMC : MovieClip
	{
		public MovieClip collect = new MovieClip();
		public MovieClip combat = new MovieClip();
		public MovieClip exit = new MovieClip();

		public InstructionsMC () {
			combat.x = 10;
			combat.y = 10;
			combat.width = 82;
			collect.x = 10;
			collect.y = 75;
			collect.width = 82;
			exit.x = 10;
			exit.y = 140;
			exit.width = 82;
		}
	}
}

