using System;
using System.Diagnostics;

namespace App
{
	public class Rand
	{
		public static Rand Instance = new Rand();

		public Random Random { get; private set; }

		public Rand()
		{
			Random = new Random();
		}

		public static double Math_random() {
			return Instance.Random.NextDouble();
		}
	}
}
