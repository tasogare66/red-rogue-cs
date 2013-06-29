using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

namespace redroguecs
{
	public class AppMain
	{

		public static void Main (string[] args)
		{
			using( GameFrameworkRedRogueCs game = new GameFrameworkRedRogueCs() )
			{
				game.Run(args);
			}
		}

	}
}
