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

    public Image lowPulseImage;

    private bool isLowPulsing;
    private Vector3 origLPScale;
    
    void Start(){
        isLowPulsing = false;
        origLPScale = lowPulseImage.gameObject.transform.localScale;
    }

    void Update(){
        if (slider.value <= 25){
            if (!isLowPulsing){
                StartCoroutine(LowPulse());
            }
        }
        else{
            isLowPulsing = false;
        }
    }

    public void SetSliderAndNumber(int n)
    {
        slider.value = n;
        fill.color   = gradient.Evaluate(slider.normalizedValue);
        number.text  = n.ToString();
    }

    private IEnumerator FadeImageToZeroFrom(float startAlpha, Image i, float t)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, startAlpha);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    private IEnumerator LowPulse(){
        isLowPulsing = true;
        lowPulseImage.gameObject.transform.localScale = origLPScale;
        StartCoroutine(FadeImageToZeroFrom(1f, lowPulseImage, 2f));
        while (lowPulseImage.gameObject.transform.localScale.y <= 1.5f){
            Vector3 newLPScale = new Vector3(lowPulseImage.gameObject.transform.localScale.x + (Time.deltaTime / 3f), lowPulseImage.gameObject.transform.localScale.y + (Time.deltaTime / 3f), lowPulseImage.gameObject.transform.localScale.z + (Time.deltaTime / 3f));
            lowPulseImage.gameObject.transform.localScale = newLPScale;
            yield return null;
        }
        isLowPulsing = false;
        yield return null;
    }
}
