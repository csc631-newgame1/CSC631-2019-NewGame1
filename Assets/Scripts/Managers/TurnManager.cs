using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //static instance of game manager which allows it to be accessed by any other script
    public static TurnManager instance = null;
	
    //boolean to check if it's player's turn
    [HideInInspector] public bool playersTurn = true;
	[HideInInspector] public bool enemiesTurn = false;

    // lists of all active player/enemies
    private List<Player> players = new List<Player>();
    private List<Enemy> enemies = new List<Enemy>();
	
	private GameManager parentManager = null;
	
	void Awake()
	{
		instance = this;
	}
	
	public void Init(GameManager parent)
	{
		parentManager = parent;
		StartCoroutine(TurnLoop());
	}

    IEnumerator TurnLoop()
	{	
		while (true) {
			yield return null;
			
			enemiesTurn = false;
			playersTurn = true;
			
			Debug.Log("Beginning player turn!");
			
			foreach (Player player in players) {
				player.take_turn();
                UpdatePlayerStats();
			}
			
			while (!playerTurnOver()) yield return null;
			ClearDead();
			
			Debug.Log("Beginning enemy turn!");
			
			enemiesTurn = true;
			playersTurn = false;

            // Little delay between turns
            yield return new WaitForSeconds(1);

            foreach (Enemy enemy in enemies) {
                enemy.take_turn();
                UpdatePlayerStats();
            }
			
			while (!enemyTurnOver()) yield return null;
			ClearDead();
		}
    }
	
	bool enemyTurnOver()
	{
		foreach (Enemy enemy in enemies)
			if (enemy.enemy_turn) return false;
			
		return true;
	}

    bool playerTurnOver()
    {
        foreach (Player player in players) {
            player.CheckIfPlayerTurnHasEnded();
            if (player.player_turn) return false;
        }

        return true;
    }

    void UpdatePlayerStats() {
        if (players.Count > 0) {
            foreach (Player player in players) {
                player.UpdatePlayerStatsMenu(players);
            }
        }
    }
	
	void ClearDead()
	{
		foreach (Enemy enemy in enemies)
			if (enemy.currentHealth <= 0)
				parentManager.kill(enemy);
			
		foreach (Player player in players)
			if (player.currentHealth <= 0)
				parentManager.kill(player);
	}
	
	public void AddPlayerToList(Player player)
    {
        players.Add(player);
    }

    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void RemovePlayerFromList(Player player)
    {
        players.Remove(player);
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void ClearEnemyList()
    {
        enemies.Clear();
    }
	
	public void ClearPlayerList()
    {
        players.Clear();
    }
}
