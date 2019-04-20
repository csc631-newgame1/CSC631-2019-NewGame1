using MapUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterClass;

public class MeleeAttack : Action
{
    static public MeleeAttack instance;

    private void Awake() {
        instance = this;
    }

    public static IEnumerator attack(GameAgent target, Pos grid_pos, int damage, CharacterAnimator animator, AudioSource source, ActionEnded actionEnded) {
        MapManager map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();

        Debug.Log("Starting attack");
        attacking = true;

        // insert audio sound here
        //source.PlayOneShot(randomSFX(swordSwing));

        // get target position, and distance between us and the enemy
        Vector3 targetPos = map_manager.grid_to_world(target.grid_pos);
        Vector3 ownPos = map_manager.grid_to_world(grid_pos);
        float distance = Vector3.Distance(ownPos, targetPos);

        // look at enemy and start attack animation
        map_manager.GetUnitTransform(grid_pos).LookAt(targetPos);
        instance.StartCoroutine(animator.PlayAttackAnimation());
        
        // wait for animation trigger
        while (map_manager.GetGameAgentAttackState(grid_pos)) yield return null;
        // wait a little longer based on projectile distance
        yield return new WaitForSeconds(distance / 10f);

        target.take_damage(damage);
        map_manager.GetUnitTransform(grid_pos).position = ownPos; // reset position after animation, which sometimes offsets it

        actionEnded?.Invoke();
        Debug.Log("Ended attack");
    }
}
