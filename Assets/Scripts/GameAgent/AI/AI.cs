using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class AIComponent
{
	private enum STATE { IDLE, ATTACK, REINFORCE, FLEE };
	
	#region Behaviour			// Range	| Description
	private float ambition;		// [ 0,  1] | Likelihood to target players based on their stats 
	private float pragmatism;	// [ 0,  1] | Likelihood to target players based on proximity
	private float brutality;	// [ 0,  1] | Likelihood to target players based on health
	private float cowardice;	// [-1,  1] | Likelihood to flee at low health
	private float comraderie;	// [-1,  1] | Likelihood to reinforce allies, scales with range
	private float complacency;	// [-1,  1] | Likelihood to attack players, scales with range
	#endregion
	
	private int MAX_REINFORCE_RANGE;
	private int MAX_ATTACK_RANGE;
	private int FLEE_THRESHOLD;
	
	private GameAgent parent;
	private GameAgent attacking;
	private GameAgent reinforcing;
	private STATE state;
	public  bool finished;
	
	private MapManager mapManager;
	private List<GameAgent> enemyPool;
	private List<int> enemyDistances;
	private List<GameAgent> alliedPool;
	private List<int> alliedDistances;
	
	public AIComponent(GameAgent parentAgent)
	{	
		ambition = 0;
		pragmatism = 1;
		brutality = 0;
		cowardice = -1;
		comraderie = 1;
		complacency = -1;
		
		MAX_REINFORCE_RANGE = 25;
		MAX_ATTACK_RANGE = 25;
		FLEE_THRESHOLD = 0;
		
		parent = parentAgent;
		attacking = null;
		reinforcing = null;
		state = STATE.IDLE;
		finished = false;
		
		mapManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
		enemyPool = new List<GameAgent>();
		alliedPool = new List<GameAgent>();
	}
	
	public void reset()
	{
		state = STATE.IDLE;
		enemyPool.Clear();
		alliedPool.Clear();
		attacking = null;
		reinforcing = null;
		finished = false;
	}
	
	public IEnumerator advance()
	{	
		switch (state) {
		case STATE.IDLE: 
			// do nothing
			break;
		case STATE.ATTACK: 
			// pathfind towards attacker, attack if possible
			int result = pathTowards(attacking, (int)parent.stats.range, 1);
			if (result == -1) {
				state = STATE.IDLE;
				attacking = null;
				calcReinforce();
				// fallback to reinforcing allies, if attack move is not possible
				if (state == STATE.REINFORCE) {
					goto case STATE.REINFORCE;
				}
			}
			else if (result == 1) {
				while (parent.animating) yield return null; // waits for move animation to finish before moving on
				mapManager.attack(parent, attacking.grid_pos); // attack enemy
				while (parent.animating) yield return null; // waits for attack animation to finish
			}
			else if (result == 2) {
				while (parent.animating) yield return null; // waits for move animation to finish
			}
			break;
		case STATE.REINFORCE: 
			// pathfind towards character we're reinforcing
			if (pathTowards(reinforcing, 3, 1) == -1) {
				reinforcing = null;
				state = STATE.IDLE;
			}
			else {
				while (parent.animating) yield return null; // waits for move animation to finish before moving on
			}
			break;
		case STATE.FLEE: 
			// run away from enemies, heal once safe
			break;
		}
		
		if (state == STATE.ATTACK && attacking != null) {
			mapManager.DrawLine(parent.grid_pos, attacking.grid_pos, Color.red);
		}
		else if (state == STATE.REINFORCE && reinforcing != null) {
			mapManager.DrawLine(parent.grid_pos, reinforcing.grid_pos, Color.green);
		}
		
		finished = true;
	}
	
	// return values:
	// -1: indicates that pathfinding has failed
	//  0: indicates that we are already within attack range
	//  1: indicates that pathfinding has succeeded, and it is ok to attack
	//  2: indicates that pathfinding has succeeded, but it is not ok to attack
	private int pathTowards(GameAgent agent, int maxStopDistance, int minStopDistance)
	{
		Pos source = parent.grid_pos;
		Pos target = agent.grid_pos;
		List<Pos> testDestinations = new List<Pos>();
		
		for (int x = target.x - maxStopDistance; x <= target.x + maxStopDistance; x++) {
			for (int y = target.y - maxStopDistance; y <= target.y + maxStopDistance; y++) {
				if (x == source.x && y == source.y) continue;
				
				Pos candidate = new Pos(x, y);
				if (Pos.abs_dist(candidate, target) <= maxStopDistance && Pos.abs_dist(candidate, target) >= minStopDistance)
					if (mapManager.IsWalkable(candidate) && Pos.abs_dist(candidate, source) <= MAX_ATTACK_RANGE) {
						testDestinations.Add(candidate);
					}
			}
		}
		List<int> testDistances = mapManager.getDistances(source, testDestinations, true, MAX_ATTACK_RANGE);
		if (testDistances == null) return -1;
		
		Pos bestDest = null;
		int bestDist = int.MaxValue;
		for (int i = 0; i < testDistances.Count; i++) {
			if (testDistances[i] == -1) continue;
			
			if (testDistances[i] < bestDist) {
				bestDest = testDestinations[i];
				bestDist = testDistances[i];
			}
		}
		if (bestDest == null) return -1;
		if (bestDist == 0) return 0;
		
		Path path = mapManager.get_path(source, bestDest);
		path.truncateTo(parent.move_budget);
		
		bestDest = path.endPos();
		mapManager.move(source, bestDest);
		
		// if enemy path has been truncated
		int trunc_amount = bestDist - parent.move_budget;
		if (trunc_amount > 0 && trunc_amount > maxStopDistance)
			return 2;
		else
			return 1;
	}

	public void addEnemyToPool(GameAgent enemy)
	{
		if (Pos.abs_dist(enemy.grid_pos, parent.grid_pos) <= MAX_ATTACK_RANGE) {
			enemyPool.Add(enemy);
		}
	}
	
	public void addAllyToPool(GameAgent ally)
	{
		if (Pos.abs_dist(ally.grid_pos, parent.grid_pos) <= MAX_REINFORCE_RANGE) {
			alliedPool.Add(ally);
		}
	}
	
	public void calcDistances()
	{
		List<Pos> tempPositions = new List<Pos>();
		foreach (GameAgent e in enemyPool) tempPositions.Add(e.grid_pos);
		
		enemyDistances = mapManager.getDistances(parent.grid_pos, tempPositions, true, MAX_ATTACK_RANGE);
		
		tempPositions.Clear();
		foreach (GameAgent a in alliedPool) tempPositions.Add(a.grid_pos);
		
		alliedDistances = mapManager.getDistances(parent.grid_pos, tempPositions, true, MAX_REINFORCE_RANGE);
	}
	
	public bool calcAttack()
	{
		GameAgent mostDesireable = null;
		float bestRating = -1;
		for (int i = 0; i < enemyPool.Count; i++) {
			if (enemyDistances[i] == -1) continue;
			
			float rating = attackRating(enemyPool[i], enemyDistances[i]);
			if (rating > bestRating) {
				mostDesireable = enemyPool[i];
				bestRating = rating;
			}
		}
		if (mostDesireable != null) {
			attacking = mostDesireable;
			state = STATE.ATTACK;
			return true;
		}
		return false;
	}
	
	public void calcReinforce()
	{
		if (state == STATE.ATTACK) return;
		
		GameAgent mostDesireable = null;
		float bestRating = -1;
		for (int i = 0; i < alliedPool.Count; i++) {
			if (alliedDistances[i] == -1) continue;
			
			float rating = reinforceRating(alliedPool[i], alliedDistances[i]);
			if (rating > bestRating) {
				mostDesireable = alliedPool[i];
				bestRating = rating;
			}
		}
		if (mostDesireable != null) {
			reinforcing = mostDesireable;
			state = STATE.REINFORCE;
		}
	}
	
	private float reinforceRating(GameAgent ally, int distance)
	{
		if (ally.AI.state == STATE.REINFORCE)
			return (float)distance / (float)MAX_REINFORCE_RANGE;
		if (ally.AI.state == STATE.ATTACK)
			return (float)distance / (float)MAX_REINFORCE_RANGE + 2;
		return -1;
	}
	
	private float attackRating(GameAgent enemy, int distance)
	{
		return 1;
	}
	
	public string getStateString()
	{
		switch (state) {
			case STATE.IDLE: return "IDLE";
			case STATE.ATTACK: return "ATTACK";
			case STATE.REINFORCE: return "REINFORCE";
			case STATE.FLEE: return "FLEE";
		}
		return "";
	}
}
