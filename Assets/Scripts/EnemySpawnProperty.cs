using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnProperty : MonoBehaviour
{
    public GameAgent enemy;
    public float attack;
    public float attackVariance;
    public float health;
    public float healthVariance;
    public float range;
    public float rangeVariance;
    public float speed;
    public float speedVariance;

    public int quantityOfEnemyInGroup;

    public bool randomNumberOfEnemies;
    public int minNumberOfEnemiesInGroup;
    public int maxNumberOfEnemiesInGroup;

    public float powerLevel = 0;

    // Variance should be between 0 and 1
    public EnemySpawnProperty(GameAgent enemy, float attack, float health, float range, float speed, int quantityOfEnemyInGroup, 
                                float attackVariance = 0f,  float healthVariance = 0f,
                                float rangeVariance = 0f, float speedVariance = 0f,
                                bool randomNumberOfEnemies = false, 
                                int minNumberOfEnemiesInGroup = -1, int maxNumberOfEnemiesInGroup = -1) {
        this.attack = attack;
        this.attackVariance = attackVariance;
        this.health = health;
        this.healthVariance = healthVariance;
        this.range = range;
        this.rangeVariance = rangeVariance;
        this.speed = speed;
        this.speedVariance = speedVariance;
        this.quantityOfEnemyInGroup = quantityOfEnemyInGroup;
        this.randomNumberOfEnemies = randomNumberOfEnemies;
        this.minNumberOfEnemiesInGroup = minNumberOfEnemiesInGroup;
        this.maxNumberOfEnemiesInGroup = maxNumberOfEnemiesInGroup;

        CalculatePowerLevel();
    }

    public float GetAttackWithVariance() {
        if (attackVariance >= 1f || attackVariance <= -1f) {
            attackVariance /= 100f;
        }

        return (attack + (Random.Range(-attackVariance, attackVariance) * attack));
    }

    public float GetHealthWithVariance() {
        if (healthVariance >= 1f || healthVariance <= -1f) {
            healthVariance /= 100f;
        }

        return (health + (Random.Range(-healthVariance, healthVariance) * health));
    }

    public float GetRangeWithVariance() {
        if (rangeVariance >= 1f || rangeVariance <= -1f) {
            rangeVariance /= 100f;
        }

        return (range + (Random.Range(-rangeVariance, rangeVariance) * range));
    }

    public float GetSpeedWithVariance() {

        if (speedVariance >= 1f || speedVariance <= -1f) {
            speedVariance /= 100f;
        }

        return (speed + (Random.Range(-speedVariance, speedVariance) * speed));
    }

    public float GetPowerLevel() {
        return powerLevel;
    }

    private void CalculateEnemyStats() {
        if (attackVariance > 0f) {
            attack = GetAttackWithVariance();
        }

        if (healthVariance > 0f) {
            health = GetHealthWithVariance();
        }

        if (rangeVariance > 0f) {
            range = GetRangeWithVariance();
        }

        if (speedVariance > 0f) {
            speed = GetSpeedWithVariance();
        }
    }

    private void CalculatePowerLevel() {
        powerLevel += attack * attack;
        powerLevel += health * health;
        //powerLevel *= Mathf.sqr(range);
        //powerLevel *= Mathf.sqr(speed);
    }
}
