using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    //derive item types from this class
    public int maxAmount; //max amount of item that can be held
    public string Name; //name of the item
    public int ID; //unique ID used to identity item in code (if necessary)
    public int Amount; //current amount of item held
	public Item() {}
	public Item(int maxAmount, string Name, int ID, int Amount) {
		this.maxAmount = maxAmount;
		this.Name = Name;
		this.ID = ID;
		this.Amount = Amount;
	}
}

public class ConsumableItem : Item
{
	public int effectiveness;
	public ConsumableItem(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(maxAmount, Name, ID, Amount) {
		this.effectiveness = effectiveness;
	}
}

public class HealthPot : ConsumableItem
{
	public static int _ID = 1;
	//public HealthPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public HealthPot(int amount) : base(1, 64, "Health Potion", _ID, amount) {}
}
/*public class HealthPotSmall : HealthPot
{
	public HealthPotSmall() : base(1, 64, "Small Health Potion", _ID, 1) {}
}
public class HealthPotMedium : HealthPot
{
	public HealthPotMedium() : base(2, 64, "Medium Health Potion", _ID, 1) {}
}*/

public class ManaPot : ConsumableItem
{
	public static int _ID = 2;
	//public ManaPot(int effectiveness, int maxAmount, string Name, int ID, int Amount) : base(effectiveness, maxAmount, Name, ID, Amount) {}
	public ManaPot(int amount) : base(1, 64, "Mana Potion", _ID, amount) {}
}
/*public class ManaPotSmall : ManaPot
{
	public ManaPotSmall() : base(1, 64, "Small Mana Potion", _ID, 1) {}
}
public class ManaPotMedium : ManaPot
{
	public ManaPotMedium() : base(2, 64, "Medium Mana Potion", _ID, 1) {}
}*/

public enum EquipType
{
    HELMET,
    ARMOR,
    GLOVE,
    BOOT,
    WEAPON,
}

public class EquipItem : Item
{
    public EquipType type;
    public int atkbonus;
    public int defbonus;

    public EquipItem() { }
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

public class Helmet : EquipItem
{
    public static int _ID = 3;
    public Helmet() : base("Helmet", _ID, EquipType.HELMET, 0, 10) { }
}

public class Armor : EquipItem
{
    public static int _ID = 4;
    public Armor() : base("Armor", _ID, EquipType.ARMOR, 0, 10) { }
}

public class Glove : EquipItem
{
    public static int _ID = 5;
    public Glove() : base("Glove", _ID, EquipType.GLOVE, 0, 10) { }
}

public class Boot : EquipItem
{
    public static int _ID = 6;
    public Boot() : base("Boot", _ID, EquipType.BOOT, 0, 10) { }
}

public class EquipWeapon : EquipItem
{
    public static int _ID = 7;
    public EquipWeapon() : base("Weapon", _ID, EquipType.WEAPON, 10, 0) { }
}

