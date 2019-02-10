using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using MapUtils;
using LoopExtractor;

namespace RegionUtils
{
	public class Connection : IComparable // connections must be sorted in post processing of map data
	{
		public Pos endpt1;
		public Pos endpt2;
		public Region ID_1;
		public Region ID_2;
		public Connection(Pos endpt1, Pos endpt2, Region ID_1, Region ID_2)
		{
			this.endpt1 = endpt1;
			this.endpt2 = endpt2;
			this.ID_1 = ID_1;
			this.ID_2 = ID_2;
		}
		public int CompareTo(object obj)
		{
			if (obj == null) return 1;
			Connection other = obj as Connection;
			int dist_self = Pos.abs_dist(this.endpt1, this.endpt2);
			int dist_other = Pos.abs_dist(other.endpt1, other.endpt2);
			return dist_self.CompareTo(dist_other);
		}
	}
	
	public class Region
	{
		public int count;
		public int ID;
		public Pos startpos;
		public List<Connection> closest; // stores closest connections of each region
		public List<Cmd> cmds; // stores geometry of the wall of each region
		public List<Region> connections; // stores list or regions that connect to this one (only one way)
		public bool connected_to_main;
		
		private static int START_REGION_ID = 2;
		
		public Region(int[,] map, int ID, int count, Pos pos)
		{
			this.ID = ID;
			this.count = count;
			this.startpos = pos;
			this.closest = new List<Connection>();
			LoopTurtle turtle = new LoopTurtle(map, pos);
			this.cmds = turtle.cmdlist;
			this.connected_to_main = false;
			this.connections = new List<Region>();
		}
		
		public bool has_connection_to(Region reg)
		{
			foreach (Connection c in closest) {
				if (c.ID_1 == reg || c.ID_2 == reg) {
					return true;
				}
			}
			return false;
		}
		
		public override string ToString()
		{
			return "Region #" + (ID - START_REGION_ID).ToString() + " | count = " + count.ToString() + " | start = " + startpos.ToString();
		}
		
		private static int flood_fill(int[,] map, int x, int y, int ID, int fill_ID)
		{
			int count = 1;
			map[x, y] = ID;
			Stack<Pos> fill_tiles = new Stack<Pos>();
			fill_tiles.Push(new Pos(x, y));
			
			while (fill_tiles.Count > 0) {
				
				Pos tile = fill_tiles.Pop();
				int sx = tile.x;
				int sy = tile.y;
				
				if (map[sx-1, sy] == fill_ID) {
					fill_tiles.Push(new Pos(sx-1, sy));
					map[sx-1, sy] = ID;
					count += 1;
				}
				if (map[sx, sy-1] == fill_ID) {
					fill_tiles.Push(new Pos(sx, sy-1));
					map[sx, sy-1] = ID;
					count += 1;
				}
				if (map[sx+1, sy] == fill_ID) {
					fill_tiles.Push(new Pos(sx+1, sy));
					map[sx+1, sy] = ID;
					count += 1;
				}
				if (map[sx, sy+1] == fill_ID) {
					fill_tiles.Push(new Pos(sx, sy+1));
					map[sx, sy+1] = ID;
					count += 1;
				}
			}
			
			return count;
		}
		
		public static List<Region> extract_regions(int[,] map, int threshold)
		{
			int width = map.GetLength(0);
			int height = map.GetLength(1);
			int ID = START_REGION_ID;
			List<Region> regions = new List<Region>();
			
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < height - 1; y++) {
					if (map[x, y] == 0) {
						int count = flood_fill(map, x, y, ID, 0);
						if (count > threshold) {
							regions.Add(new Region(map, ID, count, new Pos(x, y)));
							ID++;
						} else {
							flood_fill(map, x, y, 1, ID); // if region is too small (below the threshold) fill it in
						}
					}
				}
			}
			return regions;
		}
		
		public static void clear_regions(int[,] map)
		{
			int width = map.GetLength(0);
			int height = map.GetLength(1);
			for (int x = 1; x < width - 1; x++) {
				for (int y = 1; y < height - 1; y++) {
					if (map[x, y] > 1) {
						map[x, y] = 0;
					}
				}
			}
		}
	}
}