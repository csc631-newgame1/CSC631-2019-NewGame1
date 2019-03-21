// Holds information regarding the GameAgents ingame stats
using UnityEngine;

public class GameAgentStats
{
    public int characterClass;
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
    // player level
    public float level;
    // experience points
    public int xp;

    // This is used to determine how much xp is awarded for death
    // the scale works by awarding the scaleFactor amount of xp for that level
    // ex. if it takes 1000xp to reach level, and the player kills a level 5 monster
    // the player will receive 1000xp * scaleFactor worth of xp
    private float scaleFactor = 0.25f;

    public GameAgentStats(int characterClass, float attack, float health, float range, float speed) {
        this.characterClass = characterClass;
        this.attack = attack;
        this.maxHealth = health;
        this.currentHealth = health;
        this.range = range;
        this.speed = speed;
    }

    public GameAgentStats(int characterClass) {
        this.characterClass = characterClass;
        this.attack = 20;
        this.maxHealth = 50;
        this.currentHealth = maxHealth;
        this.range = 1;
        this.speed = 7;
    }

    public void LevelUp() {
        switch (characterClass) {

        }
    }

    public void GainXP(int xpGained) {
        xp += xpGained;
        CheckLevelProgression();
    }

    private void CheckLevelProgression() {
        // This formula is used for a linearly rising level gap
        float progressionTowardsLevel = (Mathf.Sqrt(100f * (2 * xp + 25f))+50f)/ 100f;

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
}
