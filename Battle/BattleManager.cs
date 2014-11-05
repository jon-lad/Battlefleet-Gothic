/* BattleManager Script v1.0 This Script is Attached to the Battle Manager Object in combat scenes
 * this script organises the generation of units and controls the game turns. This script also
 * removes temporary effects at the end of the turn. written by Jonathan Ward modied 09/10/14 */




using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BattleManager : MonoBehaviour {

	public static BattleManager instance;
	
	public bool playersLoaded = false;// make sure players are loaded before taking any actions

	public Vector2 worldSize; //size of the Game board
 	// size of deploment area may become a list for none rectangular deployments
	public Vector2 deploymentArea;

	//Table for working out how many attack dice are rolled in shooting
	public int[,] gunneryTable = new int[5, 20];

	//temporary grid to check model sizes
	List <List<Tile>> map = new List <List<Tile>>();

	//cache of player in the battle
	public List <Player> players = new List<Player>();





	// Use this for initialization

	void Awake(){
		instance = this;
	}

	void Start () {
		if (GameData.instance.levelLoaded == false) {
			GameData.instance.battleData.turnNumber = 1;
			GameData.instance.battleData.currentPlayerIndex = 0;
		

			for (int i = 0; i < GameData.instance.Players.Count; i++) {
				if (GameData.instance.Players [i].active == true){
					GameData.instance.battleData.activePlayers.Add (GameData.instance.Players[i]);
				}
			}
		}



		generateMap ();

		generatePlayers ();
		playersLoaded = true;

		#region Generate Gunnery Table
		for(int i = 0; i < 5; i++){
			for(int j = 0; j < 20; j++){
				if(i == 0 && j < 5){
					gunneryTable[i, j] = j + 1;
				}else if (i == 0 && j < 15){
					gunneryTable[i, j] = j;
				}else if (i == 0){
					gunneryTable[i, j] = j - 1;

				}else if(i == 1 && j < 1){
					gunneryTable[i, j] = j + 1;
				}else if(i == 1 && j < 5){
					gunneryTable[i, j] = j;
				}else if(i == 1 && j < 8){
					gunneryTable[i, j] = j - 1;
				}else if(i == 1 && j < 11){
					gunneryTable[i, j] = j - 2;
				}else if(i == 1 && j < 15){
					gunneryTable[i, j] = j - 3;
				}else if(i == 1 && j < 18){
					gunneryTable[i, j] = j - 4;
				}else if(i == 1){
					gunneryTable[i, j] = j - 5;

				}else if(i == 2 && j <= 1){
					gunneryTable[i, j] = 1;
				}else if(i == 2 && j <= 3){
					gunneryTable[i, j] = 2;
				}else if(i == 2 && j <= 5){
					gunneryTable[i, j] = 3;
				}else if(i == 2 && j <= 7){
					gunneryTable[i, j] = 4;
				}else if(i == 2 && j <= 9){
					gunneryTable[i, j] = 5;
				}else if(i == 2 && j <= 11){
					gunneryTable[i, j] = 6;
				}else if(i == 2 && j <= 13){
					gunneryTable[i, j] = 7;
				}else if(i == 2 && j <= 15){
					gunneryTable[i, j] = 8;
				}else if(i == 2 && j <= 17){
					gunneryTable[i, j] = 9;
				}else if(i == 2 && j <= 19){
					gunneryTable[i, j] = 10;
				}
				else if(i == 3 && j < 2){
					gunneryTable[i, j] = 0;
				}else if(i == 3 && j < 3){
					gunneryTable[i, j] = 1;
				}else if(i == 3 && j < 5){
					gunneryTable[i, j] = 2;
				}else if(i == 3 && j < 6){
					gunneryTable[i, j] = 3;
				}else if(i == 3 && j < 8){
					gunneryTable[i, j] = 4;
				}else if(i == 3 && j < 10){
					gunneryTable[i, j] = 5;
				}else if(i == 3 && j < 11){
					gunneryTable[i, j] = 6;
				}else if(i == 3 && j < 13){
					gunneryTable[i, j] = 7;
				}else if(i == 3 && j < 14){
					gunneryTable[i, j] = 8;
				}else if(i == 3 && j < 16){
					gunneryTable[i, j] = 9;
				}else if(i == 3 && j < 17){
					gunneryTable[i, j] = 10;
				}else if(i == 3 && j < 19){
					gunneryTable[i, j] = 11;
				}else if(i == 3){
					gunneryTable[i, j] = 12;

				}else if (i ==4 && j < 2){
					gunneryTable[i, j] = 0;
				}else if (i ==4 && j < 7){
					gunneryTable[i, j] = 1;
				}else if (i ==4 && j < 12){
					gunneryTable[i, j] = 2;
				}else if (i ==4 && j < 17){
					gunneryTable[i, j] = 3;
				}else if (i ==4){
					gunneryTable[i, j] = 4;
				}
			
			}

		}
		#endregion

		startTurn ();


		}

	
	// Update is called once per frame
	void Update () {

		// update animations and effects
		for (int i =0; i < players.Count; i++) {
			players[i].Update();
		}
		//Update current player actions
 		players [GameData.instance.battleData.currentPlayerIndex].TurnUpdate();
	}

	public void NextTurn(){

		if (GameData.instance.battleData.currentPlayerIndex + 1 < players.Count){
			GameData.instance.battleData.currentPlayerIndex++;
			startTurn ();
		
		} else {
			GameData.instance.battleData.turnNumber++;
			GameData.instance.battleData.currentPlayerIndex = 0;
			startTurn();

		}


	}

	void generateMap(){


		map = new List <List<Tile>>{};
		for (int i = 0; i < deploymentArea.x; i++) {

			List<Tile> row = new List<Tile>{};
			for(int j = 0; j < deploymentArea.y; j++){

				Tile tile = ((GameObject)Instantiate(GameData.instance.TilePrefab, new Vector3(i+ (worldSize.x - deploymentArea.x)/2, 0 , j), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
				tile.gridPosition = new Vector2(i, j);
				tile.transform.parent = GameObject.Find("Deployment").transform;
				row.Add(tile);
			}
			map.Add(row);
		}

	}

	void generatePlayers(){
		if (GameData.instance.levelLoaded == false) {
			for (int i = 0; i < GameData.instance.battleData.activePlayers.Count; i++) {
				if (GameData.instance.battleData.activePlayers[i].humanPlayer == true) {
				
					UserPlayer player;
				
					player = ((GameObject)Instantiate (GameData.instance.UserPlayerPrefab)).GetComponent<UserPlayer> ();

					players.Add (player);

					player.race = GameData.instance.battleData.activePlayers[i].race;

					player.shipPrefabs = GameData.instance.battleData.activePlayers[i].shipBattlePrefabs;

					player.generateShips (GameData.instance.battleData.activePlayers[i].playerName);

					for (int j = 0; j < player.shipInstances.Count; j++) {
						if (i == 0) {
							player.shipInstances [j].bearing = 0;
							Vector3 temp = new Vector3 (0, 0, deploymentArea.y / 2);
							player.shipInstances [j].transform.position += temp; 
						} else if (i == 1) {
							player.shipInstances [j].bearing = 180;
							Vector3 temp = new Vector3 (0, 0, worldSize.y - deploymentArea.y / 2);
							player.shipInstances [j].transform.position += temp;
						}
					}
				} else if (GameData.instance.Players [i].humanPlayer == false) {
						
					AIPlayer player = new AIPlayer ();

					player = ((GameObject)Instantiate (GameData.instance.UserPlayerPrefab)).GetComponent<AIPlayer> ();
		
					players.Add (player);

					player.shipPrefabs = GameData.instance.battleData.activePlayers[i].shipBattlePrefabs;

					player.race = GameData.instance.Players [i].race;

					player.generateShips (GameData.instance.Players [i].playerName);

					for (int j = 0; j < player.shipInstances.Count; j++) {
						if (i == 0) {
							player.shipInstances [j].bearing = 0;
							Vector3 temp = new Vector3 (0, 0, deploymentArea.y / 2);
							player.shipInstances [j].transform.position += temp; 
						} else if (i == 1) {
							player.shipInstances [j].bearing = 180;
							Vector3 temp = new Vector3 (0, 0, worldSize.y - deploymentArea.y / 2);
							player.shipInstances [j].transform.position += temp;
						}
					}
				}
			}
		} else {

			GameData.instance.levelLoaded = false;

		for (int i = 0; i < GameData.instance.battleData.activePlayers.Count; i++) {
				if (GameData.instance.battleData.activePlayers[i].humanPlayer == true) {
					
				UserPlayer player;
					
				player = ((GameObject)Instantiate (GameData.instance.UserPlayerPrefab)).GetComponent<UserPlayer> ();
					
				players.Add (player);
					
				player.race = GameData.instance.Players [i].race;

				// instantise everything from save data
			} else {
				AIPlayer player = new AIPlayer ();
					
				player = ((GameObject)Instantiate (GameData.instance.UserPlayerPrefab)).GetComponent<AIPlayer> ();
					
				players.Add (player);

				player.race = GameData.instance.Players [i].race;

				// instantise everything from save data

				}
			}
		}

	}



	void startTurn(){


		Camera.main.transform.rotation = Quaternion.Euler (new Vector3 (90,GameData.instance.battleData.currentPlayerIndex * 180,0));

		PlayerLog.instance.AddEvent ("Player " + (GameData.instance.battleData.currentPlayerIndex +1) + "'s Turn");
		players[GameData.instance.battleData.currentPlayerIndex].phaze = 0;
		players[GameData.instance.battleData.currentPlayerIndex].shipUnMoved = players[GameData.instance.battleData.currentPlayerIndex].remainingShips.Count;
		players [GameData.instance.battleData.currentPlayerIndex].moving = true;
		players [GameData.instance.battleData.currentPlayerIndex].specialOrdersAlowed = true;
		players [GameData.instance.battleData.currentPlayerIndex].enemyShips.Clear();

		for (int i = 0; i< players.Count; i++) {
			if(i != GameData.instance.battleData.currentPlayerIndex){
				for(int j = 0; j < players[i].remainingShips.Count; j++){
					players [GameData.instance.battleData.currentPlayerIndex].enemyShips.Add( players[i].remainingShips[j]);
				}
			}
		}

		if (GameData.instance.battleData.currentPlayerIndex > 0) {
			for (int i = 0; i <	players[GameData.instance.battleData.currentPlayerIndex - 1].remainingShips.Count; i++) {
				if (players [GameData.instance.battleData.currentPlayerIndex - 1].remainingShips [i].specialOrder == 6) {

					players [GameData.instance.battleData.currentPlayerIndex - 1].remainingShips [i].specialOrder = 0;
					players [GameData.instance.battleData.currentPlayerIndex - 1].remainingShips [i].specialOrderAllowed = true;
					players [GameData.instance.battleData.currentPlayerIndex - 1].remainingShips [i].fireStrength = 1;
				}
			
			}
		} else {
			for (int i = 0; i <	players[players.Count - 1].remainingShips.Count; i++) {
				if (players [players.Count - 1].remainingShips [i].specialOrder == 6) {

					players [players.Count - 1].remainingShips [i].specialOrder = 0;
					players [players.Count - 1].remainingShips [i].specialOrderAllowed = true;
					players [players.Count - 1].remainingShips [i].fireStrength = 1;
				}
			}
		}
			


		for(int i = 0; i <players[GameData.instance.battleData.currentPlayerIndex].remainingShips.Count; i++){
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].movedThisTurn = 0;
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].rotated = 0;
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].minTurnDistance = players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].baseMinTurnDistance;
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].turnsRemaining = 1;
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].turnsUsed = 0;
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].turnDistance.Clear();
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].turnDistance.Add(0);
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].originalBearing.Clear();
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].originalBearing.Add (0);
			if(	players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].specialOrder != 6){
				players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].specialOrder = 0;
				players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].specialOrderAllowed = true;
				players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].fireStrength = 1;
				players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].maxMove = players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].speed;
			}
			if(players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].maxMove >= players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].speed /2){
				players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].minMove = players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].speed /2;
			}else{
				players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].minMove = players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].maxMove;
			}
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].moveComplete = false;
			players[GameData.instance.battleData.currentPlayerIndex].remainingShips[i].target = players[GameData.instance.battleData.currentPlayerIndex].enemyShips[0];
			players[GameData.instance.battleData.currentPlayerIndex].weaponsAvailable.Clear ();
		}



		players [GameData.instance.battleData.currentPlayerIndex].selectedWeaponsType = -1;
		StartCoroutine (players[GameData.instance.battleData.currentPlayerIndex].MovementPhaze ());
		players [GameData.instance.battleData.currentPlayerIndex].selectedShip = players [GameData.instance.battleData.currentPlayerIndex].remainingShips [0];
		Camera.main.transform.position = new Vector3 (players [GameData.instance.battleData.currentPlayerIndex].selectedShip.transform.position.x, Camera.main.transform.position.y, players [GameData.instance.battleData.currentPlayerIndex].selectedShip.transform.position.z);


	}

	public void NextPhaze(){
		if (players [GameData.instance.battleData.currentPlayerIndex].phaze == 0) {
			players [GameData.instance.battleData.currentPlayerIndex].phaze = 1;
			PlayerLog.instance.AddEvent("Shooting Phase");
			StartCoroutine (players [GameData.instance.battleData.currentPlayerIndex].ShootingPhaze());
		}else if (players [GameData.instance.battleData.currentPlayerIndex].phaze == 1) {
			players [GameData.instance.battleData.currentPlayerIndex].phaze = 2;
			PlayerLog.instance.AddEvent("Ordnance Phase");
			StartCoroutine (players [GameData.instance.battleData.currentPlayerIndex].OrdnancePhaze());
		}else if (players [GameData.instance.battleData.currentPlayerIndex].phaze == 2) {
			players [GameData.instance.battleData.currentPlayerIndex].phaze = 3;
			PlayerLog.instance.AddEvent("End Phase");
			StartCoroutine (players [GameData.instance.battleData.currentPlayerIndex].EndPhaze());
		}
	}


	
}
