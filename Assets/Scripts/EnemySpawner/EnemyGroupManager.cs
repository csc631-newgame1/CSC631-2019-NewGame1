using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupManager
{
    [Header("Enemy Group Settings")]
    public int maxNumberOfEnemyGroups;
    public int minNumberOfEnemyGroups;

    private List<EnemyGroup> enemyGroups;
    private List<SpawnZone> spawnZones;
    private List<GameAgent> enemies;

    public EnemyGroupManager(List<EnemyGroup> enemyGroups, List<SpawnZone> spawnZones) {
        this.enemyGroups = enemyGroups;
        this.spawnZones = spawnZones;
        enemies = new List<GameAgent>();

        // Sort based on group size
        QuickSortEnemyGroups(enemyGroups, 0, enemyGroups.Count - 1);
    }

    private void PopulateSpawnZone(EnemyGroup group, SpawnZone spawnZone) {
        List<GameAgentStats> enemyStats = group.GetEnemiesStatsForSpawn();
        List<Vector3> zoneTiles = spawnZone.GetUnpopulatedZoneTiles();
        // Enter distribution of enemies here

        // Random distribution in zone
        List<int> exclusion = new List<int>();

        foreach(GameAgentStats stats in enemyStats) {
            int randomIndex = GetRandomIntWithExclusion(0, zoneTiles.Count - 1, exclusion);
            TestEnemy enemy = new TestEnemy();
            MapUtils.Pos enemyPos = new MapUtils.Pos((int)zoneTiles[randomIndex].x, (int)zoneTiles[randomIndex].y);
            enemy.init_agent(enemyPos, stats);
            enemies.Add(enemy);
        }

    }

    public List<GameAgent> GetEnemiesToSpawn() {
        foreach (EnemyGroup group in enemyGroups) {
            if (spawnZones.Count <= 0) {
                break;
            }

            List<int> exclusion = new List<int>();

            while (exclusion.Count != spawnZones.Count) {
                int randomIndex = GetRandomIntWithExclusion(0, spawnZones.Count - 1, exclusion);

                if(group.count <= spawnZones[randomIndex].GetNumberOfUnpopulatedTilesInZone()) {
                    PopulateSpawnZone(group, spawnZones[randomIndex]);
                    spawnZones.RemoveAt(randomIndex);
                    break;
                } else {
                    exclusion.Add(randomIndex);
                }
            }
        }
        return enemies;
    }

    private int GetRandomIntWithExclusion(int start, int end, List<int> exclude) {
        int random = start + Random.Range(0, (end - start + 1 - exclude.Count));
        foreach (int ex in exclude) {
            if (random < ex) {
                break;
            }
            random++;
        }
        return random;
    }

    // Sorts the EnemyGroups in descending order based on the size of the group
    private void QuickSortEnemyGroups(List<EnemyGroup> enemyGroups, int left, int right) {
        if (left < right) {
            int pivot = Partition(enemyGroups, left, right);

            if (pivot > 1) {
                QuickSortEnemyGroups(enemyGroups, left, pivot - 1);
            }
            if (pivot + 1 < right) {
                QuickSortEnemyGroups(enemyGroups, pivot + 1, right);
            }
        }
    }

    private int Partition(List<EnemyGroup> enemyGroups, int left, int right) {
        EnemyGroup pivot = enemyGroups[left];
        while (true) {
            while (enemyGroups[left].count > pivot.count) {
                left++;
            }

            while (enemyGroups[right].count < pivot.count) {
                right--;
            }

            if (left < right) {
                EnemyGroup temp = enemyGroups[left];
                enemyGroups[left] = enemyGroups[right];
                enemyGroups[right] = temp;

                if (enemyGroups[left].count == enemyGroups[right].count) {
                    left++;
                }
            } else {
                return right;
            }
        }
    }
}
