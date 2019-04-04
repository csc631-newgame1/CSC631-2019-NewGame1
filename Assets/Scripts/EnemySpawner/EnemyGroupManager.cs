using MapUtils;
using RegionUtils;
using System;
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
    private MapGenerator mapGenerator;
    private System.Random rng;

    public EnemyGroupManager(List<EnemyGroup> enemyGroups, List<SpawnZone> spawnZones) {
        this.enemyGroups = enemyGroups;
        this.spawnZones = spawnZones;
        enemies = new List<EnemyToSpawn>();

        mapGenerator = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        rng = config.GetRNG();

        // Sort based on group size
        QuickSortEnemyGroups(enemyGroups, 0, enemyGroups.Count - 1);
        // Sort based on spawn zone size
        QuickSortSpawnZones(spawnZones, 0, spawnZones.Count - 1);
        
        foreach (SpawnZone spawnZone in spawnZones) {
            Debug.Log("region: " + spawnZone.region + " region size: " + mapGenerator.GetRegionSize(spawnZone.region));
        }
    }

    // Takes the group of enemies and creates EnemyToSpawn objects with the location
    // within the spawn zone of where to spawn them
    private void PopulateSpawnZone(EnemyGroup group, SpawnZone spawnZone) {
        List<GameAgentStats> enemyStats = group.GetEnemiesStatsForSpawn();
        List<Pos> zoneTiles = spawnZone.GetUnpopulatedZoneTiles();
        List<Pos> populatedZoneTiles = new List<Pos>();

        // Random distribution in zone
        List<int> exclusion = new List<int>();

        foreach(GameAgentStats stats in enemyStats) {
            int randomIndex = Utility.GetRandomIntWithExclusion(0, zoneTiles.Count - 1, rng, exclusion);
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
                int randomIndex = Utility.GetRandomIntWithExclusion(0, spawnZones.Count - 1, rng, exclusion);

                if(group.count <= spawnZones[randomIndex].GetNumberOfUnpopulatedTilesInZone()) {
                    PopulateSpawnZone(group, spawnZones[randomIndex]);
                    break;
                }

                exclusion.Add(randomIndex);
            }
        }
        return enemies;
    }

    private int[] DistributeGroupsIntoRegions() {
        int numOfRegions = mapGenerator.getRegions().Count;
        int[] countOfGroupsInRegions = new int[Enum.GetNames(typeof(EnemyGroupDifficulty)).Length];
        int mean = (0 + numOfRegions) / 2;

        // Guassian distribution to place enemies
        for (int i = 0; i < numOfRegions; i++) {
            // Put one group of enemies in spawn zones
            if (countOfGroupsInRegions[i % countOfGroupsInRegions.Length - 1] == 0) {
                countOfGroupsInRegions[i % countOfGroupsInRegions.Length - 1]++;
            } else {
                //
                countOfGroupsInRegions[Utility.NextGaussian(mean, 1, 0, numOfRegions, rng)]++;
            }
        }
        return countOfGroupsInRegions;
    }

    // Sorts the EnemyGroups in descending order based on the size of the group
    private void QuickSortEnemyGroups(List<EnemyGroup> enemyGroups, int left, int right) {
        if (left < right) {
            int pivot = PartitionEnemyGroups(enemyGroups, left, right);

            if (pivot > 1) {
                QuickSortEnemyGroups(enemyGroups, left, pivot - 1);
            }
            if (pivot + 1 < right) {
                QuickSortEnemyGroups(enemyGroups, pivot + 1, right);
            }
        }
    }

    private int PartitionEnemyGroups(List<EnemyGroup> enemyGroups, int left, int right) {
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

    // Sorts the Spawn Zones in descending order based on the size of the region size
    private void QuickSortSpawnZones(List<SpawnZone> spawnZones, int left, int right) {
        if (left < right) {
            int pivot = PartitionSpawnZones(spawnZones, left, right);

            if (pivot > 1) {
                QuickSortSpawnZones(spawnZones, left, pivot - 1);
            }
            if (pivot + 1 < right) {
                QuickSortSpawnZones(spawnZones, pivot + 1, right);
            }
        }
    }

    private int PartitionSpawnZones(List<SpawnZone> spawnZones, int left, int right) {
        SpawnZone pivot = spawnZones[left];
        while (true) {
            while (mapGenerator.GetRegionSize(spawnZones[left].region) > mapGenerator.GetRegionSize(pivot.region)) {
                left++;
            }

            while (mapGenerator.GetRegionSize(spawnZones[right].region) < mapGenerator.GetRegionSize(pivot.region)) {
                right--;
            }

            if (left < right) {
                SpawnZone temp = spawnZones[left];
                spawnZones[left] = spawnZones[right];
                spawnZones[right] = temp;

                if (mapGenerator.GetRegionSize(spawnZones[left].region) == mapGenerator.GetRegionSize(spawnZones[right].region)) {
                    left++;
                }
            } else {
                return right;
            }
        }
    }
}
