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

    // player turn options
	public bool player_turn = true;
    private bool playerMovedThisTurn = false;
    private bool playerActedThisTurn = false;
    private bool playerUsedPotionThisTurn = false;
    private bool playerWaitingThisTurn = false;

    [Header("Player Stats")]
    public string name;
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
    // This menu is specific to the player character class
    PlayerActMenu actMenu;

    // Get rid of this when you get rid of using keys to change player class
    List<Player> playersForTestingPurposes;

    // Gets references to necessary game components
    public override void init_agent(Pos position, GameAgentStats stats, string name = null)
    {
		map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
        grid_pos = position;

        this.name = name;

        this.stats = stats;
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        actMenu = GetComponent<PlayerActMenu>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);
        actMenu.init();

        selectableTiles = new List<Pos>();

		tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
		tile_selector.setPlayer(this);

        TurnManager.instance.AddPlayerToList(this); //add player to player list

        currentState = GameAgentState.Alive;
    }

	// if right mouse button is pressed, move player model to hover position
    public void RespondToMouseClick()
    {
		if (!moving && !isAttacking && hoveringActionTileSelector) {

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
                case GameAgentAction.MagicAttackSingleTarget:
                case GameAgentAction.RangedAttack:
                case GameAgentAction.RangedAttackMultiShot:
				    if ((tile_selector.hoveringValidSelectTile() || godMode) && map_manager.IsOccupied(tile_selector.grid_position)) {

					    Pos attackPos = tile_selector.grid_position;
                        StartCoroutine(animator.PlayAttackAnimation());
					    StartCoroutine(WaitForAttackEnd(attackPos));
                    }
                    break;
            }
		}
	}

	public void RespondToKeyboardInput(char key)
	{
		switch (key) {
		    case '1': StartCoroutine(animator.PlayRotateAnimation()); break;
		    case '2': StartCoroutine(animator.PlayAttackAnimation()); break;
		    case '3': StartCoroutine(animator.PlayUseItemAnimation()); break;
		    case '4': StartCoroutine(animator.PlayHitAnimation()); break;
		    case '5': StartCoroutine(animator.PlayBlockAnimation()); break;
		    case '6': StartCoroutine(animator.PlayKilledAimation()); break;
            case 'a': TestCharacterClass(CharacterClassOptions.Knight); break;
            case 's': TestCharacterClass(CharacterClassOptions.Hunter); break;
            case 'd': TestCharacterClass(CharacterClassOptions.Mage); break;
            case 'f': TestCharacterClass(CharacterClassOptions.Healer); break;
        }
    }

    public void DeactivatePlayerActionMenu() {
        if (tile_selector.showSelectableMoveTiles) {
            hoveringActionTileSelector = false;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = false;
            tile_selector.showPathLine = false;
            tile_selector.clear_path_line();
        }

        if (actMenu.IsPlayerActMenuActive()) {
            actMenu.SetPlayerActMenuActive(false);
            tile_selector.showSelectableActTiles = false;
            hoveringActionTileSelector = false;
        }
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
        //actMenu.SetPlayerActMenuActive(false);
        action4();
        playerActedThisTurn = true;
        actMenu.MakeButtonNoninteractable(ActMenuButtons.ACT);
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
        playerActedThisTurn = false;
        playerUsedPotionThisTurn = false;
        playerWaitingThisTurn = false;
        actMenu.MakeAllButtonsInteractable(true);
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
        actMenu.MakeButtonNoninteractable(ActMenuButtons.MOVE);
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
        if (isAttacking) isAttacking = false;
    }

	public void WeaponSwitch(){}

    public override void move() {
        // Hide move selection if open
        if (tile_selector.showSelectableMoveTiles) {
            hoveringActionTileSelector = false;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = false;
            tile_selector.showPathLine = false;
            tile_selector.clear_path_line();
            return;
        }

        

        if (playerMovedThisTurn || !player_turn)
            return;

        currentAction = GameAgentAction.Move;
		tile_selector.CreateListOfSelectableMovementTiles(grid_pos, (int)stats.speed, currentAction);
        
        hoveringActionTileSelector = true;
        tile_selector.showPathLine = true;
        tile_selector.showSelectableMoveTiles = true;
    }

    public override void act() {
        // Hide move selection if open
        if (tile_selector.showSelectableMoveTiles) {
            hoveringActionTileSelector = false;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = false;
            tile_selector.showPathLine = false;
            tile_selector.clear_path_line();
        }

        if (!player_turn || playerActedThisTurn)
            return;

        actMenu.SetPlayerActMenuActive(true, stats.playerCharacterClass.GetAvailableActs());
    }

    public void action1() {
        if (stats.playerCharacterClass.GetAvailableActs().Length >= 1) {
            currentAction = (stats.playerCharacterClass.GetAvailableActs())[0];
        }

        // If other action is shown, hide it
        if (tile_selector.showSelectableActTiles) {
            hoveringActionTileSelector = false;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = false;
            return;
        }

        if (currentAction == GameAgentAction.MeleeAttack || currentAction == GameAgentAction.MagicAttackSingleTarget
            || currentAction == GameAgentAction.RangedAttack) {
            tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range, currentAction);

            hoveringActionTileSelector = true;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = true;
        }
    }

    public void action2() {
        if (stats.playerCharacterClass.GetAvailableActs().Length >= 2) {
            currentAction = (stats.playerCharacterClass.GetAvailableActs())[1];
        }

        // If other action is shown, hide it
        if (tile_selector.showSelectableActTiles) {
            hoveringActionTileSelector = false;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = false;
            return;
        }

        if (currentAction == GameAgentAction.Heal || currentAction == GameAgentAction.MagicAttackAOE
            || currentAction == GameAgentAction.Taunt || currentAction == GameAgentAction.RangedAttackMultiShot) {
            tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range, currentAction);

            hoveringActionTileSelector = true;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = true;
        }
    }

    public void action3() {
    
    }

    public void action4() {
        // Return to the battle menu
        if (actMenu.IsPlayerActMenuActive()) {
                actMenu.SetPlayerActMenuActive(false);
                tile_selector.showSelectableActTiles = false;
                hoveringActionTileSelector = false;
        }
    }

    public override void wait() {
        if (!player_turn || playerWaitingThisTurn)
            return;
        currentAction = GameAgentAction.Wait;

        actMenu.SetPlayerActMenuActive(false);
        tile_selector.showPathLine = false;
        tile_selector.showSelectableMoveTiles = false;
        tile_selector.showSelectableActTiles = false;
        tile_selector.clear_path_line();

        playerWaitingThisTurn = true;
    }

    public override void potion() {
        if (!player_turn || playerUsedPotionThisTurn)
            return;
        actMenu.SetPlayerActMenuActive(false);
        hoveringActionTileSelector = false;
        tile_selector.showPathLine = false;
        tile_selector.showSelectableMoveTiles = false;
        tile_selector.showSelectableActTiles = false;
        tile_selector.clear_path_line();

        currentAction = GameAgentAction.Potion;
        StartCoroutine(animator.PlayUseItemAnimation());
        stats.currentHealth += 10;
        Debug.Log("Potion Action");
        playerUsedPotionThisTurn = true;
        actMenu.MakeButtonNoninteractable(ActMenuButtons.POTION);
    }

    public void UpdateViewableEditorPlayerStats() {
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;
        range = stats.range;
        _speed = stats.speed;
    }

    public void TestCharacterClass(int characterClassToTest) {
        int weapon = CharacterClassOptions.RandomClassWeapon;
        if (characterClassToTest == CharacterClassOptions.Knight) {
            weapon = CharacterClassOptions.Sword;
        }

        stats = new GameAgentStats(CharacterRaceOptions.Human, characterClassToTest, 1, weapon);
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;

        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);
        DeactivatePlayerActionMenu();

        if (playersForTestingPurposes != null) {
            UpdatePlayerStatsMenu(playersForTestingPurposes);
        }
    }

    public void CheckIfPlayerTurnHasEnded() {
        // Player chose to wait
        if (playerWaitingThisTurn) {
            player_turn = false;
            actMenu.MakeAllButtonsInteractable(false);
        // Player moved and either used Act or used a Potion
        } else if (playerMovedThisTurn && (playerActedThisTurn || playerUsedPotionThisTurn)) {
            player_turn = false;
            actMenu.MakeAllButtonsInteractable(false);
            // Player used Act and used a Potion
        } else if (playerActedThisTurn && playerUsedPotionThisTurn) {
            player_turn = false;
            actMenu.MakeAllButtonsInteractable(false);
        }
    }

    public void UpdatePlayerStatsMenu(List<Player> players) {
        // Get rid of this when you get rid of using keys to change player class
        playersForTestingPurposes = players;

        int[] sortedPlayersIndex = SortPlayerListAlphabetically(players);
        for (int i = 0; i < sortedPlayersIndex.Length; i++) {
            actMenu.UpdatePlayerStatsMenu(i, players[sortedPlayersIndex[i]].name, players[sortedPlayersIndex[i]].stats);
        }

        // Deactivate the other nonactive players
        for (int i = sortedPlayersIndex.Length; i < 4; i++ ) {
            actMenu.UpdatePlayerStatsMenu(i, "", null, true);
        }
    }

    // returns an array of alphabetically arranged player indexes based on player.name
    private int[] SortPlayerListAlphabetically(List<Player> players) {
        var playerNames = new List<string>();
        int[] sortedPlayersIndex = new int[players.Count];
        int playerNameIndex = 0;

        foreach (Player player in players) {
            playerNames.Add(player.name);
        }

        // sorts alphabetically
        playerNames.Sort();

        // get a list of sorted alphabetical indexs
        for (int i=0; i < playerNames.Count; i++) {
            for (int j=0; j < players.Count; j++) {
                if (playerNames[i] == players[j].name) {
                    if (name == players[j].name) {
                        playerNameIndex = i;
                    }
                    sortedPlayersIndex[i] = j;
                    break;
                }
            }
        }

        // Move this player to the first spot
        if (playerNameIndex != 0) {
            int temp = sortedPlayersIndex[0];
            sortedPlayersIndex[0] = sortedPlayersIndex[playerNameIndex];
            sortedPlayersIndex[playerNameIndex] = temp;
        }

        return sortedPlayersIndex;
    }
}
