using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.Dir;
using static MapUtils.Type;

namespace LoopExtractor
{
	/* Creates a "turtle" that walks around the perimeter of each region
	 * and generates commands that describe the geometry of that perimeter.
	 * This allows detailed 3D walls to be created in-game.
	 *
	 * Since this turtle generates a loop, UV coordinates can be continuous
	 * around the entirety of the perimeter. This also opens the perimeter
	 * walls to more sophisticated effects, like fractal or perlin deformations
	 * that stay continuous around the perimeter.
	 * 
	 * It's important that all loops are closed in the given map, otherwise
	 * there will probably be an IndexOutOfBounds exception, because no bounds
	 * checking occurs within the turtle
	 */
	public class LoopTurtle
	{
		private int [,] map;
		private Pos pos;
		private Dir dir;
		public List<Cmd> cmdlist;
		
		public LoopTurtle(int[,] map, Pos pos)
		{
			this.map = map;
			this.pos = pos;
			if      (map[pos.x-1, pos.y] == 1) {
				dir = UP;
			}
			else if (map[pos.x, pos.y-1] == 1) {
				dir = RIGHT;
			}
			else if (map[pos.x+1, pos.y] == 1) {
				dir = DOWN;
			}
			else if (map[pos.x, pos.y+1] == 1) {
				dir = LEFT;
			}
			cmdlist = new List<Cmd>();
			cmdlist.Add(new Cmd(dir, this.pos, LINE));
			advance_until_loop();
			collapse_cmds();
		}
		
		private void advance_until_loop()
		{
			Cmd top = cmdlist[0];
			advance();
			while (cmdlist[0] != top) {
				advance();
			}
			cmdlist.RemoveAt(0);
		}
		
		private void collapse_cmds()
		{
			int len = cmdlist.Count;
			
			for (int i = len - 1; i >= 0; i--) {
				
				int j = 1;
				while (is_ccw(cmdlist[i].dir, cmdlist[(i+j)%len].dir, j)) {
					cmdlist[i].type = (Type)((int)cmdlist[i].type + 1);
					cmdlist[(i+j)%len].type = INVALID;
					j++;
				}
			}
			
			List<Cmd> new_cmds = new List<Cmd>();
			for (int i = len - 1; i >= 0; i--) {
				if (cmdlist[i].type != INVALID) {
					new_cmds.Add(cmdlist[i]);
				}
			}
			cmdlist.Clear();
			
			cmdlist = new_cmds;
		}
		
		// checks if d2 is amt*90 degrees counter-clockwise from d1
		private bool is_ccw(Dir d1, Dir d2, int amt)
		{
			return (((int)d2 + amt) % 4) == (int)d1;
		}
		
		private void advance()
		{
			if (detect_forward()) { // if tile is in front of turtle, turn clockwise
				turn_right();
			}
			else if (detect_forward_left()) { // else, if no tile is in front of turtle, and there is a tile to the forward left, move forward and maintain direction
				pos += forward_dict(dir);
			}
			else if (detect_left()) { // else, if there is only a tile to the left, move diagonally and turn counter-clockwise
				pos += forward_dict(dir) + left_dict(dir); 
				turn_left();
			}
			cmdlist.Insert(0, new Cmd(dir, pos, LINE));
		}
		
		private void turn_right()
		{
			dir = (Dir)((int)(dir + 1) % 4);
		}
		
		private void turn_left()
		{
			dir = (Dir)((int)(dir - 1) < 0 ? 3 : (int)(dir - 1));
		}
		
		private Pos forward_dict(Dir dir)
		{
			switch (dir)
			{
				case LEFT  : return Pos.LEFT;
				case UP    : return Pos.UP;
				case RIGHT : return Pos.RIGHT;
				case DOWN  : return Pos.DOWN;
				
				// this shouldn't happen
				default : return null;
			}
		}
		
		private Pos left_dict(Dir dir)
		{
			switch (dir)
			{
				case LEFT  : return Pos.DOWN;
				case UP    : return Pos.LEFT;
				case RIGHT : return Pos.UP;
				case DOWN  : return Pos.RIGHT;
				
				// this shouldn't happen
				default : return null;
			}
		}
		
		private bool detect_forward()
		{
			Pos check_pos = pos + forward_dict(dir);
			if (map[check_pos.x, check_pos.y] == 1) {
				return true;
			}
			return false;
		}
		
		private bool detect_forward_left()
		{
			Pos check_pos = pos + forward_dict(dir) + left_dict(dir);
			if (map[check_pos.x, check_pos.y] == 1) {
				return true;
			}
			return false;
		}
		
		private bool detect_left()
		{
			Pos check_pos = pos + left_dict(dir);
			if (map[check_pos.x, check_pos.y] == 1) {
				return true;
			}
			return false;
		}
	}
}
