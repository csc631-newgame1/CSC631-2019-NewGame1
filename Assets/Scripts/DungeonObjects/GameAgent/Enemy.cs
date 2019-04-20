using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MapUtils;
using UnityEngine;
using static Constants;

public class Enemy : GameAgent
{
    private MapManager map_manager; // reference to MapManager instance with map data

	private bool enemy_turn = false;

    private CharacterAnimator animator;
    private CharacterClassDefiner classDefiner;

    [Header("Enemy Stats")]
    public float attack;
    public float maxHealth;
    public float currentHealth;
    public float range;
    public float _speed;
    public float moveTime = 0.1f;

	public int moveBudget;
    public int level;
    public GameAgentState viewableState;

    //sound effects
    private AudioSource source;
    public AudioClip[] swordSwing;
    public AudioClip[] axeSwing;
    public AudioClip[] bowShot;
    public AudioClip[] fireSpell;
    public AudioClip[] footsteps;
	public AudioClip[] deathRattle;
	public AudioClip[] hitNoise;
	
	void Update()
	{
		if (FogOfWar.IsVisible(grid_pos)) {
			EnableRendering();
		}
		else {
			DisableRendering();
		}
	}

    public override void init_agent(Pos position, GameAgentController stats, string name = null) 
	{
        map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
        grid_pos = position;

        animator = GetComponent<CharacterAnimator>();

        this.controller = stats;
        attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;
		this.nickname = CharacterRaceOptions.getString(stats.characterRace) + " " + CharacterClassOptions.getWeaponDescriptor(stats.playerCharacterClass.weapon);
		
		speed = 10;
		move_budget = 10;
		
        level = stats.level;
        viewableState = stats.currentState;

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);

        source = GetComponent<AudioSource>();

        // AI init
        team = 1;
		AI = new AIComponent(this); // AI component that decides the actions for this enemy to take
		TurnManager.instance.addToRoster(this);
    }

    public override IEnumerator smooth_movement(List<Pos> path) 
	{
		//Debug.Log("started...");
		grid_pos = path.Last();
        animating = true;
        StartCoroutine(animator.StartMovementAnimation());

        //source.PlayOneShot(footsteps);

			Vector3 origin, target;
			foreach(Pos step in path) {

				origin = transform.position;
				target = map_manager.grid_to_world(step);
				float dist = Vector3.Distance(origin, target);
				float time = 0f;

				transform.LookAt(target);

					while(time < 1f && dist > 0f) {
						time += (Time.deltaTime * speed) / dist;
						transform.position = Vector3.Lerp(origin, target, time);
						yield return null;
					}
			}
			transform.position = map_manager.grid_to_world(path[path.Count - 1]);

        StartCoroutine(animator.StopMovementAnimation());
        animating = false;
		//Debug.Log("ended...");
    }
	
	public override IEnumerator animate_attack(GameAgent target)
	{
		//Debug.Log("started...");
		animating = true;
		attacking = true;

        switch (classDefiner.weaponNum)
        {
            case 1:
                source.PlayOneShot(randomSFX(swordSwing));
                break;
            case 4:
                source.PlayOneShot(randomSFX(bowShot));
                break;
            case 6:
                source.PlayOneShot(randomSFX(fireSpell));
                break;
            default:
                source.PlayOneShot(randomSFX(axeSwing));
                break;
        }

        // get target position, and distance between us and the enemy
        Vector3 targetPos = map_manager.grid_to_world(target.grid_pos);
		Vector3 ownPos = map_manager.grid_to_world(grid_pos);
		float distance = Math.Max(1, Vector3.Distance(ownPos, targetPos));
		
		// look at enemy and start attack animation
		transform.LookAt(targetPos);
		StartCoroutine(animator.PlayAttackAnimation());
		
		// wait for animation trigger
		while (attacking) yield return null;
		// wait a little longer based on projectile distance
		yield return new WaitForSeconds(distance / 100f);
		
		target.take_damage(controller.DealDamage());
		transform.position = ownPos; // reset to previous position after animation
		
		//Debug.Log("ended...");
		animating = false;
	}
	
    public void Hit() { attacking = false; }
    public void Shoot() { attacking = false; }
	
    public override void take_damage(int amount) 
	{	
        controller.TakeDamage(amount);

        if (controller.currentState == GameAgentState.Unconscious) {
            StartCoroutine(animator.PlayKilledAimation());
            controller.currentState = GameAgentState.Dead;
			source.PlayOneShot(randomSFX(deathRattle));
			GameManager.kill(this);
        } else {
            StartCoroutine(animator.PlayHitAnimation());
			source.PlayOneShot(randomSFX(hitNoise));
        }
		
		//StartCoroutine(wait_to_reset_position());
        currentHealth = controller.currentHealth;
    }
	
	/*private IEnumerator wait_to_reset_position()
	{
		Vector3 pos = transform.position;
		yield return new WaitForSeconds(1f);
		transform.position = pos;
	}*/

    public override void GetHealed(int amount) 
	{
        if (controller.currentState == GameAgentState.Alive) {
            controller.GetHealed(amount);

            StartCoroutine(animator.PlayUseItemAnimation());
        }
        currentHealth = controller.currentHealth;
    }

    public override void take_turn() 
	{	
		StartCoroutine(AI.advance());
    }
	
	public override bool turn_over() {
		return AI.finished;
	}

    public override void move() {
    }

    public override void act() {
    }

    public override void wait() {
    }

    public override void potion() {
    }

    public void FootR() { }
    public void FootL() { }
    public void WeaponSwitch() { }
	
	private static int nextSFX = 0;
	private AudioClip randomSFX(AudioClip[] library)
	{
		return library[nextSFX++%library.Length];
	}
	
	public void DisableRendering()
	{
		GetComponent<HealthBarController>().Disable();
		classDefiner.DisableRendering();
	}
	
	public void EnableRendering()
	{
		GetComponent<HealthBarController>().Enable();
		classDefiner.EnableRendering();
	}
}
