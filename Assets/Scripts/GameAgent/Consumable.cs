using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    HPPOTION,
    MPPOTION,
}

public class Consumable : Item
{
    public Consumable(string name, int id, int max, int amt)
    {
        Name = name;
        ID = id;
        maxAmount = max;
        Amount = amt;
    }
}
