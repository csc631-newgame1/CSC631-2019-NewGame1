using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack
{
	public int range;
	public int AOE;
	public bool attacking;
	public abstract IEnumerator Execute(GameAgent attacker, GameAgent target);
	public abstract string toString();
	
	public static Dictionary<string, Attack> Get = new Dictionary<string, Attack>() {
		["Melee"] = new MeleeAttack(),
		["Shortbow"] = new ShortbowAttack(),
		["Longbow"] = new LongbowAttack(),
		["Fire"] = new FireSpell()
	};
}

public class MeleeAttack : Attack
{
	public MeleeAttack() { this.range = 1; this.AOE = 1; }
	public override IEnumerator Execute(GameAgent attacker, GameAgent target)
	{
		attacking = true;
		
		attacker.transform.LookAt(target.transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Melee");
		
		while (attacker.animating) yield return null;
		
		target.playHitAnimation();
		target.playHitNoise("Melee");
		target.take_damage(attacker.stats.DealDamage());
		
		attacking = false;
	}
	public override string toString() { return "Swing"; }
}

public class ShortbowAttack : Attack
{
	public ShortbowAttack() { this.range = 7; this.AOE = 1; }
	public override IEnumerator Execute(GameAgent attacker, GameAgent target)
	{
		attacking = true;
		
		attacker.transform.LookAt(target.transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Bow");
		
		while (attacker.animating) yield return null;
		
		Projectile arrow = MapManager.AnimateProjectile(attacker.grid_pos, target.grid_pos, "arrow");
		
		while (!arrow.reachedDestination) yield return null;
		
		target.playHitAnimation();
		target.playHitNoise("Bow");
		target.take_damage(attacker.stats.DealDamage());
		
		attacking = false;
	}
	public override string toString() { return "Shoot (short)"; }
}

public class LongbowAttack : Attack
{
	public LongbowAttack() { this.range = 11; this.AOE = 1; }
	public override IEnumerator Execute(GameAgent attacker, GameAgent target)
	{
		attacking = true;
		
		attacker.transform.LookAt(target.transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Bow");
		
		while (attacker.animating) yield return null;
		
		var arrow = MapManager.AnimateProjectile(attacker.grid_pos, target.grid_pos, "arrow");
		
		while (!arrow.reachedDestination) yield return null;
		
		target.playHitAnimation();
		target.playHitNoise("Bow");
		target.take_damage(attacker.stats.DealDamage());
		
		attacking = false;
	}
	public override string toString() { return "Shoot (long)"; }
}

public class FireSpell : Attack
{
	public FireSpell() { this.range = 6; this.AOE = 0; }
	public override IEnumerator Execute(GameAgent attacker, GameAgent target)
	{
		attacking = true;
		
		attacker.transform.LookAt(target.transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Fire");
		
		while (attacker.animating) yield return null;
		
		Projectile fire = MapManager.AnimateProjectile(attacker.grid_pos, target.grid_pos, "fire");
		
		while (!fire.reachedDestination) yield return null;
		
		target.playHitAnimation();
		target.playHitNoise("Fire");
		target.take_damage(attacker.stats.DealDamage());
		
		attacking = false;
	}
	public override string toString() { return "Fire Burst"; }
}

/*public class StormSpell : Attack
{

}

public class Taunt : Attack
{
	
}*/