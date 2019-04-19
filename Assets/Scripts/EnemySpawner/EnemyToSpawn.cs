using MapUtils;
using UnityEngine;

public class EnemyToSpawn
{
    public Pos gridPosition;
    public GameAgentController stats;
    GameObject enemy;

    public EnemyToSpawn(Pos gridPosition, GameAgentController stats) {
        this.gridPosition = gridPosition;
        this.stats = stats;
    }
}
