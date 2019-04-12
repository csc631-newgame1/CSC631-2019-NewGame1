using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameObject theMenu;
    public GameObject[] panels;
    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item2 activeItem;
    public Text itemName, itemDescription, useButtonText;

    public GameObject itemCharChoiceMenu;
    public Text[] itemCharChoiceNames;


    public static GameMenu instance; //Make it an instance of itself so we call it on for other scripts

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    /// Update is called once per frame
	void Update()
    {
       // CloseMenu();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (theMenu.activeInHierarchy)
            {
                // theMenu.SetActive(false);
                CloseMenu();

            }
            else
            {
              theMenu.SetActive(true);

            }
            


        }
    }

    public void TogglePanel(int panelNumber)
    {
       

        for (int i = 0; i < panels.Length; i++)
        {
            if (i == panelNumber)
            {
                panels[i].SetActive(!panels[i].activeInHierarchy);
            }
            else
            {
                panels[i].SetActive(false);
            }
        }
       

    }


public void CloseMenu()
 {
      for (int i = 0; i < panels.Length; i++)
       {
          panels[i].SetActive(false);
      }
 theMenu.SetActive(false);

   
    }


    public void ShowItems()
    {
       InventuryManager2.instance.SortItems();

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].buttonValue = i;

            if (InventuryManager2.instance.itemsHeld[i] != "")
            {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = InventuryManager2.instance.GetItemDetails(InventuryManager2.instance.itemsHeld[i]).itemSprite;
                itemButtons[i].amountText.text = InventuryManager2.instance.numberOfItems[i].ToString();
            }
            else
            {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].amountText.text = "";
            }
        }
    }


    public void SelectItem(Item2 newItem)
    {
        activeItem = newItem;

        if (activeItem.isItem)
        {
            useButtonText.text = "Use";
        }

        if (activeItem.isWeapon || activeItem.isArmour)
        {
            useButtonText.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;
    }



    public void DiscardItem()
    {
        if (activeItem != null)
        {
            InventuryManager2.instance.RemoveItem(activeItem.itemName);
        }
    }


    public void OpenItemCharChoice()
    {
        itemCharChoiceMenu.SetActive(true);

        for (int i = 0; i < itemCharChoiceNames.Length; i++)
        {
         //   itemCharChoiceNames[i].text = InventuryManager2.instance.playerStats[i].charName;
           // itemCharChoiceNames[i].transform.parent.gameObject.SetActive(InventuryManager2.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

}///CLASSSSSSSSSSSSSSSSSSSSSSSSSSSSSSs
