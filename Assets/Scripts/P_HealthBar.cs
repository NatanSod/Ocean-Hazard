using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_HealthBar : MonoBehaviour
{

    public Slider slider;

    public void SetMaxHealth(float MaxHealth)
    {
        slider.maxValue = MaxHealth;
        SetHealth(MaxHealth);
    }

    public void SetHealth(float Health)
    {
        slider.value = Health;
    }
}
