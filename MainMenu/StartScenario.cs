using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartScenario : MonoBehaviour {


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


		GameData.instance.Players.Add (new playerData ());
		GameData.instance.Players.Add (new playerData ());

		GameData.instance.Players [0].playerName = "Imperial";
		GameData.instance.Players [0].active = true;
		GameData.instance.Players [0].humanPlayer = true;
		GameData.instance.Players [0].race = 0;

		GameData.instance.Players [1].playerName = "Chaos";
		GameData.instance.Players [1].active = true;
		GameData.instance.Players [1].humanPlayer = true;
		GameData.instance.Players [1].race = 1;

		for(int i = 0 ; i < GameData.instance.Players.Count; i++){

			if(i == 0){
				GameData.instance.Players[i].shipBattlePrefabs.Add(GameData.instance.GothicPrefab);
				GameData.instance.Players[i].shipBattlePrefabs.Add(GameData.instance.DauntlessPrefab);
				GameData.instance.Players[i].shipBattlePrefabs.Add(GameData.instance.TyrantPrefab);
			}else{
				GameData.instance.Players[i].shipBattlePrefabs.Add(GameData.instance.MurderPrefab);
				GameData.instance.Players[i].shipBattlePrefabs.Add(GameData.instance.CarnagePrefab);
				GameData.instance.Players[i].shipBattlePrefabs.Add(GameData.instance.SlaughterPrefab);
			}
		}

		Application.LoadLevel ("CruiserClash");
	}
}
