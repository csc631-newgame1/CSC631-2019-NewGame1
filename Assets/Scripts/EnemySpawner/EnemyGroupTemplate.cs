using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Melee - group of powerful knight attackers
// BalancedAllUnits - group of knights, hunters, mages, and healers
// BalancedSingleRange - group of knights, (hunters or mages), and healers
// MeleeAndSingleRange - group of knights and (hunters or mages)
// MeleeAndRange - group of knights, hunters, and mages
// MeleeAndHealers - group of knights and healers
// RangeAndHealers - group of (hunters or mages) and healers
// LessKnightsAndMoreMages - group of few knights and more mages
// MoreKnightsAndLessMages - group of more knights and few mages
// LessKnightsAndMoreHunters - group of few knights and more hunters
// MoreKnightsAndLessHunters - group of more knights and few hunters
// MixedRange - group of hunters and mages
// MixedRangeAndHealers - group of hunters, mages, and healers
public enum EnemyGroupType { Melee, BalancedAllUnits, BalancedSingleRange, MeleeAndSingleRange,
                             MeleeAndRange, MeleeAndHealers, RangeAndHealers, LessKnightsAndMoreMages,
                             MoreKnightsAndLessMages, LessKnightsAndMoreHunters, MoreKnightsAndLessHunters,
                             MixedRange, MixedRangeAndHealers };

// Determines the level of the enemies
public enum EnemyGroupDifficulty { Trivial, Average, Difficult, Impossible };

public static class EnemyGroupSize {
    public const int Small = 3;
    public const int MediumLowerBound = 4;
    public const int Medium = 5;
    public const int MediumUpperBound = 6;
    public const int Large = 7;
    public const int LargeUpperBound = 8;
    public const int XLarge = 9;
};

public static class EnemyGroupTemplate
{
    public static EnemyGroup GetEnemyGroup(EnemyGroupType groupType, EnemyGroupDifficulty difficulty, int enemyGroupSize, int race, int level) {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        System.Random rng = config.GetRNG();
        List<EnemyGroupDescription> enemyGroupDescriptions = new List<EnemyGroupDescription>();

        switch (groupType) {
            case EnemyGroupType.Melee:
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, GetLevel(difficulty, rng, level)), rng.Next(1, 2 + 1)));
                break;
            case EnemyGroupType.BalancedAllUnits:
                break;
            case EnemyGroupType.BalancedSingleRange:
                break;
            case EnemyGroupType.MeleeAndSingleRange:
                break;
            case EnemyGroupType.MeleeAndRange:
                break;
            case EnemyGroupType.MeleeAndHealers:
                break;
            case EnemyGroupType.RangeAndHealers:
                break;
            case EnemyGroupType.LessKnightsAndMoreMages:
                break;
            case EnemyGroupType.MoreKnightsAndLessMages:
                break;
            case EnemyGroupType.LessKnightsAndMoreHunters:
                break;
            case EnemyGroupType.MoreKnightsAndLessHunters:
                break;
            case EnemyGroupType.MixedRange:
                break;
            case EnemyGroupType.MixedRangeAndHealers:
                break;
        }

        return new EnemyGroup();
    }

    private static List<int> GetEnemyQuantities(EnemyGroupType groupType, System.Random rng, int enemyGroupSize) {
        List<int> enemyQuantities = new List<int>();
        int unitsRemaining = enemyGroupSize;
        List<int> exclusion = new List<int>();

        switch (groupType) {
            case EnemyGroupType.Melee:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        enemyQuantities.Add(EnemyGroupSize.Small);
                        break;
                    case EnemyGroupSize.Medium:
                        enemyQuantities.Add(rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1));
                        break;
                    case EnemyGroupSize.Large:
                        enemyQuantities.Add(rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1));
                        break;
                    case EnemyGroupSize.XLarge:
                        enemyQuantities.Add(rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1));
                        break;
                }
                break;
            case EnemyGroupType.BalancedAllUnits:
                switch (enemyGroupSize) {
                    case EnemyGroupSize.Small:
                        // Knight, Hunter or Mage, then Healer
                        while (unitsRemaining > 0) {
                            enemyQuantities.Add(1);
                            unitsRemaining--;
                        }
                        break;
                    case EnemyGroupSize.Medium:
                        // Knight, Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumLowerBound, EnemyGroupSize.MediumUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;

                            // Less healers
                            if (randomIndex == 3 || rng.Next(0, 1 + 1) == 1) {
                                exclusion.Add(randomIndex);
                            }

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        break;
                    case EnemyGroupSize.Large:
                        // Knight, Hunter, Mage, then Healer
                        enemyQuantities = new List<int> { 1, 1, 1, 1 };
                        unitsRemaining = rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1);
                        unitsRemaining -= enemyQuantities.Count;

                        // Randomly increase enemy units
                        while (unitsRemaining > 0) {
                            int randomIndex = Utility.GetRandomIntWithExclusion(0, enemyQuantities.Count - 1, rng, exclusion);
                            enemyQuantities[randomIndex]++;
                            exclusion.Add(randomIndex);

                            if (exclusion.Count >= enemyQuantities.Count) {
                                exclusion.Clear();
                            }
                        }
                        enemyQuantities.Add(rng.Next(EnemyGroupSize.MediumUpperBound, EnemyGroupSize.LargeUpperBound + 1));
                        break;
                    case EnemyGroupSize.XLarge:
                        enemyQuantities.Add(rng.Next(EnemyGroupSize.LargeUpperBound, EnemyGroupSize.XLarge + 1));
                        break;
                }
                break;
            case EnemyGroupType.BalancedSingleRange:
                break;
            case EnemyGroupType.MeleeAndSingleRange:
                break;
            case EnemyGroupType.MeleeAndRange:
                break;
            case EnemyGroupType.MeleeAndHealers:
                break;
            case EnemyGroupType.RangeAndHealers:
                break;
            case EnemyGroupType.LessKnightsAndMoreMages:
                break;
            case EnemyGroupType.MoreKnightsAndLessMages:
                break;
            case EnemyGroupType.LessKnightsAndMoreHunters:
                break;
            case EnemyGroupType.MoreKnightsAndLessHunters:
                break;
            case EnemyGroupType.MixedRange:
                break;
            case EnemyGroupType.MixedRangeAndHealers:
                break;
        }
        return enemyQuantities;
    }

    private static int GetLevel(EnemyGroupDifficulty difficulty, System.Random rng, int level) {
        switch (difficulty) {
            case EnemyGroupDifficulty.Trivial:
                if (level > 3) {
                    return rng.Next(level - 2, level + 1);
                } else if (level > 2) {
                    return rng.Next(level - 1, level + 1);
                }
                break;
            case EnemyGroupDifficulty.Average:
                if (level > 2) {
                    return rng.Next(level - 1, level + 1 + 1);
                } else {
                    return rng.Next(level, level + 1 + 1);
                }
            case EnemyGroupDifficulty.Difficult:
                return rng.Next(level + 1, level + 2 + 1);
            case EnemyGroupDifficulty.Impossible:
                return rng.Next(level + 2, level + 3 + 1);
        }
        return level;
    }
}
