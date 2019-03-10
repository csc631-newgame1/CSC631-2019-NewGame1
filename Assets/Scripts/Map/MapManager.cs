using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class MapManager : MonoBehaviour
{
	// config variables
	private int width;
	private int height;
	private float cell_size;
	private Vector3 offset;
	
	// map data
	private int[,] map_raw;
    private MapCell[,] map;
	private NavigationHandler nav_map;
	
	private void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.width = config.width;
		this.height = config.height;
		this.cell_size = config.cell_size;
		this.offset = config.GetOffset();
	}
	
	// called by gamemanager once map is done being generated
	public void init(int[,] map_raw)
	{
		set_config_variables();
		this.map_raw = map_raw;
		map = new MapCell[width, height];
		
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map[x, y] = new MapCell(traversable(map_raw[x, y]));
			}
		}
		
		nav_map = new NavigationHandler(map_raw);
	}
	
	public GameObject instantiate_randomly(GameObject type)
	{
		System.Random rng = new System.Random(0);
		
		int x = rng.Next(0, width - 1);
		int y = rng.Next(0, height - 1);
		
		while (!map[x, y].traversable) {
			x = rng.Next(0, width - 1);
			y = rng.Next(0, height - 1);
		}
		
		return instantiate(type, new Pos(x, y));
	}

    public GameObject instantiate(GameObject prefab, Pos pos, GameAgentStats stats = null)
	{
		GameObject clone = Instantiate(prefab, grid_to_world(pos), Quaternion.identity);
		GameAgent agent = clone.GetComponent<GameAgent>();

        if (stats == null) {
            agent.init_agent(pos, new GameAgentStats(GameAgentType.Player));
        } else {
            agent.init_agent(pos, stats);
        }

		map[pos.x, pos.y].resident = agent;
		map[pos.x, pos.y].occupied = true;
		return clone;
	}
	
	// destroys all game objects currently on the map
	public void clear_map()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (map[x, y].occupied)
					Destroy(map[x, y].resident.gameObject);
			}
		}
	}
	
	public bool move(Pos source, Pos dest)
	{
		if (!map[source.x, source.y].occupied 
		 || map[dest.x, dest.y].occupied 
		 || !map[dest.x, dest.y].traversable)
			return false;
		
		List<Pos> path = nav_map.find_shortest_path(source, dest);
		GameAgent agent = map[source.x, source.y].resident;
		
		map[dest.x, dest.y].occupied = true;
		map[dest.x, dest.y].resident = agent;
		map[source.x, source.y].occupied = false;
		map[source.x, source.y].resident = null;
		
		StartCoroutine(agent.smooth_movement(path));
		return true;
	}

    public bool IsTraversable(Pos pos) {
        if (pos.x < 0 || pos.x >= map.GetLength(0) || pos.y < 0 || pos.y >= map.GetLength(1)) {
            return false;
        }
        return map[pos.x, pos.y].traversable;
    }
	
	public bool attack(Pos dest, int damage_amount)
	{
		if (!map[dest.x, dest.y].occupied)
			return false;
		
		map[dest.x, dest.y].resident.take_damage(damage_amount);
		return true;
	}
	
	public Vector3 grid_to_world(Pos pos)
	{
		return new Vector3(pos.x * cell_size + cell_size / 2f, 0f, pos.y * cell_size + cell_size / 2f) - offset;
	}
	
	public Pos world_to_grid(Vector3 pos)
	{
		pos = pos + offset;
		return new Pos((int) pos.x, (int) pos.z);
	}
}
