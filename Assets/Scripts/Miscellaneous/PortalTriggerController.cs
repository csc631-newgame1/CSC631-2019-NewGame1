using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTriggerController : MonoBehaviour
{

    private GameManager manager;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            manager.rebuild();
        }
    }


}