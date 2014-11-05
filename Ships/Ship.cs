using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour {

	#region variables

	//Ship Stats
	public string shipName; //Name of ship
	public int hits;  // total Number of Hp
	public int speed; // Base movement rate of ship
	public int turns; // Base number of turns a ship can make
	public int shields;	//Total number of shields
	public int armour;	//Armour rating for Ship
	public int turrets; //Number Of turrets on ship
	public int leadership; //leadership value for ship 
	public int baseMinTurnDistance; // base minimum distance until a turn is allowed
	public float minMove; // minimum movement a ship has to make
	public float bearing; // direction ship is facing
	public int shipType; // class of ship 0 - capital ship, 1 - escort; 
	public bool smallBase = true;
	protected GameObject stand; // the stand object for the ship
	public List<Weapon> weapons = new List<Weapon>(); // list of weapons for the ship


	//modifiable ship Stats
	public float maxMove; // current maximum move this turn (modified Speed)
	public float fireStrength; // current strength of weapons 1- normal(used for secial orders)
	public int turnsRemaining; //how many ship-turns are available this turn
	public int turnsUsed; //how many times a ship has turned this turn
	public int minTurnDistance; // current minimum turn distance(used for Burn retros)


	//special order variables
	public bool specialOrderAllowed = true; //(has the ship made a special order)
	public int specialOrder; //the special order the ship is on 0 - none, 1- Ahead full 2-new heading 3 - burn retros 4- lock on 5 - reload 6 brace
	public int orderCounter; //

	//status Variables
	public bool onFire = false; //Is ship on fire
	public bool crippled = false; //is ship crippled
	public bool moveComplete = false; //has the ship finished its move

	//record variables
	public float movedThisTurn; // how far the ship has moved this turn
	public int remainingHits; // remaining HP
	public int activeShields; // number of shields active
	public float rotated; // amount ship has turned
	public List<float> originalBearing; // directions ship was facing before turns
	public List<float> turnDistance; //the distance the ship made the turn at
	public Vector3 forwardVector; // a unit vector in the direction the ship is facing
	public Ship target; // the ship that is targeted

	#endregion

	// Use this for initialization
	public virtual void Start () {

		//set random leadership at start of battle
		int die = Random.Range( 1, 7);
		if (die == 1) {
			leadership = 6;
		} else if (die == 2 || die == 3) {
			leadership = 7;
		} else if (die == 4 || die == 5) {
			leadership = 8;
		} else {
			leadership = 9;
		}

		//initalize amount ship can turn
		turnsUsed = 0;
		turnsRemaining = 1;
		// initalize special orders
		specialOrder = 0;
	
		//initalize movement
		movedThisTurn = 0;
		rotated = 0;



	}

	// Update is called once per frame
	public virtual void Update () {

		//change the facing of the ship to mstch the bearing
		this.transform.rotation = Quaternion.Euler(new Vector3(90, bearing,0));


	}

	void OnMouseDown(){

		int playerNumber = GameData.instance.battleData.currentPlayerIndex;
		//if the ship is till alive make ship the selected ship
		if (BattleManager.instance.players [playerNumber].remainingShips.Contains (this)) {
			BattleManager.instance.players [playerNumber].selectedShip = this;
			BattleManager.instance.players [playerNumber].moving = true;
		}
	}

	#region Special Orders

	// function to apply the ahead full special order
	 public virtual void AheadFull(){
		//leadership may be +1 if enemy ships are on special orders
		int modifiedLeadership = leadership;

		for(int i = 0; i < BattleManager.instance.players.Count; i++){//goes through each player
			if(i != GameData.instance.battleData.currentPlayerIndex){// if the player is not the current player
				for(int j = 0; j < BattleManager.instance.players[i].remainingShips.Count; j++){//go through the players ships
					//if one ship is on special orders
					if(BattleManager.instance.players[i].remainingShips[j].specialOrder != 0 && leadership < 10){
						//leadership is modified
						modifiedLeadership = leadership +1;
					}
				}
			}
		}

		//if special order are allowed for the player and ship and he ship hasnt moved
		if(BattleManager.instance.players[GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed && specialOrderAllowed && movedThisTurn == 0){
			//roll 2 dice
			int die1 =  Random.Range(1, 7);
			int die2 =  Random.Range(1, 7);
			int total = die1 + die2;
			//let player know out come of dice rolls
			PlayerLog.instance.AddEvent("Atempting All Ahead Full Special Order");
			PlayerLog.instance.AddEvent("Need a roll of " + modifiedLeadership + " or under to succeed.");
			PlayerLog.instance.AddEvent("Rolled " + (total) + " (" + die1 +", " + die2 + ")");
			//if roll less than leadership
			if(total <= modifiedLeadership){
				//let player know
				PlayerLog.instance.AddEvent("Sucess!");
				//strength of weapons is halved
				fireStrength = 0.5f;
				//ship cannot make any more special orders
				specialOrderAllowed = false;
				//roll dice to see how far extra the ship can move
				int die3 =  Random.Range(1, 7);
				int die4 =  Random.Range(1, 7);
				int die5 =  Random.Range(1, 7);
				int die6 =  Random.Range(1, 7);
				int moveTotal = die3 + die4 + die5 + die6;
				//inform player of extra move distance
				PlayerLog.instance.AddEvent("Rolling for extra movement");
				PlayerLog.instance.AddEvent("Rolled (" +die3 + ", " +die4 + ", " + die5 + ", " + die6 + ")");
				PlayerLog.instance.AddEvent(shipName + " Moved " + moveTotal + " extra cm");
				//set special order to ahead full
				specialOrder = 1 ;
				transform.position += new Vector3 ((speed + moveTotal) * Mathf.Sin (bearing * Mathf.Deg2Rad),0, (speed + moveTotal) * Mathf.Cos (bearing * Mathf.Deg2Rad)); 
				movedThisTurn = speed + moveTotal;

				moveComplete = true;
				BattleManager.instance.players[GameData.instance.battleData.currentPlayerIndex].shipUnMoved -= 1;
			
			}else{				
				PlayerLog.instance.AddEvent("Failure can't use special orders until next turn");
				BattleManager.instance.players[GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed = false;
			}
		}
	}

	public void ComeToNewHeading(){

		int modifiedLeadership = leadership;

		for(int i = 0; i < BattleManager.instance.players.Count; i++){
			if(i != GameData.instance.battleData.currentPlayerIndex){
				for(int j = 0; j < BattleManager.instance.players[i].remainingShips.Count; j++){
					if(BattleManager.instance.players[i].remainingShips[j].specialOrder != 0 && leadership < 10){
						modifiedLeadership = leadership +1;
					}
				}
			}
		}
		
		
		if (BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed && specialOrderAllowed && movedThisTurn == 0) {
			int die1 =  Random.Range(1, 7);
			int die2 =  Random.Range(1, 7);
			int total = die1 + die2;
			PlayerLog.instance.AddEvent("Atempting Come To New Heading Special Order");
			PlayerLog.instance.AddEvent("Need a roll of " + modifiedLeadership + " or under to succeed.");
			PlayerLog.instance.AddEvent("Rolled " + (total) + " (" + die1 +", " + die2 + ")");

			
			if(total <= modifiedLeadership){
				PlayerLog.instance.AddEvent("Sucess!");
				fireStrength = 0.5f;
				specialOrderAllowed = false;
				specialOrder = 2;
				turnsRemaining = 2;
			}else{				
				PlayerLog.instance.AddEvent("Failure can't use special orders until next turn");
				BattleManager.instance.players[GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed = false;
			}
		}
	}

	public void BurnRetros(){
		int modifiedLeadership = leadership;
		
		for (int i = 0; i < BattleManager.instance.players.Count; i++) {
				if (i != GameData.instance.battleData.currentPlayerIndex) {
						for (int j = 0; j < BattleManager.instance.players[i].remainingShips.Count; j++) {
								if (BattleManager.instance.players [i].remainingShips [j].specialOrder != 0 && leadership < 10) {
										modifiedLeadership = leadership + 1;
								}
						}
				}
		}
		
		
		if (BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed && specialOrderAllowed && movedThisTurn == 0) {
			int die1 = Random.Range (1, 7);
			int die2 = Random.Range (1, 7);
			int total = die1 + die2;
			PlayerLog.instance.AddEvent ("Atempting Burn Retros Special Order");
			PlayerLog.instance.AddEvent ("Need a roll of " + modifiedLeadership + " or under to succeed.");
			PlayerLog.instance.AddEvent ("Rolled " + (total) + " (" + die1 + ", " + die2 + ")");
			
			
			if (total <= modifiedLeadership) {
				PlayerLog.instance.AddEvent ("Sucess!");
				fireStrength = 0.5f;
				specialOrderAllowed = false;
				specialOrder = 3;
				maxMove = speed/2;
				minMove = 0;
				minTurnDistance = 0;


			} else {				
				PlayerLog.instance.AddEvent ("Failure can't use special orders until next turn");
				BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed = false;
			}
		
		}
	}

	public void LockOn(){
		int modifiedLeadership = leadership;
		
		for (int i = 0; i < BattleManager.instance.players.Count; i++) {
			if (i != GameData.instance.battleData.currentPlayerIndex) {
				for (int j = 0; j < BattleManager.instance.players[i].remainingShips.Count; j++) {
					if (BattleManager.instance.players [i].remainingShips [j].specialOrder != 0 && leadership < 10) {
						modifiedLeadership = leadership + 1;
					}
				}
			}
		}
		
		
		if (BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed && specialOrderAllowed && movedThisTurn == 0) {
			int die1 = Random.Range (1, 7);
			int die2 = Random.Range (1, 7);
			int total = die1 + die2;
			PlayerLog.instance.AddEvent ("Atempting Lock On Special Order");
			PlayerLog.instance.AddEvent ("Need a roll of " + modifiedLeadership + " or under to succeed.");
			PlayerLog.instance.AddEvent ("Rolled " + (total) + " (" + die1 + ", " + die2 + ")");
			
			
			if (total <= modifiedLeadership) {
				PlayerLog.instance.AddEvent ("Sucess!");
				specialOrderAllowed = false;
				specialOrder = 4;
				turnsRemaining = 0;					
			} else {				
				PlayerLog.instance.AddEvent ("Failure can't use special orders until next turn");
				BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed = false;
			}
			
		}
	}

	public void Reload(){
		int modifiedLeadership = leadership;
		
		for (int i = 0; i < BattleManager.instance.players.Count; i++) {
			if (i != GameData.instance.battleData.currentPlayerIndex) {
				for (int j = 0; j < BattleManager.instance.players[i].remainingShips.Count; j++) {
					if (BattleManager.instance.players [i].remainingShips [j].specialOrder != 0 && leadership < 10) {
						modifiedLeadership = leadership + 1;
					}
				}
			}
		}
		
		
		if (BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed && specialOrderAllowed && movedThisTurn == 0) {
			int die1 = Random.Range (1, 7);
			int die2 = Random.Range (1, 7);
			int total = die1 + die2;
			PlayerLog.instance.AddEvent ("Atempting Reload Ordnance Special Order");
			PlayerLog.instance.AddEvent ("Need a roll of " + modifiedLeadership + " or under to succeed.");
			PlayerLog.instance.AddEvent ("Rolled " + (total) + " (" + die1 + ", " + die2 + ")");
			
			
			if (total <= modifiedLeadership) {
				PlayerLog.instance.AddEvent ("Sucess!");
				specialOrderAllowed = false;
				specialOrder = 5;
				
			} else {				
				PlayerLog.instance.AddEvent ("Failure can't use special orders until next turn");
				BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed = false;
			}
			
		}
	}

	public void Brace(){
		int modifiedLeadership = leadership;
		
		for (int i = 0; i < BattleManager.instance.players.Count; i++) {
			if (i != GameData.instance.battleData.currentPlayerIndex) {
				for (int j = 0; j < BattleManager.instance.players[i].remainingShips.Count; j++) {
					if (BattleManager.instance.players [i].remainingShips [j].specialOrder != 0 && leadership < 10) {
						modifiedLeadership = leadership + 1;
					}
				}
			}
		}
		

		int die1 = Random.Range (1, 7);
		int die2 = Random.Range (1, 7);
		int total = die1 + die2;
		PlayerLog.instance.AddEvent ("Atempting Brace for Impact Special Order");
		PlayerLog.instance.AddEvent ("Need a roll of " + modifiedLeadership + " or under to succeed.");
		PlayerLog.instance.AddEvent ("Rolled " + (total) + " (" + die1 + ", " + die2 + ")");
			
			
		if (total <= modifiedLeadership) {
			PlayerLog.instance.AddEvent ("Sucess!");
			fireStrength = 0.5f;
			maxMove = speed/2;

			specialOrderAllowed = false;
			specialOrder = 6;

				
		} else {				
			PlayerLog.instance.AddEvent ("Failure can't use special orders until next turn");
			BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed = false;
		}
			

	}

	#endregion

   public virtual void TakeHits(List<int> dieRolls, int type, int side){

		int damage = 0;
		string criticalRolls = "";
		int criticals = 0;

		#region Lance hit
		if (type == 0) {
			for (int i =0; i < dieRolls.Count; i++) {
				if (dieRolls [i] >= 4) {
					damage += 1;
				}
			}
			#region Shields absorbtion
			int shieldCount = 0;
			int startDamage = damage;
			for(int i = 0 ; i < startDamage; i++){
				if(activeShields > 0){
					damage -= 1;
					activeShields -= 1;
					shieldCount += 1;
				}
			}
			Vector3 blastDistance = new Vector3();
			for(int i = 0; i < shieldCount; i++){ 
				if(smallBase == true){
					if(side == 0){
						blastDistance = new Vector3((float)( transform.position.x + (1.48 * Mathf.Sin ((bearing + (i * 30))*Mathf.Deg2Rad))) , 0 ,(float) (transform.position.z + (1.5 * Mathf.Cos(bearing + (i * 30) * Mathf.Deg2Rad))));
					}
					if(side == 1){
						blastDistance = new Vector3( (float)(transform.position.x + (1.48 * Mathf.Sin ((bearing - 90 + (i * 30)) * Mathf.Deg2Rad))), 0 , (float)(transform.position.z + (1.5 * Mathf.Cos((bearing - 90 + (i * 30))* Mathf.Deg2Rad))));
					}
					if(side == 2){
						blastDistance = new Vector3( (float)(transform.position.x + (1.48 * Mathf.Sin ((bearing + 90 + (i * 30)) * Mathf.Deg2Rad))) , 0 , (float)(transform.position.z + (1.5 * Mathf.Cos((bearing + 90 + (i * 30))* Mathf.Deg2Rad))));
					}
					if(side == 3){
						blastDistance = new Vector3( (float)(transform.position.x + (1.5 * Mathf.Sin ((bearing - 180 + (i * 30)) * Mathf.Deg2Rad))) , 0 ,(float)( transform.position.z + (1.5 * Mathf.Cos((bearing - 180 + (i * 30))* Mathf.Deg2Rad))));
					}
				}else{
					if(side == 0){
						blastDistance = new Vector3((float) (transform.position.x + (3 * Mathf.Sin ((bearing + (i *30)) * Mathf.Deg2Rad))) , 0 , (float)(transform.position.z + (3 * Mathf.Cos(bearing*(i *30)* Mathf.Deg2Rad))));
					}
					if(side == 1){
						blastDistance = new Vector3((float) (transform.position.x + (3 * Mathf.Sin ((bearing - 90 + (i * 30)) * Mathf.Deg2Rad))) , 0 ,(float) (transform.position.z + (3 * Mathf.Cos((bearing -90 + (i * 30))* Mathf.Deg2Rad))));
					}
					if(side == 2){
						blastDistance = new Vector3( (float)(transform.position.x + (3 * Mathf.Sin ((bearing + 90 + (i * 30)) * Mathf.Deg2Rad))) , 0 ,(float) (transform.position.z + (3 * Mathf.Cos((bearing + 90 + (i * 30))* Mathf.Deg2Rad))));
					}
					if(side == 3){
						blastDistance = new Vector3((float) (transform.position.x + (3 * Mathf.Sin ((bearing - 180 + (i * 30)) * Mathf.Deg2Rad))) , 0 , (float)(transform.position.z + (3 * Mathf.Cos((bearing - 180 + (i * 30))* Mathf.Deg2Rad))));
					}
				}
			}
			
			Instantiate(GameData.instance.blastmakerPrefab, blastDistance, new Quaternion());
			
			
			if(shieldCount > 0){
				PlayerLog.instance.AddEvent ("Shields Absored " + shieldCount + " Damage");
			}
			#endregion

		remainingHits -= damage;
		PlayerLog.instance.AddEvent ("Scored " + damage + " Damage!");
			dieRolls.Clear ();
			#region criticals
			if(damage > 0){
				PlayerLog.instance.AddEvent("Rolling for criticals");
				
				for(int i =0; i < damage; i++){
					dieRolls.Add(Random.Range (1, 7));
					criticalRolls += dieRolls[i] + ", ";
				}
				
				criticalRolls = criticalRolls.Substring (0, criticalRolls.Length - 2);
				PlayerLog.instance.AddEvent("Rolled (" + criticalRolls + ")");
				for(int i =0; i < dieRolls.Count; i++){
					if(dieRolls[i] == 6){
						criticals += 1;
					}
				}
				
				PlayerLog.instance.AddEvent("Caused " + criticals + " Critical Hits");

				criticalRolls = "";
				dieRolls.Clear ();

				#region critical Damage

				List<int> dieRoll1 = new List<int>();

				if(criticals > 0){
					
					for(int i = 0; i < criticals; i++){
						dieRolls.Add(Random.Range (1, 7));
						dieRoll1.Add(Random.Range (1, 7));
						criticalRolls += "(" + dieRolls[i] + " ," + dieRoll1[i] + " ), ";
					}
					
					PlayerLog.instance.AddEvent("Rolling For Critical Damage");
					
					for(int i =0; i < dieRolls.Count; i++){
						PlayerLog.instance.AddEvent("Rolled " + criticalRolls.Substring (0,criticalRolls.Length - 2));
						if(dieRolls[i]+ dieRoll1[i] == 2){
							PlayerLog.instance.AddEvent("Dorsal Armament Damaged!");
							
						}if(dieRolls[i]+ dieRoll1[i] == 3){
							PlayerLog.instance.AddEvent("Starboard Armament Damaged!");
							
						}if(dieRolls[i]+ dieRoll1[i] == 4){
							PlayerLog.instance.AddEvent("Port Armament Damaged!");
							
						}if(dieRolls[i]+ dieRoll1[i] == 5){
							PlayerLog.instance.AddEvent("Prow Armament Damaged!");
						}if(dieRolls[i]+ dieRoll1[i] == 6){
							PlayerLog.instance.AddEvent("Engine Room Damaged!");
							
						}if(dieRolls[i]+ dieRoll1[i] == 7){
							PlayerLog.instance.AddEvent("Fire!");
							onFire = true;
							
						}if(dieRolls[i]+ dieRoll1[i] == 8){
							PlayerLog.instance.AddEvent("Thrusters Damaged!");
							
						}if(dieRolls[i]+ dieRoll1[i] == 9){
							PlayerLog.instance.AddEvent("Bridge Smashed!");
							leadership -= 3;
						}if(dieRolls[i]+ dieRoll1[i] == 10){
							PlayerLog.instance.AddEvent("Shields Collapse!");
							activeShields = 0;
							shields = 0;
							
						}if(dieRolls[i]+ dieRoll1[i] == 11){
							PlayerLog.instance.AddEvent("Hull breach!"); 
							
							
						}if(dieRolls[i]+ dieRoll1[i] == 12){
							PlayerLog.instance.AddEvent("Bulkheads Collapse!");
						}
					}
				}
				#endregion
			}
			#endregion
		}
		#endregion

		#region Weapon hit
		if (type == 1) {

			for (int i =0; i < dieRolls.Count; i++) {
				if (dieRolls [i] >= armour) {
					damage += 1;
				}
			}

			#region lock on special order
			if (BattleManager.instance.players [GameData.instance.battleData.currentPlayerIndex].selectedShip.specialOrder == 4) {

				PlayerLog.instance.AddEvent ("Re-Rolling Misses");
				int misses = dieRolls.Count - damage;
				string reRollString = "";
				dieRolls.Clear ();

				for (int i =0; i < misses; i++) {
					dieRolls.Add (Random.Range (1, 7));
					reRollString += dieRolls [i] + ", ";
				}
				reRollString = reRollString.Substring (0, reRollString.Length - 2);
				PlayerLog.instance.AddEvent ("Rolled (" + reRollString + ")");

				for (int i =0; i < dieRolls.Count; i++) {
					if (dieRolls [i] >= armour) {
						damage += 1;
					}
				}
			}
			#endregion

			#region Shields absorbtion
			int shieldCount = 0;
			int startDamage = damage;
			for(int i = 0 ; i < startDamage; i++){
				if(activeShields > 0){
					damage -= 1;
					activeShields -= 1;
					shieldCount += 1;
				}
			}
			Vector3 blastDistance = new Vector3();
				for(int i = 0; i < shieldCount; i++){ 
					if(smallBase == true){
						if(side == 0){
							blastDistance = new Vector3((float)( transform.position.x + (1.48 * Mathf.Sin ((bearing + (i * 30))*Mathf.Deg2Rad))) , 0 ,(float) (transform.position.z + (1.5 * Mathf.Cos(bearing + (i * 30) * Mathf.Deg2Rad))));
						}
						if(side == 1){
							blastDistance = new Vector3( (float)(transform.position.x + (1.48 * Mathf.Sin ((bearing - 90 + (i * 30)) * Mathf.Deg2Rad))), 0 , (float)(transform.position.z + (1.5 * Mathf.Cos((bearing - 90 + (i * 30))* Mathf.Deg2Rad))));
						}
						if(side == 2){
							blastDistance = new Vector3( (float)(transform.position.x + (1.48 * Mathf.Sin ((bearing + 90 + (i * 30)) * Mathf.Deg2Rad))) , 0 , (float)(transform.position.z + (1.5 * Mathf.Cos((bearing + 90 + (i * 30))* Mathf.Deg2Rad))));
						}
						if(side == 3){
							blastDistance = new Vector3( (float)(transform.position.x + (1.5 * Mathf.Sin ((bearing - 180 + (i * 30)) * Mathf.Deg2Rad))) , 0 ,(float)( transform.position.z + (1.5 * Mathf.Cos((bearing - 180 + (i * 30))* Mathf.Deg2Rad))));
						}
					}else{
						if(side == 0){
							blastDistance = new Vector3((float) (transform.position.x + (3 * Mathf.Sin ((bearing + (i *30)) * Mathf.Deg2Rad))) , 0 , (float)(transform.position.z + (3 * Mathf.Cos(bearing*(i *30)* Mathf.Deg2Rad))));
						}
						if(side == 1){
							blastDistance = new Vector3((float) (transform.position.x + (3 * Mathf.Sin ((bearing - 90 + (i * 30)) * Mathf.Deg2Rad))) , 0 ,(float) (transform.position.z + (3 * Mathf.Cos((bearing -90 + (i * 30))* Mathf.Deg2Rad))));
						}
						if(side == 2){
							blastDistance = new Vector3( (float)(transform.position.x + (3 * Mathf.Sin ((bearing + 90 + (i * 30)) * Mathf.Deg2Rad))) , 0 ,(float) (transform.position.z + (3 * Mathf.Cos((bearing + 90 + (i * 30))* Mathf.Deg2Rad))));
						}
						if(side == 3){
							blastDistance = new Vector3((float) (transform.position.x + (3 * Mathf.Sin ((bearing - 180 + (i * 30)) * Mathf.Deg2Rad))) , 0 , (float)(transform.position.z + (3 * Mathf.Cos((bearing - 180 + (i * 30))* Mathf.Deg2Rad))));
						}
					}
				}
					
					Instantiate(GameData.instance.blastmakerPrefab, blastDistance, new Quaternion());
					

			if(shieldCount > 0){
				PlayerLog.instance.AddEvent ("Shields Absored " + shieldCount + " Damage");
			}

			#endregion


			remainingHits -= damage;

			PlayerLog.instance.AddEvent ("Scored " + damage + " Damage!");

			dieRolls.Clear ();

			#region criticals
			if(damage > 0){
				PlayerLog.instance.AddEvent("Rolling for criticals");
				
			for(int i =0; i < damage; i++){
				dieRolls.Add(Random.Range (1, 7));
				criticalRolls += dieRolls[i] + ", ";
			}
				
			criticalRolls = criticalRolls.Substring (0, criticalRolls.Length - 2);
			PlayerLog.instance.AddEvent("Rolled (" + criticalRolls + ")");
			for(int i =0; i < dieRolls.Count; i++){
				if(dieRolls[i] == 6){
					criticals += 1;
				}
			}
			
			PlayerLog.instance.AddEvent("Caused " + criticals + " Critical Hits");
				
			criticalRolls = "";
			dieRolls.Clear ();

			#region critical Damage
				
			List<int> dieRoll1 = new List<int>();
				
			if(criticals > 0){
					
				for(int i = 0; i < criticals; i++){
					dieRolls.Add(Random.Range (1, 7));
					dieRoll1.Add(Random.Range (1, 7));
					criticalRolls += "(" + dieRolls[i] + " ," + dieRoll1[i] + " ), ";
				}
					
				PlayerLog.instance.AddEvent("Rolling For Critical Damage");
					
				for(int i =0; i < dieRolls.Count; i++){
					PlayerLog.instance.AddEvent("Rolled " + criticalRolls.Substring (0,criticalRolls.Length - 2));
					if(dieRolls[i]+ dieRoll1[i] == 2){
						PlayerLog.instance.AddEvent("Dorsal Armament Damaged!");
							
					}if(dieRolls[i]+ dieRoll1[i] == 3){
						PlayerLog.instance.AddEvent("Starboard Armament Damaged!");
							
					}if(dieRolls[i]+ dieRoll1[i] == 4){
						PlayerLog.instance.AddEvent("Port Armament Damaged!");
							
					}if(dieRolls[i]+ dieRoll1[i] == 5){
						PlayerLog.instance.AddEvent("Prow Armament Damaged!");
					}if(dieRolls[i]+ dieRoll1[i] == 6){
						PlayerLog.instance.AddEvent("Engine Room Damaged!");
							
					}if(dieRolls[i]+ dieRoll1[i] == 7){
						PlayerLog.instance.AddEvent("Fire!");
						onFire = true;
							
					}if(dieRolls[i]+ dieRoll1[i] == 8){
						PlayerLog.instance.AddEvent("Thrusters Damaged!");
							
					}if(dieRolls[i]+ dieRoll1[i] == 9){
						PlayerLog.instance.AddEvent("Bridge Smashed!");
						leadership -= 3;
					}if(dieRolls[i]+ dieRoll1[i] == 10){
						PlayerLog.instance.AddEvent("Shields Collapse!");
						activeShields = 0;
						shields = 0;
							
					}if(dieRolls[i]+ dieRoll1[i] == 11){
						PlayerLog.instance.AddEvent("Hull breach!"); 
							
							
					}if(dieRolls[i]+ dieRoll1[i] == 12){
						PlayerLog.instance.AddEvent("Bulkheads Collapse!");
					}
				}
			}
				#endregion
		}
			#endregion
	}
			#endregion
	}


	public int SideHit(Vector3 attackDirection){


		Vector3 referenceRight= Vector3.Cross(Vector3.up, forwardVector);

		
		// Get the angle in degrees between 0 and 180
		float angle = Vector3.Angle(attackDirection, forwardVector);
		
		// Determine if the degree value should be negative. Here, a positive value
		// from the dot product means that our vector is on the right of the reference vector
		// whereas a negative value means we're on the left.
		float sign = Mathf.Sign(Vector3.Dot(attackDirection, referenceRight));
		
		float finalAngle = sign * angle;
		if (finalAngle >= -45 && finalAngle < 45) {
			return 0;
		}
		if (finalAngle < -45 && finalAngle >= -135) {
			return 1;
		}
		if (finalAngle >= 45 && finalAngle < 135) {
			return 2;
		} else {
			return 3;
		}

	}

	void BlastmakerCollision(){

	}
	
}
