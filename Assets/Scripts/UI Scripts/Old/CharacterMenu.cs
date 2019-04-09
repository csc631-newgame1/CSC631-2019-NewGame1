using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMenu : MonoBehaviour
{

    public GameObject[] characters;
    public GameObject charPosition;


    private int warrior = 0;
    private int warrior1 = 1;
    private int warrior2 = 2;
    private int warrior3 = 3;

    void Start()
    {
        characters[warrior].SetActive(true);
        characters[warrior].transform.position = charPosition.transform.position;
        
    }
    
    public void SelectCharacter()
    {
       int index=int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        TurnOffCharacters();
        characters[index].SetActive(true);
        characters[index].transform.position = charPosition.transform.position;

    }

    void TurnOffCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
        }

    }





















}///CLASSSSSSSSS
