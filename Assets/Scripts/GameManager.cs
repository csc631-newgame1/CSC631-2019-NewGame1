using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private GameObject map;
	private MapGenerator map_gen;
	private MapManager map_manager;
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
		//map_manager.init(map_gen.map);
		//map_manager.instantiate_randomly(player);

        enemySpawner = GetComponent<EnemySpawner>();
        enemySpawner.init();
        points = enemySpawner.GeneratePoints();
        DebugPrint();
    }

    void OnDrawGizmos() {
        if (points != null) {
            foreach (Vector2 point in points) {
                Gizmos.DrawSphere(point, displayRadius);
            }
        }
    }

    public void DebugPrint() {
        Debug.Log("Points contents:\n");
        if (points != null) {
            foreach (Vector2 point in points) {
                Debug.Log(point);
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
