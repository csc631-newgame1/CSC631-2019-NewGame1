using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public GameObject healthbar;
    HealthBar bar;
    private static GameObject canvas;
    GameObject instance;
    GameAgent parent;

    void Start()
    {
        canvas = GameObject.Find("UICanvas"); //get canvas ref

        instance = Instantiate(healthbar); //instantiate prefab
        Vector2 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        parent = GetComponent<GameAgent>();
        bar = instance.GetComponentInChildren<HealthBar>();

        instance.transform.SetParent(canvas.transform, false); //set canvas as parent
        instance.transform.position = screenPos;

        bar.SetSliderValue(1);
    }

    void Update()
    {
        bar.SetSliderValue(parent.stats.currentHealth / parent.stats.maxHealth);
        Vector3 offset = new Vector3(0, 20, 0);
        Vector3 wantedPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position) + offset;
        instance.transform.position = wantedPosition;
    }

    private void OnDestroy()
    {
        Destroy(instance);
    }
}
