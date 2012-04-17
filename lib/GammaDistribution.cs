using System;
using NPack;

namespace URandom
{
	// Returns a deviate distributed as a gamma distribution of integer of order ia, i.e. the waiting time for the iath event in a Poisson process of unit mean 
	// Use _rand as a source of uniform deviates
	// http://www.nrbook.com/a/bookcpdf.php
	public static class GammaDistribution
	{
		public static float Normalize(ref NPack.MersenneTwister _rand, int ia)
		{
			int j;
			float am, e, s, v1, v2, x, y;
			
			if (ia < 1)
				throw new ArgumentException("Error in Gamma Distribution. Argument ia should be an integer > 1");
			
			if (ia < 6) {
				// use direct method, addin waiting times 
				x = 1.0f;
				for (j=1; j<=ia; j++) x *= _rand.NextSingle(true);
				x = (float) - (Math.Log(x));
			} else {
				do {
					do {
						// This four lines generate the tanget of random angle
						// Equivalent to y = tan( pi * rand())
						do {
							v1 = _rand.NextSingle(true);
							v2 = 2.0f * _rand.NextSingle(true) - 1.0f;
						} while (v1 * v1+v2 * v2 > 1.0f);
						y = v2/v1;
						am = ia-1;
						s = (float) Math.Sqrt(2.0*am+1.0f);
						x = s * y + am;
						// We decide wheter to reject x, Reject in 0 probability region
					} while (x <= 0.0f);
					e = (float) ((1.0f + y*y) * Math.Exp(am * Math.Log(x/am) -s*y));
				} while (_rand.NextSingle(true) > e);
			}
			return x;
		}
	}
}