using UnityEngine;

public class Healer : CharacterClass
{
    public Healer() {
        baseStats = new GameAgentStats(15, 32, 7, 10);
    }

    public override int GetAttackStatIncreaseFromLevelUp(int level = -1) {
        return Random.Range(1, 3);
    }

    public override int GetHealthStatIncreaseFromLevelUp(int level = -1) {
        return Random.Range(1, 2);
    }

    public override int GetRangeStatIncreaseFromLevelUp(int level = -1) {
        if (level % 10 == 0 && level < 40 && level != -1) {
            return 1;
        }
        return 0;
    }

    public override int GetSpeedStatIncreaseFromLevelUp(int level = -1) {
        if (level % 10 == 0 && level < 40 && level != -1) {
            return 1;
        }
        return 0;
    }

    public override GameAgentAction[] GetAvailableActs() {
        return new GameAgentAction[] { GameAgentAction.MagicAttackSingleTarget, GameAgentAction.Heal };
    }

    public override void HandleAct(GameAgentAction action) {
    }

    public override void LevelUp() {
    }


}
