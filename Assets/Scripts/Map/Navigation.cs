using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
// includes definition for tile types, as well as traversable()
using static MapUtils.MapConstants;

public class NavigationHandler
{
	private class Vertex
	{
		public int dist;
		public bool visited;
		public Pos pos;
		public List<Vertex> visible;
		public Vertex prev;
		
		public Vertex(int x, int y)
		{
			dist 	 = int.MaxValue / 2; // to avoid overflow errors
			visited  = false;
			pos 	 = new Pos(x, y);
			visible  = new List<Vertex>();
			prev	 = null;
		}
		
		public Vertex(Pos p)
		{
			dist 	 = int.MaxValue / 2;
			visited  = false;
			pos 	 = p;
			visible  = new List<Vertex>();
			prev	 = null;
		}
		
		public void reset()
		{
			dist 	 = int.MaxValue / 2;
			visited  = false;
			prev	 = null;
		}
		
		public override string ToString()
		{
			return pos.ToString() + "|" + dist.ToString() + "|" + visited.ToString() + "|" + visible.Count.ToString();
		}
	}
	
	private Vertex[,] vertex_map;
	private List<Vertex> nav_graph;
	
	private int[,] map;
	private int width;
	private int height;
	
	private static float total_verts = 0.0f;
	private static float total_edges = 0.0f;
	
	public NavigationHandler(int[,] map)
	{
		this.map = map;
		width = map.GetLength(0);
		height = map.GetLength(1);
		vertex_map = new Vertex[width, height];
		nav_graph = new List<Vertex>();
		
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				if (tile_traversable(x, y)) {
					Vertex vert = new Vertex(x, y);
					vertex_map[x, y] = vert;
					nav_graph.Add(vert);
				}
				
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				if (tile_traversable(x, y)) {
					
					Vertex vert = vertex_map[x, y];
					
					if (tile_traversable(x-1, y))
						vert.visible.Add(vertex_map[x-1,y]);
					
					if (tile_traversable(x+1, y))
						vert.visible.Add(vertex_map[x+1,y]);
					
					if (tile_traversable(x, y-1))
						vert.visible.Add(vertex_map[x,y-1]);
					
					if (tile_traversable(x, y+1))
						vert.visible.Add(vertex_map[x,y+1]);
				}
		
		total_verts += nav_graph.Count;
		foreach (Vertex vert in nav_graph) {
			total_edges += vert.visible.Count;
		}
	}
	
	bool tile_traversable(int x, int y)
	{
		if (x < 0 || x >= width || y < 0 || y >= height)
			return false;
		
		else if (traversable(map[x, y]))
			return true;
		
		else return false;
	}
	
	void make_impassable(Pos tilePos)
	{
		Vertex toDie = vertex_map[tilePos.x, tilePos.y];
		
		foreach (Vertex neighbor in toDie.visible) {
			neighbor.visible.Remove(toDie);
		}
		
		vertex_map[tilePos.x, tilePos.y] = null;
	}
	
	void make_passable(Pos tilePos)
	{
		Vertex newVert = new Vertex(tilePos.x, tilePos.y);
		
		int x = tilePos.x, y = tilePos.y;
		vertex_map[x, y] = newVert;
		
		if (tile_traversable(x-1, y))
			newVert.visible.Add(vertex_map[x-1,y]);
		
		if (tile_traversable(x+1, y))
			newVert.visible.Add(vertex_map[x+1,y]);
		
		if (tile_traversable(x, y-1))
			newVert.visible.Add(vertex_map[x,y-1]);
		
		if (tile_traversable(x, y+1))
			newVert.visible.Add(vertex_map[x,y+1]);
	}
	
	// EVERYTHING PAST THIS LINE IS OK (MOSTLY)
	
	Vertex pop_min_dist_vert(List<Vertex> graph)
	{
		Vertex min = graph[0];
		for (int i = 1; i < graph.Count; i++) {
			min = graph[i].dist < min.dist ? graph[i] : min;
		}
		graph.Remove(min);
		return min;
	}
	
	// finds a single path from point a to point b
	public List<Pos> shortestPath(Pos p_origin, Pos p_target)
	{
		if (p_origin == p_target)
			return null;
		
		Vertex source = vertex_map[p_origin.x, p_origin.y];
		Vertex target = vertex_map[p_target.x, p_target.y];
		
		//display_vertices();
		//print_debug_info();
		
		// create a temporary graph of vertices to be pulled from during pathfinding
		List<Vertex> tmp_graph = new List<Vertex>(nav_graph);
		
		source.dist = 0;
		Vertex min_vert = pop_min_dist_vert(tmp_graph);
		
		// uses dijkstra method to find minimum distance from origin to target
		while (tmp_graph.Count > 0 && min_vert != target) {
			foreach (Vertex neighbor in min_vert.visible) {
				int alt_dist = min_vert.dist + Pos.abs_dist(min_vert.pos, neighbor.pos);
				if (alt_dist < neighbor.dist) {
					neighbor.dist = alt_dist;
					neighbor.prev = min_vert;
				}
			}
			min_vert = pop_min_dist_vert(tmp_graph);
		}
		
		List<Pos> path = constructPath(target, source);

        // post path-finding cleanup
		foreach(Vertex vertex in nav_graph)
			vertex.reset();
		
		return path;
	}
	
	// finds an entire batch of paths
	public List<List<Pos>> shortestPathBatched(Pos p_origin, List<Pos> targetPositions)
	{	
		Vertex source = vertex_map[p_origin.x, p_origin.y];
		List<Vertex> targetVerts = new List<Vertex>();
		foreach (Pos p in targetPositions)
			targetVerts.Add(vertex_map[p.x, p.y]);

		// create a temporary graph of vertices to be pulled from during pathfinding
		List<Vertex> tmp_graph = new List<Vertex>(nav_graph);
		
		source.dist = 0;
		Vertex min_vert = pop_min_dist_vert(tmp_graph);
			
		int positionsToSearch = targetPositions.Count;
		int searchedPositions = 0;
		
		// uses dijkstra method to find minimum distance from origin to target
		while (tmp_graph.Count > 0 && searchedPositions < positionsToSearch) {
			foreach (Vertex neighbor in min_vert.visible) {
				int alt_dist = min_vert.dist + Pos.abs_dist(min_vert.pos, neighbor.pos);
				if (alt_dist < neighbor.dist) {
					neighbor.dist = alt_dist;
					neighbor.prev = min_vert;
				}
			}
			if (targetVerts.Contains(min_vert))
				searchedPositions++;
			min_vert = pop_min_dist_vert(tmp_graph);
		}
		
		// construct paths leading back to source
		List<List<Pos>> paths = new List<List<Pos>>();
		foreach (Vertex target in targetVerts) {
			paths.Add(constructPath(target, source));
		}

        // post path-finding cleanup
		foreach(Vertex vertex in nav_graph)
			vertex.reset();
		
		return paths;
	}
	
	List<Pos> constructPath(Vertex target, Vertex source)
	{
		Stack<Pos> path = new Stack<Pos>();
		Vertex curr_vert = target;
		while (curr_vert != null && curr_vert != source) {
			path.Push(curr_vert.pos);
			curr_vert = curr_vert.prev;
		}
		path.Push(source.pos);
		return clean_up_path(path);
	}
	
	List<Pos> clean_up_path(Stack<Pos> path)
	{
		List<Pos> new_path = new List<Pos>();
		Pos p1 = path.Pop(), p2, mp;
		new_path.Add(p1);
		
		while (path.Count > 0) {
			p2 = path.Pop();
			mp = get_valid_midpoint(p1, p2);
			if (mp != null)
				new_path.Add(mp);
			new_path.Add(p2);
			p1 = p2;
		}
		
		return new_path;
	}
	
	Pos get_valid_midpoint(Pos p1, Pos p2)
	{
		if (p1.x == p2.x || p1.y == p2.y)
			return null;
		
		for (int x = p1.x; x != p2.x; x += Math.Sign(p2.x - p1.x))
			if (!traversable(map[x, p1.y]))
				return new Pos(p1.x, p2.y);
		for (int y = p1.y; y != p2.y; y += Math.Sign(p2.y - p1.y))
			if (!traversable(map[p2.x, y]))
				return new Pos(p1.x, p2.y);
			
		return new Pos(p2.x, p1.y);
	}
	
	void display_vertices()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (vertex_map[x, y] != null) {
					Vector3 pos = new Vector3(-width/2 + x + .5f, 0f, -height/2 + y + .5f);
					Debug.DrawLine(pos, pos + Vector3.up, Color.green, 2f, false);
				}
			}
		}
	}
	
	void print_debug_info()
	{
		Stack<Vertex> connected_graph = new Stack<Vertex>();
		connected_graph.Push(nav_graph[0]);
		
		float verts = 0;
		float edges = 0;
		while (connected_graph.Count > 0) {
			verts++;
			Vertex current = connected_graph.Pop();
			draw_lines_to_neighbors(current);
			foreach (Vertex vertex in current.visible) {
				edges++;
				if (!vertex.visited) {
					vertex.visited = true;
					connected_graph.Push(vertex);
				}
			}
			current.visited = true;
		}
		
		foreach(Vertex vertex in nav_graph)
			vertex.reset();
			
		Debug.Log("Verts: " + verts + ", Edges: " + edges);
		if (verts < nav_graph.Count) {
			Debug.Log("Some areas are not connected!");
		}
		
		Debug.Log("Curr E/V ratio: " + (edges / verts));
		Debug.Log("Avg E/V ratio: " + (total_edges / total_verts));
	}
	
	void draw_lines_to_neighbors(Vertex v)
	{
		Vector3 origin = new Vector3(-width/2 + v.pos.x + .5f, 0f, -height/2 + v.pos.y + .5f);
		foreach(Vertex n in v.visible) {
			Vector3 neighbor = new Vector3(-width/2 + n.pos.x + .5f, 0f, -height/2 + n.pos.y + .5f);
			Debug.DrawLine(origin, neighbor, Color.white, 1.5f, false);
		}
	}
}

/*bool is_vertex(int x, int y)
	{	
		if (tile_traversable(x, y) == 0)
			return false;
		
		int t_l	= tile_traversable(x - 1, y + 0);
		int t_r	= tile_traversable(x + 1, y + 0);
		int t_u	= tile_traversable(x + 0, y - 1);
		int t_d	= tile_traversable(x + 0, y + 1);
		
		int c_ul	= tile_traversable(x - 1, y - 1);
		int c_ur	= tile_traversable(x + 1, y - 1);
		int c_dl	= tile_traversable(x - 1, y + 1);
		int c_dr	= tile_traversable(x + 1, y + 1);
		
		int adjacent_horizontal = t_l + t_r;
		int adjacent_vertical	= t_u + t_d;
		
		int corners_total	= c_ul + c_ur + c_dl + c_dr;
		int adjacent_total	= adjacent_horizontal + adjacent_vertical;

		bool clinching 	= (((c_ul == 0 && c_dr == 0) || (c_ur == 0 && c_dl == 0)) && (adjacent_total == 3 || adjacent_total == 2));
		bool passage 	= ((adjacent_horizontal + adjacent_vertical == 2) && (adjacent_horizontal == 0 || adjacent_vertical == 0));
		bool corner		= (corners_total < 4 && adjacent_total == 4);
		bool isolated	= (corners_total <= 1);
		
		return clinching || passage || corner || isolated;
	}
	
	void build_visibility_graph()
	{
		foreach (Vertex vertex in nav_graph) {
			get_visible_vertices(vertex);
		}
	}
	
	void get_visible_vertices(Vertex vertex)
	{
		Pos vpos = vertex.pos;
		int max_x = vpos.x, min_x = vpos.x;
		int max_y = vpos.y, min_y = vpos.y;
		
		// get the maximum and minimum visible x and y values (checks straight lines up, down, left, and right from the vertex origin)
		for (int x = vpos.x + 1; x < width; x++) {
			if (tile_traversable(x, vpos.y) == 0)
				break;
			else if (vertex_map[x, vpos.y] != null) {
				vertex.visible.Add(vertex_map[x, vpos.y]); break; }
			max_x = x;
		}
		
		for (int x = vpos.x - 1; x >= 0; x--) {
			if (tile_traversable(x, vpos.y) == 0)
				break;
			else if (vertex_map[x, vpos.y] != null) {
				vertex.visible.Add(vertex_map[x, vpos.y]); break; }
			min_x = x;
		}
		
		for (int y = vpos.y + 1; y < height; y++) {
			if (tile_traversable(vpos.x, y) == 0)
				break;
			else if (vertex_map[vpos.x, y] != null) { 
				vertex.visible.Add(vertex_map[vpos.x, y]); break; }
			max_y = y;
		}
		
		for (int y = vpos.y - 1; y >= 0; y--) {
			if (tile_traversable(vpos.x, y) == 0)
				break;
			else if (vertex_map[vpos.x, y] != null) {
				vertex.visible.Add(vertex_map[vpos.x, y]); break; }
			min_y = y;
		}
		
		/* Visualization of quadrants:
		 * 
		 * -- -- -- ■ -- -- --
		 * -- UL -- ■ -- UR --
		 * -- -- -- ■ -- -- --
		 * ■■ ■■ ■■ C ■■ ■■ ■■
		 * -- -- -- ■ -- -- --
		 * -- LL -- ■ -- LR --
		 * -- -- -- ■ -- -- --
		 * 
		 * Example visibility check:
		 * KEY { C: current vertex being tested, X: non-visible vertex, V: visible vertex, [space]: walkable, ■: unwalkable, ~: in purview of visibility check }
		 * 
		 * ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■
		 * ■     ■ ■ ■ ~ ~ ~ ~ ~ ■ ■ ■
		 * ■   X ~ ~ ■ ~ ~ ~ ~ ~ ■ ■ ■
		 * ■ ■ ■ V V ~ C V ~ V V ~ ■ ■
		 * ■   ■ ~ ~ ~ V ~ ■ ~ ~ V   ■
		 * ■   ■ V ~ ~ ~ ■ ■ ■ ■ ~ X ■
		 * ■ ~ V ~ ~ ~ ~ ■ ■ ■ ■ ■ X ■
		 * ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■
		 * 
		 * Placement of vertices in this map follows the placement of vertices in an actual navmap, i.e: at the corners of unwalkable tiles
		 */
		 
		// find vertices that are visible to this vertex
		// if this is confusing, ask me (Ben) about it and I'll try and give you a better explanation than what I have here
		/*int t_maxy = max_y;
		int t_miny = min_y;
		int t_maxx = max_x;
		int t_minx = min_x;
		
		for (int x = min_x; x <= max_x; x++) {
			int y = vpos.y + 1;
			while (y <= max_y && tile_traversable(x, y) == 1) {
				if (vertex_map[x, y] != null) {
					vertex.visible.Add(vertex_map[x, y]); break;}
				y++;
			}
			if (tile_traversable(x, y) == 0)
				max_y = y;
			
			y = vpos.y - 1;
			while (y >= min_y && tile_traversable(x, y) == 1) {
				if (vertex_map[x, y] != null) {
					vertex.visible.Add(vertex_map[x, y]); break;}
				y--;
			}
			if (tile_traversable(x, y) == 0)
				min_y = y;
		}
		
		max_y = t_maxy;
		min_y = t_miny;
		
		for (int y = min_y; y <= max_y; y++) {
			int x = vpos.x + 1;
			while (x <= max_x && tile_traversable(x, y) == 1) {
				if (vertex_map[x, y] != null) {
					vertex.visible.Add(vertex_map[x, y]); break;}
				x++;
			}
			if (tile_traversable(x, y) == 0)
				max_x = x;
			
			x = vpos.x - 1;
			while (x >= min_x && tile_traversable(x, y) == 1) {
				if (vertex_map[x, y] != null) {
					vertex.visible.Add(vertex_map[x, y]); break;}
				x--;
			}
			if (tile_traversable(x, y) == 0)
				min_x = x;
		}
	}*/