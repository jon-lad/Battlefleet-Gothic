using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {


	public Ship selectedShip;
	
	public List<GameObject> shipPrefabs = new List<GameObject>();

	public List<Ship> shipInstances = new List<Ship>();
	public List<Ship> remainingShips = new List<Ship>();
	public int shipUnMoved = 0;
	public List<Ship> shipNotFired = new List<Ship>();


	public LineRenderer line;
	public Transform activeLance;


	public bool specialOrdersAlowed = true;

	//variables used to calculate mouse movements
	public float mouseDistance = 0;
	public Vector3 lastPosition;
	public bool trackMouse = false;

	public int race; //0 imperial, 1 chaos

	public List<Ship> enemyShips = new List<Ship> ();
	public List<Weapon> weaponsAvailable = new List<Weapon> ();
	public List<Weapon> weaponsSelected = new List<Weapon>();
	public int  selectedWeaponsType; //-1 no weapon selected
	// bool if true moving if false turning
	public bool moving = true;
	
	//phazes 0 = movement, 1 = shooting, 2  = ordanance, 3 = end
	public int phaze = 0;

	//at start of turn do this
	void Awake(){

	}

	// Use this for initialization
	void Start () {
		selectedWeaponsType = -1;
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
		for (int i = 0; i < shipInstances.Count; i++) {
						shipInstances [i].Update ();
			}
	}
	//turn sequence
	public virtual void TurnUpdate(){

	}

	//Add ships to map
	public void generateShips(string playerName){
		for (int i= 0; i < shipPrefabs.Count; i++) {
			Ship temp;
			temp = ((GameObject)Instantiate (shipPrefabs[i],new Vector3(((i +1)* BattleManager.instance.deploymentArea.x/(shipPrefabs.Count +1)) + ((BattleManager.instance.worldSize.x - BattleManager.instance.deploymentArea.x)/2) , 1.5f , 0 ), Quaternion.Euler(new Vector3(90,0,0)))).GetComponent<Ship>();
			temp.transform.parent = transform;
			if(race == 0){
				int die = Random.Range(0, 97);
				temp.shipName =  GameData.instance.imperialShipNames[die];
			}else if (race == 1){
				int die = Random.Range(0,55);
				temp.shipName =  GameData.instance.chaosShipNames[die];
			}
			shipInstances.Add(temp);

			for(int j =0; j < GameData.instance.Players.Count;j++){
				if (GameData.instance.Players[j].playerName == playerName){
					GameData.instance.Players[j].activeShipsPrefabs.Add(shipPrefabs[i]);
				}
			}

			remainingShips.Add(temp);



		}
		selectedShip = shipInstances [0];
	}
	// 
	public virtual IEnumerator MovementPhaze(){
		yield return null;
	}

	public virtual IEnumerator ShootingPhaze(){

		yield return null;
	}

	public virtual IEnumerator OrdnancePhaze(){
		yield return null;
	}

	public virtual IEnumerator EndPhaze(){
		yield return null;
	}

	public void FindWeaponsAvailable(){

		float distanceToTarget = Mathf.Sqrt((selectedShip.transform.position.x - selectedShip.target.transform.position.x) * (selectedShip.transform.position.x - selectedShip.target.transform.position.x) + (selectedShip.transform.position.z - selectedShip.target.transform.position.z) * (selectedShip.transform.position.z - selectedShip.target.transform.position.z));

		float finalAngle = findAngleFromShip ();

		weaponsAvailable.Clear ();


		for (int i = 0; i < selectedShip.weapons.Count; i++) {
			if(distanceToTarget <= selectedShip.weapons[i].range &&(finalAngle <= selectedShip.weapons[i].maxFireArc && finalAngle >= selectedShip.weapons[i].minFireArc )){

				weaponsAvailable.Add(selectedShip.weapons[i]);

			}
		}

	}

	public void Fire(){

		Vector3 attackDirection =  selectedShip.transform.position - selectedShip.target.transform.position;

		int enemySideHit = selectedShip.target.SideHit (attackDirection);
		float enemyRange = Mathf.Sqrt ((selectedShip.target.transform.position.x - selectedShip.transform.position.x) * (selectedShip.target.transform.position.x - selectedShip.transform.position.x) + (selectedShip.target.transform.position.z - selectedShip.transform.position.z) * (selectedShip.target.transform.position.z - selectedShip.transform.position.z));

		if (selectedWeaponsType == 0) {

			float attackAngle = findAngleFromShip();
			if(attackAngle <= -135 || attackAngle > 135){

				activeLance = selectedShip.transform.FindChild ("Stern Lance");
				line = activeLance.GetComponent<LineRenderer>();
			}
			else if(attackAngle > -135 && attackAngle <= -45){
				activeLance = selectedShip.transform.FindChild ("Port Lance");
				line = activeLance.GetComponent<LineRenderer>();
			}
			else if(attackAngle > -45 && attackAngle <= 45){
				activeLance = selectedShip.transform.FindChild ("Prow Lance");
				line = activeLance.GetComponent<LineRenderer>();
			}
			else if(attackAngle > 45 && attackAngle <= 135){
				activeLance = selectedShip.transform.FindChild ("Starboard Lance");
				line = activeLance.GetComponent<LineRenderer>();
			}


			StartCoroutine("ShootLance");

			int attackStrength = 0;
			List<int> dieRolls = new List<int> ();
			string dieRollsString = "";
			PlayerLog.instance.AddEvent ("Firing Lance(s)");
			for (int i = 0; i < weaponsSelected.Count; i++) {
				attackStrength += Mathf.CeilToInt (selectedShip.fireStrength * weaponsSelected [i].strength);
				weaponsSelected [i].fired = true;
			}
			PlayerLog.instance.AddEvent ("Strength " + attackStrength);

			for (int i = 0; i < attackStrength; i ++) {
				dieRolls.Add (Random.Range (1, 7));
					dieRollsString += dieRolls [i] + ", ";
				}
				dieRollsString = dieRollsString.Substring (0, dieRollsString.Length - 2);
				PlayerLog.instance.AddEvent ("Rolled (" + dieRollsString + ")");
		
				selectedShip.target.TakeHits (dieRolls, 0, enemySideHit);

				} else if (selectedWeaponsType == 1) {
		
				int attackStrength = 0;
				List<int> dieRolls = new List<int> ();
				string dieRollsString = "";
				PlayerLog.instance.AddEvent ("Firing Weapon(s)");

				for (int i = 0; i < weaponsSelected.Count; i++) {
					attackStrength += Mathf.CeilToInt (selectedShip.fireStrength * weaponsSelected [i].strength);
					weaponsSelected [i].fired = true;
				}

				PlayerLog.instance.AddEvent ("Strength " + attackStrength);

				int attackColumn = selectedShip.target.shipType;

				if (selectedShip.target.shipType == 1 || selectedShip.target.shipType == 2) {
					if (enemySideHit == 0) {
					PlayerLog.instance.AddEvent ("Target Closing");
				}
				if (enemySideHit == 3) {
					PlayerLog.instance.AddEvent ("Target Moving Away");
					attackColumn += 1;
				}
				if (enemySideHit == 1 || enemySideHit == 2) {
					PlayerLog.instance.AddEvent ("Target Abeam");
					attackColumn += 2;
				}
				if (selectedShip.target.movedThisTurn < 5) {
					attackColumn = 0;
					PlayerLog.instance.AddEvent ("(Target counts as Defence");
				}
			}
			if (enemyRange < 15) {
				attackColumn -= 1;
				PlayerLog.instance.AddEvent ("Target Close Range");
			}
			if (enemyRange > 30) {
				attackColumn += 1;
				PlayerLog.instance.AddEvent ("Target Long Range");
			}
			if (attackColumn < 0) {
				attackColumn = 0;
			}
			if (attackColumn > 4) {
				attackColumn = 4;
			}

			if (BattleManager.instance.gunneryTable [attackColumn, (attackStrength - 1)] != 0) {
				if (attackStrength <= 20) {
					for (int i = 0; i < BattleManager.instance.gunneryTable[attackColumn, (attackStrength - 1)]; i++) {
						dieRolls.Add (Random.Range (1, 7)); 
						dieRollsString += dieRolls [i] + ", ";
					}
				} else {
					for (int i = 0; i < BattleManager.instance.gunneryTable[attackColumn, 19]; i++) {
						dieRolls.Add (Random.Range (1, 7));  
						dieRollsString += dieRolls [i] + ", ";
					}
					for (int i = 0; i < BattleManager.instance.gunneryTable[attackColumn, (attackStrength - 21)]; i++) {
						dieRolls.Add (Random.Range (1, 7));   
						dieRollsString += dieRolls [i] + ", ";
					}
				}
				dieRollsString = dieRollsString.Substring (0, dieRollsString.Length - 2);
				PlayerLog.instance.AddEvent ("Rolled (" + dieRollsString + ")");

				selectedShip.target.TakeHits (dieRolls, 1, enemySideHit);
			} else {
				PlayerLog.instance.AddEvent("Unable to Hit target");
			}
	
		}
	}
	float findAngleFromShip(){

		Vector3 targetDir = selectedShip.target.transform.position - selectedShip.transform.position;
		
		Vector3 referenceForward = selectedShip.forwardVector;/* some vector that is not Vector3.up */
		
		// the vector perpendicular to referenceForward (90 degrees clockwise)
		// (used to determine if angle is positive or negative)
		Vector3 referenceRight= Vector3.Cross(Vector3.up, referenceForward);
		
		// the vector of interest
		Vector3 newDirection = targetDir; /* some vector that we're interested in */
		
		// Get the angle in degrees between 0 and 180
		float angle = Vector3.Angle(newDirection, referenceForward);
		
		// Determine if the degree value should be negative. Here, a positive value
		// from the dot product means that our vector is on the right of the reference vector
		// whereas a negative value means we're on the left.
		float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
		
		return sign * angle;
	}

	IEnumerator ShootLance(){
		line.enabled = true;

		for(float i = 5f ; i >= 0; i -= 0.1f){

			line.SetPosition(0, activeLance.transform.position);
			line.SetPosition(1, selectedShip.target.transform.position);

			yield return null;
		}

			line.enabled = false;
		
	}
		

}
