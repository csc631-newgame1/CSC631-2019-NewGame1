﻿using System.Collections;
using System.Collections.Generic;
using MapUtils;
using UnityEngine;

public class TestEnemy : GameAgent
{
    private MapManager map_manager; // reference to MapManager instance with map data
    private TileSelector tile_selector; // reference to map tile selector

    // private reference to position in map grid
    private bool moving;

    private int move_budget;
    private bool player_turn;

    [Header("Enemy Stats")]
    public float attack;
    public float maxHealth;
    public float currentHealth;
    public float range;
    public float speed;

    public override void init_agent(Pos position, GameAgentStats stats) {
        tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
        map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();
        grid_pos = position;

        this.stats = stats;
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        speed = stats.speed;
    }

    public override IEnumerator smooth_movement(List<Pos> locations) {
        return null;
    }

    public override void take_damage(int amount) {
        stats.currentHealth -= amount;
        if (stats.currentHealth <= 0) {
            stats.currentHealth = 0;
        }

        currentHealth = stats.currentHealth;
    }

    public override void take_turn() {
    }

    public override void move() {
    }

    public override void act() {
    }

    public override void wait() {
    }

    public override void potion() {
    }
}
