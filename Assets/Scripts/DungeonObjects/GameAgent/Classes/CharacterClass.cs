using static Constants;

public abstract class CharacterClass
{
    public GameAgentController baseStats;

    public int weapon;

    public System.Random rng;

    public abstract void LevelUp();

    public abstract GameAgentAction[] GetAvailableActs();

    public abstract void HandleAct(GameAgentAction action);

    public abstract int GetAttackStatIncreaseFromLevelUp(int level = -1);

    public abstract int GetHealthStatIncreaseFromLevelUp(int level = -1);

    public abstract int GetRangeStatIncreaseFromLevelUp(int level = -1);

    public abstract int GetSpeedStatIncreaseFromLevelUp(int level = -1);

    public abstract void SetWeapon(int weapon);

    protected abstract void GenerateRandomClassWeapon();
}
