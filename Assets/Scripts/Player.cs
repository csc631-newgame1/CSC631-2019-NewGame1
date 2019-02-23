using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class Player : MonoBehaviour
{
	private NavigationHandler nav_map;
	private MapGenerator map_instance; // reference to MapGenerator instance with map data
	private TileSelector tile_selector; // reference to map tile selector
	
	private Pos grid_pos;
	private static Pos INVALID = new Pos(-1, -1);
	private bool moving = false;
	private int current_map_iteration = -1;
	private int width;
	private int height;
	
	private Pos[] path;
	public int move_budget;
	public float speed;
	
    // Gets references to necessary game components
    void Start()
    {
        tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
		map_instance = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
		set_config_variables();
		grid_pos = new Pos(-1, -1);
		nav_map = null;
    }
	
	void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		width = config.width;
		height = config.height;
	}

	// if right mouse button is pressed, move player model to hover position
	// if hover position is on a bridge tile, change the player model
    void Update()
    {
        if (map_instance.map != null) {
			
			if (grid_pos == INVALID) {
				place_randomly_on_map();
			}
			
			if (current_map_iteration != map_instance.iteration) {
				
				current_map_iteration = map_instance.iteration;
				nav_map = new NavigationHandler(map_instance.map);
				place_randomly_on_map();
				set_config_variables();
				transform.position = map_instance.grid_to_world(grid_pos);
			}
			
			else if (Input.GetMouseButtonDown(1) && !moving) {
				
				Stack<Pos> path = nav_map.find_shortest_path(grid_pos, tile_selector.grid_position);
				if (path != null) {
					StartCoroutine(smooth_movement(path));
					grid_pos = tile_selector.grid_position;
				}
				
				transform.position = map_instance.grid_to_world(tile_selector.grid_position);
				
			}
		}
    }
	
	void place_randomly_on_map()
	{
		System.Random rng = new System.Random(0);
		
		int x = rng.Next(0, width - 1);
		int y = rng.Next(0, height - 1);
		
		while (map_instance.map[x, y] == EMPTY || map_instance.map[x, y] == EDGE) {
			x = rng.Next(0, width - 1);
			y = rng.Next(0, height - 1);
		}
		
		grid_pos = new Pos(x, y);
	}
	
	IEnumerator smooth_movement(Stack<Pos> path)
	{
		moving = true;
		
		Vector3 origin, target;
		while (path.Count > 0) {
			origin = transform.position;
			target = map_instance.grid_to_world(path.Pop());
			float dist = Vector3.Distance(origin, target);
			float time = 0f;
			while(time < 1f && dist > 0f) {
				
				time += (Time.deltaTime * speed) / dist;
				transform.position = Vector3.Lerp(origin, target, time);
				yield return null;
			}
		}
		
		moving = false;
	}
}
