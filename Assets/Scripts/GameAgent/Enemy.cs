using System;
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
	
	public bool enemy_turn = false;

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

        TurnManager.instance.AddEnemyToList(this);

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);
    }

    public override IEnumerator smooth_movement(List<Pos> locations) {
        return null;
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
        StartCoroutine(animator.PlayAttackAnimation());
		enemy_turn = false;
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
