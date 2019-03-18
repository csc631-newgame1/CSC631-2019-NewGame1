using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class TileSelector : MonoBehaviour
{
	private class Path
	{
		List<Pos> path_raw;
		
		public Path(List<Pos> _path)
		{
			this.path_raw = _path;
		}
		
		public List<Pos> getPath() {
			return path_raw;
		}
		
		public Pos startPos() {
			return path_raw[0];
		}
		
		public Pos endPos() {
			return path_raw[path_raw.Count - 1];
		}
		
		// Gets the distance of the shortest path to the tiles
		public int distance() 
		{
			int distance = 0;
			for (int i = 1; i < path_raw.Count; i++)
				distance += Pos.abs_dist(path_raw[i], path_raw[i-1]);
			return distance;
		}
	}
	
	private int width;
	private int height;
	private float cell_size;
	private Vector3 offset;

    private Transform select_square;
	private MapManager map_manager;
	private LineRenderer path_render;
	private Player player_main;

    private List<Path> selectableMovementTiles;

    private int[,] map;

    public bool showPathLine = false;
    public bool showSelectableTiles = false;

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
        if (showSelectableTiles) {
            // TODO Consider showing selectable tiles to the user here
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				
				Vector3 diff = hit.point + offset;
				int hitx = (int)(diff.x / cell_size);
				int hity = (int)(diff.z / cell_size);
				Pos hitp = new Pos(hitx, hity);
				
				// subtract one half of a cell's length so that the square will be centered
				if (Pos.in_bounds(hitp, width, height) && hitp != grid_position) {
					
					Path hit_path = getSelectableTilePath(hitp);
					
					if (hit_path != null) {
						
						if (hitp != player_main.grid_pos && !player_main.moving && showPathLine)
							render_path_line(hit_path);

						hover_position = map_manager.grid_to_world(hitp);
						select_square.gameObject.SetActive(true);
						select_square.position = hover_position;
						grid_position = hitp;
						
					} else {
						select_square.gameObject.SetActive(false);
					}
				}
            }
        }
	}
	
	void render_path_line(Path path)
	{
		List<Pos> path_raw = path.getPath();
		Vector3[] path_verts = new Vector3[path_raw.Count];
		
		for(int i = 0; i < path_raw.Count; i++) {
			path_verts[i] = map_manager.grid_to_world(path_raw[i]) + Vector3.up * 0.2f;
		}
		
		path_render.positionCount = path_raw.Count;
		path_render.SetPositions(path_verts);
	}
	
	public void clear_path_line()
	{
		path_render.positionCount = 0;
		
		Vector3[] blank = new Vector3[0];
		path_render.SetPositions(blank);
	}
	
    // Creates a list of all selectable tiles within a given radius of a position
    // Consider turning this static for enemy AI if this is the only method they need from this class
    public void CreateListOfSelectableTiles(Pos position, int move_budget, GameAgentAction action) 
	{	
        selectableMovementTiles = new List<Path>();
		
		int startx 	= position.x - move_budget >= 0 ? position.x - move_budget : 0;
		int endx	= position.x + move_budget < width ? position.x + move_budget : width - 1;
		
		int starty 	= position.y - move_budget >= 0 ? position.y - move_budget : 0;
		int endy	= position.y + move_budget < height ? position.y + move_budget : height - 1;

        if (action == GameAgentAction.Move) {
            for (int x = startx; x <= endx; x++) {
			    for (int y = starty; y <= endy; y++) {
				
				    Pos candidate = new Pos(x, y);
                    if (map_manager.IsTraversable(candidate) && candidate != position && Pos.abs_dist(position, candidate) <= move_budget) {

                        Path path = new Path(map_manager.get_path(position, candidate));
                        if (path.distance() <= move_budget) {
                            selectableMovementTiles.Add(path);
                        }
                    }
                }
			}
		}
        // TODO figure out if you are going to create a new method that doesn't fuck around with the List<Path>
        if (action == GameAgentAction.MeleeAttack) {
            for (int x = startx; x <= endx; x++) {
                for (int y = starty; y <= endy; y++) {

                    Pos candidate = new Pos(x, y);
                    if (map_manager.IsOccupied(candidate) && candidate != position && Pos.abs_dist(position, candidate) <= move_budget) {

                        Path path = new Path(map_manager.get_path(position, candidate));
                        if (path.distance() <= move_budget) {
                            selectableMovementTiles.Add(path);
                        }
                    }
                }
            }
        }
    }

    private Path getSelectableTilePath(Pos tile_pos) 
	{
        foreach(Path path in selectableMovementTiles)
			if (path.endPos() == tile_pos)
				return path;
		return null;
    }
	
	void OnDrawGizmos() {
        if (showSelectableTiles) {
            if (selectableMovementTiles.Count > 0) {
                foreach (Path path in selectableMovementTiles) {
					Pos tile = path.endPos();
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(map_manager.grid_to_world(new Pos(tile.x, tile.y)), new Vector3(1f, 0, 1f));
                }
            }
        }
    }
}
