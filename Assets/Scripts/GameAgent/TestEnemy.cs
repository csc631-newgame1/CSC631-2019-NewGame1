using System.Collections;
using System.Collections.Generic;
using MapUtils;
using UnityEngine;

public class TestEnemy : GameAgent
{
    private MapManager map_manager; // reference to MapManager instance with map data
    private TileSelector tile_selector; // reference to map tile selector

    // private reference to position in map grid
    private bool moving = false;

    private int move_budget;
    private int health = 100;
    private bool player_turn = false;
    public float speed;

    Animator animator;

    public override void init_agent(Pos position, GameAgentStats stats) {
        tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
        map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();
        grid_pos = position;
        animator = GetComponent<Animator>();

        this.stats = stats;
    }

    public override IEnumerator smooth_movement(List<Pos> locations) {
        return null;
    }

    public override void take_damage(int amount) {
    }

    public override void take_turn() {
    }
}
