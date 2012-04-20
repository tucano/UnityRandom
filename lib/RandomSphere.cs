using System;
using UnityEngine;
using NPack;

namespace URandom
{
	public static class RandomSphere
	{
		// RADIUS = 1
		
		// Marsaglia Method (1972) I use for Surface and Volume
		// FROM http://stackoverflow.com/questions/5531827/random-point-on-a-given-sphere
		// ALSO: http://mathworld.wolfram.com/SpherePointPicking.html
		// 1. Pick a random point inside the [-1,1]x[-1,1]x[-1,1] cube
		// 2. If x*x + y*y + z*z > 1 repeat from 1
		// 3. Normalize dividing x, y and z by Math.sqrt(x*x + y*y + z*z)
		
		public static Vector3 Surface(ref NPack.MersenneTwister _rand)
		{
			Vector3 pos = PickCubePoints(ref _rand);
			while ( IsNotOnSurface(pos) )
			{
				pos = PickCubePoints(ref _rand);
			}
			return Normalize(pos);
		}
		
		public static Vector3 Volume(ref NPack.MersenneTwister _rand)
		{
			Vector3 pos = PickCubePoints(ref _rand);
			while ( isNotInsideSphere(pos) )
			{
				pos = PickCubePoints(ref _rand);
			}
			return pos;
		}
		
		private static Vector3 PickCubePoints( ref NPack.MersenneTwister _rand )
		{
			
			float x = SpecialFunctions.ScaleFloatToRange( _rand.NextSingle(true), -1, 1, 0, 1 );
			float y = SpecialFunctions.ScaleFloatToRange( _rand.NextSingle(true), -1, 1, 0, 1 );
			float z = SpecialFunctions.ScaleFloatToRange( _rand.NextSingle(true), -1, 1, 0, 1 );			
			
			return new Vector3(x,y,z);
		}
		
		private static bool isNotInsideSphere(Vector3 pos)
		{
			return( (pos.x * pos.x) + (pos.y * pos.y) + (pos.z * pos.z) > 1 );
		}
		
		private static bool IsNotOnSurface(Vector3 pos)
		{
			return ( (pos.x * pos.x) + (pos.y * pos.y) + (pos.z * pos.z) > 1 );
		}
		
		private static Vector3 Normalize( Vector3 pos)
		{
			float k = (float) Math.Sqrt( (pos.x * pos.x) + (pos.y * pos.y) + (pos.z * pos.z) );
			return new Vector3( pos.x / k, pos.y / k, pos.z / k );
		}
		
		/// FROM: http://unifycommunity.com/wiki/index.php?title=UnitSphere
		/// <summary>
	    /// Returns a point on the unit sphere that is within a cone along the z-axis
	    /// </summary>
	    /// <param name="spotAngle">[0..180] specifies the angle of the cone. </param>
	    public static Vector3 GetPointOnCap(float spotAngle, ref NPack.MersenneTwister _rand)
	    {
	        float angle1 = SpecialFunctions.ScaleFloatToRange(_rand.NextSingle(true),0.0f, Mathf.PI*2, 0, 1);
	        float angle2 = SpecialFunctions.ScaleFloatToRange(_rand.NextSingle(true), 0.0f,spotAngle * Mathf.Deg2Rad, 0, 1);
	        Vector3 V = new Vector3(Mathf.Sin(angle1),Mathf.Cos(angle1),0);
	        V *= Mathf.Sin(angle2);
	        V.z = Mathf.Cos(angle2);
	        return V;
	    }
		
		// FIXME: NOT YET IMPLEMENTED IN UNITYRANDOM
		public static Vector3 GetPointOnCap(float spotAngle, ref NPack.MersenneTwister _rand, Quaternion orientation)
	    {
	        return orientation * GetPointOnCap(spotAngle, ref _rand);
	    }
		
		// FIXME: NOT YET IMPLEMENTED IN UNITYRANDOM
	    public static Vector3 GetPointOnCap(float spotAngle, ref NPack.MersenneTwister _rand, Transform relativeTo, float radius)
	    {
	        return relativeTo.TransformPoint( GetPointOnCap(spotAngle, ref _rand)*radius );
	    }

		/// FROM: http://unifycommunity.com/wiki/index.php?title=UnitSphere
		/// <summary>
	    /// Returns a point on the unit sphere that is within the outer cone along the z-axis
	    /// but not inside the inner cone. The resulting area describes a ring on the sphere surface.
	    /// </summary>
	    /// <param name="innerSpotAngle">[0..180] specifies the inner cone that should be excluded.</param>
	    /// <param name="outerSpotAngle">[0..180] specifies the outer cone that should be included.</param>
	    public static Vector3 GetPointOnRing(float innerSpotAngle, float outerSpotAngle, ref NPack.MersenneTwister _rand)
	    {
	        float angle1 = SpecialFunctions.ScaleFloatToRange(_rand.NextSingle(true),0.0f, Mathf.PI*2, 0, 1);
	        float angle2 = SpecialFunctions.ScaleFloatToRange(_rand.NextSingle(true),innerSpotAngle, outerSpotAngle, 0, 1) * Mathf.Deg2Rad;
	        Vector3 V = new Vector3(Mathf.Sin(angle1),Mathf.Cos(angle1),0);
	        V *= Mathf.Sin(angle2);
	        V.z = Mathf.Cos(angle2);        
	        return V;
	    }
	
		// FIXME: NOT YET IMPLEMENTED IN UNITYRANDOM
	    public static Vector3 GetPointOnRing(float innerSpotAngle, float outerSpotAngle, ref NPack.MersenneTwister _rand, Quaternion orientation)
	    {
	        return orientation * GetPointOnRing(innerSpotAngle, outerSpotAngle, ref _rand);
	    }
	
		// FIXME: NOT YET IMPLEMENTED IN UNITYRANDOM
	    public static Vector3 GetPointOnRing(float innerSpotAngle, float outerSpotAngle, ref NPack.MersenneTwister _rand, Transform relativeTo, float radius)
	    {
	        return relativeTo.TransformPoint( GetPointOnRing(innerSpotAngle, outerSpotAngle, ref _rand)*radius );
	    }
	}
}