public class GameAgentStats
{
    public string gameAgentType;
    public float attack;
    public float health;
    public float range;
    public float speed;

    public GameAgentStats(string gameAgentType, float attack, float health, float range, float speed) {
        this.gameAgentType = gameAgentType;
        this.attack = attack;
        this.health = health;
        this.range = range;
        this.speed = speed;
    }

    public GameAgentStats(string gameAgentType) {
        this.gameAgentType = gameAgentType;
        this.attack = 0;
        this.health = 0;
        this.range = 0;
        this.speed = 0;
    }
}
