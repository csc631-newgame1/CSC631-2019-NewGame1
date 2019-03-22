// Holds information regarding the GameAgents ingame stats
using System;
using UnityEngine;

public class GameAgentStats
{
    public int characterClassOption;
    public CharacterClass playerCharacterClass;
    // unit attack damage
    public float attack;
    // unit maxium health
    public float maxHealth;
    // unit current health
    public float currentHealth;
    // unit attack radius
    public float range;
    // unit move radius
    public float speed;
    // game agent level
    public int level = 1;
    // experience points
    public int xp;

    // This is used to determine how much xp is awarded for death
    // the scale works by awarding the scaleFactor amount of xp for that level
    // ex. if it takes 1000xp to reach level, and the player kills a level 5 monster
    // the player will receive 1000xp * scaleFactor worth of xp
    private float scaleFactor = 0.25f;

    public GameAgentStats(int characterClass, float attack, float health, float range, float speed, int desiredLevel) {
        characterClassOption = characterClass;
        SetGameAgentCharacterClass();
        this.attack = attack;
        maxHealth = health;
        currentHealth = health;
        this.range = range;
        this.speed = speed;

        LevelUpToDesiredLevel(desiredLevel);
    }

    // Only used for creating base stats
    public GameAgentStats(float attack, float health, float range, float speed) {
        this.attack = attack;
        maxHealth = health;
        currentHealth = health;
        this.range = range;
        this.speed = speed;
    }

    public GameAgentStats(int characterClass, int desiredLevel) {
        characterClassOption = characterClass;
        SetGameAgentCharacterClass();
        GetBaseCharacterClassStats();
        LevelUpToDesiredLevel(desiredLevel);
    }

    private void GetBaseCharacterClassStats() {
        attack = playerCharacterClass.baseStats.attack;
        maxHealth = playerCharacterClass.baseStats.maxHealth;
        currentHealth = maxHealth;
        range = playerCharacterClass.baseStats.range;
        speed = playerCharacterClass.baseStats.speed;
    }

    public void LevelUp() {
        level++;
        attack += playerCharacterClass.GetAttackStatIncreaseFromLevelUp();
        range += playerCharacterClass.GetRangeStatIncreaseFromLevelUp();
        speed += playerCharacterClass.GetSpeedStatIncreaseFromLevelUp(level);
        int healthIncrease = playerCharacterClass.GetHealthStatIncreaseFromLevelUp();

        maxHealth += healthIncrease;
        if (currentHealth > 0) {
            currentHealth += healthIncrease;
        }
    }

    public void LevelUpToDesiredLevel(int desiredLevel) {
        while (level < desiredLevel) {
            LevelUp();
        }
    }

    public void GainXP(int xpGained) {
        xp += xpGained;
        CheckLevelProgression();
    }

    private void CheckLevelProgression() {
        // This formula is used for a linearly rising level gap
        float progressionTowardsLevel = (Mathf.Sqrt(100f * (2 * xp + 25f))+50f)/ 100f;

        // Level Up multiple times if needed
        if (progressionTowardsLevel >= level + 1) {
            while(progressionTowardsLevel > 1) {
                LevelUp();
                progressionTowardsLevel--;
            }
        }
    }
    
    public int RewardXPFromDeath() {
        return Mathf.RoundToInt((level * level + level) / 2 * 100 - (level * 100) * scaleFactor);
    }

    private void SetGameAgentCharacterClass() {
        switch (characterClassOption) {
            case CharacterClassOptions.Knight:
                playerCharacterClass = new Knight();
                break;
            case CharacterClassOptions.Hunter:
                playerCharacterClass = new Hunter();
                break;
            case CharacterClassOptions.Mage:
                playerCharacterClass = new Mage();
                break;
            case CharacterClassOptions.Healer:
                playerCharacterClass = new Healer();
                break;
            default:
                playerCharacterClass = new Knight();
                break;
        }
    }
}
