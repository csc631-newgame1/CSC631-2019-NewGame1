using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapUtils;
using System;

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
	public bool godMode = false;

    // player turn options
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
    public int level;
    public string viewableState;

    // 0 - unarmed, 1 - sword, 2 - bow, 3 - staff
    public int weapon = 1;

	CharacterAnimator animator;
    CharacterClassDefiner classDefiner;

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
		move_budget = 10;
        UpdateViewableEditorPlayerStats();

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);
		
		PlayerActMenu.init();

        selectableTiles = new List<Pos>();

		tile_selector = GameObject.FindGameObjectWithTag("Map").transform.Find("TileSelector").GetComponent<TileSelector>();
		tile_selector.setPlayer(this);

        currentState = GameAgentState.Alive;
		
		// AI init
		team = 0;
		AI = null; // players don't have AI
		TurnManager.instance.addToRoster(this); //add player to player list
    }

	// if right mouse button is pressed, move player model to hover position
    public void RespondToMouseClick()
    {
		if (!moving && !isAttacking && hoveringActionTileSelector) {
			switch (currentAction) {
			    case GameAgentAction.Move:
				    if ((tile_selector.hoveringValidMoveTile() || godMode) && map_manager.move(grid_pos, tile_selector.grid_position)) {

                        hoveringActionTileSelector = false;
                        tile_selector.showSelectableMoveTiles = false;
                        tile_selector.showPathLine = false;
                    }
				    break;
			    case GameAgentAction.MeleeAttack:
                case GameAgentAction.MagicAttackSingleTarget:
                case GameAgentAction.RangedAttack:
				    if ((tile_selector.hoveringValidSelectTile() || godMode) && map_manager.IsOccupied(tile_selector.grid_position) && map_manager.GetGameAgentState(tile_selector.grid_position) == GameAgentState.Alive) {

					    Pos attackPos = tile_selector.grid_position;
                        StartCoroutine(animator.PlayAttackAnimation());
					    StartCoroutine(WaitForAttackEnd(attackPos));
                    }
                    break;
                case GameAgentAction.RangedAttackMultiShot:
                    if ((tile_selector.hoveringValidSelectTile() || godMode) && map_manager.IsOccupied(tile_selector.grid_position) && map_manager.GetGameAgentState(tile_selector.grid_position) == GameAgentState.Alive) {

                        Pos attackPos = tile_selector.grid_position;
                        StartCoroutine(animator.PlayAttackAnimation());
                        StartCoroutine(WaitForRangedAttackMultiShotEnd(attackPos, stats.GetMultiShotCount()));
                    }
                    break;
                case GameAgentAction.Taunt:
                    StartCoroutine(animator.PlayTauntAnimation());
                    StartCoroutine(WaitForTauntEnd());
                    break;
                case GameAgentAction.MagicAttackAOE:
                case GameAgentAction.Heal:
                    if (tile_selector.hoveringValidActAOETile() || godMode) {

                        Pos attackPos = tile_selector.grid_position;
                        StartCoroutine(animator.PlayAttackAnimation());
                        StartCoroutine(WaitForAOEEnd(tile_selector.GetPositionOfAgentsInActAOETiles()));
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

	IEnumerator WaitForAttackEnd(Pos attackPos)
	{
		isAttacking = true;
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        this.transform.LookAt(map_manager.GetUnitTransform(attackPos));

        while (isAttacking) yield return null;
        map_manager.attack(attackPos, stats.DealDamage());

        TurnOffSelectors();
        action4();
        playerActedThisTurn = true;
        PlayerActMenu.MakeButtonNoninteractable(ActMenuButtons.ACT);
    }

    IEnumerator WaitForRangedAttackMultiShotEnd(Pos attackPos, int multiShotCount) {
        isAttacking = true;
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        this.transform.LookAt(map_manager.GetUnitTransform(attackPos));

        while (isAttacking) yield return null;
        map_manager.attack(attackPos, stats.GetMultiShotDamage());

        // Stop attacking if target is dead
        if (map_manager.GetGameAgentState(attackPos) != GameAgentState.Alive) {
            multiShotCount = 0;
        }

        if (multiShotCount > 0) {
            while (animator.AnimatorIsPlaying()) yield return null;
            StartCoroutine(animator.PlayAttackAnimation());
            StartCoroutine(WaitForRangedAttackMultiShotEnd(attackPos, --multiShotCount));
        } else {
            playerActedThisTurn = true;
        }

        TurnOffSelectors();
        action4();
        PlayerActMenu.MakeButtonNoninteractable(ActMenuButtons.ACT);

    }

    IEnumerator WaitForTauntEnd() {
        isAttacking = true;
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        Transform lookDirection = map_manager.GetNearestUnitTransform(grid_pos, tile_selector.GetPositionOfAgentsInNonselectableActTiles());
        if (lookDirection != null) {
            this.transform.LookAt(lookDirection);
        }
         //&& map_manager.GetGameAgentState(tile_selector.grid_position) == GameAgentState.Alive
        while (animator.AnimatorIsPlaying()) yield return null;
        isAttacking = false;

        TurnOffSelectors();
        action4();
        playerActedThisTurn = true;
        PlayerActMenu.MakeButtonNoninteractable(ActMenuButtons.ACT);
    }

    IEnumerator WaitForAOEEnd(List<Pos> targetTiles) {
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        Transform lookDirection = map_manager.GetNearestUnitTransform(grid_pos, tile_selector.GetPositionOfAgentsInActAOETiles());
        if (lookDirection != null) {
            this.transform.LookAt(lookDirection);
        }

        if (currentAction == GameAgentAction.MagicAttackAOE) {
            isAttacking = true;
            while (isAttacking) yield return null;
            foreach (Pos tile in targetTiles) {
                map_manager.attack(tile, stats.DealDamage());
            }
        } else if (currentAction == GameAgentAction.Heal) {
            isAttacking = true;
            while (isAttacking) yield return null;
            foreach (Pos tile in targetTiles) {
                map_manager.GetHealed(tile, stats.GetHealAmount());
            }
        }

        TurnOffSelectors();
        action4();
        playerActedThisTurn = true;
        PlayerActMenu.MakeButtonNoninteractable(ActMenuButtons.ACT);
    }

    public override void take_damage(int amount)
	{
        if (stats.currentState == GameAgentState.Alive) {
            if (!godMode) stats.TakeDamage(amount);

            if (stats.currentState == GameAgentState.Unconscious) {
                StartCoroutine(animator.PlayKilledAimation());
            } else {
                StartCoroutine(animator.PlayHitAnimation());
            }
        }

        UpdateViewableEditorPlayerStats();
    }

    public override void GetHealed(int amount) {
        if (stats.currentState == GameAgentState.Alive) {
            if (!godMode) stats.GetHealed(amount);

            StartCoroutine(animator.PlayUseItemAnimation());
        }

        UpdateViewableEditorPlayerStats();
    }

    public override void take_turn()
	{
        if (stats.currentState == GameAgentState.Alive) {
            playerMovedThisTurn = false;
            playerActedThisTurn = false;
            playerUsedPotionThisTurn = false;
            playerWaitingThisTurn = false;
		    PlayerActMenu.MakeAllButtonsInteractable(true);
        }

        UpdateViewableEditorPlayerStats();
    }

    private void UpdateViewableEditorPlayerStats() {
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;
        range = stats.range;
        _speed = stats.speed;
        level = stats.level;

        switch (stats.currentState) {
            case GameAgentState.Alive:
                viewableState = "Alive";
                break;
            case GameAgentState.Unconscious:
                viewableState = "Unconscious";
                break;
            case GameAgentState.Dead:
                viewableState = "Dead";
                break;
        }
    }

    public override IEnumerator smooth_movement(List<Pos> path)
	{
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
		tile_selector.clear_path_line();
        playerMovedThisTurn = true;
        PlayerActMenu.MakeButtonNoninteractable(ActMenuButtons.MOVE);
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

    public void TurnOffSelectors() {
        hoveringActionTileSelector = false;
        tile_selector.showSelectableMoveTiles = false;
        tile_selector.showSelectableActTiles = false;
        tile_selector.showPathLine = false;
        tile_selector.showAOETiles = false;
        tile_selector.clear_path_line();
    }

    public override void move() {
		if (playerMovedThisTurn || turn_over())
            return;
		
        // Hide move selection if open
        if (tile_selector.showSelectableMoveTiles) {
            TurnOffSelectors();
            return;
        }

        if (playerMovedThisTurn || turn_over() || stats.currentState != GameAgentState.Alive)
            return;

        currentAction = GameAgentAction.Move;
		tile_selector.CreateListOfSelectableMovementTiles(grid_pos, (int)stats.speed);//, currentAction);

        hoveringActionTileSelector = true;
        tile_selector.showPathLine = true;
        tile_selector.showSelectableMoveTiles = true;
    }

    public override void act() {
        // Hide move selection if open
        if (tile_selector.showSelectableMoveTiles) {
            TurnOffSelectors();
        }

        if (turn_over() || playerActedThisTurn || stats.currentState != GameAgentState.Alive)
            return;

        PlayerActMenu.SetPlayerActMenuActive(true, stats.playerCharacterClass.GetAvailableActs());
    }

    public void action1() {
        // Stop showing action2
        if (currentAction == GameAgentAction.Heal || currentAction == GameAgentAction.MagicAttackAOE
            || currentAction == GameAgentAction.Taunt || currentAction == GameAgentAction.RangedAttackMultiShot) {
            TurnOffSelectors();
        // stop showing action1
        } else if ((currentAction == GameAgentAction.MeleeAttack || currentAction == GameAgentAction.MagicAttackSingleTarget
            || currentAction == GameAgentAction.RangedAttack) && tile_selector.showSelectableActTiles) {
            TurnOffSelectors();
            return;
        }

        if (stats.playerCharacterClass.GetAvailableActs().Length >= 1) {
            currentAction = (stats.playerCharacterClass.GetAvailableActs())[0];
        }

        if (currentAction == GameAgentAction.MeleeAttack || currentAction == GameAgentAction.MagicAttackSingleTarget
            || currentAction == GameAgentAction.RangedAttack) {
            tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range);//, currentAction);

            hoveringActionTileSelector = true;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = true;
            tile_selector.showAOETiles = false;
        }
    }

    public void action2() {
		if (turn_over())
            return;
		
        // stop showing action1
        if (currentAction == GameAgentAction.MeleeAttack || currentAction == GameAgentAction.MagicAttackSingleTarget
            || currentAction == GameAgentAction.RangedAttack) {
            TurnOffSelectors();
        // stop showing action2
        } else if ((currentAction == GameAgentAction.Heal || currentAction == GameAgentAction.MagicAttackAOE
            || currentAction == GameAgentAction.Taunt || currentAction == GameAgentAction.RangedAttackMultiShot)
            && tile_selector.showSelectableActTiles) {
            TurnOffSelectors();
            return;
        }

        if (stats.playerCharacterClass.GetAvailableActs().Length >= 2) {
            currentAction = (stats.playerCharacterClass.GetAvailableActs())[1];
        }
        if (currentAction == GameAgentAction.Heal || currentAction == GameAgentAction.MagicAttackAOE) {
            tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range);//, currentAction);
            hoveringActionTileSelector = true;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = true;
            tile_selector.showAOETiles = true;
        }

        if (currentAction == GameAgentAction.Taunt || currentAction == GameAgentAction.RangedAttackMultiShot) {
            tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range);//, currentAction);
            hoveringActionTileSelector = true;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = true;
        }
    }

    public void action3() {

    }

    public void action4() {
        // Return to the battle menu
        if (PlayerActMenu.IsPlayerActMenuActive()) {
            PlayerActMenu.SetPlayerActMenuActive(false);
            TurnOffSelectors();
        }
    }

    public override void wait() {
        if (turn_over() || playerWaitingThisTurn || stats.currentState != GameAgentState.Alive)
            return;
		
        currentAction = GameAgentAction.Wait;

        PlayerActMenu.SetPlayerActMenuActive(false);
        TurnOffSelectors();

        playerWaitingThisTurn = true;
    }

    public override void potion() {

        if (turn_over() || playerUsedPotionThisTurn || stats.currentState != GameAgentState.Alive)
            return;
        PlayerActMenu.SetPlayerActMenuActive(false);
        TurnOffSelectors();

        currentAction = GameAgentAction.Potion;
        StartCoroutine(animator.PlayUseItemAnimation());
        stats.UsePotion();
        playerUsedPotionThisTurn = true;

        UpdateViewableEditorPlayerStats();
		PlayerActMenu.MakeButtonNoninteractable(ActMenuButtons.POTION);
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

        if (playersForTestingPurposes != null) {
            UpdatePlayerStatsMenu(playersForTestingPurposes);
        }
    }

    public override bool turn_over() {
        // Player chose to wait
        if (playerWaitingThisTurn) {
            PlayerActMenu.MakeAllButtonsInteractable(false);
            return true;
        // Player used Act and used a Potion
        } else if (playerActedThisTurn) {
            PlayerActMenu.MakeAllButtonsInteractable(false);
            return true;
        // Player moved and either used Act or used a Potion
        } else if (playerMovedThisTurn && playerUsedPotionThisTurn) {
            PlayerActMenu.MakeAllButtonsInteractable(false);
            return true;
        }
        return false;
    }

    public void UpdatePlayerStatsMenu(List<Player> players) {
        // Get rid of this when you get rid of using keys to change player class
        playersForTestingPurposes = players;

        int[] sortedPlayersIndex = SortPlayerListAlphabetically(players);
        for (int i = 0; i < sortedPlayersIndex.Length; i++) {
            PlayerActMenu.UpdatePlayerStatsMenu(i, players[sortedPlayersIndex[i]].name, players[sortedPlayersIndex[i]].stats);
        }

        // Deactivate the other nonactive players
        for (int i = sortedPlayersIndex.Length; i < 4; i++ ) {
            PlayerActMenu.UpdatePlayerStatsMenu(i, "", null, true);
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
