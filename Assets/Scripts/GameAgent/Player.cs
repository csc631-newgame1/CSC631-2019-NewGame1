using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapUtils;


public class Player : GameAgent
{
	private MapManager map_manager; // reference to MapManager instance with map data
    private MapConfiguration config;
	private TileSelector tile_selector; // reference to map tile selector
    private List<Pos> selectableTiles;
	
	// private reference to position in map grid
    public bool hoveringMovementTileSelector = false;
	public bool moving = false;
    public bool hoveringActionTileSelector = false;
	
	private int move_budget = 8;
	private int currentHealth;
	private bool player_turn = false;
	
    // 0 - unarmed, 1 - sword, 2 - bow, 3 - staff
    public int weapon = 1;

	CharacterAnimator animator;
	
    // Gets references to necessary game components
    public override void init_agent(Pos position, GameAgentStats stats)
    {
		map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();
        config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        grid_pos = position;
		animator = GetComponent<CharacterAnimator>();
        this.stats = stats;
        selectableTiles = new List<Pos>();
		
		tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
		tile_selector.setPlayer(this);
    }

	// if right mouse button is pressed, move player model to hover position
	// if hover position is on a bridge tile, change the player model
    void Update()
    {
		if (Input.GetMouseButtonDown(1) && !moving && hoveringActionTileSelector) {
            if (currentAction == GameAgentAction.Move) {
                if (map_manager.move(grid_pos, tile_selector.grid_position)) {
                    grid_pos = tile_selector.grid_position;
                    hoveringActionTileSelector = false;
                    tile_selector.showSelectableMoveTiles = false;
                    tile_selector.showPathLine = false;
                }
            } else if (currentAction == GameAgentAction.MeleeAttack) {
                if (map_manager.attack(tile_selector.grid_position, (int)stats.attack)) {
                    this.transform.LookAt(map_manager.GetUnitTransform(tile_selector.grid_position));
                    hoveringActionTileSelector = false;
                    tile_selector.showSelectableActTiles = false;
                    StartCoroutine(animator.PlayAttackAnimation());
                }
            }
		}

        // For testing animations.
        if (Input.GetKeyDown("1")) StartCoroutine(animator.PlayRotateAnimation());
        if (Input.GetKeyDown("2")) StartCoroutine(animator.PlayAttackAnimation());
        if (Input.GetKeyDown("3")) StartCoroutine(animator.PlayUseItemAnimation());
        if (Input.GetKeyDown("4")) StartCoroutine(animator.PlayHitAnimation());
        if (Input.GetKeyDown("5")) StartCoroutine(animator.PlayBlockAnimation());
        if (Input.GetKeyDown("6")) StartCoroutine(animator.PlayKilledAimation());
    }
	
	public override void take_damage(int amount)
	{
        stats.currentHealth -= amount;
        if (stats.currentHealth <= 0) {
            stats.currentHealth = 0;
            StartCoroutine(animator.PlayKilledAimation());
        } else {
            StartCoroutine(animator.PlayHitAnimation());
        }
    }

    public override void take_turn()
	{
		player_turn = true;
	}
	
	public override IEnumerator smooth_movement(List<Pos> path)
	{
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
				
				grid_pos = step;
			}
			transform.position = map_manager.grid_to_world(path[path.Count - 1]);

        StartCoroutine(animator.StopMovementAnimation());
        moving = false;
		tile_selector.clear_path_line();
	}
	
    void spawnActionRadius()
    {
        var exp = GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(gameObject, exp.duration);
    }

    bool isWithinActionReadius()
    {
        return false;
    }

	public void FootR(){}
	public void FootL(){}
	public void Hit(){}
	public void Shoot(){}
	public void WeaponSwitch(){}

    public override void move() {
        currentAction = GameAgentAction.Move;
		tile_selector.CreateListOfSelectableMovementTiles(grid_pos, move_budget, currentAction);

        hoveringActionTileSelector = true;
        tile_selector.showPathLine = true;
        tile_selector.showSelectableMoveTiles = true;
    }

    public override void act() {
        currentAction = GameAgentAction.MeleeAttack;
        tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range, currentAction);

        hoveringActionTileSelector = true;
        tile_selector.showSelectableActTiles = true;
    }

    public override void wait() {
        currentAction = GameAgentAction.Wait;
        Debug.Log("Wait Action");
    }

    public override void potion() {
        currentAction = GameAgentAction.Potion;
        StartCoroutine(animator.PlayUseItemAnimation());
        stats.currentHealth += 10;
        Debug.Log("Potion Action");
    }
}