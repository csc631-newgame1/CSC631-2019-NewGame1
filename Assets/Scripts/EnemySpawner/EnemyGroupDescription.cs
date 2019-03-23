using UnityEngine;

public class EnemyGroupDescription
{
    public GameAgentStats stats;
    public float attackVariance;
    public float healthVariance;
    public float rangeVariance;
    public float speedVariance;
    public int levelVariance;

    public int quantityOfEnemyInGroup;

    public bool randomNumberOfEnemies;
    public int minNumberOfEnemiesInGroup;
    public int maxNumberOfEnemiesInGroup;

    public float powerLevel = 0;

    // Variance is based on a percentage from 0 to 1 (1 = 100%)
    // The stat can be potentially rasied to any percetage, but cannot fall below 50% the original stat
    public EnemyGroupDescription(GameAgentStats stats, int quantityOfEnemyInGroup, 
                                float attackVariance = 0f,  float healthVariance = 0f,
                                float rangeVariance = 0f, float speedVariance = 0f,
                                bool randomNumberOfEnemies = false, 
                                int minNumberOfEnemiesInGroup = -1, int maxNumberOfEnemiesInGroup = -1, int levelVariance = 0) {
        this.stats = stats;
        this.attackVariance = attackVariance;
        this.healthVariance = healthVariance;
        this.rangeVariance = rangeVariance;
        this.speedVariance = speedVariance;
        this.quantityOfEnemyInGroup = quantityOfEnemyInGroup;
        this.randomNumberOfEnemies = randomNumberOfEnemies;
        this.minNumberOfEnemiesInGroup = minNumberOfEnemiesInGroup;
        this.maxNumberOfEnemiesInGroup = maxNumberOfEnemiesInGroup;
        this.levelVariance = levelVariance;

        CalculatePowerLevel();
    }

    public float GetAttackWithVariance() {
        return Mathf.RoundToInt(Mathf.Max(stats.attack / 2, (stats.attack + (Random.Range(-attackVariance, attackVariance) * stats.attack))));
    }

    public float GetHealthWithVariance() {
        return Mathf.RoundToInt(Mathf.Max(stats.maxHealth / 2, (stats.maxHealth + (Random.Range(-healthVariance, healthVariance) * stats.maxHealth))));
    }

    public float GetRangeWithVariance() {
        return Mathf.RoundToInt(Mathf.Max(stats.range / 2, (stats.range + (Random.Range(-rangeVariance, rangeVariance) * stats.range))));
    }

    public float GetSpeedWithVariance() {
        return Mathf.RoundToInt(Mathf.Max(stats.speed / 2, (stats.speed + (Random.Range(-speedVariance, speedVariance) * stats.speed))));
    }

    public int GetLevelWithVariance() {
        return Mathf.RoundToInt(Mathf.Max(stats.level / 2, (stats.level + Random.Range(-levelVariance, levelVariance+1))));
    }

    public float GetPowerLevel() {
        return powerLevel;
    }

    // Consider removing this - or implementing it
    private void CalculatePowerLevel() {
        powerLevel += stats.attack * stats.attack;
        powerLevel += stats.maxHealth * stats.maxHealth;
        powerLevel *= Mathf.Sqrt(stats.range);
        powerLevel *= Mathf.Sqrt(stats.speed);
        powerLevel = Mathf.RoundToInt(powerLevel);
    }
}
