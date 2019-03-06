using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Distribution { Balanaced, Strong, Weak, Random };

public class EnemyGroup : MonoBehaviour
{
    [Header("Random Number of Enemies in Groups")]
    [Tooltip("If true, select minumum value and maximum value for number of enemies to be spawned in the group.")]
    public bool randomRangeNumberOfEnemies;
    public int minNumberOfEnemies;
    public int maxNumberOfEnemies;

    [Header("Customize Group based on Power")]
    public bool powerBalance = false;

    private List<EnemySpawnProperty> typesOfEnemies;
    private List<GameAgent> enemies;
    private Distribution dist;

    public EnemyGroup(List<EnemySpawnProperty> typesOfEnemies, Distribution dist, bool randomRangeNumberOfEnemies = false,
                        int minNumberOfEnemies = -1, int maxNumberOfEnemies = -1, bool powerBalance = false) {

        this.typesOfEnemies = typesOfEnemies;
        this.dist = dist;
        this.randomRangeNumberOfEnemies = randomRangeNumberOfEnemies;
        this.minNumberOfEnemies = minNumberOfEnemies;
        this.maxNumberOfEnemies = maxNumberOfEnemies;
        this.powerBalance = powerBalance;

        CreateEnemiesInGroup();
    }

    void CreateEnemiesInGroup() {
        foreach (EnemySpawnProperty enemy in typesOfEnemies) {
            //numberOfEnemies += enemy.quantityOfEnemyInGroup;
        }
    }
}
