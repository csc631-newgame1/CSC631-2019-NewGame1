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

        //Debug.Log("I am an upset child");
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
		float camera_zoom_ratio = 10 / CameraControl.currentZoom;
        Vector3 offset = new Vector3(0, 40, 0) * camera_zoom_ratio;
        Vector3 wantedPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position) + offset;
		bar.transform.localScale = Vector3.one * camera_zoom_ratio;
        instance.transform.position = wantedPosition;
    }

    private void OnDestroy()
    {
        Destroy(instance);
    }
}
