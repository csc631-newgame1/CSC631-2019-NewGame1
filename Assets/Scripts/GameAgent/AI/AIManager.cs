﻿using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager
{
    public static List<GameAgent>[] roster;
	
	public static void update(int team)
	{
		if (roster[team].Count == 0) return;
		resetAgents(team);
		
		foreach (GameAgent agent in roster[team]) {
			
			if (agent.AI != null) {
				updatePools(agent);
				agent.AI.calcDistances();
				agent.AI.calcAttack();
				agent.AI.calcReinforce();
			}
			agent.take_turn();
		}
	}
	
	public static bool turnOver(int team)
	{
		bool over = true;
		foreach (GameAgent agent in roster[team])
			if (!agent.turn_over()) over = false;
		return over;
	}
	
	private static void resetAgents(int team)
	{
		foreach (GameAgent agent in roster[team]) {
			if (agent.AI != null)
				agent.AI.reset();
		}
	}
	
	private static void updatePools(GameAgent agent) 
	{	
		for (int j = 0; j < roster.Length; j++) {
			if (j == agent.team) continue;
			
			foreach (GameAgent enemy in roster[j]) {
				agent.AI.addEnemyToPool(enemy);
			}
		}
		
		foreach (GameAgent ally in roster[agent.team]) {
			if (ally == agent || ally.AI == null) continue;
			
			agent.AI.addAllyToPool(ally);
		}
	}
}