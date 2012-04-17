using System;
using UnityEngine;
using NPack;

namespace URandom
{
	public static class RandomSquare
	{
		public static Vector2 Area( ref NPack.MersenneTwister _rand )
		{
			// Make a grid 0-1
			return new Vector2(_rand.NextSingle(true), _rand.NextSingle(true));
		}
		
		public static Vector2 Area( ref NPack.MersenneTwister _rand, UnityRandom.Normalization n, float t )
		{
			float x,y;
			x = y = 0;
			switch (n) {
			case UnityRandom.Normalization.STDNORMAL:
				x = (float) NormalDistribution.Normalize(_rand.NextSingle(true), t);
				y = (float) NormalDistribution.Normalize(_rand.NextSingle(true), t);
			break;
			case UnityRandom.Normalization.POWERLAW:
				x = (float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1);
				y = (float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1);
			break;
			default:
				x = _rand.NextSingle(true);
				y = _rand.NextSingle(true);
			break;
			}
			
			return new Vector2(x,y);			
		}
	}
}