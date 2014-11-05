using UnityEngine;
using System.Collections;

public class Weapon {

	public int type; // lance = 0, weapons  = 1
	public int range;
	public int strength;
	public int maxFireArc;
	public int minFireArc;
	public string weaponName;

	public bool fired = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	bool ShipInRange(Ship targetShip){

		return true;
	}
}
