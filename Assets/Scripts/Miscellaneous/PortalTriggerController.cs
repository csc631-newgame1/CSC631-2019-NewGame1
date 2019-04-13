using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTriggerController : MonoBehaviour
{

    private GameManager gameManager;
    private MapManager mapManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mapManager = FindObjectOfType<MapManager>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            mapManager.clear_map();
            mapManager.Init(gameManager);
        }
    }


}