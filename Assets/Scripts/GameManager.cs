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
        spawnZones = enemySpawner.GenerateSpawnZones();
    }

    void OnDrawGizmos() {
        List<Color> gizColors = new List<Color> {Color.red, Color.yellow, Color.blue, Color.cyan, Color.green, Color.white, Color.grey};
        if (debugEnemySpawnZones) {
            if (spawnZones != null) {
                for(int i=0; i<spawnZones.Count-1; i++) {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(map_manager.grid_to_world(new MapUtils.Pos((int)spawnZones[i].GetPosition().x, (int)spawnZones[i].GetPosition().y)), spawnZones[i].GetRadius());
                    List<Vector3> zoneTiles = spawnZones[i].GetZoneTiles();
                    foreach(Vector3 tile in zoneTiles) {
                        Gizmos.color = gizColors[i % gizColors.Count];
                        Gizmos.DrawWireCube(map_manager.grid_to_world(new MapUtils.Pos((int)tile.x, (int)tile.y)), new Vector3(mapConfiguration.cell_size, 0, mapConfiguration.cell_size));
                    }
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
