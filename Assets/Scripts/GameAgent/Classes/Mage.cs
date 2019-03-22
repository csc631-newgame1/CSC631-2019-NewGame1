using UnityEngine;

public class Mage : CharacterClass
{
    public Mage() {
        baseStats = new GameAgentStats(23, 40, 7, 6);
    }

    public override int GetAttackStatIncreaseFromLevelUp(int level = -1) {
        int min = 2;
        int max = 4;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max);
    }

    public override int GetHealthStatIncreaseFromLevelUp(int level = -1) {
        int min = 1;
        int max = 2;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max);
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
        return new GameAgentAction[] { GameAgentAction.MagicAttackSingleTarget, GameAgentAction.MagicAttackAOE };
    }

    public override void HandleAct(GameAgentAction action) {
    }

    public override void LevelUp() {
    }
}
