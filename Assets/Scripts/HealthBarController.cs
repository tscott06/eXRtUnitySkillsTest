using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public HealthBase healthBase;
    public Slider slider;

    private void Start()
    {
        slider.value = healthBase.PercentHealth;
    }

    private void OnEnable()
    {
        healthBase.OnHealthChange.AddListener(HealthChangeHandler);
    }
    private void OnDisable()
    {
        healthBase.OnHealthChange.RemoveListener(HealthChangeHandler);
    }

    private void HealthChangeHandler(HealthChangeEventArgs args)
    {
        slider.value = args.HealthHaver.PercentHealth;
    }
}
