using System;

//
// FROM HERE:
// 
// http://stackoverflow.com/questions/918736/random-number-generator-that-produces-a-power-law-distribution
//
//

namespace URandom 
{
	public static class PowerLaw 
	{
		public static double Normalize( float x, float t, float min, float max)
		{
			return Math.Pow( ( Math.Pow(max, (t + 1)) - Math.Pow(min,(t+1)) ) * x + Math.Pow(min,(t+1)) , ( 1 / (t+1) ));
		}
	}
}