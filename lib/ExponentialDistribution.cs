using System;

namespace URandom
{
	public static class ExponentialDistribution
	{
		// FROM http://stackoverflow.com/questions/2106503/pseudorandom-number-generator-exponential-distribution
		public static float Normalize( float randx, float lambda )
		{
			return Convert.ToSingle((Math.Log(1-randx) / (-lambda)));
		}
	}
}