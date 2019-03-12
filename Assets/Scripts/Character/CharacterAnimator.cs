using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    #region Variables
    // Required conponents.
    Player character;
    Animator animator;

    // Animation variables
    int animationNumber;
    string attack;
    const float actionDuration = 0.5f;
    const float particleDuration = 1f;

    // Animation controller constants.
    const int maxAttackAnimations = 3;
    const int maxHitAnimations = 5;
    const int maxBlockedAnimations = 2;

    // Particle variables.
    public ParticleSystem magicAura;
    public ParticleSystem healAura;
    public ParticleSystem blood;
    public ParticleSystem sparks;
    public ParticleSystem ghosts;
    #endregion

    void Start()
    {
        // Get required components.
        character = GetComponent<Player>();
        animator = GetComponent<Animator>();

        // Determine weapon stance.
        if (character.weapon == 0)
        {
            animator.SetInteger("Weapon", 1);
            attack = "Attack";
        }
        else if (character.weapon == 0)
        {
            animator.SetInteger("Weapon", 4);
            attack = "Attack";
        }
        else if (character.weapon == 1)
        {
            animator.SetInteger("Weapon", 6);
            attack = "CastAttack";

        } else
        {
            animator.SetInteger("Weapon", 0);
            attack = "Attack";
        }

        // Size up particle effects.
        magicAura.transform.localScale = new Vector3(3f, 3f, 3f);
        magicAura.transform.localPosition = new Vector3(0f, 0.05f, 0f);
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
        animator.SetTrigger(attack + (animationNumber).ToString() + "Trigger");
        if (attack == "CastAttack")
        {
            SpawnMagicAura();
            yield return new WaitForSeconds(actionDuration);
            animator.SetTrigger("CastEndTrigger");
        }
        yield return null;
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
        yield return null;
    }

    public IEnumerator PlayBlockedAnimation()
    {
        animator.SetBool("Blocking", true);
        animator.SetTrigger("BlockTrigger");
        animationNumber = Random.Range(1, maxBlockedAnimations + 1);
        animator.SetTrigger("BlockGetHit" + (animationNumber).ToString() + "Trigger");
        SpawnSparks();
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
        Destroy(magicAuraClone.gameObject, particleDuration);
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

    void SpawnSparks()
    {
        var sparksClone = Instantiate(sparks, character.transform);
        Destroy(sparksClone.gameObject, particleDuration);
    }

    void SpawnGhosts()
    {
        var ghostClone = Instantiate(ghosts, character.transform);
        Destroy(ghostClone.gameObject, particleDuration);
    }
    #endregion

}
