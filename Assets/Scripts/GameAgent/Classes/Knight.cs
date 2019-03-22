using UnityEngine;

public class Knight : CharacterClass {

    public Knight() {
        baseStats = new GameAgentStats(17, 50, 1, 7);
    }

    public override int GetAttackStatIncreaseFromLevelUp(int level = -1) {
        return Random.Range(2, 4);
    }

    public override GameAgentAction[] GetAvailableActs() {
        return new GameAgentAction[] {GameAgentAction.MeleeAttack, GameAgentAction.Taunt};
    }

    public override int GetHealthStatIncreaseFromLevelUp(int level = -1) {
        return Random.Range(3, 4);
    }

    public override int GetRangeStatIncreaseFromLevelUp(int level = -1) {
        return 0;
    }

    public override int GetSpeedStatIncreaseFromLevelUp(int level = -1) {
        if (level % 10 == 0 && level < 40 && level != -1) {
            return 1;
        }
        return 0;
    }

    public override void HandleAct(GameAgentAction action) {
    }

    public override void LevelUp() {
    }
}
