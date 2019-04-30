using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class EnvironmentObject : DungeonObject, Damageable, Environment
{
	public int health;
	public void init_environment(Pos position, int health = 10)
	{
		grid_pos = position;
		this.health = health;
	}
    public void take_damage(int amount)
	{
		health -= amount;
		if (health < 0)
			GameManager.kill(this);
	}
}
