using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public enum GameAgentAction { Move, Wait, Potion, MeleeAttack, Taunt, RangedAttack, RangedAttackMultiShot, MagicAttackSingleTarget, MagicAttackAOE, Heal, Neutral };

    public enum GameAgentState { Alive, Unconscious, Dead, Null };

    public enum GameAgentStatusEffect { None, Taunted, Taunting };

    public class ActMenuButtons {
        // Button indexes
        public const int MOVE = 0;
        public const int ACT = 1;
        public const int POTION = 2;
        public const int WAIT = 3;
        public const int ACTION1 = 4;
        public const int ACTION2 = 5;
        public const int BLANK = 6;
        public const int BACK = 7;
    }

    public static class CharacterClassOptions {
        public const int Knight = 1;
        public const int Hunter = 2;
        public const int Mage = 3;
        public const int Healer = 6;

        public const int Sword = 1;
        public const int Bow = 4;
        public const int Staff = 6;
        public const int Axe = 3;
        public const int Club = 9;
        public const int Unarmed = 0;
        public const int RandomClassWeapon = -1;

        public static string getWeaponDescriptor(int weapon) {
            switch (weapon) {
                case Sword: return "Swordsman";
                case Bow: return "Archer";
                case Staff: return "Mage";
                case Axe: return "Destroyer";
                case Club: return "Barbarian";
                case Unarmed: return "Brawler";
                default: return "";
            }
        }
    };

    public static class CharacterRaceOptions {
        public const int Human = 1;
        public const int Orc = 4;
        public const int Skeleton = 5;
        public static string getString(int race) {
            switch (race) {
                case Human: return "Human";
                case Orc: return "Orc";
                case Skeleton: return "Skeleton";
                default: return "Cryptid";
            }
        }
    };

    // Melee - group of knights
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
    public static class EnemyGroupType {
        public const int Melee = 1;
        public const int BalancedAllUnits = 2;
        public const int BalancedSingleRange = 3;
        public const int MeleeAndSingleRange = 4;
        public const int MeleeAndRange = 5;
        public const int MeleeAndHealers = 6;
        public const int RangeAndHealers = 7;
        public const int LessKnightsAndMoreMages = 8;
        public const int MoreKnightsAndLessMages = 9;
        public const int LessKnightsAndMoreHunters = 10;
        public const int MoreKnightsAndLessHunters = 11;
        public const int MixedRange = 12;
        public const int MixedRangeAndHealers = 13;
    };

    // Determines the level of the enemies
    public static class EnemyGroupDifficulty {
        public const int Trivial = 0;
        public const int Average = 1;
        public const int Difficult = 2;
        public const int Impossible = 3;

        public const int count = 4;
    };

    public static class EnemyGroupSize {
        public const int Small = 3;
        public const int MediumLowerBound = 4;
        public const int Medium = 5;
        public const int MediumUpperBound = 6;
        public const int Large = 7;
        public const int LargeUpperBound = 8;
        public const int XLarge = 9;
    };
}
