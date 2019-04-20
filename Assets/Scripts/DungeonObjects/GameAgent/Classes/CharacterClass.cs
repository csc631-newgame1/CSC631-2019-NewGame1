using MapUtils;
using UnityEngine;
using static Constants;

public abstract class CharacterClass
{
    public GameAgentController baseStats;
    public int weapon;
    public System.Random rng;

    public abstract void LevelUp();

    public abstract GameAgentAction[] GetAvailableActs();
    public abstract void HandleAction(GameAgentAction action, GameAgent target, Pos grid_pos, int damage, CharacterAnimator animator, AudioSource source);

    public abstract int GetAttackStatIncreaseFromLevelUp(int level = -1);
    public abstract int GetHealthStatIncreaseFromLevelUp(int level = -1);
    public abstract int GetRangeStatIncreaseFromLevelUp(int level = -1);
    public abstract int GetSpeedStatIncreaseFromLevelUp(int level = -1);

    public abstract void SetWeapon(int weapon);
    protected abstract void GenerateRandomClassWeapon();
}
