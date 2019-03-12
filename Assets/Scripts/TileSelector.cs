using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class TileSelector : MonoBehaviour
{
	private int width;
	private int height;
	private float cell_size;
	private Vector3 offset;
	
	private Transform select_square;
	private MapManager map_manager;
	private LineRenderer path_render;
	private Player player_main;
	
	private int[,] map;
	
	public Vector3 hover_position;
	public Pos grid_position;
	
	// called by the mapGenerator script
	public void init_tile_selector(int[,] map)
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		cell_size = config.cell_size;
		width = config.width;
		height = config.height;
		offset = config.GetOffset();
		
		this.map = map;
		this.map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();
		this.path_render = GetComponent<LineRenderer>();
		
		Vector3 collider_size = new Vector3(width * cell_size, 0.1f, height * cell_size);
		BoxCollider selection_collider = GetComponent<BoxCollider>();
		selection_collider.size = collider_size;
		
		select_square = transform.Find("Selector");
	}
	
	// called by the Player script
	public void setPlayer(Player p) 
	{
		this.player_main = p;
		grid_position = p.grid_pos;
	}
	
	void Update()
	{
		if (player_main.grid_pos != null) {
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				Vector3 diff = hit.point + offset;
				int hitx = (int) (diff.x / cell_size);
				int hity = (int) (diff.z / cell_size);
				// subtract one half of a cell's length so that the square will be centered
				if (hitx >= 0 && hitx < width && hity >= 0 && hity < height) {
					if (map_manager.IsTraversable(new Pos(hitx, hity))) {
						
						Pos test_grid_position = new Pos(hitx, hity);
						
						if (grid_position != test_grid_position)
							grid_position = test_grid_position;
							
						if (grid_position != player_main.grid_pos && !player_main.moving)
							render_path_line(player_main.grid_pos, grid_position);
						
						hover_position = map_manager.grid_to_world(grid_position);
						select_square.gameObject.SetActive(true);
						select_square.position = hover_position;
					}
					else {
						select_square.gameObject.SetActive(false);
					}
				}
			}
		}
	}
	
	void render_path_line(Pos source, Pos dest)
	{
		List<Pos> path = map_manager.get_path(source, dest);
		Vector3[] path_verts = new Vector3[path.Count];
		
		for(int i = 0; i < path.Count; i++) {
			path_verts[i] = map_manager.grid_to_world(path[i]) + Vector3.up * 0.2f;
		}
		
		path_render.positionCount = path.Count;
		path_render.SetPositions(path_verts);
	}
	
	public void clear_path_line()
	{
		path_render.positionCount = 0;
		
		Vector3[] blank = new Vector3[0];
		path_render.SetPositions(blank);
	}
}
