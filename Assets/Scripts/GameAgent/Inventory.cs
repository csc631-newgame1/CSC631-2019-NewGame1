using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory
{

    public Item[] items = new Item[numItemSlots];
    public const int numItemSlots = 4;

    public EquipItem helmet;
    public EquipItem armor;
    public EquipItem gloves;
    public EquipItem boots;
    public EquipItem weapon;

    //used for adding items to the inventory
    //item MUST have amount specified
    public void AddItemToSlot(int slot, Item item)
    {
        if (slot >= numItemSlots)
        {
            return; //out of bounds
        }
        items[slot] = item;
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < numItemSlots; i++)
        {
            if (items[i].ID == item.ID)
            {
                items[i].Amount = Mathf.Max(items[i].maxAmount, (items[i].Amount + item.Amount));
                return;
            }
            else if (items[i] == null)
            {
                items[i] = item;
                return;
            }
        }
    }

    //used to remove item completely from inventory
    //used for throwing away items (not using them)
    public void RemoveItemFromSlot(int slot)
    {
        if (slot >= numItemSlots)
        {
            return; //out of bounds
        }
        items[slot] = null;
    }

    //returns item in slot
    public Item GetItemFromSlot(int slot)
    {
        if (slot >= numItemSlots)
        {
            return null; //out of bounds
        }
        return items[slot];
    }

    public void IncrementItemAtSlot(int slot)
    {
        if (slot >= numItemSlots)
        {
            return;
        }
        items[slot].Amount++;
    }

    //decreases amount of item by 1
    //used when item is being used, i.e. potion consumed
    public void DecrementItemAtSlot(int slot)
    {
        if (slot >= numItemSlots)
        {
            return; //out of bounds
        }
        if ((items[slot].Amount--) <= 0)
        {
            items[slot] = null; //remove item if amount is zero
        }
    }

    private void RearrangeSlots()
    {
        for (int i = 0; i < numItemSlots; i++)
        {

        }
    }
}
