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
    delegate void ItemFunc(GameAgent agent);
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

    public void UseItem(int itemID, GameAgent agent)
    {
        //uses itemID to know which item func to call, will pass game agent 
        itemFuncs[itemID](agent);
    }

    private void ApplyHealthPotion(GameAgent agent)
    {
        //decrement item in player inventory, call HP increase func (if applicable)
        Debug.Log("Applying HP potion!");
    }

    private void ApplyManaPotion(GameAgent agent)
    {
        Debug.Log("Applying MP potion!");
    }
}
