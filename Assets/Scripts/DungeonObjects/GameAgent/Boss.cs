using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MapUtils;
using UnityEngine;

public class Boss : GameAgent
{
    /**TODO:
     * Create new BossAnimator for bosses?
     * Check if we need class definer at all
     * Designate names for bosses    
     */

    private MapManager map_manager; // reference to MapManager instance with map data

    private bool enemy_turn = false;

    private BossAnimator animator;
    private CharacterClassDefiner classDefiner;

    [Header("Enemy Stats")]
    public float _attack;
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
    public AudioClip[] axeSwing;
    public AudioClip[] footsteps;
    public AudioClip[] deathRattle;
    public AudioClip[] hitNoise;

    private Attack currentAttack;

    void Update()
    {

        if (FogOfWar.IsVisible(grid_pos))
        {
            EnableRendering();
        }
        else
        {
            DisableRendering();
        }

        if (Input.anyKey)
        {
            animate_attack(this);
        }
    }

    public override void init_agent(Pos position, GameAgentStats stats, string name = null)
    {
        map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
        grid_pos = position;

        animator = GetComponent<BossAnimator>();

        this.stats = stats;
        _attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;
        this.nickname = CharacterRaceOptions.getString(stats.characterRace) + " " + CharacterClassOptions.getWeaponDescriptor(stats.playerCharacterClass.weapon);

        speed = 10;
        move_budget = 10;

        level = stats.level;
        viewableState = stats.currentState;

        animator = GetComponent<BossAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);

        source = GetComponent<AudioSource>();

        // AI init
        team = 1;
        AI = new AIComponent(this); // AI component that decides the actions for this enemy to take
        TurnManager.instance.addToRoster(this);
    }

    private bool moving = false;
    public override IEnumerator smooth_movement(List<Pos> path)
    {
        //Debug.Log("started...");
        grid_pos = path.Last();
        moving = true;
        StartCoroutine(animator.StartMovementAnimation());

        //source.PlayOneShot(footsteps);

        Vector3 origin, target;
        foreach (Pos step in path)
        {

            origin = transform.position;
            target = map_manager.grid_to_world(step);
            float dist = Vector3.Distance(origin, target);
            float time = 0f;

            transform.LookAt(target);

            while (time < 1f && dist > 0f)
            {
                time += (Time.deltaTime * speed) / dist;
                transform.position = Vector3.Lerp(origin, target, time);
                yield return null;
            }
        }
        transform.position = map_manager.grid_to_world(path[path.Count - 1]);

        StartCoroutine(animator.StopMovementAnimation());
        moving = false;
        //Debug.Log("ended...");
    }

    private bool attacking = false;

    public IEnumerator animate_attack(GameAgent target)
    {
        //Debug.Log("started...");
        animating = true;
        attacking = true;

        source.PlayOneShot(randomSFX(axeSwing));

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

        target.take_damage(stats.DealDamage());
        transform.position = ownPos; // reset to previous position after animation

        //Debug.Log("ended...");
        animating = false;
    }

    public override void attack(GameAgent target)
    {
        animating = true;
        currentAttack = stats.playerCharacterClass.GetAvailableActs()[0];
        currentAttack.Execute(this, target);
    }

    public override void playAttackAnimation()
    {
        StartCoroutine(animator.PlayAttackAnimation());
    }

    public override void playHitAnimation()
    {
        StartCoroutine(animator.PlayHitAnimation());
    }

    public override void playAttackNoise(string type)
    {
        source.PlayOneShot(randomSFX(axeSwing));
    }

    public override void playHitNoise(string type)
    {
        switch (type)
        {
            default:
                source.PlayOneShot(randomSFX(hitNoise));
                break;
        }
    }

    public override bool animationFinished()
    {
        return (currentAttack == null || !currentAttack.attacking) && !moving;
    }

    public void Hit() { attacking = false; }
    public void Shoot() { attacking = false; }

    public override void take_damage(int amount)
    {
        stats.TakeDamage(amount);

        if (stats.currentState == GameAgentState.Unconscious)
        {
            StartCoroutine(animator.PlayKilledAimation());
            stats.currentState = GameAgentState.Dead;
            source.PlayOneShot(randomSFX(deathRattle));
            GameManager.kill(this);
        }
        else
        {
            StartCoroutine(animator.PlayHitAnimation());
            source.PlayOneShot(randomSFX(hitNoise));
        }

        //StartCoroutine(wait_to_reset_position());
        currentHealth = stats.currentHealth;
    }

    /*private IEnumerator wait_to_reset_position()
    {
        Vector3 pos = transform.position;
        yield return new WaitForSeconds(1f);
        transform.position = pos;
    }*/

    public override void GetHealed(int amount)
    {
        if (stats.currentState == GameAgentState.Alive)
        {
            stats.GetHealed(amount);

            StartCoroutine(animator.PlayUseItemAnimation());
        }
        currentHealth = stats.currentHealth;
    }

    public override void take_turn()
    {
        StartCoroutine(AI.advance());
    }

    public override bool turn_over()
    {
        return AI.finished;
    }

    public override void move()
    {
    }

    public override void act()
    {
    }

    public override void wait()
    {
    }

    public override void potion()
    {
    }

    public void FootR() { }
    public void FootL() { }
    public void WeaponSwitch() { }

    private static int nextSFX = 0;
    private AudioClip randomSFX(AudioClip[] library)
    {
        return library[nextSFX++ % library.Length];
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
