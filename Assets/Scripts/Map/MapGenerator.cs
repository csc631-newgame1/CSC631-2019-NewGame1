﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using RegionUtils;
using MapUtils;
using static MapUtils.Type;
using static MapUtils.MapConstants;

public class MapGenerator : MonoBehaviour
{
	public bool debug;
	
	private int width;
	private int height;
	private int fill_percent;
	private int smoothness;
	private int region_cull_threshold;
	private float cell_size;
    private float object_size_scale;
	private string seed;
	private Vector3 offset;
	
	[HideInInspector]
	public int[,] map;
	private List<Region> regions;
	private List<List<Cmd>> cmds;
	private Region main_region;
	
	private List<Connection> debug_connections;
	
	void set_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.width = config.width;
		this.height = config.height;
		this.fill_percent = config.fill_percent;
		this.smoothness = config.smoothness;
		this.region_cull_threshold = config.region_cull_threshold;
		this.cell_size = config.cell_size;
        this.object_size_scale = config.object_size_scale;
		this.seed = config.seed;
		this.offset = config.GetOffset();
	}

    public int[,] generate_map()
	{
		set_variables();
		map = new int[width,height];
		
		Debug.Log("Generating map data...");
		
		// binary map generation, using cellular automata method
		random_fill();
		smooth(smoothness);
		pad_edges();
		
		// map post-processing
		extract_regions();
		create_adjacency_lists();
		bridge_all_regions();

		Debug.Log("Done generating map! Creating meshes...");

		MapMeshGenerator surface_gen = transform.Find("Surface").GetComponent<MapMeshGenerator>();
		surface_gen.generate_map_mesh(map);
		
		BridgeMeshGenerator bridge_mesh_gen = transform.Find("Bridges").GetComponent<BridgeMeshGenerator>();
		bridge_mesh_gen.generate_bridge_mesh(map);
		
		Debug.Log("Mesh generation complete!");

        return map;
	}
	
	/*****************/
	/* MAP CREATION */
	/*****************/
	
	void random_fill()
	{
		int seed = 0;
		if (this.seed == string.Empty) {
			seed = Time.time.ToString().GetHashCode();
		}
		else {
			seed = this.seed.GetHashCode();
		}
		System.Random rng = new System.Random(seed);
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map[x, y] = rng.Next(0, 100) <= fill_percent ? FILLED : EMPTY;
			}
		}
	}
	
	void smooth(int smoothness)
	{
		for(int i = 0; i < smoothness; i++) {
			int[,] temp = map.Clone() as int[,];
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					temp[x, y] = neighbors(x, y) > 4 ? EMPTY : FILLED;
				}
			}
			map = temp.Clone() as int[,];
		}
	}
	
	int neighbors(int x, int y)
	{
		int n = 0;
		for (int xn = x - 1; xn <= x + 1; xn++) {
			for (int yn = y - 1; yn <= y + 1; yn++) {
				
				if (yn != y || xn != x) {
					if (yn >= 0 && yn < height && xn >= 0 && xn < width) {
						n += map[xn, yn];
					} else {
						n++;
					}
				}
			}
		}
		return n;
	}
	
	void pad_edges()
	{
		for (int x = 0; x < width; x++) {
			map[x,0] = EMPTY;
			map[x,height-1] = EMPTY;
		}
		
		for (int y = 0; y < height; y++) {
			map[0,y] = EMPTY;
			map[width-1,y] = EMPTY;
		}
	}
	
	/************************/
	/* MAP POST-PROCESSING */
	/************************/
	
	void extract_regions()
	{
		// inner regions, or pools, are regions of empty space within other regions.
		// since these pools are not actually distinct regions, only their wall geometries are extracted so that they can be properly rendered
		cmds = Region.extract_inner_regions(map, region_cull_threshold);
		
		regions = Region.extract_regions(map, region_cull_threshold);
		main_region = regions[0];
		foreach (Region region in regions) {
			if (region.count > main_region.count) {
				main_region = region;
			}
		}
		main_region.connected_to_main = true;
		
		foreach(Region region in regions) {
			cmds.Add(region.cmds);
		}
		foreach (List<Cmd> cmdlist in cmds) {
			foreach (Cmd cmd in cmdlist) {
				if (cmd.type != LINE) {
					map[cmd.pos.x, cmd.pos.y] = EDGE;
				}
			}
		}
	}
	
	// For each region, generates a list of the closest possible connection points to every other region
	void create_adjacency_lists()
	{
		foreach (Region region in regions) {
			foreach (Region candidate in regions) {
				
				if (region == candidate || region.has_connection_to(candidate))
					continue;
				
				Connection closest = null;
				int min_dist = int.MaxValue;
				int dist = 0;
				foreach (Cmd rc in region.cmds) {
					foreach (Cmd cc in candidate.cmds) {
						
						dist = Pos.abs_dist(rc.pos, cc.pos);
						if (dist < min_dist) {
							min_dist = dist;
							closest = new Connection(rc.pos, cc.pos, region, candidate);
						}
					}
				}
				region.closest.Add(closest);
				candidate.closest.Add(closest);
			}
			// sorts adjacency list by minimum distance -> maximum distance
			region.closest.Sort();
		}
	}
	
	// creates connected "chains" of nearby regions that continue until the main region (or a region connected to the main region)
	// is connected to. Performs this on all regions until they are all connected to the main region
	void bridge_all_regions()
	{
		debug_connections = new List<Connection>();
		foreach (Region region in regions) {
			
			if (region.connected_to_main)
				continue;
			
			Stack<Region> chain = new Stack<Region>();
			Stack<Region> discard = new Stack<Region>();
			chain.Push(region);
			while (!chain.Peek().connected_to_main) {
				
				Region closest = find_and_connect_closest_region(chain.Peek(), chain, discard);
				if (closest != null) {
					chain.Push(closest);
				}
				else {
					if (chain.Count > 1) {
						// backtracks if there is no bridge that doesn't overlap another region
						Region new_discard = chain.Pop();
						Region prev_region = chain.Peek();
						new_discard.connections.Remove(prev_region);
						prev_region.connections.Add(new_discard);
						discard.Push(new_discard);
					}
					else {
						break;
					}
				}
			}
			if (chain.Peek().connected_to_main) {
				foreach(Region item in chain) {
					item.connected_to_main = true;
				}
				foreach(Region item in discard) {
					item.connected_to_main = true;
				}
			}
		}
	}
	
	// finds and connects a region to its closest neighbor, provided the bridge that connects the two does not overlap any other regions
	Region find_and_connect_closest_region(Region region, Stack<Region> chain, Stack<Region> discard)
	{
		foreach (Connection candidate in region.closest) {
			Region target_reg = candidate.ID_1 == region ? candidate.ID_2 : candidate.ID_1;
			if (!chain.Contains(target_reg) && !discard.Contains(target_reg)) {
				if (create_bridge(candidate) == true) {
					debug_connections.Add(candidate);
					target_reg.connections.Add(region);
					return target_reg;
				}
			}
		}
		return null;
	}
	
	bool create_bridge(Connection conn)
	{
		Pos diff = conn.endpt2 - conn.endpt1;
		int ix = Math.Sign(diff.x);
		int iy = Math.Sign(diff.y);
		
		// preliminary check for bridge validity
		for (int x = conn.endpt1.x + ix; x != conn.endpt2.x; x += ix) {
			if (map[x, conn.endpt2.y] > FILLED) {
				return false;
			}
		}
		for (int y = conn.endpt1.y + iy; y != conn.endpt2.y; y += iy) {
			if (map[conn.endpt1.x, y] > FILLED) {
				return false;
			}
		}
		
		map[conn.endpt1.x, conn.endpt1.y] = PLATFORM;
		map[conn.endpt2.x, conn.endpt2.y] = PLATFORM;
		map[conn.endpt1.x, conn.endpt2.y] = PLATFORM;
		
		for (int x = conn.endpt1.x + ix; x != conn.endpt2.x; x += ix) {
			map[x, conn.endpt2.y] = BRIDGE;
		}
		for (int y = conn.endpt1.y + iy; y != conn.endpt2.y; y += iy) {
			map[conn.endpt1.x, y] = BRIDGE;
		}
		
		return true;
	}
	
	void clear_regions()
	{
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				if (map[x,y] > 1) {
					map[x,y] = EMPTY;
				}
			}
		}
	}
	
	void negate_map()
	{
		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				switch (map[x,y]) {
					case FILLED:
						map[x,y] = EMPTY; break;
					case EMPTY:
						map[x,y] = FILLED; break;
					case BRIDGE:
						break;
				}
			}
		}
	}
	
	/*********************/
	/* UTILITY FUNCTIONS */
	/*********************/
	
	public Vector3 grid_to_world(Pos pos)
	{
		return new Vector3(pos.x * cell_size + cell_size / 2f, 0f, pos.y * cell_size + cell_size / 2f) - offset;
	}
	
	/*******************/
	/* DEBUG FUNCTIONS */
	/*******************/
	
	void print_connection_tree(Region region)
	{
		Debug.Log("Region #" + region.ID.ToString());
		string tree_str = string.Empty;
		foreach(Region connection in region.connections) {
			tree_str += " " + connection.ID.ToString();
		}
		Debug.Log(tree_str);
		foreach(Region connection in region.connections) {
			print_connection_tree(connection);
		}
	}
	
	void print_debug_connections()
	{
		foreach(Connection c in debug_connections) {
			Debug.Log(
				   c.endpt1.ToString() + " => " + c.endpt2.ToString() + " "
				+ "[ " + Pos.abs_dist(c.endpt1, c.endpt2).ToString() + " ] "
				+ "{ " + c.ID_1.ID.ToString() + " => " + c.ID_2.ID.ToString() + " }");
		}
	}
	
	void print_region_connections()
	{
		for(int i = 0; i < regions.Count; i++) {
			Debug.Log("REGION " + (i+2).ToString());
			foreach(Connection c in regions[i].closest) {
				Debug.Log(
				   c.endpt1.ToString() + " => " + c.endpt2.ToString() + " "
				+ "[ " + Pos.abs_dist(c.endpt1, c.endpt2).ToString() + " ] "
				+ "{ " + c.ID_1.ID.ToString() + " => " + c.ID_2.ID.ToString() + " }");
			}
		}
	}
	
	void print_cmds()
	{
		for (int i = 0; i < cmds.Count; i++) {
			Debug.Log("REGION " + i.ToString() + " : ");
			foreach(Cmd cmd in cmds[i]) {
				Debug.Log(cmd);
			}
		}
	}
	
	void print_regions()
	{
		for (int i = 0; i < regions.Count; i++) {
			Debug.Log(regions[i]);
			Debug.Log(cmds[i].Count);
		}
	}
	
	void OnDrawGizmos() 
	{
        if (map != null && debug) {
            for (int x = 0; x < width; x ++) {
                for (int y = 0; y < height; y++) {
					switch (map[x, y]) {
						case -3: Gizmos.color = Color.grey; break;
						case -2: Gizmos.color = Color.red; break;
						case -1: Gizmos.color = Color.red; break;
						case 0 : Gizmos.color = Color.white; break;
						case 1 : Gizmos.color = Color.black; break;
						case 2 : Gizmos.color = Color.yellow; break;
						case 3 : Gizmos.color = Color.blue; break;
						case 4 : Gizmos.color = Color.green; break;
						case 5 : Gizmos.color = Color.magenta; break;
						case 6 : Gizmos.color = Color.cyan; break;
						default : Gizmos.color = Color.black; break;
					}
                    Vector3 pos = new Vector3(-width/2 + x + .5f, 0f, -height/2 + y + .5f);
                    Gizmos.DrawCube(pos,Vector3.one);
                }
            }
        }
    }
}
