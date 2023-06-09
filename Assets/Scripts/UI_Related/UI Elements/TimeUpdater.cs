using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class TimeUpdater : MonoBehaviour
{
    [Header ("Variables")]
    public  float startingTime;
    public  float warnPlayerOfTime;
    public float timeLeft;
    public bool cheatsEnabled;
    private bool unlimitedTime;

    [Header ("UI Elements")]
    public  GameObject UICanvas;
    public  GameObject TimerPrefab;
    public  GameObject AddTimerPrefab;
    private GameObject Timer;
    private GameObject AddTimer;
    private TextMeshProUGUI TimerText;
    private TextMeshProUGUI AddTimerText;

    [Header ("SFX Key and Events")]
    public string sfxKey;
    private FMOD.Studio.EventInstance sfxEvent;
    private float sfxVolume;

    [Header ("Sources")]
    public List<TimeSource> Sources;

    void Start()
    {
        timeLeft   = startingTime;
        Timer      = Instantiate(TimerPrefab, UICanvas.transform, false);
        AddTimer   = Instantiate(AddTimerPrefab, UICanvas.transform, false);
        TimerText  = Timer.GetComponent<TextMeshProUGUI>();
        AddTimerText  = AddTimer.GetComponent<TextMeshProUGUI>();
        SetUpAudio();
        SetUpModifiers();
        InitializeSources();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote)) cheatsEnabled = true;
        if (Input.GetKeyDown(KeyCode.Alpha9) && cheatsEnabled) unlimitedTime = true;
        if (unlimitedTime) return;
        if (timeLeft == -1) return;

        timeLeft -= Time.deltaTime;
        timeLeft = timeLeft > -1 ? timeLeft : -1f;
        updateTimerText(timeLeft);
        if (timeLeft <= warnPlayerOfTime) changeTextColor(TimerText, timeLeft);
        else TimerText.color = Color.white;
    }

    public void ReceiveTime(float amount)
    {
        timeLeft += amount;
        AddTimerText.text = "+ " + amount.ToString() + " S";
        StartCoroutine(FadeTextToZeroAlpha(2f, AddTimerText));
    }

    public void updateTimerText(float currentTime)             // 80
    {
        currentTime += 1;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        int milliseconds = Mathf.FloorToInt((currentTime % 60 * 100) % 100);
        string secStr;
        string milliStr;
        secStr = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
        milliStr = milliseconds < 10 ? ".0" + milliseconds.ToString() : "." + milliseconds.ToString();
        //update the text 
        TimerText.text = minutes.ToString() + ":" + secStr + milliStr;
    }

    public void changeTextColor(TextMeshProUGUI textToChange, float t)
    {   
        textToChange.color = (int)t % 2 == 0 ? Color.white : Color.red;
        if (t < 0f) textToChange.color = Color.red;
        if (textToChange.color == Color.white) sfxEvent.start();
    }

    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    private void InitializeSources()
    {
        foreach(TimeSource source in Sources) source.Initialize(this);
    }

    private void SetUpAudio()
    {
        sfxVolume = PlayerPrefs.GetFloat("Master", 0.75f) * PlayerPrefs.GetFloat("SFX", 1f);
        sfxEvent = RuntimeManager.CreateInstance(sfxKey);
        sfxEvent.setVolume(sfxVolume);
    }

    private void SetUpModifiers()
    {
        ApplyModifier("Starting Time", ref timeLeft);
    }

    private void ApplyModifier(string modifierName, ref float value)
    {
        value *= PlayerPrefs.GetFloat(modifierName, 1);
    }

    private void ApplyModifier(string modifierName, ref int value)
    {
        value *= (int)PlayerPrefs.GetFloat(modifierName, 1);
    }
    
}
