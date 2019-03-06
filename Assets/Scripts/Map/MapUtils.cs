using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using static MapUtils.Dir;
using static MapUtils.Type;

namespace MapUtils 
{
	public enum Dir  { LEFT = 0, UP, RIGHT, DOWN };
	
	public enum Type { LINE = 1, CORNER, ALLEY, INVALID };
	
	public static class EnumUtils
	{
		public static Dir right(this Dir dir, int amt = 1)
		{
			return (Dir) (((int)dir + amt) % 4);
		}
		public static Dir left(this Dir dir, int amt = 1)
		{
			amt %= 4;
			int ndir = (int)dir - amt;
			if (ndir < 0) {
				return (Dir) (4 + ndir);
			}
			else {
				return (Dir) ndir;
			}
		}
		public static string GetString(this Dir dir)
		{
			switch (dir) {
				case LEFT  : return "LEFT ";
				case UP    : return "UP   ";
				case RIGHT : return "RIGHT";
				case DOWN  : return "DOWN ";
			}
			return "NONE ";
		}
		public static string GetString(this Type type)
		{
			switch (type) {
				case LINE   : return "LINE  ";
				case CORNER : return "CORNER";
				case ALLEY  : return "ALLEY ";
			}
			return "NONE  ";
		}
	}
	
	public class Pos 
	{
		public static Pos LEFT = new Pos(-1, 0);
		public static Pos UP = new Pos(0, -1);
		public static Pos RIGHT = new Pos(1, 0);
		public static Pos DOWN = new Pos(0, 1);
		
		public int x;
		public int y;
		public Pos(int x, int y) 
		{
			this.x = x;
			this.y = y;
		}
		public static int abs_dist(Pos a, Pos b)
		{
			return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
		}
		public static Pos operator +(Pos a, Pos b)
		{
			return new Pos(a.x + b.x, a.y + b.y);
		}
		public static Pos operator -(Pos a, Pos b)
		{
			return new Pos(a.x - b.x, a.y - b.y);
		}
		public static bool operator ==(Pos a, Pos b)
		{
			if (object.ReferenceEquals(a, null))
				return object.ReferenceEquals(b, null);
			if (object.ReferenceEquals(b, null))
				return object.ReferenceEquals(a, null);
			return a.x == b.x && a.y == b.y;
		}
		public static bool operator !=(Pos a, Pos b)
		{
			if (object.ReferenceEquals(a, null))
				return !object.ReferenceEquals(b, null);
			if (object.ReferenceEquals(b, null))
				return !object.ReferenceEquals(a, null);
			return a.x != b.x || a.y != b.y;
		}
		public override string ToString()
		{
			return "(" + x.ToString() + "," + y.ToString() + ")";
		}
	}
	
	public class Cmd
	{
		public Dir dir;
		public Type type;
		public Pos pos;
		public Cmd(Dir dir, Pos pos, Type type)
		{
			this.dir = dir;
			this.pos = pos;
			this.type = type;
		}
		public static bool operator ==(Cmd a, Cmd b)
		{
			return a.dir == b.dir && a.type == b.type && a.pos == b.pos;
		}
		public static bool operator !=(Cmd a, Cmd b)
		{
			return a.dir != b.dir || a.type != b.type || a.pos != b.pos;
		}
		public override string ToString()
		{
			return pos.ToString() + " | " + dir.GetString() + " | " + type.GetString();
		}
	}
	
	public static class MapConstants
	{
		// all values greater than filled indicate a region marker
		public const int FILLED = 1;
		public const int EMPTY = 0;
		public const int BRIDGE = -1;
		public const int PLATFORM = -2;
		public const int EDGE = -3;
		public const int INNER_REGION = -4;
		
		public static bool traversable(int tile)
		{
			if (tile >= FILLED || tile == BRIDGE || tile == PLATFORM)
				return true;
			return false;
		}
	}

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
}