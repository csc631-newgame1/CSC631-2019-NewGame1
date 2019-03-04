﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public abstract class GameAgent : MonoBehaviour
{
	private Pos grid_pos;
	
    public abstract IEnumerator smooth_movement(List<Pos> locations);
	
	public abstract void take_damage(int amount);
	
	public abstract void init_agent(Pos position);
	
	// for enemies, this will make them go through their AI motions
	// for players, this will trigger the boolean value that allows them to take their turn
	public abstract void take_turn();
}
