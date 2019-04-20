using MapUtils;
using UnityEngine;
using static Constants;

public class Healer : CharacterClass
{
    public Healer() {
        baseStats = new GameAgentController(15, 32, 7, 10, true);
        rng = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>().GetRNG();
    }

    public override int GetAttackStatIncreaseFromLevelUp(int level = -1) {
        int min = 1;
        int max = 3;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max, rng);
    }

    public override int GetHealthStatIncreaseFromLevelUp(int level = -1) {
        int min = 1;
        int max = 2;
        int mean = (min + max) / 2;
        return Utility.NextGaussian(mean, 1, min, max, rng);
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

    public override void HandleAction(GameAgentAction action, GameAgent target, Pos grid_pos, int damage, CharacterAnimator animator, AudioSource source) {
    }

    public override void LevelUp() {
    }

    public override void SetWeapon(int weapon) {
        if (weapon == CharacterClassOptions.RandomClassWeapon) {
            GenerateRandomClassWeapon();
        } else {
            this.weapon = weapon;
        }
    }

    protected override void GenerateRandomClassWeapon() {
        weapon = CharacterClassOptions.Staff;
    }
}
