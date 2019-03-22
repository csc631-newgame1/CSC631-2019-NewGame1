using MapUtils;
using System.Collections.Generic;
using UnityEngine;

// Responsible for taking a List of enemy groups and spawn zones
// and figuring out where to spawn the enemies within the spawn zones
public class EnemyGroupManager
{
    [Header("Enemy Group Settings")]
    public int maxNumberOfEnemyGroups;
    public int minNumberOfEnemyGroups;

    private List<EnemyGroup> enemyGroups;
    private List<SpawnZone> spawnZones;
    private List<EnemyToSpawn> enemies;

    public EnemyGroupManager(List<EnemyGroup> enemyGroups, List<SpawnZone> spawnZones) {
        this.enemyGroups = enemyGroups;
        this.spawnZones = spawnZones;
        enemies = new List<EnemyToSpawn>();

        // Sort based on group size
        QuickSortEnemyGroups(enemyGroups, 0, enemyGroups.Count - 1);
    }

    // Takes the group of enemies and creates EnemyToSpawn objects with the location
    // within the spawn zone of where to spawn them
    private void PopulateSpawnZone(EnemyGroup group, SpawnZone spawnZone) {
        List<GameAgentStats> enemyStats = group.GetEnemiesStatsForSpawn();
        List<Pos> zoneTiles = spawnZone.GetUnpopulatedZoneTiles();
        List<Pos> populatedZoneTiles = new List<Pos>();
        // Enter distribution of enemies here

        // Random distribution in zone
        List<int> exclusion = new List<int>();

        foreach(GameAgentStats stats in enemyStats) {
            int randomIndex = GetRandomIntWithExclusion(0, zoneTiles.Count - 1, exclusion);
            Pos enemyPos = new Pos((int)zoneTiles[randomIndex].x, (int)zoneTiles[randomIndex].y);
            populatedZoneTiles.Add(zoneTiles[randomIndex]);
            EnemyToSpawn enemy = new EnemyToSpawn(enemyPos, stats);

            exclusion.Add(randomIndex);
            enemies.Add(enemy);
        }
        spawnZone.PopulateTiles(populatedZoneTiles);
    }

    // Call this to get a list of EnemiesToSpawn ready to be initialized
    public List<EnemyToSpawn> GetEnemiesToSpawn() {
        foreach (EnemyGroup group in enemyGroups) {
            if (spawnZones.Count <= 0) {
                break;
            }

            List<int> exclusion = new List<int>();

            while (exclusion.Count != spawnZones.Count) {
                int randomIndex = GetRandomIntWithExclusion(0, spawnZones.Count - 1, exclusion);

                if(group.count <= spawnZones[randomIndex].GetNumberOfUnpopulatedTilesInZone()) {
                    PopulateSpawnZone(group, spawnZones[randomIndex]);
                    break;
                }

                exclusion.Add(randomIndex);
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
