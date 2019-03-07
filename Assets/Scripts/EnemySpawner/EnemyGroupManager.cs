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

        QuickSortEnemyGroups(enemyGroups, 0, enemyGroups.Count - 1);
        foreach (EnemyGroup group in enemyGroups) {
            Debug.Log(group.count);
        }
    }

    private void PopulateSpawnZone() {

    }

    public List<GameAgent> GetEnemiesToSpawn() {
        List<int> exclusion = new List<int>();
        
        foreach (EnemyGroup group in enemyGroups) {

            while (exclusion.Count != spawnZones.Count) {
                int randomIndex = GetRandomIntWithExclusion(0, spawnZones.Count - 1, exclusion);
                //if()
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
