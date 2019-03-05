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

    private List<Vector3> points;
    public float displayRadius = 1f;


    public GameAgent player;
	
    void Start()
    {
		map = GameObject.FindGameObjectWithTag("Map");
		map_gen = map.GetComponent<MapGenerator>();
        map_gen.generate_map();
		
		map_manager = map.GetComponent<MapManager>();
        map_manager.init(map_gen.map);
        map_manager.instantiate_randomly(player);

        mapConfiguration = map.GetComponent<MapConfiguration>();

        enemySpawner = GetComponent<EnemySpawner>();
        enemySpawner.Init(map_manager);
        points = enemySpawner.GeneratePoints();
        //DebugPrint();
    }

    void OnDrawGizmos() {
        if (points != null) {
            foreach (Vector3 point in points) {
                //Gizmos.DrawSphere(point - mapConfiguration.GetOffset(), displayRadius);
                Gizmos.DrawSphere(map_manager.grid_to_world(new MapUtils.Pos((int)point.x, (int)point.y)), displayRadius);
            }
        }
    }

    public void DebugPrint() {
        Debug.Log("Points contents:\n");
        if (points != null) {
            foreach (Vector3 point in points) {
                Debug.Log(point - mapConfiguration.GetOffset());
            }
        } else {
            Debug.Log("Empty");
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
