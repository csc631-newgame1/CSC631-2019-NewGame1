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
    private List<GameAgent>[] teamRoster = new List<GameAgent>[16];
	
	private GameManager parentManager = null;
	private IEnumerator mainLoop = null;
	
	void Awake()
	{
		instance = this;
		for (int i = 0; i < 16; i++) 
			teamRoster[i] = new List<GameAgent>();
	}
	
	public void Init(GameManager parent)
	{
		parentManager = parent;
		if (mainLoop != null) StopCoroutine(mainLoop);
		
		AIManager.roster = teamRoster;
		
		StartCoroutine(TurnLoop());
	}

    IEnumerator TurnLoop()
	{	
		while (true) {
			
			yield return null;
			for (int team = 0; team < 16; team++) {
				
				AIManager.update(team);
				while (!AIManager.turnOver(team)) yield return null;
				ClearDead();
			}
		}
    }
	
	void ClearDead()
	{
		foreach (List<GameAgent> faction in teamRoster) {
			foreach (GameAgent agent in faction.ToArray()) {
				if (agent.stats.currentHealth <= 0) {
					faction.Remove(agent);
					parentManager.kill(agent);
				}
			}
		}
	}
	
	public void addToRoster(GameAgent agent)
	{
		teamRoster[agent.team].Add(agent);
	}

    public void clearRoster()
	{
		foreach (List<GameAgent> faction in teamRoster)
			faction.Clear();
	}
}
