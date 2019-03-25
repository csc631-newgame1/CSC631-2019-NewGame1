using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MapUtils;
using UnityEngine;

public class Enemy : GameAgent
{
    private MapManager map_manager; // reference to MapManager instance with map data
    private TileSelector tile_selector; // reference to map tile selector

    // private reference to position in map grid
    private bool moving;
	
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
	public string state = "IDLE";

    public override void init_agent(Pos position, GameAgentStats stats) 
	{
        tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
        map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
        grid_pos = position;

        animator = GetComponent<CharacterAnimator>();

        this.stats = stats;
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;
		speed = 10;
		move_budget = 10;

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);
		
		// AI init
		team = 1;
		AI = new AIComponent(this); // AI component that decides the actions for this enemy to take
		TurnManager.instance.addToRoster(this);
    }

    public override IEnumerator smooth_movement(List<Pos> path) {
		grid_pos = path.Last();
        moving = true;
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
        moving = false;
    }
	
	public override void take_damage(int amount) {
        stats.currentHealth -= amount;
        if (stats.currentHealth <= 0) {
			
            stats.currentHealth = 0;
            StartCoroutine(animator.PlayKilledAimation());
			
        } else {
            StartCoroutine(animator.PlayHitAnimation());
        }

        currentHealth = stats.currentHealth;
    }

    public override void take_turn() {
		enemy_turn = true;
		AI.advance();
		state = AI.getStateString();
        StartCoroutine(animator.PlayAttackAnimation());
		enemy_turn = false;
    }
	
	public override bool turn_over() {
		return !enemy_turn;
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
    public void Hit() { }
    public void Shoot() { }
    public void WeaponSwitch() { }
}
