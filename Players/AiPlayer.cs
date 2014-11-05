using UnityEngine;
using System.Collections;

public class AIPlayer : Player {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public  override void TurnUpdate() {
		base.TurnUpdate ();
	}


	public override IEnumerator MovementPhaze(){
		while(phaze == 0){

			// AI moves Ships here
			yield return null;
		}
	}
	public override IEnumerator ShootingPhaze(){
		while (phaze == 1) {
			//AI shoots Here
			FindWeaponsAvailable();
			yield return null;
		}
	}
}
