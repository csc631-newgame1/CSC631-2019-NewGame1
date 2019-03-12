using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    public Item[] items = new Item[numItemSlots];
    public const int numItemSlots = 4;

    /*
     * Adds an item to the items array. Returns true if success, false if fail  
     */  
    public bool AddItem(Item addItem)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = addItem;
                return true;
            }
        }
        return false;
    }

    /*
     * TODO: create a method for adding more of one item? but Item class already
     * comes with amount field, so maybe not needed   
     */

    /*
     * Removes an item from the items array. Returns true if success, false if fail  
     */  
    public bool RemoveItem(Item removeItem)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == removeItem)
            {
                items[i] = null;
                return true;
            }
        }
        return false;
    }

    /*
     *   Removes a designated amount of an item. Returns true if success, false
     * if fail.
     *   NOTE: Doesn't account for if attempting to remove more than amount of 
     * item player has yet. Need to handle this case somehow.
     */  
    public bool RemoveAmtofItem(Item removeItem, int amount)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].ID == removeItem.ID)
            {
                items[i].Amount -= amount;
                if (items[i].Amount <= 0)
                {
                    items[i] = null;
                }
                return true;
            }
        }
        return false;
    }
}
