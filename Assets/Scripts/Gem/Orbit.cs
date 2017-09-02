using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {
	
	public float degreesPerSecond = 10;
	public Vector3 rotationAxis = Vector3.up;
	Vector3 centerOfObject;

	void Awake(){
		centerOfObject = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (centerOfObject, rotationAxis, degreesPerSecond * Time.deltaTime);
	}
}
