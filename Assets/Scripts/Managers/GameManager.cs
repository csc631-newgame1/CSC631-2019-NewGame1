using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class GameManager : MonoBehaviour
{
	private GameObject map;
	private MapGenerator map_gen;
	private MapManager map_manager;

    private EnemySpawner enemySpawner;
	private EnvironmentSpawner environmentSpawner;
    private Player localPlayer;
	private TileSelector tileSelector;

    private TurnManager turn_manager;

    private List<SpawnZone> spawnZones;
	
    public GameObject playerPrefab;

    public static GameManager instance; //static so we can carry oour levels and st
    
	void Start()
	{
		Init();
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
	
    void Init()
    {
		map_manager = GetComponent<MapManager>();
		enemySpawner = GetComponent<EnemySpawner>();
		environmentSpawner = GetComponent<EnvironmentSpawner>();
		tileSelector = GameObject.FindGameObjectWithTag("Selector").GetComponent<TileSelector>();

		map_manager.Init(this);
        enemySpawner.Init(map_manager);
		tileSelector.Init(map_manager);
        //environmentSpawner.Init(map_manager);

		localPlayer = map_manager.instantiate_randomly(playerPrefab).GetComponent<Player>();
		Camera.main.GetComponent<CameraControl>().SetTarget(localPlayer.gameObject);
		UI_BattleMenu.SetActButtons(localPlayer.getActions());

		turn_manager = GetComponent<TurnManager>();
		turn_manager.Init(this);
    }

	void DeInit()
	{
		turn_manager.Terminate();
		map_manager.clear_map();
	}

    void Update()
	{
		foreach (char key in Input.inputString) {

			localPlayer.RespondToKeyboardInput(key);

			switch (key) {
			case 'r':
				DeInit();
				Init();
				break;
			}
        }

		if (Input.GetMouseButtonDown(1)) {
			switch (tileSelector.mode) {
				case "MOVE":
					if (tileSelector.hoveringValidMoveTile()) {
						map_manager.move(
							localPlayer.grid_pos, tileSelector.grid_position);
						tileSelector.mode = "NONE";
					}
					break;
				case "ACT":
					if (tileSelector.hoveringValidActTile()) {
						map_manager.attack(
							tileSelector.grid_position, localPlayer.stats.DealDamage());
						tileSelector.mode = "NONE";
					}
					break;
				case "NONE": 
					break;
			}
		}
	}
	
    public static void MovePlayer() {
		if (instance.localPlayer.taking_action()) return;
		
		if (instance.tileSelector.mode == "MOVE")
			instance.tileSelector.mode = "NONE";
		else
			instance.tileSelector.mode = "MOVE";
    }

	private static int last_action = -1;
	public static void ActionPlayer(int action) {
		if (instance.localPlayer.taking_action()) return;
		
        //instance.tileSelector.setMode(instance.localPlayer.getActionMode(action));
		if (instance.tileSelector.mode == "ACT" && action == last_action)
			instance.tileSelector.mode = "NONE";
		else
			instance.tileSelector.mode = "ACT";
		
		last_action = action;
    }
	
	public static void ClearPlayerAction() {
		instance.tileSelector.mode = "NONE";
	}
	
	public static void WaitPlayer() {
		if (instance.localPlayer.taking_action()) return;
		
        instance.localPlayer.wait();
    }

    public static void PotionPlayer() {
		if (instance.localPlayer.taking_action()) return;
		
        instance.localPlayer.potion();
    }
	
	public void kill(GameAgent character)
	{
		map_manager.de_instantiate(character.grid_pos);
	}
}
