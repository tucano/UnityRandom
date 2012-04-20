using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using System.IO;
using URandom;

public class UnityRandomEditorDraw
{
	public static Texture2D aaLineTex = null;
	public static Texture2D lineTex = null;
	public static Texture2D tex = null;
	
	public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
	{
		Color savedColor = GUI.color;
    	Matrix4x4 savedMatrix = GUI.matrix;
		
		if (!lineTex)
        {
            lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
            lineTex.SetPixel(0, 1, Color.white);
            lineTex.Apply();
        }
		
		if (!aaLineTex)
		{
			aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, true);
        	aaLineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
        	aaLineTex.SetPixel(0, 1, Color.white);
        	aaLineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
        	aaLineTex.Apply();
		}
		
		if (antiAlias) width *= 3;
		
		float angle = Vector3.Angle(pointB - pointA, Vector2.right) * (pointA.y <= pointB.y?1:-1);
		
		float m = (pointB - pointA).magnitude;
		
		if (m > 0.01f)
        {
            Vector3 dz = new Vector3(pointA.x, pointA.y, 0);

            GUI.color = color;
            GUI.matrix = translationMatrix(dz) * GUI.matrix;
            GUIUtility.ScaleAroundPivot(new Vector2(m, width), new Vector3(-0.5f, 0, 0));
            GUI.matrix = translationMatrix(-dz) * GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, Vector2.zero);
            GUI.matrix = translationMatrix(dz + new Vector3(width / 2, -m / 2) * Mathf.Sin(angle * Mathf.Deg2Rad)) * GUI.matrix;

            if (!antiAlias)
                GUI.DrawTexture(new Rect(0, 0, 1, 1), lineTex);
            else
                GUI.DrawTexture(new Rect(0, 0, 1, 1), aaLineTex);
        }
        GUI.matrix = savedMatrix;
        GUI.color = savedColor;
	}
	
	private static Matrix4x4 translationMatrix(Vector3 v)
    {
        return Matrix4x4.TRS(v,Quaternion.identity,Vector3.one);
    }	
	
	public static void DrawXYPlot(ArrayList rlist, int width, int height)
	{
		DrawXYPoints(rlist, 0, 1, width, height);
	}
	
	public static void DrawXYPlot(ArrayList rlist, int width, int height, bool scale)
	{
		// SCALE
		float _min, _max;
		if (scale) {
			_min = Convert.ToSingle(rlist[0]);
			_max = Convert.ToSingle(rlist[rlist.Count - 1]);
		} else {
			_min = 0;
			_max = 1;
		}		
		DrawXYPoints(rlist, _min, _max, width, height);
	}
	
	public static void DrawXYPlot(ArrayList rlist, int width, int height, float min, float max)
	{	
		DrawXYPoints(rlist, min, max, width, height);
	}
	
	private static void DrawXYPoints(ArrayList rlist, float _min, float _max, int width, int height)
	{
		int counter = 0;
		foreach ( object obj in rlist ) 
		{
			counter++;
			float x, y, val;
			val = Convert.ToSingle(obj);
			val = ( (val - _min) / (_max - _min) );
			y = val * height;
			x = counter * (Convert.ToSingle(width) / rlist.Count);
			DrawPoint(new Vector2(x,y));
		}	
	}
	
	public static void DrawV3Plot(ArrayList rlist, int width, int height, UnityRandomEditorWindow.RandomVector3DType _type, float alpha, float beta)
	{
		foreach ( object obj in rlist ) 
		{
			Vector2 _pos = MapTo2D( (Vector3) obj, alpha, beta);    // rotation (alpha,beta)
			// move from space -1 - 1 to space 0-1
			_pos = ((_pos + Vector2.one) * 0.5f);
			DrawPoint( Vector2.Scale(_pos, new Vector2(width, height)));
		}
	}
	
	private static Vector2 MapTo2D(Vector3 pos, float a, float b)
	{
		// MAP a 3D point in a 2D space (a and b gives you the rotation)
		float ca = (float) Math.Cos(a); float sa = (float) Math.Sin(a);
		float cb = (float) Math.Cos(b); float sb = (float) Math.Sin(b);		
		float xx = ((pos.x * ca) + (pos.y*sa))*cb + pos.z*sb;
		float yy = (pos.y*ca) - (pos.x*sa);
		return new Vector2(xx,yy);
	}
	
	public static void DrawV2Plot(ArrayList rlist, int width, int height, UnityRandomEditorWindow.RandomVector2DType _type)
	{	
		foreach ( object obj in rlist ) 
		{
			Vector2 pos = ((Vector2) obj + Vector2.one) * 0.5f;
			switch (_type) {
			case UnityRandomEditorWindow.RandomVector2DType.SQUARE:
				DrawPoint( Vector2.Scale( pos, new Vector2(width, height)) );
			break;
			case UnityRandomEditorWindow.RandomVector2DType.CIRCLE:
				DrawPoint( Vector2.Scale( pos, new Vector2(width, height) ) );
			break;
			case UnityRandomEditorWindow.RandomVector2DType.DISK:
				DrawPoint( Vector2.Scale( pos, new Vector2(width, height) ) );
			break;
			default:
			break;
			}
		}
	}
	
	private static void DrawPoint(Vector2 pos)
	{		
		// 1. make a Cross around zero
		Vector2 _pointA = new Vector2(0.0f,-1.0f);
		Vector2 _pointB = new Vector2(0.0f,1.0f);				
		Vector2 _pointC = new Vector2(1.0f,0.0f);
		Vector2 _pointD = new Vector2(-1.0f,0.0f);
		
		// 2. move the cross into place
		// Y: value
		// X: Position
		Vector2 _a = _pointA + pos;
		Vector2 _b = _pointB + pos;
		Vector2 _c = _pointC + pos;
		Vector2 _d = _pointD + pos;
		
		UnityRandomEditorDraw.DrawLine(_a,_b,Color.blue,1.0f,true);
		UnityRandomEditorDraw.DrawLine(_c,_d,Color.blue,1.0f,true);
	}

	public static void DrawColorPlot(ArrayList rlist, int width, int height)
	{
		float x,y;
		x = y = 0.0f;
		float grid_space = 25.0f;
		if (!tex) {
			tex = new Texture2D(1,1,TextureFormat.ARGB32,false);
		}
		// make a simple grid
		foreach ( object obj in rlist ) 
		{
			Vector2 a = new Vector2(x,y);
			Vector2 b = new Vector2((x+grid_space),y);
			Vector2 c = new Vector2(x,(y+grid_space));
			Vector2 d = new Vector2((x+grid_space),(y+grid_space));
			DrawPoint(a);
			DrawPoint(b);
			DrawPoint(c);
			DrawPoint(d);
			
			// make a new texture with the color
			tex.SetPixel(0, 1, (Color) obj);
			tex.Apply();
			GUI.DrawTexture(new Rect(a.x,a.y,grid_space,grid_space), tex);
			
			if (x < (width - grid_space)) {
				x += grid_space;
			} else {
				x = 0;
				y += grid_space;
			}
		}
	}
}
