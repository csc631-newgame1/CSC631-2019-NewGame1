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
	
	public NavigationHandler(int[,] map)
	{
		this.map = map;
		width = map.GetLength(0);
		height = map.GetLength(1);
		vertex_map = new Vertex[width, height];
		nav_graph = new List<Vertex>();
		
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				if (is_corner(map, x, y)) {
					Vertex new_vert = new Vertex(x, y);
					vertex_map[x, y] = new_vert;
					nav_graph.Add(new_vert);
				}
				
		build_visibility_graph();
	}
	
	bool is_corner(int[,] map, int x, int y)
	{
		if (x == 0 || x == width - 1 || y == 0 || y == height - 1 || !traversable(map[x, y]))
			return false;
		
		bool has_immediate =
			traversable(map[x - 1, y]) && traversable(map[x + 1, y]) &&
			traversable(map[x, y - 1]) && traversable(map[x, y + 1]);
			
		int num_adjacent_corners =
			(!traversable(map[x - 1, y - 1]) ? 1 : 0) + (!traversable(map[x + 1, y - 1]) ? 1 : 0) +
			(!traversable(map[x - 1, y + 1]) ? 1 : 0) + (!traversable(map[x + 1, y + 1]) ? 1 : 0);
			
		return num_adjacent_corners > 0 && !has_immediate;
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
			if (!traversable(map[x, vpos.y]))// || map[x, vpos.y] == EDGE)
				break;
			else if (vertex_map[x, vpos.y] != null)
				vertex.visible.Add(vertex_map[x, vpos.y]);
			max_x = x;
		}
		
		for (int x = vpos.x - 1; x >= 0; x--) {
			if (!traversable(map[x, vpos.y]))// || map[x, vpos.y] == EDGE)
				break;
			else if (vertex_map[x, vpos.y] != null)
				vertex.visible.Add(vertex_map[x, vpos.y]);
			min_x = x;
		}
		
		for (int y = vpos.y + 1; y < height; y++) {
			if (!traversable(map[vpos.x, y]))// || map[vpos.x, y] == EDGE)
				break;
			else if (vertex_map[vpos.x, y] != null)
				vertex.visible.Add(vertex_map[vpos.x, y]);
			max_y = y;
		}
		
		for (int y = vpos.y - 1; y >= 0; y--) {
			if (!traversable(map[vpos.x, y]))// || map[vpos.x, y] == EDGE)
				break;
			else if (vertex_map[vpos.x, y] != null)
				vertex.visible.Add(vertex_map[vpos.x, y]);
			min_y = y;
		}
		
		int temp_maxy = max_y;
		
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
		
		for (int x = min_x; x <= max_x; x++) {
			int y = vpos.y + 1;
			while (y < height && !traversable(map[x, y])) {
				if (vertex_map[x, y] != null)
					vertex.visible.Add(vertex_map[x, y]);
				y++;
			}
			y = vpos.y - 1;
			while (y >= 0 && traversable(map[x, y])) {
				if (vertex_map[x, y] != null)
					vertex.visible.Add(vertex_map[x, y]);
				y--;
			}
		}
		
		for (int y = min_y; y <= max_y; y++) {
			int x = vpos.x + 1;
			while (x < width && traversable(map[x, y])) {
				if (vertex_map[x, y] != null)
					vertex.visible.Add(vertex_map[x, y]);
				x++;
			}
			x = vpos.x - 1;
			while (x >= 0 && traversable(map[x, y])) {
				if (vertex_map[x, y] != null)
					vertex.visible.Add(vertex_map[x, y]);
				x--;
			}
		}
	}
	
	Vertex insert_vertex_at(Pos pos)
	{
		Vertex new_vert = new Vertex(pos.x, pos.y);
		vertex_map[pos.x, pos.y] = new_vert;
		get_visible_vertices(new_vert);
		foreach (Vertex vertex in new_vert.visible)
			vertex.visible.Add(new_vert);
		nav_graph.Add(new_vert);
		return new_vert;
	}
	
	void remove_vertex_at(Pos pos)
	{
		Vertex old_vert = vertex_map[pos.x, pos.y];
		foreach (Vertex vertex in old_vert.visible)
			vertex.visible.Remove(old_vert);
		nav_graph.Remove(old_vert);
		vertex_map[pos.x, pos.y] = null;
	}
	
	void clean_up_graph()
	{
		foreach(Vertex vertex in nav_graph)
			vertex.reset();
	}
	
	Vertex pop_min_dist_vert(List<Vertex> graph)
	{
		Vertex min = graph[0];
		for (int i = 1; i < graph.Count; i++) {
			min = graph[i].dist < min.dist ? graph[i] : min;
		}
		graph.Remove(min);
		return min;
	}
	
	public List<Pos> find_shortest_path(Pos p_origin, Pos p_target)
	{
		Vertex source = insert_vertex_at(p_origin);
		Vertex target = insert_vertex_at(p_target);
		
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
		
		// construct path leading back from target
		Stack<Pos> path = new Stack<Pos>();
		Vertex curr_vert = target;
		while (curr_vert != null && curr_vert != source) {
			path.Push(curr_vert.pos);
			curr_vert = curr_vert.prev;
		}
		path.Push(source.pos);
		
		// post path-finding cleanup
		remove_vertex_at(p_origin);
		remove_vertex_at(p_target);
		clean_up_graph();
		
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
	
	void print_debug_info()
	{
		Stack<Vertex> connected_graph = new Stack<Vertex>();
		connected_graph.Push(nav_graph[0]);
		
		int count = 0;
		while (connected_graph.Count > 0) {
			count++;
			Vertex current = connected_graph.Pop();
			draw_lines_to_neighbors(current);
			foreach (Vertex vertex in current.visible) {
				if (!vertex.visited) {
					vertex.visited = true;
					connected_graph.Push(vertex);
				}
			}
			current.visited = true;
		}
		
		foreach(Vertex vertex in nav_graph)
			vertex.reset();
			
		Debug.Log("Vertices connected together: " + count.ToString());
		if (count < nav_graph.Count) {
			Debug.Log("Some areas are not connected!");
		}
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
