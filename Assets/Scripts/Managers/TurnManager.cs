using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //static instance of game manager which allows it to be accessed by any other script
    public static TurnManager instance = null;
	
    // lists of all active players/enemies
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
		
		AIManager.roster = teamRoster;
		
		mainLoop = TurnLoop();
		StartCoroutine(mainLoop);
	}
	
    IEnumerator TurnLoop()
	{	
		while (true) {
			
			yield return null;
			for (int team = 0; team < 16; team++) {
				
				StartCoroutine(AIManager.update(team));
				while (!AIManager.turnOver(team)) {
					yield return null;
				}
				ClearDead();
			}
		}
    }
	
	void ClearDead()
	{
		foreach (List<GameAgent> faction in teamRoster) {
			foreach (GameAgent agent in faction.ToArray()) {
				if (agent.stats.currentState == GameAgentState.Dead) {
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

    public void Terminate()
	{
		foreach (List<GameAgent> faction in teamRoster)
			faction.Clear();
		StopAllCoroutines(); // terminate any running coroutines
	}
}
