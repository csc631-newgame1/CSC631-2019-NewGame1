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

public enum EnemyGroupDifficulty { Trivial, Average, Difficult, Impossible };

public static class EnemyGroupTemplate
{
    public static EnemyGroup GetEnemyGroup(EnemyGroupType groupType, EnemyGroupDifficulty difficulty, int race, int maxNumberOfEnemies, int level) {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        System.Random rng = config.GetRNG();
        List<EnemyGroupDescription> enemyGroupDescriptions = new List<EnemyGroupDescription>();
        int enemyLevel = level;

        switch (difficulty) {
            case EnemyGroupDifficulty.Trivial:
                if (level > 3) {
                    enemyLevel = rng.Next(level - 2, level + 1);
                } else if (level > 2) {
                    enemyLevel = rng.Next(level - 1, level + 1);
                }
                break;
            case EnemyGroupDifficulty.Average:
                if (level > 2) {
                    enemyLevel = rng.Next(level - 1, level + 1 + 1);
                } else {
                    enemyLevel = rng.Next(level, level + 1 + 1);
                }
                break;
            case EnemyGroupDifficulty.Difficult:
                enemyLevel = rng.Next(level + 1, level + 2 + 1);
                break;
            case EnemyGroupDifficulty.Impossible:
                enemyLevel = rng.Next(level + 2, level + 3 + 1);
                break;
        }

        switch (groupType) {
            case EnemyGroupType.Melee:
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(race, CharacterClassOptions.Knight, rng.Next(, 2 + 1)),
                                                                    rng.Next(1, 2 + 1)));
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
}
