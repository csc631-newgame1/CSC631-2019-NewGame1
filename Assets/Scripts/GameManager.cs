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
    public float displayRadius;

    public GameAgent player;
    public bool debugEnemySpawnZones = true;
	
    void Start()
    {
		map = GameObject.FindGameObjectWithTag("Map");
		map_gen = map.GetComponent<MapGenerator>();
        map_gen.generate_map();
		
		map_manager = map.GetComponent<MapManager>();
        map_manager.init(map_gen.map);
        map_manager.instantiate_randomly(player);

        mapConfiguration = map.GetComponent<MapConfiguration>();
        displayRadius = mapConfiguration.cell_size * Mathf.Sqrt(2);

        enemySpawner = GetComponent<EnemySpawner>();
        enemySpawner.Init(map_manager);
        spawnZones = enemySpawner.GeneratePoints();
    }

    void OnDrawGizmos() {
        if (debugEnemySpawnZones) {
            if (spawnZones != null) {
                foreach (SpawnZone zone in spawnZones) {
                    Gizmos.DrawSphere(map_manager.grid_to_world(new MapUtils.Pos((int)zone.GetPosition().x, (int)zone.GetPosition().y)), zone.GetRadius());
                }
            }
        }
    }

    void Update()
	{
		if (Input.GetKeyDown("r")) {
			map_gen.generate_map();
			
			map_manager.clear_map();
			map_manager.init(map_gen.map);
			map_manager.instantiate_randomly(player);
		}
	}
}
