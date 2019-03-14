// Holds information regarding the GameAgents ingame stats
public class GameAgentStats
{
    public string gameAgentType;
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

    public GameAgentStats(string gameAgentType, float attack, float health, float range, float speed) {
        this.gameAgentType = gameAgentType;
        this.attack = attack;
        this.maxHealth = health;
        this.currentHealth = health;
        this.range = range;
        this.speed = speed;
    }

    public GameAgentStats(string gameAgentType) {
        this.gameAgentType = gameAgentType;
        this.attack = 20;
        this.maxHealth = 50;
        this.currentHealth = maxHealth;
        this.range = 1;
        this.speed = 7;
    }
}
