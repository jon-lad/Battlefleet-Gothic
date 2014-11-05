using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	

	public bool paused = false;
		
	void Start (){

		Time.timeScale = 1;

		
	}
	

			
	void Update (){
		if (Input.GetKeyUp (KeyCode.Escape)) {
			paused = togglePause ();
		}
	}
			
	bool togglePause(){
		if(paused == true){
			Time.timeScale = 1;
			return(false);
		}else{
			Time.timeScale = 0;
			return(true);
		}
	}
}﻿
