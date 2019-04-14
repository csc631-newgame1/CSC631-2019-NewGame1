using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthbar;

    public void SetSliderValue(float value)
    {
        healthbar.value = value;
    }
}
