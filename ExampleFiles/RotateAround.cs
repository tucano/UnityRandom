using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {
	
	public Transform pivotPoint;
	public bool rotating = true;
	
	void Start()
	{
		if (!pivotPoint)
			Debug.LogError("I need an object to rotate around");
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		if (rotating)
		 	transform.RotateAround(pivotPoint.position, Vector3.up, 20 * Time.deltaTime);
	}
}
