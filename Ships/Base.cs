using UnityEngine;
using System.Collections;

public class Base : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnColisionEnter(Collision col){
		if (col.gameObject.name == "Blast Marker") {
			transform.parent.SendMessage("BlastMarkerCollision");
		}
	}
}
