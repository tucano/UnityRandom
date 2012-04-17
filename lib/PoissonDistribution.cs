using System;
using NPack;

//
// NEW VERSION FROM NUMERICAL RECIPES http://www.nrbook.com/a/bookcpdf.php
// 7.3 Rejection Method: Gamma, Poisson, Binomial Deviates

namespace URandom
{
	public static class PoissonDistribution
	{		
		
		// return as a floating point number an integer value that is a random deviate drawn 
		// from a Possion Distribution of mean xm using randx as a source of uniform deviates
		public static float Normalize( ref NPack.MersenneTwister _rand, float xm)
		{
			// Davide Rambaldi: all moved to double precision			
			double sq, alxm, g, oldm; // oldm is a flag for wheter xm has changed or not sincle last call
			sq = alxm = g = oldm = (-1.0);
			double em, t, y;			
			
			if (xm < 12.0f) {      // Use direct method				
				if (xm != oldm) {
					oldm = xm;
					g = Math.Exp(-xm); // if x is new, compute the exponential
				}
				em = -1;
				t = 1.0f;				
				// Instead of adding exponential deviates it is equivalent to multiply unifomr deviates
				// We never actually take the log, we compare the precomupted exponential				
				do {
					++em;
					t *= _rand.NextSingle(true);
				} while (t > g);				
			} else {
				// Use REJECTION method
				// xm has changed?
				if ( xm != oldm) {
					oldm = xm;
					sq = Math.Sqrt(2.0 * xm);
					alxm = Math.Log(xm);
					
					// Gammln is the natural log of a gamma function
					g = xm * alxm - SpecialFunctions.gammln(xm + 1.0f);
				}
				do {	
					do {
						// y is the deviate from a Lorentzian comparison function
						y = Math.Tan(Math.PI*_rand.NextSingle(true));
						em=sq*y+xm;						
					} while (em < 0.0);
					em = Math.Floor(em);
					t = 0.9 * (1.0+y*y) * Math.Exp(em*alxm-SpecialFunctions.gammln(em+1.0f)-g);
				} while (_rand.NextSingle(true) > t);
			}			
			return (float) em;
		}
		
	}
	
}