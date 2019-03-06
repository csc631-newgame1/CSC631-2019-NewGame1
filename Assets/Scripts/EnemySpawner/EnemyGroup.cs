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

    private List<EnemySpawnProperty> enemies;
    private int numberOfEnemies = 0;
    private Distribution dist;

    public EnemyGroup(List<EnemySpawnProperty> enemies, Distribution dist, bool randomRangeNumberOfEnemies = false,
                        int minNumberOfEnemies = -1, int maxNumberOfEnemies = -1, bool powerBalance = false) {

        this.enemies = enemies;
        this.dist = dist;
        this.randomRangeNumberOfEnemies = randomRangeNumberOfEnemies;
        this.minNumberOfEnemies = minNumberOfEnemies;
        this.maxNumberOfEnemies = maxNumberOfEnemies;
        this.powerBalance = powerBalance;

        CalculateNumberOfEnemies();
    }

    void CalculateNumberOfEnemies() {
        foreach (EnemySpawnProperty enemy in enemies) {
            numberOfEnemies += enemy.quantityOfEnemyInGroup;
        }
    }
}
