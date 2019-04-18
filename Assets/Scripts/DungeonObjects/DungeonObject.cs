using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

// all objects on the map have a grid position and some way to take damage
public abstract class DungeonObject : MonoBehaviour
{
    public Pos grid_pos;
	public abstract void take_damage(int amount);
}
