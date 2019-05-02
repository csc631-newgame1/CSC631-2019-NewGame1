﻿using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using MapUtils;

public class GameManager : MonoBehaviour
{
	private MapGenerator map_gen;
	private MapManager map_manager;

    private EnemySpawner enemySpawner;
	private EnvironmentSpawner environmentSpawner;
    private Player localPlayer;
	private TileSelector tileSelector;

    private TurnManager turn_manager;

    private List<SpawnZone> spawnZones;
	
    public GameObject playerPrefab, endportal;

    public static GameManager instance; //static so we can carry oour levels and st
    
	void Start()
	{
		Init();
        instance = this;
    }
	
    void Init()
    {
		map_manager = GetComponent<MapManager>();
		enemySpawner = GetComponent<EnemySpawner>();
		environmentSpawner = GetComponent<EnvironmentSpawner>();
		tileSelector = GameObject.FindGameObjectWithTag("Selector").GetComponent<TileSelector>();

		map_manager.Init(this);
		tileSelector.Init(map_manager);
        environmentSpawner.Init(map_manager);
		
		turn_manager = GetComponent<TurnManager>();
		turn_manager.Init(this);
		
		List<Client> players = Network.getPeers();
		List<Pos> spawn_locations = map_manager.findSpawnpoints(players.Count);
		
		Pos level_end = spawn_locations[0];
		map_manager.instantiate_environment(endportal, level_end, false);
		
		Debug.Log("Spawned " + players.Count + " players");
		// spawn players
		for (int i = 0; i < players.Count; i++) {
			if (players[i].playerObject == null) {
				Player instance = map_manager.instantiate(playerPrefab, spawn_locations[i+1], null, players[i].nickname).GetComponent<Player>();
				instance.SetCharacterClass(players[i].classname);
				players[i].playerObject = instance;
				if (players[i].ID == NetworkManager.clientID) localPlayer = instance;
			}
			else {
				Debug.Log("Respawning player");
				map_manager.re_instantiate(players[i].playerObject.gameObject, spawn_locations[i+1]);
			}
		}
		// spawn enemies
		enemySpawner.Init(map_manager);

		tileSelector.setPlayer(localPlayer);
		Camera.main.GetComponent<CameraControl>().SetTarget(localPlayer.gameObject);
		UI_BattleMenu.SetActButtons(localPlayer.getActionNames());
		
		turn_manager.StartLoop();
    }
	
	void DeInit()
	{
		turn_manager.Terminate();
		map_manager.clear_map();
	}
	
	public static void NextLevel()
	{
		instance.DeInit();
		instance.Init();
	}
	
	public static void GameOver()
	{
		Quit();
	}
	
	private static void Quit()
	{
		Network.disconnectFromServer();
		instance.DeInit();
		SceneManager.LoadScene("NewMenu");
	}

    void Update()
	{
		if (Input.GetKeyDown("escape")) {
			Quit();
		}
		
		foreach (char key in Input.inputString) {
			
			switch (key) {
			case 'r':
				foreach (Player player in Network.getPlayers())
					MapManager.ExtractAgent(player);
				NextLevel();
				break;
			}
			
        }

		if (Input.GetMouseButtonDown(1)) {
			switch (tileSelector.mode) {
				case "MOVE":
					if (tileSelector.hoveringValidMoveTile()) {
						Network.submitCommand(new MoveCommand(NetworkManager.clientID, tileSelector.grid_position));
						tileSelector.mode = "NONE";
					}
					break;
				case "ACT":
					if (tileSelector.hoveringValidActTile()) {
						Network.submitCommand(new AttackCommand(NetworkManager.clientID, tileSelector.grid_position, last_action));
						tileSelector.mode = "NONE";
					}
					break;
				case "INTERACT":
					if (tileSelector.hoveringValidActTile()) {
						Network.submitCommand(new InteractCommand(NetworkManager.clientID, tileSelector.grid_position));
						tileSelector.mode = "NONE";
					}
					break;
				case "NONE": 
					break;
			}
		}
	}
	
    public static void MovePlayer() {
		if (!instance.localPlayer.can_take_action()) return;
		
		if (instance.tileSelector.mode == "MOVE")
			instance.tileSelector.mode = "NONE";
		else
			instance.tileSelector.mode = "MOVE";
    }

	private static int last_action = -1;
	public static void ActionPlayer(int action) {
		if (!instance.localPlayer.can_take_action()) return;
		
		if(instance.localPlayer.SetCurrentAction(action)) {
			//instance.tileSelector.setMode(instance.localPlayer.getActionMode(action));
			if (instance.tileSelector.mode == "ACT" && action == last_action)
				instance.tileSelector.mode = "NONE";
			else
				instance.tileSelector.mode = "ACT";
			
			last_action = action;
		}
    }
	
	public static void ClearPlayerAction() {
		instance.tileSelector.mode = "NONE";
	}
	
	public static void InteractPlayer() {
		if (!instance.localPlayer.can_take_action()) return;
		
		if (instance.tileSelector.mode == "INTERACT")
			instance.tileSelector.mode = "NONE";
		else
			instance.tileSelector.mode = "INTERACT";
	}
	
	public static void WaitPlayer() {
		if (!instance.localPlayer.can_take_action()) return;
		
        Network.submitCommand(new WaitCommand(NetworkManager.clientID));
    }

    public static void PotionPlayer() {
		if (!instance.localPlayer.can_take_action()) return;
		
        //instance.localPlayer.potion();
    }
	
	public static void kill(DungeonObject obj)
	{
		instance.map_manager.de_instantiate(obj.grid_pos);
		if (obj is GameAgent)
			instance.turn_manager.removeFromRoster(obj as GameAgent);
	}
}
