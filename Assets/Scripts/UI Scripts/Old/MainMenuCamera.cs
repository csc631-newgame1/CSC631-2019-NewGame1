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

    private List<GameObject> position = new List<GameObject>();


    private void Awake()
    {
        position.Add(gameStartedPostion);
    }

    void Update()
    {
        // MoveToGameStartedPosition();
        // MoveToCharacterSelectMenu();
        // MoveBackToMainMenu();
        MoveToPosition();
    }

    public void ChangePosition(int index)
    {
        position.RemoveAt(0);
        if (index == 0)
        {
            position.Add(gameStartedPostion);


        }
        else
        {
            position.Add(characterSelectPosition);


        }


    }

    void MoveToPosition()
    {
        if (position.Count > 0) {
            transform.position = Vector3.Lerp(transform.position, position[0].transform.position, 1f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, position[0].transform.rotation, 1f * Time.deltaTime);
        }


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

    void MoveToCharacterSelectMenu()
    {

        if (!reached_CharacterSelectPosition)
        {
            if (Vector3.Distance(transform.position, characterSelectPosition.transform.position) < 0.2f)
            {
                reached_CharacterSelectPosition = true;
                canClick = true;
            }

        }
        if (!reached_CharacterSelectPosition)
        {
            transform.position = Vector3.Lerp(transform.position, characterSelectPosition.transform.position,
               1f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, characterSelectPosition.transform.rotation, 1f * Time.deltaTime);

        }


    }


    void MoveBackToMainMenu()
    {

        if (backToMainMenu)
        {
            if (Vector3.Distance(transform.position, gameStartedPostion.transform.position) < 0.2f)
            {
                backToMainMenu = false;
                canClick = true;
            }

        }
        if (backToMainMenu)
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















