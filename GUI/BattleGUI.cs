using UnityEngine;
using System.Collections;


public class battleGUI : MonoBehaviour {

	string turnButtonName = "Turn Ship";

	bool undoTurnVisible;
	bool specialOrdersVisible;
	bool weaponsVisible;
	bool weapon1Selected;
	bool weapon2Selected;
	bool weapon3Selected;
	bool weapon4Selected;
	bool weaponsReady;
	int buttonsCount = 0;

	private int buttonWidth = 110;
	private int buttonHeight = 20;



	void OnGUI () {

		if (BattleManager.instance.playersLoaded == true) {

			Ship currentShip = BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].selectedShip;
			Player currentPlayer = BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex];

			#region Next Ship Button

			if(currentPlayer.phaze !=2){

				// Make the first button. If it is pressed, next ship will be selected
				if (GUI.Button (new Rect (20, 40, buttonWidth, buttonHeight), "Next Ship")) {
					//if game not Paused
					if(Time.timeScale != 0){
						//for each of the players ships
						for (int i = 0; i < currentPlayer.remainingShips.Count; i ++) {
							//if the ship is the currently selected ship
							if (currentPlayer.remainingShips [i] == currentShip) {
								//if the next ship is on the list
								if (i + 1 < currentPlayer.remainingShips.Count) {
									//make the next ship the selected ship
									BattleManager.instance.players[GameData.instance.battleData.currentPlayerIndex].selectedShip = currentPlayer.remainingShips [i + 1];
									currentShip = currentPlayer.remainingShips [i+1];
									//dont check the rest of the ships
									break;
								  //if no next ship on list
								} else {
									//make first ship on list selected ship
									BattleManager.instance.players[GameData.instance.battleData.currentPlayerIndex].selectedShip = currentPlayer.remainingShips [0];
									currentShip = currentPlayer.remainingShips [0];
								//dont check any more ships
									break;
								}

							}
						}
					
				
						//Ste camera to selected ship
						Camera.main.transform.position = new Vector3(currentShip.transform.position.x,Camera.main.transform.position.y,currentShip.transform.position.z);
						//set so ship is moving not turning
						currentPlayer.moving = true;
						//set buttons to initial visability
						undoTurnVisible = false;
						weaponsVisible = false;
						specialOrdersVisible = false;

						//set weapon buttons to inital viability
						weapon1Selected = false;
						weapon2Selected = false;
						weapon3Selected = false;
						weapon4Selected = false;
						weaponsReady = false;
						//clear any weapons chosen to fire
						currentPlayer.selectedWeaponsType = -1;
						currentPlayer.weaponsSelected.Clear ();
						currentPlayer.weaponsAvailable.Clear ();

					}
				}
			}

			#endregion

			#region Movement Phase Main Buttons
			if(currentPlayer.phaze == 0){
				buttonsCount = 4;

				#region Move/Turn Button
				// If not moved enough to turn gray out turn button
				GUI.color = Color.grey;

				if (currentPlayer.moving == true) {
					turnButtonName = "Turn Ship";
				} else {
					turnButtonName = "Move Ship";
				}

						
				//when moved enough turn back to white
				if ((currentShip.movedThisTurn >= currentShip.minTurnDistance && currentShip.turnsRemaining > 0 || currentPlayer.moving == false) && currentShip.moveComplete == false) {
					GUI.color = Color.white;
				}
						

				//button to alow a turn to be made
				if (GUI.Button (new Rect (20, 70, buttonWidth, buttonHeight), turnButtonName)) {
					//if not paused
					if(Time.timeScale != 0){
						//if in move mode
						if (currentPlayer.moving == true) {
						//if moved enough to turn
							if (currentShip.movedThisTurn >= currentShip.minTurnDistance && currentShip.turnsRemaining > 0 && currentShip.moveComplete == false) {

								//set to in turn mode
								currentPlayer.moving = false;

								currentShip.rotated = 0;

								//reduce amount of turns available by 1
								currentShip.turnsRemaining -= 1;
								currentShip.turnsUsed += 1;
								//modify the distance until next turn is alowwed to be made
								currentShip.minTurnDistance = Mathf.RoundToInt (currentShip.movedThisTurn + currentShip.baseMinTurnDistance);
								//record position of turn and angle turned from
								currentShip.turnDistance.Add (currentShip.movedThisTurn);
								currentShip.originalBearing .Add (currentShip.bearing);
								//alow undo turn
								undoTurnVisible = true;

							}
						} else {
							currentPlayer.moving = true;
						}
					}
				}		

				#endregion

				#region Special Orders button
			if (currentPlayer.specialOrdersAlowed && currentShip.specialOrderAllowed && currentShip.movedThisTurn == 0) {
				GUI.color = Color.white;
			} else {
				GUI.color = Color.grey;
			}

			if (GUI.Button (new Rect (20, 100, buttonWidth, buttonHeight), "Special Orders")) {
				if(Time.timeScale != 0 ){
					if (specialOrdersVisible) {
						specialOrdersVisible = false;
					} else if (currentPlayer.specialOrdersAlowed && currentShip.specialOrderAllowed && currentShip.movedThisTurn == 0) {
						specialOrdersVisible = true;
					}
				}
			}
				#endregion

				#region Complete Move Button
				if (currentShip.movedThisTurn < currentShip.minMove) {
					GUI.color = Color.grey;
				} else if(currentShip.moveComplete == false){
					GUI.color = Color.white;
				}else{
					GUI.color = Color.grey;
				}

				if (GUI.Button (new Rect (20, 130, buttonWidth, buttonHeight), "Complete Move")) {
					if(Time.timeScale != 0){
						if (currentShip.movedThisTurn >= currentShip.minMove && currentShip.moveComplete == false) {
							currentShip.moveComplete = true;
							currentPlayer.shipUnMoved -= 1;
							undoTurnVisible = false;

						}
					}
				}
				#endregion
			}
				

			#endregion

			#region End Phase button
			if (currentPlayer.shipUnMoved > 0) {
				GUI.color = Color.grey;
			} else {
				GUI.color = Color.white;
			}

			if (GUI.Button (new Rect (20, 40 + buttonsCount * 30, buttonWidth, buttonHeight), "End Phase")) {
				if(Time.timeScale != 0){
					if (currentPlayer.phaze == 0) {
						for(int i =0; i < currentPlayer.remainingShips.Count; i++){
							currentPlayer.remainingShips[i].forwardVector = new Vector3 ( Mathf.Sin (currentPlayer.remainingShips[i].bearing * Mathf.Deg2Rad),0, Mathf.Cos (currentPlayer.remainingShips[i].bearing * Mathf.Deg2Rad));
						}
						if (currentPlayer.shipUnMoved == 0) {
							BattleManager.instance.NextPhaze ();
						}
					} else if (currentPlayer.phaze == 1) {
						weapon1Selected = false;
						weapon2Selected = false;
						weapon3Selected = false;
						weapon4Selected = false;
						weaponsReady = false;
						currentPlayer.weaponsAvailable.Clear ();
						currentPlayer.weaponsSelected.Clear ();
						currentPlayer.selectedWeaponsType = -1;
						BattleManager.instance.NextPhaze ();
					} else if (currentPlayer.phaze == 2) {
						BattleManager.instance.NextPhaze ();
					} else {
						BattleManager.instance.NextTurn ();
					}
				}
			}


			#endregion
				
			#region Movement Phase Optional Buttons

			#region Undo turn button
			GUI.color = Color.white;

			if (undoTurnVisible && currentShip.movedThisTurn == currentShip.turnDistance [currentShip.turnDistance.Count - 1]) {
				if (GUI.Button (new Rect (buttonWidth + 30, 70, buttonWidth, buttonHeight), "Undo Turn")) {
					if(Time.timeScale != 0){
						currentShip.turnsRemaining += 1;
						currentShip.turnsUsed -= 1;
						currentShip.minTurnDistance -= Mathf.RoundToInt (currentShip.movedThisTurn);
						currentShip.bearing = currentShip.originalBearing [currentShip.originalBearing.Count - 1];
						currentShip.rotated = 0;
						currentShip.originalBearing.RemoveAt (currentShip.originalBearing.Count - 1);
						currentShip.turnDistance.RemoveAt (currentShip.turnDistance.Count - 1);
						currentPlayer.moving = true;
					}
				}
			}

			#endregion

			#region Special Orders

			if (specialOrdersVisible) {

				if (BattleManager.instance.playersLoaded) {
					//when moved enough turn back to white
					if (currentShip.specialOrderAllowed == false || currentPlayer.specialOrdersAlowed == false || currentShip.movedThisTurn > 0) {
						GUI.color = Color.grey;
					} else {
						GUI.color = Color.white;
					}
				}

				if (GUI.Button (new Rect (buttonWidth + 30, 100, buttonWidth, buttonHeight), "All Ahead")) {
					if(Time.timeScale != 0){
						currentShip.AheadFull ();
							specialOrdersVisible = false;
					}
				}
		
				if (GUI.Button (new Rect (buttonWidth + 30, 130, buttonWidth, buttonHeight), "New Heading")) {
					if(Time.timeScale != 0){
						currentShip.ComeToNewHeading ();										
						specialOrdersVisible = false;
					}
				}

				if (GUI.Button (new Rect (buttonWidth + 30, 160, buttonWidth, buttonHeight), "Burn Retros")) {
					if(Time.timeScale != 0){					
						currentShip.BurnRetros ();
						specialOrdersVisible = false;
					}
				}

				if (GUI.Button (new Rect (buttonWidth + 30, 190, buttonWidth, buttonHeight), "Lock On")) {
					if(Time.timeScale != 0){					
						currentShip.LockOn ();
						specialOrdersVisible = false;
					}
				}

				if (GUI.Button (new Rect (buttonWidth + 30, 220, buttonWidth, buttonHeight), "Reload")) {
					if(Time.timeScale != 0){					
						currentShip.Reload ();						
						specialOrdersVisible = false;
					}
				}
				
			}
			#endregion

			#endregion

			#region Shooting Phase Main Buttons
			if(currentPlayer.phaze ==1){

				buttonsCount = 7;

				#region Next Target button
				if(GUI.Button (new Rect(20, 70, buttonWidth, buttonHeight), "Next Target")){
					if(Time.timeScale != 0){
						for (int i = 0; i < currentPlayer.enemyShips.Count; i ++) {
						
							if (currentPlayer.enemyShips [i] == currentShip.target) {
							
								if (i + 1 < currentPlayer.enemyShips.Count) {
									currentShip.target = currentPlayer.enemyShips [i + 1];
									PlayerLog.instance.AddEvent ("Targeting " + currentShip.target.shipName);
									break;
								} else {
									currentShip.target = currentPlayer.enemyShips [0];
									PlayerLog.instance.AddEvent ("Targeting " + currentShip.target.shipName);
									break;
								}
							
							}
						}
						currentPlayer.FindWeaponsAvailable();

						weaponsVisible = false;
						weapon1Selected = false;
						weapon2Selected = false;
						weapon3Selected = false;
						weapon4Selected = false;
						weaponsReady = false;
						currentPlayer.selectedWeaponsType = -1;
						currentPlayer.weaponsSelected.Clear ();
					}
				}
				#endregion

				#region Target Ordnance Button
				if(GUI.Button (new Rect(20, 100, buttonWidth, buttonHeight), "Target Ordnance")){
					if(Time.timeScale != 0){

					}
				}
				#endregion

				#region Select weapons button
				if(currentPlayer.weaponsAvailable.Count == 0){
					GUI.color = Color.grey;
				}else{
					GUI.color = Color.white;
				}

				if(GUI.Button (new Rect(20, 130, buttonWidth, buttonHeight), "Select Weapons")){
					if(Time.timeScale != 0){
						if(weaponsVisible == false){
							if(currentPlayer.weaponsAvailable.Count != 0){
								weaponsVisible = true;
							}
						}else{
							weaponsVisible = false;
						}
					}
				}	

				#endregion

				#region Fire Button

				if(weaponsReady == false){
					GUI.color = Color.grey;
				}else{
					GUI.color = Color.white;
				}
				if(GUI.Button (new Rect(20, 160, buttonWidth, buttonHeight), "Fire")){
					if(Time.timeScale != 0){
						if(weaponsReady){
							currentPlayer.Fire();
						}

						weapon1Selected = false;
						weapon2Selected = false;
						weapon3Selected = false;
						weapon4Selected = false;
						weaponsReady = false;
						currentPlayer.weaponsSelected.Clear ();
						currentPlayer.selectedWeaponsType = -1;
					}
				}	

				#endregion

				#region Fire Nova Cannon
				GUI.color = Color.white;
				if(GUI.Button (new Rect(20, 190, buttonWidth, buttonHeight), "Fire Nova Can.")){
					
					if(Time.timeScale != 0){
					
					}
				}
				#endregion

				#region Launch Ordnance Button
				if(GUI.Button (new Rect(20, 220, buttonWidth, buttonHeight), "Launch Ord.")){
					
					if(Time.timeScale != 0){
					
					}
				}
				#endregion
			}
			#endregion

			#region optional Shooting Phase Buttons

			#region Weapons buttons

			if(weaponsVisible){



				if(currentPlayer.weaponsAvailable.Count >= 1){

					if(weapon1Selected){
						GUI.color = Color.green;
					}else if((currentPlayer.weaponsAvailable[0].type == currentPlayer.selectedWeaponsType || currentPlayer.selectedWeaponsType == -1) && currentPlayer.weaponsAvailable[0].fired == false ){
						GUI.color = Color.white;
					}else{
						GUI.color = Color.grey;
					}

					if(GUI.Button (new Rect(buttonWidth + 30, 130, buttonWidth + 50, buttonHeight), currentPlayer.weaponsAvailable[0].weaponName)){
						if(Time.timeScale != 0){

							if(weapon1Selected){

								weapon1Selected = false;
								currentPlayer.weaponsSelected.Remove(currentPlayer.weaponsAvailable[0]);

								if(weapon1Selected == false && weapon2Selected == false && weapon3Selected == false && weapon4Selected == false){
									currentPlayer.selectedWeaponsType = -1;
									weaponsReady = false;
								}

							}else if((currentPlayer.weaponsAvailable[0].type == currentPlayer.selectedWeaponsType || currentPlayer.selectedWeaponsType == -1) && currentPlayer.weaponsAvailable[0].fired == false){
								weapon1Selected = true;
								weaponsReady = true;
								currentPlayer.selectedWeaponsType = currentPlayer.weaponsAvailable[0].type;
								currentPlayer.weaponsSelected.Add (currentPlayer.weaponsAvailable[0]);
							}
						}
					}
				}



				if(currentPlayer.weaponsAvailable.Count >= 2){

					if(weapon2Selected){
						GUI.color = Color.green;
					}else if((currentPlayer.weaponsAvailable[1].type == currentPlayer.selectedWeaponsType || currentPlayer.selectedWeaponsType == -1) && currentPlayer.weaponsAvailable[1].fired == false){
						GUI.color = Color.white;
					}else{
						GUI.color = Color.grey;
					}

					if(GUI.Button (new Rect(buttonWidth + 30, 160, buttonWidth + 50, buttonHeight), currentPlayer.weaponsAvailable[1].weaponName)){
						if(weapon2Selected){
							
							weapon2Selected = false;
							currentPlayer.weaponsSelected.Remove(currentPlayer.weaponsAvailable[1]);
						
							if(weapon1Selected == false && weapon2Selected == false && weapon3Selected == false && weapon4Selected == false){
								currentPlayer.selectedWeaponsType = -1;
								weaponsReady = false;
							}
							
						}else if((currentPlayer.weaponsAvailable[1].type == currentPlayer.selectedWeaponsType || currentPlayer.selectedWeaponsType == -1) && currentPlayer.weaponsAvailable[1].fired == false){
							weapon2Selected = true;
							weaponsReady = true;
							currentPlayer.weaponsSelected.Add (currentPlayer.weaponsAvailable[1]);
							currentPlayer.selectedWeaponsType = currentPlayer.weaponsAvailable[1].type;
						}
					}
				}

				if(currentPlayer.weaponsAvailable.Count >= 3){
					
					if(weapon3Selected){
						GUI.color = Color.green;
					}else if((currentPlayer.weaponsAvailable[2].type == currentPlayer.selectedWeaponsType || currentPlayer.selectedWeaponsType == -1) && currentPlayer.weaponsAvailable[2].fired == false){
						GUI.color = Color.white;
					}else{
						GUI.color = Color.grey;
					}

					if(GUI.Button (new Rect(buttonWidth + 30, 190, buttonWidth + 50, buttonHeight), currentPlayer.weaponsAvailable[2].weaponName)){
						if(weapon3Selected){
							
							weapon3Selected = false;
							currentPlayer.weaponsSelected.Remove(currentPlayer.weaponsAvailable[2]);
							
							if(weapon1Selected == false && weapon2Selected == false && weapon3Selected == false && weapon4Selected == false){
								currentPlayer.selectedWeaponsType = -1;
								weaponsReady = false;

							}
							
						}else if((currentPlayer.weaponsAvailable[2].type == currentPlayer.selectedWeaponsType || currentPlayer.selectedWeaponsType == -1) && currentPlayer.weaponsAvailable[2].fired == false){
							weapon3Selected = true;
							weaponsReady = true;
							currentPlayer.weaponsSelected.Add (currentPlayer.weaponsAvailable[2]);
							currentPlayer.selectedWeaponsType = currentPlayer.weaponsAvailable[2].type;
						}
					}
				}


				if(currentPlayer.weaponsAvailable.Count >= 4){

					if(weapon4Selected){
						GUI.color = Color.green;
					}else if((currentPlayer.weaponsAvailable[3].type == currentPlayer.selectedWeaponsType || currentPlayer.selectedWeaponsType == -1) && currentPlayer.weaponsAvailable[3].fired == false){
						GUI.color = Color.white;
					}else{
						GUI.color = Color.grey;
					}

					if(GUI.Button (new Rect(buttonWidth + 30, 220, buttonWidth + 50, buttonHeight), currentPlayer.weaponsAvailable[3].weaponName)){
						if(weapon4Selected){
							
							weapon4Selected = false;
							currentPlayer.weaponsSelected.Remove(currentPlayer.weaponsAvailable[3]);
							
							if(weapon1Selected == false && weapon2Selected == false && weapon3Selected == false && weapon4Selected == false){
								currentPlayer.selectedWeaponsType = -1;
								weaponsReady = false;
							}
							
						}else if((currentPlayer.weaponsAvailable[3].type == currentPlayer.selectedWeaponsType || currentPlayer.selectedWeaponsType == -1) && currentPlayer.weaponsAvailable[3].fired == false){
							weapon2Selected = true;
							weaponsReady = true;
							currentPlayer.weaponsSelected.Add (currentPlayer.weaponsAvailable[3]);
							currentPlayer.selectedWeaponsType = currentPlayer.weaponsAvailable[3].type;
						}
					}
				}
				
			}
			#endregion

			#endregion
		}

		#region Pause menu buttons



		if(BattleManager.instance.GetComponent<PauseMenu>().paused ){

			int pButtonWidth = 200;
			int pButtonHeight = 50;
			int pGroupWidth = 200;
			int pGroupHeight = 170;

			GUI.BeginGroup(new Rect(((Screen.width/2) - (pGroupWidth/2)),((Screen.height/2)),pGroupWidth, pGroupHeight));
			
			if(GUI.Button(new Rect(0,0,pButtonWidth,pButtonHeight),"Main Menu")){
				Application.LoadLevel(0);
			}
			if(GUI.Button(new Rect(0,60,pButtonWidth,pButtonHeight),"Restart Level")){
				Application.LoadLevel(Application.loadedLevelName);
			}
			if(GUI.Button(new Rect(0,120,pButtonWidth,pButtonHeight),"Save Game")){
				GameData.instance.Save("save");
			}
			if(GUI.Button(new Rect(0,180,pButtonWidth,pButtonHeight),"Quit Game")){
				Application.Quit();
			}
			GUI.EndGroup();
		}
		#endregion
	}
}
	
