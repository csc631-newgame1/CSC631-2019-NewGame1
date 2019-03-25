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

    private TurnManager turn_manager;

    private List<SpawnZone> spawnZones;
    [SerializeField]
    // Consider moving this to a different location for better handling of turn based gameplay
    private GameObject BattleMenu;
    public GameObject playerPrefab;

	void Start()
	{
		Init();
    }

    void Init()
    {
		map_manager = GetComponent<MapManager>();
		enemySpawner = GetComponent<EnemySpawner>();
		environmentSpawner = GetComponent<EnvironmentSpawner>();

		map_manager.Init(this);
		enemySpawner.Init(map_manager);
		environmentSpawner.Init(map_manager);

		localPlayer = map_manager.instantiate_randomly(playerPrefab).GetComponent<Player>();
		Camera.main.GetComponent<CameraControl>().SetTarget(localPlayer.gameObject);

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
			case 'm':
				ShowPlayerActionMenu();
				break;
			}
        }

		if (Input.GetMouseButtonDown(1)) {
			localPlayer.RespondToMouseClick();
		}
	}

    public void ShowPlayerActionMenu() {
        BattleMenu.SetActive(!BattleMenu.activeSelf);
        localPlayer.DeactivatePlayerActionMenu();
    }

    public void MovePlayer() {
        localPlayer.move();
    }

	public void ActPlayer() {
        localPlayer.act();
    }

    public void Action1Player() {
        localPlayer.action1();
    }

    public void Action2Player() {
        localPlayer.action2();
    }

    public void Action3Player() {
        localPlayer.action3();
    }

    public void Action4Player() {
        localPlayer.action4();
    }

    public void WaitPlayer() {
        localPlayer.wait();
    }

    public void PotionPlayer() {
        localPlayer.potion();
    }

	public void kill(GameAgent character)
	{
		map_manager.de_instantiate(character.grid_pos);
	}
}
