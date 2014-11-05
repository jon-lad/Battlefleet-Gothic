using UnityEngine;
using System.Collections;

public class Carnage : Ship {
	
	// Use this for initialization
	public override void  Start () {
		base.Start ();
		stand = (GameObject)Instantiate(GameData.instance.SmallStandPrefab, transform.position, Quaternion.Euler(new Vector3()));
		hits = 8;
		speed = 25;
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

		Weapon portWeaponBattery = new Weapon();
		weapons.Add (portWeaponBattery);
		Weapon starboardWeaponBattery = new Weapon();
		weapons.Add (starboardWeaponBattery);
		Weapon portWeaponBattery1 = new Weapon();
		weapons.Add (portWeaponBattery1);
		Weapon starboardWeaponBattery1 = new Weapon();
		weapons.Add (starboardWeaponBattery1);
		Weapon prowWeaponBattery = new Weapon();
		weapons.Add (prowWeaponBattery);

		portWeaponBattery.type = 1;
		portWeaponBattery.range = 45;
		portWeaponBattery.strength = 6;
		portWeaponBattery.maxFireArc = -45;
		portWeaponBattery.minFireArc = -135;
		portWeaponBattery.weaponName = "Port Weapon Batt. (6)";
		
		starboardWeaponBattery.type = 1;
		starboardWeaponBattery.range = 45;
		starboardWeaponBattery.strength = 6;
		starboardWeaponBattery.maxFireArc = 135;
		starboardWeaponBattery.minFireArc = 45;
		starboardWeaponBattery.weaponName = "Strbd Weapon Batt. (6)";
		
		portWeaponBattery1.type = 1;
		portWeaponBattery1.range = 60;
		portWeaponBattery1.strength = 4;
		portWeaponBattery1.maxFireArc = -45;
		portWeaponBattery1.minFireArc = -135;
		portWeaponBattery1.weaponName = "Port Weapon Batt. (4)";
		
		starboardWeaponBattery1.type = 1;
		starboardWeaponBattery1.range = 60;
		starboardWeaponBattery1.strength = 4;
		starboardWeaponBattery1.maxFireArc = 135;
		starboardWeaponBattery1.minFireArc = 45;
		starboardWeaponBattery1.weaponName = "Strbd Weapon Batt. (4)";

		prowWeaponBattery.type = 1;
		prowWeaponBattery.range = 60;
		prowWeaponBattery.strength = 6;
		prowWeaponBattery.maxFireArc = 135;
		prowWeaponBattery.minFireArc = -135;
		prowWeaponBattery.weaponName = "Prow Weapon Batt.";
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}
}
