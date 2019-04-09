using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public GameObject theMenu;
    public GameObject[] panels;
    // Start is called before the first frame update
    void Start()
    {
        
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


}///CLASSSSSSSSSSSSSSSSSSSSSSSSSSSSSSs
