using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class EndPortal : DungeonObject, Environment, Interactable
{
	public void interact(GameAgent interactor)
	{
		MapManager.ExtractAgent(interactor as Player);
	}
	public void init_environment(Pos grid_pos, int health = 10000000)
	{
		this.grid_pos = grid_pos;
	}
}
