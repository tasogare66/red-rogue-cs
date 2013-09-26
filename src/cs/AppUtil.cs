using System;
using System.Diagnostics;

namespace App
{
	public static class Util
	{
		[Conditional ("DEBUG")]
		public static void Assert(bool condition)
		{
			if( !condition )
			{
				Debugger.Break();
			}
		}

		[Conditional ("DEBUG")]
		public static void trace(String value)
		{
			Console.WriteLine( value );
		}
	}
}
