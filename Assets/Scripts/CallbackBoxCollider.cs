using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbackBoxCollider : MonoBehaviour {

	public Tile parentTile;

	// Update is called once per frame
	void OnMouseOver () {
		parentTile.OnMouseOver ();
	}
}
