using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class GunMovement : MonoBehaviour
{
    [Header ("Base Class Variables")]
    public KeyCode abilityKey;
    public float abilityCooldown;
    public float actualAbilityCooldown;
    public bool  IsToggleable;
    public float refundForKill;
    public bool cheatsEnabled;

    [Header("SFX Keys")]
    public string SFXKey;
    public string startSFXKey;
    public string offCDSFXKey;

    [Header("UI Elements")]
    public GameObject CooldownPrefab;
    protected GameObject UICanvas;
    protected GameObject CooldownUI;
    protected NewCooldownUpdater CooldownUpdater;

    // Player Related
    protected GameObject player;
    protected Rigidbody rb;
    protected Camera playerCamera;
    protected Movement movement;

    // Weapon Related
    protected GunAttributes gunAttributes;
    protected FOVVFX fovVFX;

    // Sound Related
    protected float sfxVolume;
    protected FMOD.Studio.EventInstance sfxEvent;
    protected FMOD.Studio.EventInstance startSFXEvent;
    protected FMOD.Studio.EventInstance offCDSFXEvent;
    protected BGMController bgmController;

    // Ability Start enum
    protected ABILITY abilityState;
    protected enum ABILITY
    {
        ACTIVE,
        ONCOOLDOWN,
        OFFCOOLDOWN
    }

    protected virtual void OnDestroy()
    {
        sfxEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    protected virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote)) cheatsEnabled = true;
        if(Input.GetKeyDown(KeyCode.Minus) && cheatsEnabled) abilityCooldown = 0f; // dev tool give no cooldown to weapon ability
        if (Input.GetKeyDown(abilityKey) && !CanActivateAbility() && !CanDeactivateAbility()){
            CooldownUpdater.CooldownNotReadyYet();
        }
    }
    
    public virtual void Initialize(GameObject playerObj, GunAttributes ga, GameObject soundSystem, GameObject Canvas)
    {
        player   = playerObj;                                                   // player related
        rb       = player.GetComponent<Rigidbody>();
        movement = player.GetComponent<Movement>();
        gunAttributes = ga;                                                     // weapon related
        fovVFX        = player.GetComponent<FOVVFX>();                          // fov
        abilityState  = ABILITY.OFFCOOLDOWN;                                    // ability stuff
        abilityKey    = (KeyCode)PlayerPrefs.GetInt("Weapon Ability", 304);     // get ability key from player prefs
        bgmController = soundSystem.GetComponent<BGMController>();
        UICanvas      = Canvas;                                                 // UI related
        SetUpUI();
        SetUpAudio();
    }

    protected virtual void DoMovementAbility()
    {
        abilityState = ABILITY.ACTIVE;
        if(!IsToggleable) StartCoroutine(StartAbilityCooldown());                             // if ability is one time use aka not toggleable start cooldown                                                                 // do sfx
    }

    protected virtual void EndMovementAbility()
    {
        if(IsToggleable) StartCoroutine(StartAbilityCooldown());                              // if ability is toggled start coold down after it ends
    }

    protected bool CanActivateAbility()
    {
        if(abilityState == ABILITY.ACTIVE || abilityState == ABILITY.ONCOOLDOWN) return false;
        return true;
    }

    protected bool CanDeactivateAbility()
    {
        if(IsToggleable && abilityState == ABILITY.ACTIVE) return true;
        return false;
    }

    IEnumerator StartAbilityCooldown()
    {
        abilityState = ABILITY.ONCOOLDOWN;
        float timeLeft = abilityCooldown;
        CooldownUpdater.icon.SetActive(false);
        CooldownUpdater.transIcon.SetActive(true);
        while (timeLeft > 0f)
        {
            if(refundFactor > 0f)
            {
                timeLeft -= refundFactor;
                refundFactor = 0f;
            }
            CooldownUpdater.UpdateCooldown(timeLeft, abilityCooldown);
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        abilityState = ABILITY.OFFCOOLDOWN;
        CooldownUpdater.transIcon.SetActive(false);
        CooldownUpdater.icon.SetActive(true);
        CooldownUpdater.SetCooldownToReady();
        offCDSFXEvent.start();
    }

    private float refundFactor;
    public void GiveCooldownRefund()
    {
        if(abilityState != ABILITY.ONCOOLDOWN) return;
        refundFactor += refundForKill;
    }

    public virtual void ReceiveHitInfo(string tag)
    {
        if(tag == "Lethal") GiveCooldownRefund();
    }

    private void SetUpUI()
    {
        CooldownUI   = Instantiate(CooldownPrefab,   UICanvas.transform, false);
        CooldownUpdater = CooldownUI.GetComponent<NewCooldownUpdater>();
        CooldownUpdater.SetSliderAndNumber(abilityCooldown);
        CooldownUpdater.SetCooldownToReady();
    }

    private void SetUpAudio()
    {
        sfxEvent      = RuntimeManager.CreateInstance(SFXKey);                  // sfx and bgm stuff
        startSFXEvent = RuntimeManager.CreateInstance(startSFXKey);
        offCDSFXEvent = RuntimeManager.CreateInstance(offCDSFXKey);
        sfxVolume = PlayerPrefs.GetFloat("Master", 0.75f) * PlayerPrefs.GetFloat("SFX", 1f);
        sfxEvent.setVolume(sfxVolume);
        startSFXEvent.setVolume(sfxVolume);
        offCDSFXEvent.setVolume(sfxVolume);
    }

    public virtual void EnableUI()
    {
        CooldownUI.SetActive(true);
    }

    public virtual void DisableUI()
    {
        CooldownUI.SetActive(false);
    }
}