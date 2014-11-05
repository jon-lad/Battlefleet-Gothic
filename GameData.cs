/* Game Data Script  v1.0 Is attached to Persistant GameData Object And stores Statistics which 
 * are needed across scenes also saves and loads vital data to a binary file.
 * written by Jonathan Ward modified 09/10/14 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameData : MonoBehaviour {

	#region variables
	public static GameData instance;

	//Ship Prefabs
	public GameObject LunarPrefab;
	public GameObject TyrantPrefab;
	public GameObject GothicPrefab;
	public GameObject DauntlessPrefab;
	
	public GameObject MurderPrefab;
	public GameObject CarnagePrefab;
	public GameObject SlaughterPrefab;

	//player Prefabs
	public GameObject UserPlayerPrefab;
	public GameObject AIPlayerPrefab;

	//other batlle object prefabs
	public GameObject blastmakerPrefab;
	public  GameObject SmallStandPrefab;
	public  GameObject LargeStandPrefab;
	
	//prefabs to be removed 
	public GameObject TilePrefab;// Tile prefab is used to make grid to check model scale.

	//Ship Names for Random Generation
	public string[] imperialShipNames = new string[]{"Godefroy Magnificat", "His Will", "Triumph", "Duke Helbrecht",
		"Divine Right", "Dominus Astra", "Indomitable Wrath", "Legatus Stygies", "Lord of Light", 
		"Corax", "Green Lake", "Justus Dominus", "Bloodhawk", "Cardinal Boras", "Saint Augusta", "Pride of Fenris",
		"Victory", "Hammer of Scaro", "Guardian of Aquinas", "The Sword Infernus", "Warrior Knight", "Light of Ascension", "Kingmaker",
		"The Covenanter", "Vigilanti Eternas", "Thunderchild", "Hammer of Light", "Ex Cathedra", 
		"Pax Imperium", "Imperious", "Marquis Lex", "Fist of Russ", "Cardinal Xian", "Cypra Probatii", "Flame of Purity", "Niobe",
		"Sword of Retribution", "Righteous Power", "Sebastian Thor", "Silent Fire", "Archon Kort", "Fortitude", "Lord Solar Macharius", 
		"Rhadamanthine", "Hammer of Justice", "Depth of Fury", "Ferrum Aeterna", "Ultima Praetor", 
		"Invincible", "Emperor's Wrath", "Pallas Imperious", "Preceptor", "Righteous Fury", "Spirit of Defiance", "Sword of Orion",
		"Lord Daros", "Retribution", "Agrippa", "Minotaur", "Iron Duke", "Justicar", "Indomitus Imperious", "Relentless", "Hammer of Thrace",
		"Goliath", "Imperial Wrath", "Zealous", "Dominion", "Incendrius", "Lord Sylvanus", "Blade of Drusus", "Cardinal Turin",
		"Vanguard", "Abdiel", "Uziel", "Baron Surtur", "Vigilant", "Havock", "Guardian", "Advocate", "Cerebus", "August", "Luxor", "Yermetov", 
		"Divine Crusade", "Saint of Scintilla", "Forebearer", "Archangel", "Leonid", "Sword of Voss", "Sanctis Legate", "Ad Liberis",
		"Cape Wrath", "Harrower", "Valiant", "Strident Virtue", "Purgation"};
	
	public string[] chaosShipNames = new string[]{"Merciless Death", "Fortress of Agony", "Damnations Fury", "Sword of Sacrilege",
		"Eternity of Pain", "Torment", "The Ninth Eye", "The Treacherous", "Vengeful Spirit", "Infidus Imperator", "Incarnadine",
		"Foebane", "Bloodied Sword", "Foe-Reaper", "Bringer of Despair", "Blood Royale", "Chaos Eternus", "Injustice", "Warmaker",
		"Malignus Maximus", "Contagion", "Horrific", "Darkblood", "Heartless Destroyer", "Initiate of Skalathrax", "Wanton Desecration",
		"Excessive", "Anarchic Vendetta", "Infidus Diabolus", "Covenant of Blood", "Piercing Nova", "Deathbane", "Unforgivable",
		"Doombringer", "Deathblade", "Steel Fang", "Monstrous", "Unholy Dominion", "Plagueclaw", "Despicable Ecstasy", 
		"Deathskull", "Heathen Promise", "Killfrenzy", "Soulless", "Fearmongers", "Lost Souls", "Carrion Squadron", "Inculpators of Harok",
		"Relatiators", "Pugatos", "Unclean Ravagers", "Khorne's Disciples", "Fellclaws", "Damnators", "Exterminators"};




	//Data For campaign
	public List<playerData> Players = new List<playerData>();

	//Data for Battle
	public int currentLevel;
	public BattleData battleData = new BattleData();
	//set to true when Loading an old battle
	public bool levelLoaded = false;
	#endregion

	#region Start
//Make Persistant
	void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else if (instance != this) {
			Destroy(gameObject);
		}
	}
	#endregion

	#region Save/Load Functions
	//Save Functiom Serialises Data
	public void Save(string filename){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		CurrentData data;
		if (File.Exists (Application.persistentDataPath + "/" + filename + ".dat")) {
			file = File.Open (Application.persistentDataPath + "/" + filename + ".dat", FileMode.Open);
			data = (CurrentData)bf.Deserialize (file);
		
		} else {
			file = File.Create(Application.persistentDataPath + "/" + filename + ".dat");
			data = new CurrentData ();
		}

		data.currentLevel = currentLevel;
		data.players = Players;

		bf.Serialize (file, data);
		file.Close ();
	}
	//load data retrives data but BattleManager Uses this Data to recreate level
	public void Load(string filename){
		levelLoaded = true;
		if (File.Exists (Application.persistentDataPath + "/" + filename + ".dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + filename + ".dat", FileMode.Open);
		
			CurrentData data = (CurrentData)bf.Deserialize (file);
			file.Close ();

			currentLevel = data.currentLevel;
			Players = data.players;
		}
	}
	#endregion
}
#region Data Classes
#region Main data
[Serializable]
class CurrentData{

	public int currentLevel;
	public List<playerData> players;

}
#endregion

//sub classes for differnt types of Data
[Serializable]
public class playerData{
	//for campaigns
	public string playerName;

	public bool humanPlayer;// is player human or Ai
	public int race;// which race player is 

	public List<GameObject> fleetPrefabs;//ships in players fleet
	public List<ShipData> fleetShipData;//data for ships 

	//for Battles
	public bool active;//is player involved in this battle
	public List<GameObject> shipBattlePrefabs = new List<GameObject>(); // ships chosen to go to battle
	public List<ShipData> shipData; // Data of ships in battle
	public int SelectedShipIndex;//Selected ship (for ship data and prefab
	public List<GameObject>activeShipsPrefabs;//ships remaining in battle

}

[Serializable]
public class BattleData{

	public int currentPlayerIndex;
	public int turnNumber;
	public List<playerData> activePlayers;
	
}

[Serializable]
public class ShipData{
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
	public  GameObject stand; // the stand object for the ship
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

}
//SubSector Data
[Serializable]
public class subSectorData{

	//list of Systems
	List<PlanetSystem> planeterySystems;

}

#endregion
