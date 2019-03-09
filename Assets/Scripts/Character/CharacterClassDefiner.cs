using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClassDefiner : MonoBehaviour
{
    #region Variables
    // Referenced conponents.    
    Player character;
    Animator animator;
    Transform characterAvatar;

    // Enemy variation variables.
    int minOrcRange = 4;
    int maxOrcRange = 10;
    int minSkeletonRange = 12;
    int maxSkeletonRange = 17;

    // Weapon variation variables.
    int weaponNum;

    // Weapon variables.
    public GameObject sword;
    public GameObject bow;
    public GameObject staff;
    public GameObject axe;
    public GameObject club;
    #endregion

    void Start()
    {
        // Get required components.
        character = GetComponent<Player>();
        animator = GetComponent<Animator>();

        // Hide weapon GameObjects.
        hideAllWeapons();
    }

    void Update()
    {
        // For testing.
        if (Input.GetKeyDown("a")) SetCharacterClass(1);
        if (Input.GetKeyDown("s")) SetCharacterClass(2);
        if (Input.GetKeyDown("d")) SetCharacterClass(3);
        if (Input.GetKeyDown("f")) SetCharacterClass(4);
        if (Input.GetKeyDown("g")) SetCharacterClass(5);
        if (Input.GetKeyDown("h")) SetCharacterClass(6);
    }

    // SetCharacterClass(int characterID), SetCharacterModel(int modelID), SetCharacterWeapon(int weaponID)
    #region Main Methods
    public void SetCharacterClass(int characterID)
    {
        if (characterID == 1)
        {
            SetCharacterModel(1);
            SetCharacterWeapon(1);
        }
        else if (characterID == 2)
        {
            SetCharacterModel(2);
            SetCharacterWeapon(2);
        }
        else if (characterID == 3)
        {
            SetCharacterModel(3);
            SetCharacterWeapon(3);
        }
        else if (characterID == 4)
        {
            SetCharacterModel(4);
            weaponNum = Random.Range(4, 6);
            Debug.Log(weaponNum);
            SetCharacterWeapon(weaponNum);
        }
        else if (characterID == 5)
        {
            SetCharacterModel(5);
            weaponNum = Random.Range(4, 6);
            SetCharacterWeapon(weaponNum);
        }
        else
        {
            SetCharacterModel(6);
            SetCharacterWeapon(6);
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

        if (weaponID == 1)
        {
            animator.SetInteger("Weapon", 1);
            sword.SetActive(true);
        }
        else if (weaponID == 2)
        {
            animator.SetInteger("Weapon", 4);
            bow.SetActive(true);
        }
        else if (weaponID == 3)
        {
            animator.SetInteger("Weapon", 6);
            staff.SetActive(true);
        }
        else if (weaponID == 4)
        {
            animator.SetInteger("Weapon", 3);
            axe.SetActive(true);
        }
        else if (weaponID == 5)
        {
            animator.SetInteger("Weapon", 9);
            club.SetActive(true);
        }
        else
        {
            animator.SetInteger("Weapon", 0);
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
