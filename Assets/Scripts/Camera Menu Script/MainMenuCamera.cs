using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{

    public GameObject gameStartedPostion;
    public GameObject characterSelectPosition;

    private bool reached_GameStartedPosition;

    private bool reached_CharacterSelectPosition = true;
    private bool canClick;
    private bool backToMainMenu;



    void Update()
    {
        MoveToGameStartedPosition();
    }


    void MoveToGameStartedPosition()
    {

        if (!reached_GameStartedPosition)
        {
            if (Vector3.Distance(transform.position, gameStartedPostion.transform.position) < 0.2f)
            {
                reached_GameStartedPosition = true;
                canClick = true;
            }

        }
        if (!reached_GameStartedPosition)
        {
            transform.position = Vector3.Lerp(transform.position, gameStartedPostion.transform.position,
               1f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, gameStartedPostion.transform.rotation, 1f * Time.deltaTime);

        }

    }


    /// <summary>
    /// GETTERS AND SETTTERS
    /// </summary>
    public bool ReachedCharacterSelectPosition {
        get { return reached_CharacterSelectPosition; }

        set { reached_CharacterSelectPosition = value; }
    }


    public bool CanCLick
    {

        get { return canClick; }

        set { canClick = value; }

    }



    public bool BackToMainMenu
    {

        get { return backToMainMenu; }

        set { backToMainMenu = value; }

    }























}















