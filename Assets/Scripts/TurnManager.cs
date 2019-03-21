using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //static instance of game manager which allows it to be accessed by any other script
    public static TurnManager instance = null;
    //boolean to check if it's player's turn
    [HideInInspector] public bool playersTurn;

    //list of all player units, probably to determine if they've all finished their turns
    private List<Player> players;
    //list of all enemy units, used to issue them commands
    //NOTE: probably make another abstract class for enemies?
    private List<TestEnemy> enemies;
    private bool enemiesMoving = false; //boolean to check if enemies are moving
    private bool gameLoading = true;

    public float turnDelay = 0.1f; //for setting a turn delay?

    void Awake()
    {
        //check if instance already exists; if not then set instance to this
        if (instance == null)
            instance = this;
        //else if instance is not this, destroy this game object
        else if (instance != this)
            Destroy(gameObject);

        //sets this to not be destroyed on reload
        DontDestroyOnLoad(gameObject);

        //create new lists for players and enemies
        //each individual player and enemy object should, upon creation, add themselves to these lists
        players = new List<Player>();
        enemies = new List<TestEnemy>();

        playersTurn = true;
    }

    public void Load_Finished()
    {
        gameLoading = false;
        Debug.Log("Player turn start");
    }

    void Update()
    {
        CheckIfPlayersMoved();

        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddPlayerToList(Player player)
    {
        players.Add(player);
    }

    public void AddEnemyToList(TestEnemy enemy)
    {
        enemies.Add(enemy);
    }

    public void RemovePlayerFromList(Player player)
    {
        players.Remove(player);
    }

    public void RemoveEnemyFromList(TestEnemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void ClearEnemyList()
    {
        enemies.Clear();
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        Debug.Log("Enemy turn");

        //possibly set turn delay?
        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].take_turn(); //or whatever the action for enemies are
            //turn delay per enemy movement?
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        Debug.Log("Enemy turn end");

        for (int i = 0; i < players.Count; i++)
        {
            players[i].take_turn(); //allow players to act again
        }

        Debug.Log("Player turn start");

        playersTurn = true; //set player turn to true

        enemiesMoving = false;
    }

    void CheckIfPlayersMoved()
    {
        if (!playersTurn || gameLoading)
        {
            return;
        }

        for (int i = 0; i < players.Count; i++) {
            if (players[i].player_turn)
              return;
        }

        Debug.Log("Player turn over");

        playersTurn = false;
    }
}
