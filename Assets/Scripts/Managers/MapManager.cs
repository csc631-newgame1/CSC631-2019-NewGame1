using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;
using System;

public class MapManager : MonoBehaviour
{
	private class MapCell {
        public bool traversable;
        public bool occupied;
        public GameAgent resident;
        public MapCell(bool traversable) {
            this.traversable = traversable;
            occupied = false;
            resident = null;
        }
    }

	public GameObject mapPrefab;

	// config variables
	private int width;
	private int height;
	private float cell_size;
	private Vector3 offset;

	// map data
	private int[,] map_raw;
    private MapCell[,] map;
	private NavigationHandler nav_map;

	private GameManager parentManager = null;
	private TileSelector tileSelector = null;

	private void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.width = config.width;
		this.height = config.height;
		this.cell_size = config.cell_size;
		this.offset = config.GetOffset();
	}

    // called by gamemanager
    public void Init(GameManager parent)
	{
		parentManager = parent;

		// begin component init
		GameObject mapObject = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
		map_raw = mapObject.GetComponent<MapGenerator>().generate_map();

		nav_map = new NavigationHandler(map_raw);

		set_config_variables();
		map = new MapCell[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map[x, y] = new MapCell(traversable(map_raw[x, y]));
			}
		}

		tileSelector = mapObject.transform.Find("TileSelector").GetComponent<TileSelector>();
		tileSelector.init_tile_selector(map_raw);
	}

	public GameObject instantiate_randomly(GameObject type)
	{
		System.Random rng = new System.Random(1);

		int x = rng.Next(0, width - 1);
		int y = rng.Next(0, height - 1);

		while (!map[x, y].traversable) {
			x = rng.Next(0, width - 1);
			y = rng.Next(0, height - 1);
		}

		return instantiate(type, new Pos(x, y));
	}

    public GameObject instantiate(GameObject prefab, Pos pos, GameAgentStats stats = null, string name = null)
	{
		GameObject clone = Instantiate(prefab, grid_to_world(pos), Quaternion.identity);
		GameAgent agent = clone.GetComponent<GameAgent>();
        string[] names = new string[] { "Keawa", "Benjamin", "Diana", "Jerry", "Joe" };

        if (stats == null) {
            agent.init_agent(pos, new GameAgentStats(CharacterRaceOptions.Human, CharacterClassOptions.Knight, 1, CharacterClassOptions.Sword), names[UnityEngine.Random.Range(0, names.Length)]);
        } else {
            agent.init_agent(pos, stats);
        }

		map[pos.x, pos.y].resident = agent;
		map[pos.x, pos.y].occupied = true;
		return clone;
	}

	public void de_instantiate(Pos pos)
	{
		Destroy(map[pos.x, pos.y].resident.gameObject);

		map[pos.x, pos.y].resident = null;
		map[pos.x, pos.y].occupied = false;
	}

	// destroys all game objects currently on the map
	public void clear_map()
	{
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				if (map[x, y].resident != null)
					Destroy(map[x, y].resident.gameObject);
	}

	public List<Pos> get_path(Pos source, Pos dest)
	{
		return nav_map.shortestPath(source, dest);
	}

	public List<List<Pos>> get_paths(Pos source, List<Pos> targetPositions)
	{
		return nav_map.shortestPathBatched(source, targetPositions);
	}

	public bool move(Pos source, Pos dest)
	{
		if (!map[source.x, source.y].occupied
		 || map[dest.x, dest.y].occupied
		 || !map[dest.x, dest.y].traversable)
			return false;

		List<Pos> path = get_path(source, dest);
		GameAgent agent = map[source.x, source.y].resident;

		map[dest.x, dest.y].occupied = true;
		map[dest.x, dest.y].resident = agent;
		map[source.x, source.y].occupied = false;
		map[source.x, source.y].resident = null;

		StartCoroutine(agent.smooth_movement(path));
		return true;
	}

    public bool IsTraversable(Pos pos)
	{
		//Debug.Log("Ok, testing, map value is " + map[pos.x, pos.y].traversable);
		if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
			return false;
		return map[pos.x, pos.y].traversable;
    }

    public bool IsOccupied(Pos pos) {
        if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
			return false;
		return map[pos.x, pos.y].occupied;
    }

	public bool attack(Pos dest, int damage_amount)
	{
		if (!IsOccupied(dest))
			return false;

		map[dest.x, dest.y].resident.take_damage(damage_amount);
		return true;
	}

    public Transform GetUnitTransform(Pos pos) {
        if (!map[pos.x, pos.y].occupied)
            return null;
        return map[pos.x, pos.y].resident.transform;
    }

    public Transform GetNearestUnitTransform(Pos pos, List<Pos> agents) {

        if (agents.Count > 0) {
            int minDistance = Int32.MaxValue;
            Pos closestAgent = agents[0];

            foreach (Pos agent in agents) {
                int distance = Pos.abs_dist(pos, agent);
                if (distance < minDistance) {
                    closestAgent = agent;
                    minDistance = distance;
                }
            }
            return map[closestAgent.x, closestAgent.y].resident.transform;
        }

        return null;
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
