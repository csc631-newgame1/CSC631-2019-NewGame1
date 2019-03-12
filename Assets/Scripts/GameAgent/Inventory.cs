using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    public Item[] items = new Item[numItemSlots];
    public const int numItemSlots = 4;

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
}
