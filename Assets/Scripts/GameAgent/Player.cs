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
    public bool isAttacking = false;
	public bool godMode = true;

	public bool player_turn = true;
    private bool playerMovedThisTurn = false;

    [Header("Player Stats")]
    public float attack;
    public float maxHealth;
    public float currentHealth;
    public float range;
    public float _speed;
	public int move_budget = 8;

    // 0 - unarmed, 1 - sword, 2 - bow, 3 - staff
    public int weapon = 1;

	CharacterAnimator animator;
    CharacterClassDefiner classDefiner;

    // Gets references to necessary game components
    public override void init_agent(Pos position, GameAgentStats stats)
    {
		map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();
        config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        grid_pos = position;

        this.stats = stats;
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterClassOption);

        selectableTiles = new List<Pos>();

		tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
		tile_selector.setPlayer(this);

        TurnManager.instance.AddPlayerToList(this); //add player to player list
    }

	// if right mouse button is pressed, move player model to hover position
	// if hover position is on a bridge tile, change the player model
    void Update()
    {
		if (Input.GetMouseButtonDown(1) && !moving && hoveringActionTileSelector) {
			
			switch (currentAction) {
				
			case GameAgentAction.Move:
			
				if ((tile_selector.hoveringValidMoveTile() || godMode) && map_manager.move(grid_pos, tile_selector.grid_position)) {
					
                    grid_pos = tile_selector.grid_position;
                    hoveringActionTileSelector = false;
                    tile_selector.showSelectableMoveTiles = false;
                    tile_selector.showPathLine = false;
                }
				break;
				
			case GameAgentAction.MeleeAttack:
			
				if ((tile_selector.hoveringValidSelectTile() || godMode) && map_manager.IsOccupied(tile_selector.grid_position)) {
					
					Pos attackPos = tile_selector.grid_position;
                    StartCoroutine(animator.PlayAttackAnimation());
					StartCoroutine(WaitForAttackEnd(attackPos));
                }
                break;
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
	
	IEnumerator WaitForAttackEnd(Pos attackPos)
	{
		isAttacking = true;
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        this.transform.LookAt(map_manager.GetUnitTransform(attackPos));

        while (isAttacking) yield return null;
		map_manager.attack(attackPos, (int)stats.attack);

        hoveringActionTileSelector = false;
        tile_selector.showSelectableActTiles = false;
	}
	
	public override void take_damage(int amount)
	{	
        if (!godMode) stats.currentHealth -= amount;
		
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
        playerMovedThisTurn = false;
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
        playerMovedThisTurn = true;
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

    // Signal end of attack once hit animation has completed
	public void Hit(){
		if (isAttacking) isAttacking = false;
    }

	public void Shoot(){
        if (isAttacking) {
            map_manager.attack(tile_selector.grid_position, (int)stats.attack);
            hoveringActionTileSelector = false;
            tile_selector.showSelectableActTiles = false;
        }
    }
	public void WeaponSwitch(){}

    public override void move() {
        if (playerMovedThisTurn || !player_turn)
            return;
		currentAction = GameAgentAction.Move;
		tile_selector.CreateListOfSelectableMovementTiles(grid_pos, move_budget, currentAction);
        hoveringActionTileSelector = true;
        tile_selector.showPathLine = true;
        tile_selector.showSelectableMoveTiles = true;
        tile_selector.showSelectableActTiles = false;
    }

    public override void act() {
        if (!player_turn)
            return;
        currentAction = GameAgentAction.MeleeAttack;
        tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range, currentAction);

        hoveringActionTileSelector = true;
        tile_selector.showSelectableMoveTiles = false;
        tile_selector.showSelectableActTiles = true;
    }

    public override void wait() {
        if (!player_turn)
            return;
        currentAction = GameAgentAction.Wait;
        tile_selector.showPathLine = false;
        tile_selector.showSelectableMoveTiles = false;
        tile_selector.showSelectableActTiles = false;
        Debug.Log("Wait Action");
        player_turn = false;
    }

    public override void potion() {
        if (!player_turn)
            return;
        currentAction = GameAgentAction.Potion;
        StartCoroutine(animator.PlayUseItemAnimation());
        stats.currentHealth += 10;
        Debug.Log("Potion Action");
        player_turn = false;
    }

    public void UpdateViewablePlayerStats() {
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;
        range = stats.range;
        _speed = stats.speed;
    }
}
