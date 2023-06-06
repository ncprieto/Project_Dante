using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarAndNumber : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI number;
    public GameObject icon;
    public GameObject transIcon;
    public TextMeshProUGUI timer;

    public void SetSliderAndNumber(int n)
    {
        slider.value = n;
        fill.color   = gradient.Evaluate(slider.normalizedValue);
        number.text  = n.ToString();
    }

    public void SetCooldownToReady()
    {
        slider.value = 1f;
        transIcon.SetActive(false);
        icon.SetActive(true);
    }

    public void UpdateCooldown(float timeLeft, float baseTime)
    {
        slider.value = 1f - (timeLeft / baseTime);
        timer.text = Mathf.CeilToInt(timeLeft).ToString();
    }
}
