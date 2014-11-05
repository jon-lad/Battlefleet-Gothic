using UnityEngine;
using System.Collections;

public class MapItem  : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject mapBounds = GameObject.CreatePrimitive (PrimitiveType.Cube);
		mapBounds.name = "MapBounds";
		mapBounds.layer = 8;
		Destroy(mapBounds.collider);
		mapBounds.transform.parent = transform;
		mapBounds.transform.localScale = Vector3.one;
		mapBounds.transform.localPosition = Vector3.zero;
		mapBounds.renderer.material.color = renderer.material.color;
	}
}
