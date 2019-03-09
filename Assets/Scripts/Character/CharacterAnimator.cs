using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    #region Variables
    // Referenced conponents.
    Player character;
    Animator animator;

    // Animation variables.
    int animationNumber;
    const float actionDuration = 0.5f;
    const float particleDuration = 1f;

    // Animation controller constants.
    const int maxAttackAnimations = 3;
    const int maxHitAnimations = 5;
    const int maxBlockedAnimations = 2;

    // Particle variables.
    public ParticleSystem magicAura;
    public ParticleSystem magicSparks;
    public ParticleSystem slash;
    public ParticleSystem shot;
    public ParticleSystem healAura;
    public ParticleSystem blood;
    public ParticleSystem hit;
    public ParticleSystem sparks;
    public ParticleSystem dust;
    public ParticleSystem ghosts;
    #endregion

    void Start()
    {
        // Get required components.
        character = GetComponent<Player>();
        animator = GetComponent<Animator>();

        // Size up particle effects.
        magicAura.transform.localScale = new Vector3(3f, 3f, 3f);
        magicSparks.transform.localScale = new Vector3(2f, 2f, 2f);
        slash.transform.localScale = new Vector3(3f, 3f, 3f);
        shot.transform.localScale = new Vector3(2f, 2f, 2f);
        healAura.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        blood.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        sparks.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        ghosts.transform.localScale = new Vector3(3f, 3f, 3f);
    }

    void Update()
    {
        
    }

    // StartMovementAnimation(), StopMovementAnimation(), PlayRotateAnimation()
    #region Character Movement
    public IEnumerator StartMovementAnimation()
    {
        animator.SetBool("Moving", true);
        animator.SetFloat("Velocity Z", character.speed);
        yield return null;
    }

    public IEnumerator StopMovementAnimation()
    {
        animator.SetBool("Moving", false);
        animator.SetFloat("Velocity Z", 0);
        yield return null;
    }

    public IEnumerator PlayRotateAnimation()
    {
        animator.SetTrigger("TurnLeftTrigger");
        yield return null;
    }
    #endregion

    // PlayAttackAnamation(), PlayUseItemAnimation()
    #region Character Action
    public IEnumerator PlayAttackAnimation()
    {
        animationNumber = Random.Range(1, maxAttackAnimations + 1);

        if (animator.GetInteger("Weapon") == 6)
        {
            animator.SetTrigger("CastAttack" + (animationNumber).ToString() + "Trigger");
            SpawnMagicAura();
            SpawnMagicSparks();
            yield return new WaitForSeconds(actionDuration);
            animator.SetTrigger("CastEndTrigger");
        } else
        {
            animator.SetTrigger("Attack" + (animationNumber).ToString() + "Trigger");
            if (animator.GetInteger("Weapon") == 4) SpawnShot();
            else SpawnSlash();
            yield return null;
        }
    }

    public IEnumerator PlayUseItemAnimation()
    {
        animator.SetTrigger("ActivateTrigger");
        SpawnHealAura();
        yield return null;
    }
    #endregion

    // PlayHitAnimation(), PlayBlockedAnimation(), PlayKilledAnimation()
    #region Character Reaction
    public IEnumerator PlayHitAnimation()
    {
        animationNumber = Random.Range(1, maxHitAnimations + 1);
        animator.SetTrigger("GetHit" + (animationNumber).ToString() + "Trigger");
        SpawnBlood();
        SpawnHit();
        yield return null;
    }

    public IEnumerator PlayBlockedAnimation()
    {
        animator.SetBool("Blocking", true);
        animator.SetTrigger("BlockTrigger");
        animationNumber = Random.Range(1, maxBlockedAnimations + 1);
        animator.SetTrigger("BlockGetHit" + (animationNumber).ToString() + "Trigger");
        SpawnSparks();
        SpawnDust();
        yield return new WaitForSeconds(actionDuration);
        animator.SetBool("Blocking", false);
    }

    public IEnumerator PlayKilledAimation()
    {
        animator.SetTrigger("Death1Trigger");
        SpawnGhosts();
        yield return new WaitForSeconds(actionDuration);
    }
    #endregion

    // SpawnMagicAura(), SpawnHealAura(), SpawnBlood(), SpawnSparks(), SpawnGhosts()
    #region Character Particle Effects
    void SpawnMagicAura()
    {
        var magicAuraClone = Instantiate(magicAura, character.transform);
        Destroy(magicAuraClone, particleDuration);
    }

    void SpawnMagicSparks()
    {
        var magicSparksClone = Instantiate(magicSparks, character.transform);
        Destroy(magicSparksClone, particleDuration);
    }

    void SpawnSlash()
    {
        var slashClone = Instantiate(slash, character.transform, particleDuration);
        Destroy(slashClone, particleDuration);
    }


    void SpawnShot()
    {
        var shotClone = Instantiate(shot, character.transform);
        Destroy(shotClone, particleDuration);
    }


    void SpawnHealAura()
    {
        var healAuraClone = Instantiate(healAura, character.transform);
        Destroy(healAuraClone.gameObject, particleDuration);
    }

    void SpawnBlood()
    {
        var bloodClone = Instantiate(blood, character.transform);
        Destroy(bloodClone.gameObject, particleDuration);
    }

    void SpawnHit()
    {
        var hitClone = Instantiate(hit, character.transform);
        Destroy(hitClone.gameObject, particleDuration);
    }

    void SpawnSparks()
    {
        var sparksClone = Instantiate(sparks, character.transform);
        Destroy(sparksClone.gameObject, particleDuration);
    }

    void SpawnDust()
    {
        var dustClone = Instantiate(dust, character.transform);
        Destroy(dustClone.gameObject, particleDuration);
    }

    void SpawnGhosts()
    {
        var ghostClone = Instantiate(ghosts, character.transform);
        Destroy(ghostClone.gameObject, particleDuration);
    }
    #endregion

}
