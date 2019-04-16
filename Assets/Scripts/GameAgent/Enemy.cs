using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MapUtils;
using UnityEngine;

public class Enemy : GameAgent
{
    private MapManager map_manager; // reference to MapManager instance with map data

	private bool enemy_turn = false;

    private CharacterAnimator animator;
    private CharacterClassDefiner classDefiner;

    [Header("Enemy Stats")]
    public float attack;
    public float maxHealth;
    public float currentHealth;
    public float range;
    public float _speed;
    public float moveTime = 0.1f;

	public int moveBudget;
    public int level;
    public GameAgentState viewableState;

    public override void init_agent(Pos position, GameAgentStats stats, string name = null) 
	{
        map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
        grid_pos = position;

        animator = GetComponent<CharacterAnimator>();

        this.stats = stats;
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;
		this.nickname = CharacterRaceOptions.getString(stats.characterRace) + " " + CharacterClassOptions.getWeaponDescriptor(stats.playerCharacterClass.weapon);
		
		speed = 100;
		move_budget = 10;
		
        level = stats.level;
        viewableState = stats.currentState;

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);
		
		// AI init
		team = 1;
		AI = new AIComponent(this); // AI component that decides the actions for this enemy to take
		TurnManager.instance.addToRoster(this);
    }

    public override IEnumerator smooth_movement(List<Pos> path) 
	{
		//Debug.Log("started...");
		grid_pos = path.Last();
        animating = true;
        StartCoroutine(animator.StartMovementAnimation());

			Vector3 origin, target;
			foreach(Pos step in path) {

				origin = transform.position;
				target = map_manager.grid_to_world(step);
				float dist = Vector3.Distance(origin, target);
				float time = 0f;

				transform.LookAt(target);

					while(time < 1f && dist > 0f) {
						time += (Time.deltaTime * speed) / dist;
						transform.position = Vector3.Lerp(origin, target, time);
						yield return null;
					}
			}
			transform.position = map_manager.grid_to_world(path[path.Count - 1]);

        StartCoroutine(animator.StopMovementAnimation());
        animating = false;
		//Debug.Log("ended...");
    }
	
	private bool attacking = false;
	
	public override IEnumerator animate_attack(GameAgent target)
	{
		//Debug.Log("started...");
		animating = true;
		attacking = true;
		
		// get target position, and distance between us and the enemy
		Vector3 targetPos = map_manager.grid_to_world(target.grid_pos);
		Vector3 ownPos = map_manager.grid_to_world(grid_pos);
		float distance = Math.Max(1, Vector3.Distance(ownPos, targetPos));
		
		// look at enemy and start attack animation
		transform.LookAt(targetPos);
		StartCoroutine(animator.PlayAttackAnimation());
		
		// wait for animation trigger
		while (attacking) yield return null;
		// wait a little longer based on projectile distance
		yield return new WaitForSeconds(distance / 100f);
		
		target.take_damage(stats.DealDamage());
		transform.position = ownPos; // reset to previous position after animation
		
		//Debug.Log("ended...");
		animating = false;
	}
	
    public void Hit() { attacking = false; }
    public void Shoot() { attacking = false; }
	
    public override void take_damage(int amount) 
	{	
        stats.TakeDamage(amount);

        if (stats.currentState == GameAgentState.Unconscious) {
            StartCoroutine(animator.PlayKilledAimation());
            stats.currentState = GameAgentState.Dead;
        } else {
            StartCoroutine(animator.PlayHitAnimation());
        }
		
		//StartCoroutine(wait_to_reset_position());
        currentHealth = stats.currentHealth;
    }
	
	/*private IEnumerator wait_to_reset_position()
	{
		Vector3 pos = transform.position;
		yield return new WaitForSeconds(1f);
		transform.position = pos;
	}*/

    public override void GetHealed(int amount) 
	{
        if (stats.currentState == GameAgentState.Alive) {
            stats.GetHealed(amount);

            StartCoroutine(animator.PlayUseItemAnimation());
        }
        currentHealth = stats.currentHealth;
    }

    public override void take_turn() 
	{	
		StartCoroutine(AI.advance());
    }
	
	public override bool turn_over() {
		return AI.finished;
	}

    public override void move() {
    }

    public override void act() {
    }

    public override void wait() {
    }

    public override void potion() {
    }

    public void FootR() { }
    public void FootL() { }
    public void WeaponSwitch() { }
}
