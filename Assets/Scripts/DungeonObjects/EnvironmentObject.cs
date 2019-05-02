using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class EnvironmentObject : DungeonObject, Damageable, Environment, Renderable
{
	public bool traversable = false;
	public int health = 0;
	public void init_environment(Pos position, int health = 10)
	{
		grid_pos = position;
		//this.health = health;
		Renderer rend = GetComponentInChildren<Renderer>();
		rend.material.SetTexture("_FOWTex", FogOfWar.fogTex);
		rend.material.SetVector("_MapWidthHeight", new Vector4(MapManager.MapWidth, MapManager.MapHeight, 0, 0));
	}
	void Update()
	{
		if (FogOfWar.IsSemiVisible(grid_pos))
			EnableRendering();
		else
			DisableRendering();
	}
    public void take_damage(int amount)
	{
		health -= amount;
		if (health < 0)
			GameManager.kill(this);
	}
	public void EnableRendering()
	{
		GetComponentInChildren<Renderer>().enabled = true;
	}
	public void DisableRendering()
	{
		GetComponentInChildren<Renderer>().enabled = false;
	}
}
