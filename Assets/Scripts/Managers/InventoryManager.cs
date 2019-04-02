using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    /*
     * TODO: Create delegate for item functions
     * Make private functions following format of delegate in this list
     * Add private functions to dictionary
     * Decide ID ranges for types of items
     * Add inventory to game agents to be accessed by inventory manager   
     */

    //define delegate to encapsulate item functions to store in dictionary
    //item functions will take in game agent to apply item effects to that agent
    delegate void ItemFunc(Item item, GameAgent agent);
    IDictionary<int, ItemFunc> itemFuncs = new Dictionary<int, ItemFunc>();

    public static InventoryManager instance = null;

    void Start()
    {
        //add all item functions here
        //item functions will be identified via item ID
        if (instance == null) instance = this;

        itemFuncs.Add(1, ApplyHealthPotion);
        itemFuncs.Add(2, ApplyManaPotion);
    }

    public void UseItem(Item item, GameAgent agent)
    {
        //uses item ID to know which item func to call
        //passes item and game agent (for use in equipment)
        itemFuncs[item.ID](item, agent);
    }

    private void ApplyHealthPotion(Item item, GameAgent agent)
    {
        //decrement item in player inventory, call HP increase func (if applicable)
        Debug.Log("Applying HP potion!");
    }

    private void ApplyManaPotion(Item item, GameAgent agent)
    {
        Debug.Log("Applying MP potion!");
    }


    //FOLLOWING METHODS FOR EQUIPMENT SYSTEM

    private void EquipHelmet(Item item, GameAgent agent)
    {
        Debug.Log("Equipping helmet!");
    }

    private void EquipArmor(Item item, GameAgent agent)
    {
        Debug.Log("Equipping armor!");
    }

    private void EquipWeapon(Item item, GameAgent agent)
    {
        Debug.Log("Equipping weapon!");
    }

    private void EquipGloves(Item item, GameAgent agent)
    {
        Debug.Log("Equipping gloves!");
    }

    private void EquipShoes(Item item, GameAgent agent)
    {
        Debug.Log("Equipping shoes!");
    }

    private void EquipOffhand(Item item, GameAgent agent)
    {
        Debug.Log("Equipping offhand item!");
    }
}
