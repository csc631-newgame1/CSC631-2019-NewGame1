using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{


    public GameObject buttonPanel, characterSelectPanel;


    private MainMenuCamera mainMenuCamera;



    // Start is called before the first frame update
    void Awake()
    {
        mainMenuCamera = Camera.main.GetComponent<MainMenuCamera>();
    }
    
   public void PlayGAme()
    {

        mainMenuCamera.ChangePosition(1);
        buttonPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
        //if (mainMenuCamera.CanCLick)
        //{
        //    mainMenuCamera.CanCLick = false;
        //    buttonPanel.SetActive(false);
        //    characterSelectPanel.SetActive(true);
        //    mainMenuCamera.ReachedCharacterSelectPosition = false;


        //}
    }


    public void BackToMainMenu()
    {


        mainMenuCamera.ChangePosition(0);
        buttonPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
        //if (mainMenuCamera.CanCLick)
        //{

        //    mainMenuCamera.CanCLick = false;
        //    mainMenuCamera.BackToMainMenu = true;
        //    buttonPanel.SetActive(true);
        //    characterSelectPanel.SetActive(false);


        //}
    }


}
