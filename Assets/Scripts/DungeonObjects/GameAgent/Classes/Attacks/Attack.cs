using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack
{
	public int range;
	protected int AOE;
	protected float damageModifier;
	
	public bool attacking;
	public abstract IEnumerator Execute(GameAgent attacker, Damageable target);
	public abstract string toString();
	
	public Attack(int range, int AOE, float damageModifier) {
		this.range = range;
		this.AOE = AOE;
		this.damageModifier = damageModifier;
	}
	public Attack(){}
	
	public static Dictionary<string, Attack> Get = new Dictionary<string, Attack>() {
		["Melee"] = new MeleeAttack(),
		["Shortbow"] = new ShortbowAttack(),
		["Longbow"] = new LongbowAttack(),
		["Fire"] = new FireSpell(),
        ["Fire Storm"] = new FireStormSpell(),
        ["Berserk"] = new Berserk()
	};
}

public class MeleeAttack : Attack
{
	public MeleeAttack() : base(1, 1, 2) {}
	public override IEnumerator Execute(GameAgent attacker, Damageable target)
	{
		attacking = true;
		
		attacker.transform.LookAt((target as DungeonObject).transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Melee");
		
		Debug.Log("Waiting for animation to finish");
		while (attacker.animating) yield return null;
		
		try {
			target.playHitAnimation();
			target.playHitNoise("Melee");
			target.take_damage((int) (attacker.stats.DealDamage() * damageModifier));
		}
		catch (Exception e) {
			// swallow the error
		}
		
		attacking = false;
	}
	public override string toString() { return "Swing"; }
}

public class ShortbowAttack : Attack
{
	public ShortbowAttack() : base(7, 1, 1) {}
	public override IEnumerator Execute(GameAgent attacker, Damageable target)
	{
		attacking = true;
		
		attacker.transform.LookAt((target as DungeonObject).transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Bow");
		
		while (attacker.animating) yield return null;
		
		Projectile arrow = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "arrow");
		
		while (!(arrow == null)) yield return null;
		
		try {
		target.playHitAnimation();
		target.playHitNoise("Bow");
		target.take_damage((int) (attacker.stats.DealDamage() * damageModifier));
		}
		catch (Exception e) {
			// swallow the error
		}
		
		attacking = false;
	}
	public override string toString() { return "Shoot (short)"; }
}

public class LongbowAttack : Attack
{
	public LongbowAttack() : base(11, 1, 0.75f) {}
	public override IEnumerator Execute(GameAgent attacker, Damageable target)
	{
		attacking = true;
		
		attacker.transform.LookAt((target as DungeonObject).transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Bow");
		
		while (attacker.animating) yield return null;
		
		var arrow = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "arrow");
		
		while (!(arrow == null)) yield return null;
		
		try {
		target.playHitAnimation();
		target.playHitNoise("Bow");
		target.take_damage((int) (attacker.stats.DealDamage() * damageModifier));
		}
		catch (Exception e) {
			// swallow the error
		}
		
		attacking = false;
	}
	public override string toString() { return "Shoot (long)"; }
}

public class FireSpell : Attack
{
	public FireSpell() : base(6, 1, 2) {}
	public override IEnumerator Execute(GameAgent attacker, Damageable target)
	{
		attacking = true;
		
		attacker.transform.LookAt((target as DungeonObject).transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Fire");
		
		while (attacker.animating) yield return null;
		
		Projectile fire = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "fire");
		
		while (!(fire == null)) yield return null;
		
		try {
		target.playHitAnimation();
		target.playHitNoise("Fire");
        target.take_damage((int)(attacker.stats.DealDamage() * damageModifier));
        }
		catch (Exception e) {
			// swallow the error
		}
		
		attacking = false;
	}
	public override string toString() { return "Fire Burst"; }
}

public class FireStormSpell : Attack
{
    public FireStormSpell() : base(6, 1, 2) {}
    public override IEnumerator Execute(GameAgent attacker, Damageable target) {
        int count = attacker.stats.GetMultiHitCount();

        while (count > 0) {
            attacking = true;

            attacker.transform.LookAt((target as DungeonObject).transform);
            attacker.playAttackAnimation();
            attacker.playAttackNoise("Fire");

            while (attacker.animating) yield return null;

            Projectile fire = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "fire");

            while (!(fire == null)) yield return null;

            try {
                target.playHitAnimation();
                target.playHitNoise("Fire");
                target.take_damage((int)(attacker.stats.GetFireStormDamage() * damageModifier));
            } catch (Exception e) {
                // swallow the error
            }

            attacking = false;

            // if Damageable is dead, stop loop
            // implement this


            count--;
        }
        Debug.Log("After while loop");
    }
    public override string toString() { return "Fire Storm"; }
}

public class Berserk : Attack {
    public Berserk() : base(1, 1, 3) { }
    public override IEnumerator Execute(GameAgent attacker, Damageable target) {
        int count = attacker.stats.GetMultiHitCount();

        while (count > 0) {
            attacking = true;

            attacker.transform.LookAt((target as DungeonObject).transform);
            attacker.playAttackAnimation();
            attacker.playAttackNoise("Melee");

            Debug.Log("Waiting for animation to finish");
            while (attacker.animating) yield return null;

            try {
                target.playHitAnimation();
                target.playHitNoise("Melee");
                target.take_damage((int)(attacker.stats.GetBerserkDamage() * damageModifier));
            } catch (Exception e) {
                // swallow the error
            }

            attacking = false;
            count--;
        }
    }
    public override string toString() {
        return "Berserk";
    }
}


    /*public class Taunt : Attack
{
	
}
*/