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
    private List<Pos> selectableActTiles;
    private List<Pos> nonselectableActTiles;

    private int[,] map;

    public bool showPathLine = false;
    public bool showSelectableMoveTiles = false;
    public bool showSelectableActTiles = false;

    public Vector3 hover_position;
	public Pos grid_position;
	
	public Mesh tileMesh;
	public Material moveableTilesMaterial;
	public Material selectableTilesMaterial;
    public Material nonselectableTilesMaterial;


    // called by the mapGenerator script
    public void init_tile_selector(int[,] map)
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		cell_size = config.cell_size;
		width = config.width;
		height = config.height;
		offset = config.GetOffset();
		
		this.map = map;
		this.map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
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
		DrawTiles();
		
		RaycastHit hit;
		Pos hitp = new Pos(-1, -1);
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out hit)) {
			
			Vector3 diff = hit.point + offset;
			hitp = new Pos((int)(diff.x / cell_size), (int)(diff.z / cell_size));
			
			if (map_manager.IsTraversable(hitp) && Pos.in_bounds(hitp, width, height)) {
				
				if (hitp != grid_position) {
					
					hover_position = map_manager.grid_to_world(hitp);
					grid_position = hitp;
					select_square.gameObject.SetActive(true);
					select_square.position = hover_position;
					
					if (showSelectableMoveTiles) {
						
						Path hit_path = getSelectableTilePath(hitp);
						
						if (hit_path != null && hitp != player_main.grid_pos && !player_main.moving && showPathLine)
							render_path_line(hit_path);
					} 
				}
			}
			else {
				select_square.gameObject.SetActive(false);
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
    public void CreateListOfSelectableMovementTiles(Pos position, int move_budget, GameAgentAction action) 
	{	
        selectableMovementTiles = new List<Path>();
		
		int startx 	= position.x - move_budget >= 0 ? position.x - move_budget : 0;
		int endx	= position.x + move_budget < width ? position.x + move_budget : width - 1;
		
		int starty 	= position.y - move_budget >= 0 ? position.y - move_budget : 0;
		int endy	= position.y + move_budget < height ? position.y + move_budget : height - 1;

		List<Pos> candidates = new List<Pos>();
        if (action == GameAgentAction.Move) {
            for (int x = startx; x <= endx; x++) {
			    for (int y = starty; y <= endy; y++) {
				
				    Pos candidate = new Pos(x, y);
                    if (map_manager.IsTraversable(candidate) && candidate != position && Pos.abs_dist(position, candidate) <= move_budget) {
						candidates.Add(candidate);
                    }
                }
			}
		}
		
		List<List<Pos>> paths = map_manager.get_paths(position, candidates);
		foreach (List<Pos> rawPath in paths) {
			Path path = new Path(rawPath);
			if (path.distance() <= move_budget)
				selectableMovementTiles.Add(path);
		}
    }
	
	public bool hoveringValidMoveTile()
	{
		return getSelectableTilePath(grid_position) != null;
	}

    // Creates a list of all selectable tiles within a given radius of a position
    // Consider turning this static for enemy AI if this is the only method they need from this class
    public void CreateListOfSelectableActTiles(Pos position, int move_budget, GameAgentAction action) {
        selectableActTiles = new List<Pos>();
        nonselectableActTiles = new List<Pos>();

        int startx = position.x - move_budget >= 0 ? position.x - move_budget : 0;
        int endx = position.x + move_budget < width ? position.x + move_budget : width - 1;

        int starty = position.y - move_budget >= 0 ? position.y - move_budget : 0;
        int endy = position.y + move_budget < height ? position.y + move_budget : height - 1;

        // TODO figure out if you are going to create a new method that doesn't fuck around with the List<Path>
        if (action == GameAgentAction.MeleeAttack || action == GameAgentAction.MagicAttackSingleTarget
            || action == GameAgentAction.RangedAttack || action == GameAgentAction.RangedAttackMultiShot) {

            for (int x = startx; x <= endx; x++) {
                for (int y = starty; y <= endy; y++) {

                    Pos candidate = new Pos(x, y);
                    if (map_manager.IsOccupied(candidate) && candidate != position && Pos.abs_dist(position, candidate) <= move_budget) {
                        selectableActTiles.Add(candidate);
                    } else if (!map_manager.IsOccupied(candidate) && map_manager.IsTraversable(candidate) 
                                && candidate != position && Pos.abs_dist(position, candidate) <= move_budget) {
                        nonselectableActTiles.Add(candidate);
                    }
                }
            }
        }
    }
	
	public bool hoveringValidSelectTile()
	{
		return GetSelectableActTile(grid_position) != null;
	}

    private Path getSelectableTilePath(Pos tile_pos) 
	{
        foreach(Path path in selectableMovementTiles)
			if (path.endPos() == tile_pos)
				return path;
		return null;
    }

    private Pos GetSelectableActTile(Pos tilePos) {
        foreach (Pos tile in selectableActTiles)
            if (tile == tilePos)
                return tile;
        return null;
    }
	
	void DrawTiles() {
        if (showSelectableMoveTiles) {
			foreach (Path path in selectableMovementTiles) {
				Pos tile = path.endPos();
				Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), moveableTilesMaterial, 0);
			}
        }
        if (showSelectableActTiles) {
			foreach (Pos tile in selectableActTiles) {
				Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), selectableTilesMaterial, 0);
			}

            foreach (Pos tile in nonselectableActTiles) {
                Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), nonselectableTilesMaterial, 0);
            }
        }
    }
}
