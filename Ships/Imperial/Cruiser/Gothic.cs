using UnityEngine;
using System.Collections;

public class Gothic : Ship {
	
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

		portLanceBattery.type = 0;
		portLanceBattery.range = 30;
		portLanceBattery.strength = 4;
		portLanceBattery.maxFireArc = -45;
		portLanceBattery.minFireArc = -135;
		portLanceBattery.weaponName = "Port Lance Batt.";
		
		starboardLanceBattery.type = 0;
		starboardLanceBattery.range = 30;
		starboardLanceBattery.strength = 4;
		starboardLanceBattery.maxFireArc = 135;
		starboardLanceBattery.minFireArc = 45;
		starboardLanceBattery.weaponName = "Strbd Lance Batt.";
	}
	
	public override void Update () {
		base.Update ();
	}
}
