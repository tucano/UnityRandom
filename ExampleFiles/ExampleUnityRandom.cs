using UnityEngine;
using System.Collections;

public class ExampleUnityRandom : MonoBehaviour 
{
	private UnityRandom urand;
	
	private bool rotate = false;
	public float sphereRadius = 20.0f;
	public int max_objects = 1000;
	public int seed = 123456;
	
	/* LateUpdate is called after all Update functions have been called. */
	void LateUpdate ()
	{
		if (Input.GetButtonDown("Jump")) {
			rotate = !rotate;
			ToggleRotation();
		}
	}
	
	private void ToggleRotation()
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			item.GetComponent<RotateAround>().rotating = rotate;
		}
	}
	
	// GUI
	void OnGUI()
	{
		GUILayout.BeginVertical("box");
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("UNITYRANDOM POINT EXAMPLES");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("SET A SEED: " + seed);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		// I cut the SEED space for the GUI
		// Because the Slider get crazy with Int32.MaxValue (it goes back to Int.32.MinValue)
		seed = (int) GUILayout.HorizontalSlider(seed, 0, UnityRandom.max_seed - 1000);
		
		if (GUILayout.Button("IN SPHERE SURFACE")) ExampleInSpheres();
		if (GUILayout.Button("IN CIRCLE")) ExampleInCircle();
		if (GUILayout.Button("IN DISK")) ExampleInDisk();
		if (GUILayout.Button("IN SPHERE VOLUME")) ExampleInSphere();
		if (GUILayout.Button("IN CUBE")) ExampleInCube();		
		if (GUILayout.Button("ON CUBE")) ExampleOnCube();
		if (GUILayout.Button("ON CAP")) ExampleOnCap();
		if (GUILayout.Button("ON RING")) ExampleOnRing();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("TOGGLE ROTATION WITH JUMP");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
	}
	
	private void ExampleOnRing()
	{
		InitRandom();
		CleanUp();
		MakeObjects();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			item.transform.position = ScalePosition(urand.PointOnRing(20.0f, 30.0f));
		}
	}
	
	private void ExampleOnCap()
	{
		InitRandom();
		CleanUp();
		MakeObjects();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			item.transform.position = ScalePosition(urand.PointOnCap(30.0f));
		}
	}
	
	// INIT a new UnityRandom object with a random seed
	private void InitRandom()
	{
		urand = new UnityRandom(seed);
	}
	
	private void ExampleInCircle()
	{
		InitRandom();
		CleanUp();		
		MakeObjects();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			item.transform.position = ScalePosition(urand.PointInACircle());
		}
	}
	
	private void ExampleInDisk()
	{
		InitRandom();
		CleanUp();		
		MakeObjects();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			item.transform.position = ScalePosition(urand.PointInADisk());
		}
	}
	
	private void ExampleInSphere()
	{
		InitRandom();
		CleanUp();		
		MakeObjects();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			item.transform.position = ScalePosition(urand.PointInASphere());
		}
	}
	
	private void ExampleInCube()
	{
		InitRandom();
		CleanUp();		
		MakeObjects();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			item.transform.position = ScalePosition(urand.PointInACube());
		}
	}
	
	private void ExampleOnCube()
	{
		InitRandom();
		CleanUp();		
		MakeObjects();
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			item.transform.position = ScalePosition(urand.PointOnACube());
		}
	}
	
	private void ExampleInSpheres()
	{
		InitRandom();
		CleanUp();		
		MakeObjects();
	}
	
	private void CleanUp()
	{
		// Clear all
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in objs) 
		{
			Destroy(item);	
		}
	}
	
	private void MakeObjects()
	{
		
		// CENTER OF THE SPHERE: x = 0, y = 20, z = 0 I generate an object there
		GameObject _pivot = new GameObject();
		_pivot.transform.position = new Vector3(0,sphereRadius,0);
			
		for (int i = 0; i < max_objects; i++) 
		{
			// WE make a small sphere
			GameObject _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			// WE SCALE THE SPHERE
			_sphere.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
			// TAKE A SCALED RANDOM POSITION
			Vector3 pos = ScalePosition(urand.PointOnASphere());			
			// WE set the sphere position
			_sphere.transform.position = pos;
			// WE give a tag to the sphere
			_sphere.tag = "Player";
			// Attach the rotation script
			RotateAround _ra = _sphere.AddComponent<RotateAround>();
			// SET the PIVOT POINT IN SCRIPT
			_ra.pivotPoint = _pivot.transform;
			// SET the MATERIAL as a RANDOM COLOR
			_sphere.renderer.material.color = urand.Rainbow();
		}
	}
	
	// CAN BE ADDED IN THE LIBRARY??? FIXME
	private Vector3 ScalePosition(Vector3 pos)
	{
		// WE scale TO 20,20,20 
		pos = Vector3.Scale(pos, new Vector3(sphereRadius,sphereRadius,sphereRadius));
		// WE move UP
		pos = pos + new Vector3(0,sphereRadius,0);
		return pos;
	}
}
