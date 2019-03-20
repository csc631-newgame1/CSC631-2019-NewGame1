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

    private MapConfiguration mapConfiguration;
    private EnemySpawner enemySpawner;
    private GameObject clone;

    private List<SpawnZone> spawnZones;
    [SerializeField]
    // Consider moving this to a different location for better handling of turn based gameplay
    private GameObject playerActionMenu;

    public GameObject player;
	public GameObject enemy;

    [Header("Debug Settings")]
    [Tooltip("Shows wire spheres where the Spawn Zones are mapped, and the wire cubes for the tiles within the Spawn Zone.")]
    public bool showEnemySpawnZones = true;
	
    void Start()
    {
		map = GameObject.FindGameObjectWithTag("Map");
		map_gen = map.GetComponent<MapGenerator>();
        map_gen.generate_map();
		
		map_manager = map.GetComponent<MapManager>();
        map_manager.init(map_gen.map);
        clone = map_manager.instantiate_randomly(player);
		Camera.main.GetComponent<CameraControl>().SetTarget(clone);

        mapConfiguration = map.GetComponent<MapConfiguration>();

        enemySpawner = GetComponent<EnemySpawner>();
        SpawnEnemies();
    }

    void SpawnEnemies() 
	{
        enemySpawner.Init(map_manager, mapConfiguration);
        enemySpawner.SpawnEnemies(enemy);
        enemySpawner.ShowEnemySpawnZones(showEnemySpawnZones);
    }

    void Update()
	{
		if (Input.GetKeyDown("r")) {
			map_gen.generate_map();
			
			map_manager.clear_map();
			map_manager.init(map_gen.map);
			clone = map_manager.instantiate_randomly(player);
			Camera.main.GetComponent<CameraControl>().SetTarget(clone);

            //SpawnEnemies();
		}

        if (Input.GetKeyDown("m")) {
            ShowPlayerActionMenu();
        }
	}

    private void OnValidate() {
        if (enemySpawner != null) {
            enemySpawner.ShowEnemySpawnZones(showEnemySpawnZones);
        }
    }

    public void ShowPlayerActionMenu() {
        playerActionMenu.SetActive(!playerActionMenu.activeSelf);
    }
    
    // TODO integrate these functions into whatever system Diana creates for the Turn Based Gameplay
    // the buttons need to reference an instance of the player to work, not just the prefab
    public void MovePlayer() {
        clone.GetComponent<Player>().move();
    }

    public void ActionPlayer() {
        clone.GetComponent<Player>().act();
    }

    public void WaitPlayer() {
        clone.GetComponent<Player>().wait();
    }

    public void PotionPlayer() {
        clone.GetComponent<Player>().potion();
    }
}
