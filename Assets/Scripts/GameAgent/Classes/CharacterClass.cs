public static class CharacterClassOptions {
    public const int Knight = 1;
    public const int Hunter = 2;
    public const int Mage = 3;
    public const int Orc = 4;
    public const int Skeleton = 5;
    public const int Healer = 6;

    public const int Sword = 1;
    public const int Bow = 4;
    public const int Staff = 6;
    public const int Axe = 3;
    public const int Club = 9;
    public const int Unarmed = 0;
};

public abstract class CharacterClass
{
    public GameAgentStats baseStats;

    public abstract void LevelUp();

    public abstract GameAgentAction[] GetAvailableActs();

    public abstract void HandleAct(GameAgentAction action);

    public abstract int GetAttackStatIncreaseFromLevelUp(int level = -1);

    public abstract int GetHealthStatIncreaseFromLevelUp(int level = -1);

    public abstract int GetRangeStatIncreaseFromLevelUp(int level = -1);

    public abstract int GetSpeedStatIncreaseFromLevelUp(int level = -1);
}
