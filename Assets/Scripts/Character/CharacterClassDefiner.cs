using UnityEngine;

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

    public void init(int characterClass = 0) {
        // Get required components.
        character = GetComponent<GameAgent>();
        animator = GetComponent<Animator>();

        // Hide all weapon objects.
        hideAllWeapons();

        if (characterClass > 0) {
            SetCharacterClass(characterClass);
        }
    }

    void Update()
    {
        
    }

    // SetCharacterClass(int characterID), SetCharacterModel(int modelID), SetCharacterWeapon(int weaponID)
    #region Main Methods
    public void SetCharacterClass(int characterID)
    {
        if (characterID == CharacterClassOptions.Knight) // Warrior
        {
            SetCharacterModel(CharacterClassOptions.Knight);
            SetCharacterWeapon(CharacterClassOptions.Knight);
        }
        else if (characterID == CharacterClassOptions.Hunter) // Hunter
        {
            SetCharacterModel(CharacterClassOptions.Hunter);
            SetCharacterWeapon(CharacterClassOptions.Hunter);
        }
        else if (characterID == CharacterClassOptions.Mage) // Mage
        {
            SetCharacterModel(CharacterClassOptions.Mage);
            SetCharacterWeapon(CharacterClassOptions.Mage);
        }
        else if (characterID == CharacterClassOptions.Orc) // Orc
        {
            SetCharacterModel(CharacterClassOptions.Orc);
            weaponNum = Random.Range(4, 6);
            SetCharacterWeapon(weaponNum);
        }
        else if (characterID == CharacterClassOptions.Skeleton) // Skeleton
        {
            SetCharacterModel(CharacterClassOptions.Skeleton);
            weaponNum = Random.Range(4, 6);
            SetCharacterWeapon(weaponNum);
        }
        else // Boss
        {
            SetCharacterModel(CharacterClassOptions.Healer);
            SetCharacterWeapon(CharacterClassOptions.Healer);
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

        if (weaponID == CharacterClassOptions.Knight) // Sword
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Sword);
            sword.SetActive(true);
        }
        else if (weaponID == CharacterClassOptions.Hunter) // Bow
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Bow);
            bow.SetActive(true);
        }
        else if (weaponID == CharacterClassOptions.Mage) // Staff
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Staff);
            staff.SetActive(true);
        }
        else if (weaponID == CharacterClassOptions.Orc) // Axe
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Axe);
            axe.SetActive(true);
        }
        else if (weaponID == CharacterClassOptions.Skeleton) // Club
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Club);
            club.SetActive(true);
        }
        else // Unarmed
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Staff);
            staff.SetActive(true);
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
