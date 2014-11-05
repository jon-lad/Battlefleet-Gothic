using UnityEngine;
using System.Collections;

public class QuitButton : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver(){
		renderer.material.color = new Color32(212,11,57, 255);
	}

	void OnMouseExit(){
		renderer.material.color = new Color(1 -(214 / 255),1 - (185 / 255),1 -(167 / 255));
	}

	void OnMouseDown(){
		Application.Quit ();
	}
}
