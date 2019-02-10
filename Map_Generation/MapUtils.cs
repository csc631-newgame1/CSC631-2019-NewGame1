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
			return a.x == b.x && a.y == b.y;
		}
		public static bool operator !=(Pos a, Pos b)
		{
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
		public const int FILLED = 1;
		public const int EMPTY = 0;
		public const int BRIDGE = -1;
		public const int EDGE = -2;
	}
	
}