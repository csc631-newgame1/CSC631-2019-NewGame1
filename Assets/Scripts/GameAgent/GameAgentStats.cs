public class GameAgentStats
{
    public float attack;
    public float health;
    public float range;
    public float speed;

    public GameAgentStats(float attack, float health, float range, float speed) {
        this.attack = attack;
        this.health = health;
        this.range = range;
        this.speed = speed;
    }

    public GameAgentStats() {
        this.attack = 0;
        this.health = 0;
        this.range = 0;
        this.speed = 0;
    }
}
