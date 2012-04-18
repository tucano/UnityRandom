using UnityEngine;
using System.Collections;

namespace URandom
{
	public class WaveToRgb 
	{
		// CONSTS
		public const float MinVisibleWaveLength = 350.0f;
		public const float MaxVisibleWaveLength = 650.0f;
		public const float Gamma = 0.80f;
		public const int IntesityMax = 255;
		
		// CORE
		public static Color LinearToRgb( float linearvalue)
		{
			return WaveLengthToRgb(GetWaveLengthFromDataPoint( linearvalue, 0.0f, 1.0f));
		}
		
		// WAVE LENGTH ALGORITHM
		private static float GetWaveLengthFromDataPoint(float x, float min, float max)
		{
			// convert data in the range min - max to wavelength
			float result = (x - min) / (max - min) * (MaxVisibleWaveLength - MinVisibleWaveLength) + MinVisibleWaveLength;
			return result;
		}
		
		// ADJUST ALGORITHM
		private static int Adjust( float color, float factor)
		{
			if (color == 0.0f) {
				return 0;
			} else {
				return (int) Mathf.Round( IntesityMax * Mathf.Pow(color * factor, Gamma) );
			}
		}
		
		// WAVELENGTH TO RGB
		private static Color WaveLengthToRgb( float wave )
		{
			wave = Mathf.Floor(wave);
			
			float r = 0.0f;
			float g = 0.0f;
			float b = 0.0f;
			float factor = 0.0f;
	
			if (wave >= 380 && wave < 440) {
				r = - (wave - 440) / (440 -380);
				g = 0.0f;
				b = 1.0f;
			} else if (wave >= 440 && wave < 490) {
				r = 0.0f;
				g = (wave - 440) / (490 - 440);
				b = 1.0f;
			} else if (wave >= 490 && wave < 510) {
				r = 0.0f;
				g = 1.0f;
				b = - (wave - 510) / (510 - 490);
			} else if (wave >= 510 && wave < 580) {
				r = (wave - 510) / (580 - 510);
				g = 1.0f;
				b = 0.0f;
			} else if (wave >= 580 && wave < 645) {
				r = 1.0f;
				g = - (wave - 645) / (645 - 580);
				b = 0.0f;
			} else if (wave >= 645 && wave <= 780) {
				r = 1.0f;
				g = 0.0f;
				b = 0.0f;
			} else {
				r = 0.0f;
				g = 0.0f;
				b = 0.0f;
			}
			
			// Let the intensity fall off near the vision limits
			if (wave >= 380 && wave	< 420) {
				factor = 0.3f + 0.7f * ( wave - 380) / (420 - 380);
			} else if (wave >= 420 && wave < 700) {
				factor = 1.0f;
			} else if (wave >= 700 && wave <= 780) {
				factor = 0.3f + 0.7f * (780 - wave) / (780 - 700);
			} else {
				factor = 0.0f;
			}
			
			r = Adjust(r, factor);
			g = Adjust(g, factor);
			b = Adjust(b, factor);
			
			// come back to 0.0 - 1.0
			r = r / 255;
			g = g / 255;
			b = b / 255;
			
			Color newColor = new Color(r,g,b);
			return newColor;
		}
	}
}

