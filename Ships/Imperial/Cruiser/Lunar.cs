using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lunar : Ship {

	

	// Use this for initialization
	public override void  Start () {
		base.Start ();
		stand = (GameObject)Instantiate(GameData.instance.SmallStandPrefab, transform.position, Quaternion.Euler(new Vector3()));
		hits = 8;
		speed = 20;
		minMove = speed / 2;
		turns = 45;
		shields = 2;
		armour = 5;
		turrets = 2;
		minTurnDistance = 10;
		maxMove = speed;
		baseMinTurnDistance = 10;
		stand.transform.parent = transform;
		remainingHits = hits;
		activeShields = shields;
		shipType = 1;

		
		Weapon portLanceBattery = new Weapon();
		weapons.Add (portLanceBattery);
		Weapon starboardLanceBattery = new Weapon();
		weapons.Add (starboardLanceBattery);
		Weapon portWeaponBattery = new Weapon();
		weapons.Add (portWeaponBattery);
		Weapon starboardWeaponBattery = new Weapon();
		weapons.Add (starboardWeaponBattery);


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
		portWeaponBattery.strength = 6;
		portWeaponBattery.maxFireArc = -45;
		portWeaponBattery.minFireArc = -135;
		portWeaponBattery.weaponName = "Port Weapon Batt.";

		starboardWeaponBattery.type = 1;
		starboardWeaponBattery.range = 30;
		starboardWeaponBattery.strength = 6;
		starboardWeaponBattery.maxFireArc = 135;
		starboardWeaponBattery.minFireArc = 45;
		starboardWeaponBattery.weaponName = "Strbd Weapon Batt.";

	}

	public override void Update () {
		base.Update ();
	}
}
