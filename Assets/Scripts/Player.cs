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
	
	private int move_budget;
	private int health = 100;
	private bool player_turn = false;
	public float speed;
	
	Animator animator;
	
    // Gets references to necessary game components
    public override void init_agent(Pos position)
    {
        tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
		map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();
		grid_pos = position;
		animator = GetComponent<Animator>();
    }

	// if right mouse button is pressed, move player model to hover position
	// if hover position is on a bridge tile, change the player model
    void Update()
    {
		if (Input.GetMouseButtonDown(1) && !moving) {
			if (map_manager.move(grid_pos, tile_selector.grid_position)) {
				grid_pos = tile_selector.grid_position;
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
		
		animator.SetBool("Moving", true);
		
		Vector3 origin, target;
		foreach(Pos step in path) {
			origin = transform.position;
			target = map_manager.grid_to_world(step);
			float dist = Vector3.Distance(origin, target);
			float time = 0f;
			
			transform.LookAt(target);
			
			while(time < 1f && dist > 0f) {
				
				animator.SetFloat("Velocity Z", speed);
				
				time += (Time.deltaTime * speed) / dist;
				transform.position = Vector3.Lerp(origin, target, time);
				yield return null;
			}
		}
		transform.position = map_manager.grid_to_world(path[path.Count - 1]);
		
		animator.SetBool("Moving", false);

		moving = false;
	}
	
	public void FootR(){}
	public void FootL(){}
	
}
