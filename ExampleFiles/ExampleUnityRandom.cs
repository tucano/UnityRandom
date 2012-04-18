using UnityEngine;
using System.Collections;

public class ExampleUnityRandom : MonoBehaviour 
{
	
	private bool randObjectsInScene;
	private UnityRandom urand;
	
	// Use this for initialization
	void Start () 
	{
		// INIT a new UnityRandom object with a random seed
		urand = new UnityRandom();
		randObjectsInScene = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	// GUI
	void OnGUI()
	{
		// Make a background box
		GUILayout.BeginArea(new Rect(10,10,250,150));
		GUILayout.BeginVertical("box");
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("UNITY RANDOM EXAMPLES");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();		
		if (GUILayout.Button("RANDOM OBJECTS")) ExampleRandomSpheres();
		GUILayout.Button("ROLL DICES");
		GUILayout.EndVertical();
		GUILayout.EndArea();
		
		// RAND OBJECTS SPECIFIC GUI
		if (randObjectsInScene) {
			
		}
	}
	
	private void ExampleRandomSpheres()
	{
		CleanUp();		
		
		for (int i = 0; i < 1000; i++) 
		{
			// WE make a small sphere
			GameObject _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			_sphere.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
			// WE scale ...
			Vector3 pos = Vector3.Scale(urand.PointInASphere(), new Vector3(20,20,20));
			// WE move UP
			pos = pos + new Vector3(0,20,0);
			// WE set the sphere position
			_sphere.transform.position = pos;
			// WE give a tag to the sphere
			_sphere.tag = "RandomTest";
		}
		
		randObjectsInScene = true;
	}
	
	private void CleanUp()
	{
		// Clear all
		GameObject[] objs = GameObject.FindGameObjectsWithTag("RandomTest");
		foreach (GameObject item in objs) 
		{
			Destroy(item);	
		}
		randObjectsInScene = false;
	}
}
