using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterClasses {
    public const int Warrior = 1;
    public const int Hunter = 2;
    public const int Mage = 3;
    public const int Orc = 4;
    public const int Skeleton = 5;
    public const int Boss = 6;

    public const int Sword = 1;
    public const int Bow = 4;
    public const int Staff = 6;
    public const int Axe = 3;
    public const int Club = 9;
    public const int Unarmed = 0;
};

public class CharacterClassDefiner : MonoBehaviour
{
    #region Variables
    // Referenced conponents.    
    GameAgent character;
    Animator animator;
    Transform characterAvatar;

    // Enemy variation variables. (Do not change)
    int minOrcRange = 4;
    int maxOrcRange = 10;
    int minSkeletonRange = 12;
    int maxSkeletonRange = 17;

    // Weapon variation variables.
    int weaponNum;

    // Weapon objects.
    public GameObject sword;
    public GameObject bow;
    public GameObject staff;
    public GameObject axe;
    public GameObject club;
    #endregion

    public void init() {
        // Get required components.
        character = GetComponent<GameAgent>();
        animator = GetComponent<Animator>();

        // Hide all weapon objects.
        hideAllWeapons();
    }

    void Update()
    {
        // For testing.
        if (character is Player) {
            if (Input.GetKeyDown("a")) SetCharacterClass(CharacterClasses.Warrior);
            if (Input.GetKeyDown("s")) SetCharacterClass(CharacterClasses.Hunter);
            if (Input.GetKeyDown("d")) SetCharacterClass(CharacterClasses.Mage);
            if (Input.GetKeyDown("f")) SetCharacterClass(CharacterClasses.Orc);
            if (Input.GetKeyDown("g")) SetCharacterClass(CharacterClasses.Skeleton);
            if (Input.GetKeyDown("h")) SetCharacterClass(CharacterClasses.Boss);
        }
    }

    // SetCharacterClass(int characterID), SetCharacterModel(int modelID), SetCharacterWeapon(int weaponID)
    #region Main Methods
    public void SetCharacterClass(int characterID)
    {
        if (characterID == CharacterClasses.Warrior) // Warrior
        {
            SetCharacterModel(CharacterClasses.Warrior);
            SetCharacterWeapon(CharacterClasses.Warrior);
        }
        else if (characterID == CharacterClasses.Hunter) // Hunter
        {
            SetCharacterModel(CharacterClasses.Hunter);
            SetCharacterWeapon(CharacterClasses.Hunter);
        }
        else if (characterID == CharacterClasses.Mage) // Mage
        {
            SetCharacterModel(CharacterClasses.Mage);
            SetCharacterWeapon(CharacterClasses.Mage);
        }
        else if (characterID == CharacterClasses.Orc) // Orc
        {
            SetCharacterModel(CharacterClasses.Orc);
            weaponNum = Random.Range(4, 6);
            SetCharacterWeapon(weaponNum);
        }
        else if (characterID == CharacterClasses.Skeleton) // Skeleton
        {
            SetCharacterModel(CharacterClasses.Skeleton);
            weaponNum = Random.Range(4, 6);
            SetCharacterWeapon(weaponNum);
        }
        else // Boss
        {
            SetCharacterModel(CharacterClasses.Boss);
            SetCharacterWeapon(CharacterClasses.Boss);
        }
    }

    public void SetCharacterModel(int modelID)
    {
        Transform currentCharacterAvatar = transform.GetChild(GetActiveChracterModel());
        currentCharacterAvatar.gameObject.SetActive(false);

        if (modelID == 1) characterAvatar = transform.Find("Chr_Dungeon_KnightMale_01");
        else if (modelID == 2) characterAvatar = transform.Find("Chr_Vikings_ShieldMaiden_01");
        else if (modelID == 3) characterAvatar = transform.Find("Chr_Fantasy_Wizard_01");
        else if (modelID == 4) characterAvatar = transform.GetChild(GetRandomOrc());
        else if (modelID == 5) characterAvatar = transform.GetChild(GetRandomSkeleton());
        else characterAvatar = transform.Find("Chr_Western_Woman_01");

        characterAvatar.gameObject.SetActive(true);
    }

    public void SetCharacterWeapon(int weaponID)
    {
        hideAllWeapons();

        if (weaponID == CharacterClasses.Warrior) // Sword
        {
            animator.SetInteger("Weapon", CharacterClasses.Sword);
            sword.SetActive(true);
        }
        else if (weaponID == CharacterClasses.Hunter) // Bow
        {
            animator.SetInteger("Weapon", CharacterClasses.Bow);
            bow.SetActive(true);
        }
        else if (weaponID == CharacterClasses.Mage) // Staff
        {
            animator.SetInteger("Weapon", CharacterClasses.Staff);
            staff.SetActive(true);
        }
        else if (weaponID == CharacterClasses.Orc) // Axe
        {
            animator.SetInteger("Weapon", CharacterClasses.Axe);
            axe.SetActive(true);
        }
        else if (weaponID == CharacterClasses.Skeleton) // Club
        {
            animator.SetInteger("Weapon", CharacterClasses.Club);
            club.SetActive(true);
        }
        else // Unarmed
        {
            animator.SetInteger("Weapon", CharacterClasses.Unarmed);
        }

        animator.SetTrigger("InstantSwitchTrigger");
    }
    #endregion

    // GetActiveChracterModel(), GetRandomOrc(), GetRandomSkeleton(), hideAllWeapons()
    #region Helper Methods
    int GetActiveChracterModel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.active) return i;
        }
        return -1;
    }

    int GetRandomOrc()
    {
        return Random.Range(minOrcRange, maxOrcRange);
    }

    int GetRandomSkeleton()
    {
        return Random.Range(minSkeletonRange, maxSkeletonRange);
    }

    void hideAllWeapons()
    {
        if (sword != null)
        {
            sword.SetActive(false);
        }
        if (bow != null)
        {
            bow.SetActive(false);
        }
        if (staff != null)
        {
            staff.SetActive(false);
        }
        if (axe != null)
        {
            axe.SetActive(false);
        }
        if (club != null)
        {
            club.SetActive(false);
        }
    }
    #endregion
}
