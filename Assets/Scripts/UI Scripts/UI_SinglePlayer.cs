using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SinglePlayer : MonoBehaviour
{
    private GameObject Panel_Main;
	
	public string characterClass { get; set;} 
	
	public void Awake()
	{
		Panel_Main = transform.parent.Find("MainMenu").gameObject;
	}
	
	public void Start()
	{
		gameObject.SetActive(false);
	}
	
    public void StartGame()
	{
		
	}
	
	public void Back()
	{
		gameObject.SetActive(false);
		Panel_Main.SetActive(true);
	}
}
