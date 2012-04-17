using System;

/////////////////////////////////////////////////////////////////////////////
//                                                                         //
// Unity Random                                                            //
//                                                                         //
// special Functions from http://www.nrbook.com/a/bookcpdf/c6-1.pdf        //
//                                                                         //
//                                                                         //
/////////////////////////////////////////////////////////////////////////////
	
namespace URandom
{
	public static class SpecialFunctions
	{
		// Builtin arrays (native .NET arrays), are extremely fast and efficient but they can not be resized.			
		static double[] cof = new double[6] {
			76.18009172947146,
			-86.50532032941677,
			24.01409824083091,
			-1.231739572450155,
			0.1208650973866179e-2,
			-0.5395239384953e-5};
		
		// 6.1 Gamma Function, Beta Function, Factorials, Binomial Coefficients 
		// http://www.nrbook.com/a/bookcpdf/c6-1.pdf
		// Return the natural log of a gamma function for xx > 0
		// Internal arithmetic in double precision.
		public static double gammln( double xx )
		{
			double x,y,tmp,ser;
			
			int j;
			
			y = x = xx;
			tmp = x + 5.5;
			tmp -= (x + 0.5) * Math.Log(tmp);
			ser=1.000000000190015;
			
			for (j=0;j<=5;j++) 
			{
				ser += cof[j]/++y;	
			}
			
			return -tmp+Math.Log(2.5066282746310005 * ser/x );
		}
		
		// Scale range old to any range
		public static float ScaleFloatToRange(float x, float newMin, float newMax, float oldMin, float oldMax)
		{
			return (x / ((oldMax - oldMin) / (newMax - newMin))) + newMin;
		}
	}
}