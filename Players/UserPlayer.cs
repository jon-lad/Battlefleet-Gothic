using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UserPlayer : Player {


	// Use this for initialization
	void Start () {
		selectedWeaponsType = -1;
		shipPrefabs = new List<GameObject>();

	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public override void TurnUpdate() {
		base.TurnUpdate();
	
	}

	public override IEnumerator MovementPhaze(){

		while (phaze == 0) {

				if(selectedShip.specialOrder != 1 && !selectedShip.moveComplete ){

					if (moving) {

						if (Input.GetMouseButtonDown (0)) {
							trackMouse = true;
							lastPosition = Input.mousePosition;
				
						}
			
						if (Input.GetMouseButtonUp (0)) {
							trackMouse = false;
							mouseDistance = 0;
						}

						if(trackMouse){
							Vector3 newPosition = Input.mousePosition;
							mouseDistance = (newPosition.y - lastPosition.y)/10;

							if(selectedShip.turnsUsed == 0){

								if(!(selectedShip.movedThisTurn >= selectedShip.maxMove && mouseDistance > 0) && !(selectedShip.movedThisTurn <= 0 && mouseDistance < 0)){
								selectedShip.transform.position += new Vector3 ((mouseDistance) * Mathf.Sin (selectedShip.bearing * Mathf.Deg2Rad)* Time.timeScale,0, (mouseDistance) * Mathf.Cos (selectedShip.bearing * Mathf.Deg2Rad)* Time.timeScale);
									selectedShip.movedThisTurn += mouseDistance * Time.timeScale;
									if(selectedShip.movedThisTurn >= selectedShip.maxMove){
									selectedShip.transform.position += new Vector3 ((selectedShip.maxMove - selectedShip.movedThisTurn) * Mathf.Sin (selectedShip.bearing * Mathf.Deg2Rad) ,0, (selectedShip.maxMove - selectedShip.movedThisTurn) * Mathf.Cos (selectedShip.bearing * Mathf.Deg2Rad));
										selectedShip.movedThisTurn = selectedShip.maxMove;
									}
				
									if(selectedShip.movedThisTurn <= 0){
										selectedShip.transform.position += new Vector3 ((- selectedShip.movedThisTurn  ) * Mathf.Sin (selectedShip.bearing * Mathf.Deg2Rad),0, ( - selectedShip.movedThisTurn) * Mathf.Cos (selectedShip.bearing * Mathf.Deg2Rad));
										selectedShip.movedThisTurn = 0;
									}
								}
							}else{
								if(!(selectedShip.movedThisTurn >= selectedShip.maxMove && mouseDistance > 0) && !(selectedShip.movedThisTurn <= selectedShip.turnDistance[selectedShip.turnDistance.Count -1] && mouseDistance < 0)){
								selectedShip.transform.position += new Vector3 ((mouseDistance) * Mathf.Sin (selectedShip.bearing * Mathf.Deg2Rad) * Time.timeScale,0, (mouseDistance) * Mathf.Cos (selectedShip.bearing * Mathf.Deg2Rad)* Time.timeScale);
									selectedShip.movedThisTurn += mouseDistance;
									if(selectedShip.movedThisTurn >= selectedShip.maxMove){
										selectedShip.transform.position += new Vector3 ((selectedShip.maxMove - selectedShip.movedThisTurn) * Mathf.Sin (selectedShip.bearing * Mathf.Deg2Rad),0, (selectedShip.maxMove - selectedShip.movedThisTurn) * Mathf.Cos (selectedShip.bearing * Mathf.Deg2Rad));
										selectedShip.movedThisTurn = selectedShip.maxMove;
									}
				
									if(selectedShip.movedThisTurn <= selectedShip.turnDistance[selectedShip.turnDistance.Count -1]){
										selectedShip.transform.position += new Vector3 ((selectedShip.turnDistance[selectedShip.turnDistance.Count -1] - selectedShip.movedThisTurn  ) * Mathf.Sin (selectedShip.bearing * Mathf.Deg2Rad),0, (selectedShip.turnDistance[selectedShip.turnDistance.Count -1] - selectedShip.movedThisTurn) * Mathf.Cos (selectedShip.bearing * Mathf.Deg2Rad));
										selectedShip.movedThisTurn = selectedShip.turnDistance[selectedShip.turnDistance.Count -1];
									}
						
								}


							}

							lastPosition = newPosition;
						}
					}else{

						if (Input.GetMouseButtonDown (0)) {
							trackMouse = true;
							lastPosition = Input.mousePosition;
							
						}
						
						if (Input.GetMouseButtonUp (0)) {
							trackMouse = false;
							mouseDistance = 0;
						}

						if(trackMouse){

							Vector3 newPosition = Input.mousePosition;
							mouseDistance = (newPosition.x - lastPosition.x)/2;

							if(!(selectedShip.rotated >= selectedShip.turns && mouseDistance > 0) && !(selectedShip.rotated < -selectedShip.turns && mouseDistance < 0)){

								selectedShip.rotated += mouseDistance * Time.timeScale;
								selectedShip.bearing += mouseDistance * Time.timeScale;

	
							}
							if(selectedShip.rotated >= selectedShip.turns){
								selectedShip.bearing += selectedShip.turns - selectedShip.rotated ;
								selectedShip.rotated = selectedShip.turns;
							}
					
							if(selectedShip.rotated <= -selectedShip.turns){
								selectedShip.bearing += -selectedShip.rotated - selectedShip.turns;
								selectedShip.rotated = - selectedShip.turns;
							}



							lastPosition = newPosition;
						}
					}
				}
				yield return null;
		}
				
		

		
	}

	public override IEnumerator ShootingPhaze(){
		while (phaze == 1) {



			yield return null;
		}
	}

	public override IEnumerator OrdnancePhaze(){
		while (phaze == 2) {
			
			yield return null;
		}
	}
	
	public override IEnumerator EndPhaze(){
		while (phaze == 3) {
			
			yield return null;
		}
	}


}
