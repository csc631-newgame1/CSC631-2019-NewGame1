using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;
using System;

public class MapManager : MonoBehaviour
{
	public class MapCell {
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
    public MapCell[,] map;
	private NavigationHandler nav_map;

	private GameManager parentManager = null;
	private TileSelector tileSelector = null;

    // called by gamemanager, initializes map components
    public void Init(GameManager parent)
	{
		parentManager = parent;

		// begin component init
		GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
		if (mapObject == null)
			mapObject = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);

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
	
	// set all map configuration variables
	private void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.width = config.width;
		this.height = config.height;
		this.cell_size = config.cell_size;
		this.offset = config.GetOffset();
	}
	
	/*****************/
	/* MAP FUNCTIONS */
	/*****************/

	// instantiates an agent into the map at a random position
	public GameObject instantiate_randomly(GameObject type)
	{
		System.Random rng = new System.Random(1);

		int x = rng.Next(0, width - 1);
		int y = rng.Next(0, height - 1);

		while (!IsWalkable(new Pos(x, y))) {
			x = rng.Next(0, width - 1);
			y = rng.Next(0, height - 1);
		}

		return instantiate(type, new Pos(x, y));
	}


	// instantiates an agent into the map
    public GameObject instantiate(GameObject prefab, Pos pos, GameAgentStats stats = null, string name = null)
	{
		if (!IsWalkable(pos)) return null;
		
		GameObject clone = Instantiate(prefab, grid_to_world(pos), Quaternion.identity);
		GameAgent agent = clone.GetComponent<GameAgent>();
        string[] names = new string[] { "Keawa", "Benjamin", "Diana", "Jerry", "Joe" };

        if (stats == null) {
            agent.init_agent(pos, new GameAgentStats(CharacterRaceOptions.Human, CharacterClassOptions.Knight, 1, CharacterClassOptions.Sword), names[UnityEngine.Random.Range(0, names.Length)]);
        } else {
            agent.init_agent(pos, stats);
        }

		nav_map.removeTraversableTile(pos);
		map[pos.x, pos.y].resident = agent;
		map[pos.x, pos.y].occupied = true;
		return clone;
	}

	// removes an agent from the map, destroying it's game object
	public void de_instantiate(Pos pos)
	{
		Destroy(map[pos.x, pos.y].resident.gameObject, 5.0f);

		nav_map.insertTraversableTile(pos);
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
	
	// move a character from source to dest
	// truncates the path to the character's move_budget, if necessary
	public bool move(Pos source, Pos dest)
	{
		if (!map[source.x, source.y].occupied) return false;
		
		GameAgent agent = map[source.x, source.y].resident;
		Path path = get_path(source, dest);
		
		if (!path.empty()) {
			
			path.truncateTo(agent.move_budget);
			Pos destPos = path.endPos();
			if (!IsWalkable(destPos)) return false;
			
			nav_map.removeTraversableTile(destPos);
			map[destPos.x, destPos.y].occupied = true;
			map[destPos.x, destPos.y].resident = agent;
			
			nav_map.insertTraversableTile(source);
			map[source.x, source.y].occupied = false;
			map[source.x, source.y].resident = null;
			
			StartCoroutine(agent.smooth_movement(path.getPositions()));
			return true;
		}
		else return false;
	}
	
	// applies damage to agent at position, if one is there
	public bool attack(Pos dest, int damage_amount)
	{
		if (!IsOccupied(dest))
			return false;

		map[dest.x, dest.y].resident.take_damage(damage_amount);
		return true;
	}
	
	/*************************/
	/* PATHFINDING FUNCTIONS */
	/*************************/
	// NOTE: if you're trying to find multiple paths, opt for get_paths or getDistances, which run much more efficiently

	// get a single path from source to dest
	public Path get_path(Pos source, Pos dest)
	{
		if (!IsWalkable(source)) nav_map.insertTraversableTile(source);
		
		Path result = new Path(nav_map.shortestPath(source, dest));
		
		if (!IsWalkable(source)) nav_map.removeTraversableTile(source);
		
		return result;
	}
	
	// get the distance of a single path from source to dest
	public int getDistance(Pos source, Pos dest)
	{
		return get_path(source, dest).distance();
	}

	// Gets a list of paths from a source point to a number of destination points
	/* <param name="source"> 
	 * 		the origin point that paths are searched relative to </param>
	 * <param name="destinations">
	 * 		the list of destination points we want to find paths to </param>
	 * <param name="preserve_null"> 
	 * 		when a path is not found, by default it is not added as an entry to the results list
	 *		when preserve_null is set to true, distances that are not found are instead added as -1 </param>
	 * <param name="maxDistance">
	 *		the maximum allowed distance for resulting paths. By default this value is zero, which means there is no limit to distance
	 *		enabling this can significantly improve pathfinding performance </param>
	 * <returns>
	 * A list of paths from the source to each of the destination points </returns> */
	public List<Path> get_paths(Pos source, List<Pos> destinations, bool preserve_null = false, int maxDistance = 0)
	{
		List<List<Pos>> results = null;
		
		if (!IsWalkable(source)) nav_map.insertTraversableTile(source);
			if (maxDistance == 0)
				results = nav_map.shortestPathBatched(source, destinations);
			else
				results = nav_map.shortestPathBatchedInRange(source, destinations, maxDistance);
		if (!IsWalkable(source)) nav_map.removeTraversableTile(source);
		
		if (preserve_null) {
			List<List<Pos>> new_results = new List<List<Pos>>();
			int i = 0, j = 0;
			while (i < destinations.Count) {
				if (j < results.Count && destinations[i] == results[j].Last()) {
					new_results.Add(results[j]);
					j++;
				}
				else {
					new_results.Add(null);
				}
				i++;
			}
			results = new_results;
		}
		
		List<Path> paths = new List<Path>();
		foreach (List<Pos> result in results)
			paths.Add(new Path(result));
		
		return paths;
	}
	
	// Gets a list of map distances from a source point to a number of destination points
	/* <param name="source"> 
	 * 		the origin point that paths are searched relative to </param>
	 * <param name="destinations">
	 * 		the list of destination points we want to find distances to </param>
	 * <param name="preserve_null"> 
	 * 		when a path is not found, by default it is not added as an entry to the results list
	 *		when preserve_null is set to true, paths that are not found are instead added as null entries </param>
	 * <param name="maxDistance">
	 *		the maximum allowed distance for resulting distances. By default this value is zero, which means there is no limit to distance </param>
	 *		enabling this can significantly improve pathfinding performance
	 * <returns>
	 * A list of distances from the source to each o the destination points </returns> */
	public List<int> getDistances(Pos source, List<Pos> destinations, bool preserve_null = false, int maxDistance=0)
	{
		// getDistances will ignore whether or not the destination tiles are traversable, just gets distances to them
		foreach (Pos dest in destinations) { if (!IsWalkable(dest)) nav_map.insertTraversableTile(dest); }
		
		List<Path> paths = get_paths(source, destinations, preserve_null, maxDistance);
		
		foreach (Pos dest in destinations) { if (!IsWalkable(dest)) nav_map.removeTraversableTile(dest); }
		
		if (paths.Count == 0) return null;
		
		List<int> distances = new List<int>();
		foreach (Path path in paths) {
			distances.Add(path.distance());
		}
		return distances;
	}
	
	/*********************/
	/* UTILITY FUNCTIONS */
	/*********************/

	// returns true if tile terrain at position is traversable
    public bool IsTraversable(Pos pos)
	{
		if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
			return false;
		return map[pos.x, pos.y].traversable;
    }

	// returns true if tile at position contains an agent
    public bool IsOccupied(Pos pos) {
        if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
			return false;
		return map[pos.x, pos.y].occupied;
    }
	
	// wrapper function, return true if tile at position is traversable AND not occupied
	public bool IsWalkable(Pos pos)
	{
		return IsTraversable(pos) && !IsOccupied(pos);
	}

    public GameAgentState GetGameAgentState(Pos dest) {
        if (!IsOccupied(dest))
            return GameAgentState.Null;

        return map[dest.x, dest.y].resident.stats.currentState;
    }

    public bool GetHealed(Pos dest, int healAmount) {
        if (!IsOccupied(dest))
            return false;

        map[dest.x, dest.y].resident.GetHealed(healAmount);
        return true;
    }
	
	// gets the transform of agent at position on map, if there is any
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

    // converts grid position (int)(x, y) to world coordinates (float)(x, y, z)
	public Vector3 grid_to_world(Pos pos)
	{
		return new Vector3(pos.x * cell_size + cell_size / 2f, 0f, pos.y * cell_size + cell_size / 2f) - offset;
	}

	// converts world position (float)(x, y, z) to grid position (int)(x, y)
	public Pos world_to_grid(Vector3 pos)
	{
		pos = pos + offset;
		return new Pos((int) pos.x, (int) pos.z);
	}
	
	/*******************/
	/* DEBUG FUNCTIONS */
	/*******************/
	
	// draws line from point a to point b on the map
	public void DrawLine(Pos a, Pos b, Color color, float time=5.0f)
	{
		Vector3 origin = grid_to_world(a);
		Vector3 destination = grid_to_world(b);
		
		Debug.DrawLine(origin, destination, color, time);
	}
}
