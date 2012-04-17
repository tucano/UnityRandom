using System;
using UnityEngine;
using NPack;

namespace URandom
{
	public static class RandomDisk
	{
		// CIRCLE with R=1
		public static Vector2 Circle( ref NPack.MersenneTwister _rand )
		{
			float t = (float) _rand.Next();
			float _2pi = (float) Math.PI * 2;
			float a = SpecialFunctions.ScaleFloatToRange(t, 0, _2pi, 0, Int32.MaxValue);
			return new Vector2( (float) Math.Cos(a) , (float) Math.Sin(a));
		}
		
		public static Vector2 Circle( ref NPack.MersenneTwister _rand, UnityRandom.Normalization n, float t )
		{
			float r;
			switch (n) {
			case UnityRandom.Normalization.STDNORMAL:
				r = SpecialFunctions.ScaleFloatToRange( (float) NormalDistribution.Normalize(_rand.NextSingle(true), t), 0, Int32.MaxValue, 0, 1);
			break;
			case UnityRandom.Normalization.POWERLAW:
				r = (float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, Int32.MaxValue);
			break;
			default:
				r = (float) _rand.Next();
			break;
			}			
			float _2pi = (float) Math.PI * 2;
			float a = SpecialFunctions.ScaleFloatToRange(r, 0, _2pi, 0, Int32.MaxValue);
			return new Vector2( (float) Math.Cos(a) , (float) Math.Sin(a));
		}
		
		
		public static Vector2 Disk( ref NPack.MersenneTwister _rand )
		{
			// t [0,1] , Theta [0,2pi)
			double t = _rand.NextSingle(true);
			// in range [0,1) then multiply this number by k to get a random number in the range [0,k)
			double theta = _rand.NextSingle(false) * 2 * Math.PI;	
			return new Vector2( (float) (Math.Sqrt(t) * Math.Cos(theta)), (float) (Math.Sqrt(t) * Math.Sin(theta)) );
		}
		
		public static Vector2 Disk( ref NPack.MersenneTwister _rand, UnityRandom.Normalization n, float temp )
		{
			double t, theta;
			
			switch (n) {
			case UnityRandom.Normalization.STDNORMAL:
				t = NormalDistribution.Normalize(_rand.NextSingle(true), temp);
				theta = NormalDistribution.Normalize(_rand.NextSingle(true), temp) * 2 * Math.PI;	
			break;
			case UnityRandom.Normalization.POWERLAW:
				t = PowerLaw.Normalize(_rand.NextSingle(true), temp, 0, 1);
				theta = PowerLaw.Normalize(_rand.NextSingle(true), temp, 0, 1) * 2 * Math.PI;	
			break;
			default:
				t = (float)  _rand.NextSingle(true);
				theta = _rand.NextSingle(false) * 2 * Math.PI;
			break;
			}
			
			return new Vector2( (float) (Math.Sqrt(t) * Math.Cos(theta)), (float) (Math.Sqrt(t) * Math.Sin(theta)) );			
		}
	}
}