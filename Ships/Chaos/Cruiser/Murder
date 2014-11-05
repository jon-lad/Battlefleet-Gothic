using UnityEngine;
using System.Collections;

public class Murder : Ship {
	
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

		Weapon portWeaponBattery = new Weapon ();
		weapons.Add (portWeaponBattery);
		Weapon starboardWeaponBattery = new Weapon ();
		weapons.Add (starboardWeaponBattery);
		Weapon prowLanceBattery = new Weapon ();
		weapons.Add (prowLanceBattery);

		portWeaponBattery.type = 1;
		portWeaponBattery.range = 45;
		portWeaponBattery.strength = 10;
		portWeaponBattery.maxFireArc = -45;
		portWeaponBattery.minFireArc = -135;
		portWeaponBattery.weaponName = "Port Weapon Batt.";

		starboardWeaponBattery.type = 1;
		starboardWeaponBattery.range = 45;
		starboardWeaponBattery.strength = 10;
		starboardWeaponBattery.maxFireArc = 135;
		starboardWeaponBattery.minFireArc = 45;
		starboardWeaponBattery.weaponName = "Strbd Weapon Batt.";

		prowLanceBattery.type = 0;
		prowLanceBattery.range = 60;
		prowLanceBattery.strength = 2;
		prowLanceBattery.maxFireArc = 45;
		prowLanceBattery.minFireArc = -45;
		prowLanceBattery.weaponName = "Prow Lance Batt.";

	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}
}

