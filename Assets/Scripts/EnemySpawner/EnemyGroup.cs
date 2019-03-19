using System.Collections.Generic;
using UnityEngine;

// Used to determine how the enemies should be created in the group
// based on power level
public enum Distribution { Balanaced, Strong, Weak, Random };

// Group of enemies to be spawned in a spawn zone
public class EnemyGroup
{
    [Header("Random Number of Enemies in Groups")]
    [Tooltip("If true, select minumum value and maximum value for number of enemies to be spawned in the group.")]
    public bool randomRangeNumberOfEnemies;
    public int minNumberOfEnemies;
    public int maxNumberOfEnemies;

    [Header("Customize Group based on Power")]
    public bool powerBalance = false;

    private List<EnemyGroupDescription> typesOfEnemies;
    private List<GameAgentStats> enemies;
    private Distribution dist;

    public int count;

    public EnemyGroup(List<EnemyGroupDescription> typesOfEnemies, Distribution dist, bool randomRangeNumberOfEnemies = false,
                        int minNumberOfEnemies = -1, int maxNumberOfEnemies = -1, bool powerBalance = false) {

        this.typesOfEnemies = typesOfEnemies;
        this.dist = dist;
        this.randomRangeNumberOfEnemies = randomRangeNumberOfEnemies;
        this.minNumberOfEnemies = minNumberOfEnemies;
        this.maxNumberOfEnemies = maxNumberOfEnemies;
        this.powerBalance = powerBalance;

        enemies = new List<GameAgentStats>();

        CreateEnemyStatsInGroup();
    }

    // Creates a list of enemy stats that will be used for placing in spawn zones
    void CreateEnemyStatsInGroup() {
        foreach (EnemyGroupDescription enemy in typesOfEnemies) {
            for (int i=0; i<enemy.quantityOfEnemyInGroup; i++) {
                GameAgentStats stats = new GameAgentStats(enemy.stats.characterClass, enemy.GetAttackWithVariance(),
                                                            enemy.GetHealthWithVariance(), enemy.GetRangeWithVariance(),
                                                            enemy.GetSpeedWithVariance());
                enemies.Add(stats);
            }
        }

        count = enemies.Count;
    }

    public List<GameAgentStats> GetEnemiesStatsForSpawn() {
        return enemies;
    }
}
