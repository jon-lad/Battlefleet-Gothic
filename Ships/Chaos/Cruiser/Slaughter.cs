using UnityEngine;
using System.Collections;

public class Slaughter : Ship {
	
	// Use this for initialization
	public override void  Start () {
		base.Start ();
		stand = (GameObject)Instantiate(GameData.instance.SmallStandPrefab, transform.position, Quaternion.Euler(new Vector3()));
		hits = 8;
		speed = 30;
		minMove = speed / 2;
		turns = 45;
		shields = 2;
		armour = 5;
		turrets = 2;
		minTurnDistance = 10;
		maxMove = speed;
		baseMinTurnDistance = 10;
		stand.transform.parent = transform;
		shipType = 1;
		remainingHits = hits;
		activeShields = shields;

		Weapon portLanceBattery = new Weapon();
		weapons.Add (portLanceBattery);
		Weapon starboardLanceBattery = new Weapon();
		weapons.Add (starboardLanceBattery);
		Weapon portWeaponBattery = new Weapon();
		weapons.Add (portWeaponBattery);
		Weapon starboardWeaponBattery = new Weapon();
		weapons.Add (starboardWeaponBattery);
		Weapon prowWeaponBattery = new Weapon();
		weapons.Add (prowWeaponBattery);


		portLanceBattery.type = 0;
		portLanceBattery.range = 30;
		portLanceBattery.strength = 2;
		portLanceBattery.maxFireArc = -45;
		portLanceBattery.minFireArc = -135;
		portLanceBattery.weaponName = "Port Lance Batt.";
		
		starboardLanceBattery.type = 0;
		starboardLanceBattery.range = 30;
		starboardLanceBattery.strength = 2;
		starboardLanceBattery.maxFireArc = 135;
		starboardLanceBattery.minFireArc = 45;
		starboardLanceBattery.weaponName = "Strbd Lance Batt.";
		
		portWeaponBattery.type = 1;
		portWeaponBattery.range = 30;
		portWeaponBattery.strength = 8;
		portWeaponBattery.maxFireArc = -45;
		portWeaponBattery.minFireArc = -135;
		portWeaponBattery.weaponName = "Port Weapon Batt.";
		
		starboardWeaponBattery.type = 1;
		starboardWeaponBattery.range = 30;
		starboardWeaponBattery.strength = 8;
		starboardWeaponBattery.maxFireArc = 135;
		starboardWeaponBattery.minFireArc = 45;
		starboardWeaponBattery.weaponName = "Strbd Weapon Batt.";

		prowWeaponBattery.type = 1;
		prowWeaponBattery.range = 30;
		prowWeaponBattery.strength = 6;
		prowWeaponBattery.maxFireArc = 135;
		prowWeaponBattery.minFireArc = -135;
		prowWeaponBattery.weaponName = "Prow Weapon Batt.";
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public override void AheadFull(){
		
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
		
		
		if(BattleManager.instance.players[GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed && specialOrderAllowed && movedThisTurn == 0){
			int die1 =  Random.Range(1, 7);
			int die2 =  Random.Range(1, 7);
			int total = die1 + die2;
			PlayerLog.instance.AddEvent("Atempting All Ahead Full Special Order");
			PlayerLog.instance.AddEvent("Need a roll of " + modifiedLeadership + " or under to succeed.");
			PlayerLog.instance.AddEvent("Rolled " + (total) + " (" + die1 +", " + die2 + ")");
			
			if(total <= modifiedLeadership){
				PlayerLog.instance.AddEvent("Sucess!");
				fireStrength = 0.5f;
				specialOrderAllowed = false;
				int die3 =  Random.Range(1, 7);
				int die4 =  Random.Range(1, 7);
				int die5 =  Random.Range(1, 7);
				int die6 =  Random.Range(1, 7);
				int die7 =  Random.Range(1, 7);
				int moveTotal = die3 + die4 + die5 + die6 + die7;
				PlayerLog.instance.AddEvent("Rolling for extra movement");
				PlayerLog.instance.AddEvent("Rolled (" +die3 + ", " +die4 + ", " + die5 + ", " + die6 + "," +die7 + ")");
				PlayerLog.instance.AddEvent(shipName + " Moved " + moveTotal + " extra cm");
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
}
