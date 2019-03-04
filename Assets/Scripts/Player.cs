using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class Player : GameAgent
{
	private MapManager map_manager; // reference to MapManager instance with map data
	private TileSelector tile_selector; // reference to map tile selector
	
	// private reference to position in map grid
	private Pos grid_pos;
	private bool moving = false;
	private int current_map_iteration = -1;
	
	private int move_budget;
	private int health = 100;
	private bool player_turn = false;
	public float speed;
	
    // Gets references to necessary game components
    public override void init_agent(Pos position)
    {
        tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
		map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();
		grid_pos = position;
    }

	// if right mouse button is pressed, move player model to hover position
	// if hover position is on a bridge tile, change the player model
    void Update()
    {
        if (map_manager.map_ready) {
			
			if (Input.GetMouseButtonDown(1) && !moving) {
				if (map_manager.move(grid_pos, tile_selector.grid_position)) {
					grid_pos = tile_selector.grid_position;
				}
			}
		}
    }
	
	public override void take_damage(int amount)
	{
		health -= amount;
	}
	
	public override void take_turn()
	{
		player_turn = true;
	}
	
	public override IEnumerator smooth_movement(List<Pos> path)
	{
		moving = true;
		
		Vector3 origin, target;
		foreach(Pos step in path) {
			origin = transform.position;
			target = map_manager.grid_to_world(step);
			float dist = Vector3.Distance(origin, target);
			float time = 0f;
			while(time < 1f && dist > 0f) {
				
				time += (Time.deltaTime * speed) / dist;
				transform.position = Vector3.Lerp(origin, target, time);
				yield return null;
			}
		}
		transform.position = map_manager.grid_to_world(path[path.Count - 1]);

		moving = false;
	}
	
}
