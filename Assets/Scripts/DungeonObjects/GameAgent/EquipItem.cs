using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipType {
    HELMET,
    ARMOR,
    GLOVE,
    BOOT,
    WEAPON,
}

public class EquipItem : Item
{
    public EquipType type;
    private int atkbonus;
    private int defbonus;

    public EquipItem(string name, int id, EquipType etype, int atk, int def)
    {
        maxAmount = 1;
        Amount = 1;
        Name = name;
        ID = id;
        type = etype;
        atkbonus = atk;
        defbonus = def;
    }
}
