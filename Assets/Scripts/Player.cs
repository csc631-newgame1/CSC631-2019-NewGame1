using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class Player : MonoBehaviour
{
	private MapGenerator map_instance; // reference to MapGenerator instance with map data
	private TileSelector tile_selector; // reference to map tile selector
	
	private Pos grid_pos;
	private bool moving = false;
	
    // Gets references to necessary game components
    void Start()
    {
        tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
		map_instance = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
    }

	// if right mouse button is pressed, move player model to hover position
	// if hover position is on a bridge tile, change the player model
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && map_instance.map != null && !moving) {
			
			grid_pos = tile_selector.grid_position;
			StartCoroutine(smooth_movement(transform.position, tile_selector.hover_position));
		}
    }
	
	IEnumerator smooth_movement(Vector3 origin, Vector3 target)
	{
		float speed = 30f;
		float distance_to_target = Vector3.Distance(origin, target);
		float time = 0f;
		
		moving = true;
		transform.LookAt(target);
		
		while(time < Mathf.PI / 2f) {
			
			time += (Time.deltaTime * speed) / distance_to_target;
			transform.position = Vector3.Lerp(origin, target, Mathf.Sin(time));
			yield return null;
		}
		
		transform.position = target;
		moving = false;
	}
}
