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
    public int level;
    public GameAgentState viewableState;

    public override void init_agent(Pos position, GameAgentStats stats, string name = null) 
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
        level = stats.level;
        viewableState = stats.currentState;

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
        stats.TakeDamage(amount);

        if (stats.currentState == GameAgentState.Unconscious) {
            StartCoroutine(animator.PlayKilledAimation());
            stats.currentState = GameAgentState.Dead;
            TurnManager.instance.RemoveEnemyFromList(this);
        } else {
            StartCoroutine(animator.PlayHitAnimation());
        }

        currentHealth = stats.currentHealth;
    }

    public override void GetHealed(int amount) {
        if (stats.currentState == GameAgentState.Alive) {
            stats.GetHealed(amount);

            StartCoroutine(animator.PlayUseItemAnimation());
        }

        currentHealth = stats.currentHealth;
    }

    public override void take_turn() {
        if (stats.currentState == GameAgentState.Alive) {
            enemy_turn = true;
            StartCoroutine(animator.PlayAttackAnimation());
            enemy_turn = false;
        }
    }

    public int RewardXPFromDeath() {
        return stats.RewardXPFromDeath();
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
