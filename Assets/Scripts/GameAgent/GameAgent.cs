using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public enum GameAgentAction {Move = 0, Wait, Potion, MeleeAttack, RangedAttack, MagicAttack};

public abstract class GameAgent : MonoBehaviour
{
	public Pos grid_pos;
    protected GameAgentStats stats;
	
    public abstract IEnumerator smooth_movement(List<Pos> locations);
	
	public abstract void take_damage(int amount);
	
	public abstract void init_agent(Pos position, GameAgentStats stats);
	
	// for enemies, this will make them go through their AI motions
	// for players, this will trigger the boolean value that allows them to take their turn
	public abstract void take_turn();


    // commands from the action menu
    public abstract void move();
    public abstract void act();
    public abstract void wait();
    public abstract void potion();
}
