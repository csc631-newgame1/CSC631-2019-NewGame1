using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private GameObject map;
	private MapGenerator map_gen;
	private MapManager map_manager;

    private MapConfiguration mapConfiguration;
    private EnemySpawner enemySpawner;

    private List<SpawnZone> spawnZones;

    public GameObject player;
    public GameObject enemyPrefab;

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
        GameObject clone = map_manager.instantiate_randomly(player);
		Camera.main.GetComponent<CameraControl>().SetTarget(clone);

        mapConfiguration = map.GetComponent<MapConfiguration>();

        enemySpawner = GetComponent<EnemySpawner>();
        SpawnEnemies();
    }

    void SpawnEnemies() {
        // Create random TestEnemies
        List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
        for (int groupIndex=0; groupIndex < 15; groupIndex++) {
            List<EnemyGroupDescription> enemyGroupDescriptions = new List<EnemyGroupDescription>();

            for (int enemyPropertyIndex=0; enemyPropertyIndex<Random.Range(1,10); enemyPropertyIndex++) {
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(10f, 10f, 4f, 4f),
                                                                 Random.Range(1, 2), 0.5f, 0.5f, 0.5f, 0.5f));
                
            }

            enemyGroups.Add(new EnemyGroup(enemyGroupDescriptions, Distribution.Balanaced));
        }

        enemySpawner.Init(map_manager, mapConfiguration);
        enemySpawner.SpawnEnemies(ref map_manager);
        enemySpawner.ShowEnemySpawnZones(showEnemySpawnZones);

        EnemyGroupManager enemyGroupManager = new EnemyGroupManager(enemyGroups, enemySpawner.GetSpawnZones());
        List<GameAgent> enemies = enemyGroupManager.GetEnemiesToSpawn();
        //GameObject enemyPrefab = (GameObject)Resources.Load("prefabs/TestEnemy", typeof(GameObject));
        foreach (GameAgent enemy in enemies) {
            GameObject clone = map_manager.instantiate(enemyPrefab, enemy.grid_pos);
        }
    }

    void Update()
	{
		if (Input.GetKeyDown("r")) {
			map_gen.generate_map();
			
			map_manager.clear_map();
			map_manager.init(map_gen.map);
			GameObject clone = map_manager.instantiate_randomly(player);
			Camera.main.GetComponent<CameraControl>().SetTarget(clone);

            SpawnEnemies();
		}
	}

    private void OnValidate() {
        if (enemySpawner != null) {
            enemySpawner.ShowEnemySpawnZones(showEnemySpawnZones);
        }
    }
}
