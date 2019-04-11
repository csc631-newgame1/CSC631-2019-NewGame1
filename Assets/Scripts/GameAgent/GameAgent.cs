using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public enum GameAgentAction { Move, Wait, Potion, MeleeAttack, Taunt, RangedAttack, RangedAttackMultiShot, MagicAttackSingleTarget, MagicAttackAOE, Heal, Neutral };

public abstract class GameAgent : MonoBehaviour
{
	public Pos grid_pos;
    public float speed;
    public GameAgentStats stats;
    public GameAgentAction currentAction;
	
    public GameAgentState currentState;
	public AIComponent AI;
	public int team;
	public int move_budget;

    public Inventory inventory = new Inventory();
	public bool animating;
	
    public abstract IEnumerator smooth_movement(List<Pos> locations);
	
	public abstract IEnumerator animate_attack(GameAgent target);
	
	public abstract void take_damage(int amount);

    public abstract void GetHealed(int amount);
	
	public abstract void init_agent(Pos position, GameAgentStats stats, string name = null);
	
	// for enemies, this will make them go through their AI motions
	// for players, this will trigger the boolean value that allows them to take their turn
	public abstract void take_turn();
	
	public abstract bool turn_over();

    // commands from the action menu
    public abstract void move();
    public abstract void act();
    public abstract void wait();
    public abstract void potion();

    public void UseItemOnSelf(int slot)
    {
        Item item = inventory.GetItemFromSlot(slot);
        InventoryManager.instance.UseItem(item, this);
        inventory.DecrementItemAtSlot(slot);
    }
}
