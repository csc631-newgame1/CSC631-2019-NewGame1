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
    private Player[] playerStats;

    public Text[] nameText, hpText, mpText, lvlText, expText;
    public Slider[] expSlider;
    public Image[] charImage;
    public GameObject[] charStatHolder;

    public GameObject[] statusButtons;
    public Text statusName, statusHP, statusMP, statusStr, statusDef, statusWpnEqpd, statusWpnPwr, statusArmrEqpd, statusArmrPwr, statusExp;
    public Image statusImage;

  



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
                UpdateMainStats();
            }
            


        }
    }

    public void UpdateMainStats()
    {
        playerStats = StatsManager.instance.playerStats;

        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                charStatHolder[i].SetActive(true);

                nameText[i].text = playerStats[i].name;
                hpText[i].text = "HP: " + playerStats[i].currentHealth + "/" + playerStats[i].currentHealth;
                mpText[i].text = "MP: " + playerStats[i].currentMP + "/" + playerStats[i].maxMP;
                lvlText[i].text = "Lvl: " + playerStats[i].level;
                expText[i].text = "" + playerStats[i].currentEXP + "/" + playerStats[i].expToNextLevel[playerStats[i].level];
                expSlider[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].level];
                expSlider[i].value = playerStats[i].currentEXP;
                charImage[i].sprite = playerStats[i].charImage;
            }
            else
            {
                charStatHolder[i].SetActive(false);
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
        itemCharChoiceMenu.SetActive(false);

    }


public void CloseMenu()
 {
      for (int i = 0; i < panels.Length; i++)
       {
          panels[i].SetActive(false);
      }
 theMenu.SetActive(false);

        itemCharChoiceMenu.SetActive(false);
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

    public void UseItem(int selectChar)
    {
        activeItem.Use(selectChar);
        CloseItemCharChoice();
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
            itemCharChoiceNames[i].text = StatsManager.instance.playerStats[i].name;
            itemCharChoiceNames[i].transform.parent.gameObject.SetActive(StatsManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void CloseItemCharChoice()
    {
        itemCharChoiceMenu.SetActive(false);
    }

   

    public void OpenStatus()
    {
        UpdateMainStats();

        //update the information that is shown
       StatusChar(0);

        for (int i = 0; i < statusButtons.Length; i++)
        {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
            statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].name;
        }
    }

    public void StatusChar(int selected)
    {
        statusName.text = playerStats[selected].name;
        statusHP.text = "" + playerStats[selected].currentHealth + "/" + playerStats[selected].currentHealth;
        statusMP.text = "" + playerStats[selected].currentMP + "/" + playerStats[selected].maxMP;
        statusStr.text = playerStats[selected].attack.ToString();
        statusDef.text = playerStats[selected].defence.ToString();
        if (playerStats[selected].equippedWpn != "")
        {
            statusWpnEqpd.text = playerStats[selected].equippedWpn;
        }
        statusWpnPwr.text = playerStats[selected].wpnPwr.ToString();
        if (playerStats[selected].equippedArmr != "")
        {
            statusArmrEqpd.text = playerStats[selected].equippedArmr;
        }
        statusArmrPwr.text = playerStats[selected].armrPwr.ToString();
        statusExp.text = (playerStats[selected].expToNextLevel[playerStats[selected].level] - playerStats[selected].currentEXP).ToString();
        statusImage.sprite = playerStats[selected].charImage;

    }




}///CLASSSSSSSSSSSSSSSSSSSSSSSSSSSSSSs
