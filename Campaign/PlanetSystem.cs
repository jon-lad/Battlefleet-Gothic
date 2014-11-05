using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetSystem : MonoBehaviour {
	public int type;
	public int controler;
	public List<Planet> planets = new List<Planet>();
	public List<PlanetSystem> connectedSystems = new List<PlanetSystem> ();
	Vector2 position;

	public GameObject planet;

	// Use this for initialization
	void Start () {
		int planNo = Random.Range (1, 7);
		int systemType = type;
		for (int i = 0; i < planNo; i++) {
			Planet currentPlanet;
			currentPlanet = (Planet) Instantiate(planet);
			planets[i] = currentPlanet;
			systemType = type - 2 * i;
			if (systemType < 0) {
				systemType = 1;
			}

			planets [i].type = PlanetType(systemType);

	
			int asteroid = Random.Range (1, 5);
			if (asteroid <= 3){
				planets[i].asteroidField = true;
			}
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	int PlanetType(int systemType){
		return 0;
	}
}
