// This class is used to tell which type of enemy should be created
// Any time a new enemy prefab is created, it should be tagged with "Enemy"
// and its name should be added here
public static class GameAgentType {
    public const string TestEnemy = "TestEnemy";
    public const string Player = "Player";
    public const string Skeleton = "Skeleton";
    public const string Orc = "Orc";
};
