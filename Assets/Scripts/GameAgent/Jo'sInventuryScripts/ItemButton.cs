using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
   * Assign Each slot this script
   * And each one is going to have a value assigned and a reference to the item
   * When the menu opens Updates all slots with the correct information.
   * Set up buttons to call information
   */


public class ItemButton : MonoBehaviour
{


    public Image buttonImage;
    public Text amountText;
    public int buttonValue;     //Tie into the items held in the array



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Press()
    {
        if (GameMenu.instance.theMenu.activeInHierarchy)
        {
            if (InventuryManager2.instance.itemsHeld[buttonValue] != "")
            {
                GameMenu.instance.SelectItem(InventuryManager2.instance.GetItemDetails(InventuryManager2.instance.itemsHeld[buttonValue]));
            }
        }

       
    }
}/////////////////// CLASSSSS






