using System;
using UnityEngine;
using NPack;

namespace URandom
{
	public static class RandomCube
	{
		public static Vector3 Surface(ref NPack.MersenneTwister _rand)
		{
			// Move to -1, 1 space as for CIRCLE and SPHERE
			Vector3 pos = GetPointOnCubeSurface(_rand.NextSingle(true),_rand.NextSingle(true),_rand.Next(5));	
			return new Vector3((2*pos.x)-1, (2*pos.y)-1, (2*pos.z)-1);
		}
	
		public static Vector3 Surface(ref NPack.MersenneTwister _rand, UnityRandom.Normalization n, float t)
		{
			Vector3 pos = new Vector3();
			switch (n) {
			case UnityRandom.Normalization.STDNORMAL:
				pos = GetPointOnCubeSurface(
					(float) NormalDistribution.Normalize(_rand.NextSingle(true), t),
					(float) NormalDistribution.Normalize(_rand.NextSingle(true), t),
					_rand.Next(5));
			break;
			case UnityRandom.Normalization.POWERLAW:
				pos = GetPointOnCubeSurface(
					(float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1),
					(float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1),
					_rand.Next(5));
			break;
			default:
				pos = GetPointOnCubeSurface(_rand.NextSingle(true),_rand.NextSingle(true),_rand.Next(5));
			break;
			}
			
			// Move to -1, 1 space as for CIRCLE and SPHERE
			return new Vector3((2*pos.x)-1, (2*pos.y)-1, (2*pos.z)-1);
		}
		
		// This should work: FIXME?
		// take 3 random numbers, use 2 for coordinates, the last one to select which face.			
		private static Vector3 GetPointOnCubeSurface( float xx, float yy, int side)
		{
			float x,y,z;

			// SIDES MAP
			switch (side) {
			case 0: z = 0; x = xx; y = yy; break;
			case 1: z = 1; x = xx; y = yy; break;
			case 2: z = xx; x = yy; y = 0; break;
			case 3: z = xx; x = yy; y = 1; break;
			case 4:	z = xx; y = yy; x = 0; break;
			case 5:	z = xx; y = yy; x = 1; break;
			default: x = 0; y = 0; z = 0; break;
			}
			
			return new Vector3(x,y,z);
		}
			
		public static Vector3 Volume(ref NPack.MersenneTwister _rand)
		{
			Vector3 pos = new Vector3(_rand.NextSingle(true), _rand.NextSingle(true), _rand.NextSingle(true));
			// Move to -1, 1 space as for CIRCLE and SPHERE
			return new Vector3((2*pos.x)-1, (2*pos.y)-1, (2*pos.z)-1);
		}
		
		public static Vector3 Volume(ref NPack.MersenneTwister _rand, UnityRandom.Normalization n, float t)
		{
			float x, y, z;
			x = y = z = 0;
			switch (n) {
			case UnityRandom.Normalization.STDNORMAL:
				x = (float) NormalDistribution.Normalize(_rand.NextSingle(true), t);
				y = (float) NormalDistribution.Normalize(_rand.NextSingle(true), t);
				z = (float) NormalDistribution.Normalize(_rand.NextSingle(true), t);
			break;
			case UnityRandom.Normalization.POWERLAW:
				x = (float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1);
				y = (float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1);
				z = (float) PowerLaw.Normalize(_rand.NextSingle(true), t, 0, 1);
			break;
			default:
				x = _rand.NextSingle(true);
				y = _rand.NextSingle(true);
				z = _rand.NextSingle(true);
			break;
			}
			// Move to -1, 1 space as for CIRCLE and SPHERE
			return new Vector3((2*x)-1, (2*y)-1, (2*z)-1);
		}
	}
}