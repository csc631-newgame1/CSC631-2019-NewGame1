using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    //derive item types from this class
    public int maxAmount; //max amount of item that can be held
    public string Name; //name of the item
    public int ID; //unique ID used to identity item in code (if necessary)
    public int Amount; //current amount of item held
}
