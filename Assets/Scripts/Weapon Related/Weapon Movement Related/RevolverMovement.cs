using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RevolverMovement : GunMovement
{
    [Header("Movement Effect Variables")]
    public float slowScale;
    public float baseSlowTime;
    public float timePerNormal;
    public float timePerCrit;

    private float timeToAdd;
    private float originalGravity;

    private UnityEngine.Rendering.VolumeProfile globalVolumeProfile;
    private UnityEngine.Rendering.VolumeProfile localVolumeProfile;
    private UnityEngine.Rendering.Universal.Vignette globalVignette;
    private UnityEngine.Rendering.Universal.Vignette localVignette;
    private UnityEngine.Rendering.Universal.MotionBlur motionBlur;

    private float gVigBaseIntensity;
    private float lVigBaseIntensity;

    void Awake()
    {
        IsToggleable = true;

        globalVolumeProfile = GameObject.Find("Global Volume").GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if(!globalVolumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
        if(!globalVolumeProfile.TryGet(out globalVignette)) throw new System.NullReferenceException(nameof(globalVignette));

        if(!globalVolumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
        if(!globalVolumeProfile.TryGet(out motionBlur)) throw new System.NullReferenceException(nameof(motionBlur));

        localVolumeProfile = GameObject.Find("Local Volume").GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if(!localVolumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
        if(!localVolumeProfile.TryGet(out localVignette)) throw new System.NullReferenceException(nameof(localVignette));

        gVigBaseIntensity = globalVignette.intensity.value;
        lVigBaseIntensity = localVignette.intensity.value;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
    }

    protected override void Update()
    {
        base.Update();
        if(Input.GetKeyDown(abilityKey))
        {
            if      (CanActivateAbility())   this.DoMovementAbility();
            else if (CanDeactivateAbility()) this.EndMovementAbility();
        }
    }

    protected override void DoMovementAbility()
    {
        base.DoMovementAbility();
        Time.timeScale = slowScale;                                           // set time scale
        chainShotCoroutine = StartChainShotWindow();                          // chain shot window
        StartCoroutine(chainShotCoroutine);
        originalGravity = Physics.gravity.y;                                  // gravity
        Physics.gravity = new Vector3(0f, Physics.gravity.y / 2, 0f);
        fovVFX.RevolverChainShotVFX();                                        // fov
        sfxEvent.start();                                                     // play SFX
        bgmController.LerpBGMPitch(0.1f, 1f, 0.1f);                           // change bgm pitch
        motionBlur.intensity.Override(0.5f);
        globalVignette.intensity.Override(0.5f);
        localVignette.intensity.Override(0.5f);
    }

    protected override void EndMovementAbility()
    {
        base.EndMovementAbility();
        if(chainShotCoroutine != null) StopCoroutine(chainShotCoroutine);     // stop chain shot
        Time.timeScale = 1f;                                                  // reset time scale
        Physics.gravity = new Vector3(0f, originalGravity, 0f);               // reset gravity to original
        fovVFX.UndoRevolverVFX();                                             // fov
        sfxEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);                    // stop sfx
        bgmController.LerpBGMPitch(1f, 0.1f, 0.1f);                           // change bgm pitch
        motionBlur.intensity.Override(0f);
        globalVignette.intensity.Override(gVigBaseIntensity);
        localVignette.intensity.Override(lVigBaseIntensity);
    }

    public override void ReceiveHitInfo(string tag)
    {
        if(tag == null && abilityState == ABILITY.ACTIVE)                        // if players miss and the ability is active, end the ability
        {
            if(CanDeactivateAbility())
            {
                this.EndMovementAbility();
                return;
            } 
        }
        base.ReceiveHitInfo(tag);
        if(tag == "NormalHitbox") timeToAdd = timePerNormal;
        if(tag == "CritHitbox" || tag == "Lethal")   timeToAdd = timePerCrit;
    }

    IEnumerator chainShotCoroutine;
    IEnumerator StartChainShotWindow()
    {
        float timeLeft = baseSlowTime;
        while(timeLeft > 0)
        {
            if(timeToAdd > 0)                      // add time to timer if player chains shots together
            {
                timeLeft += timeToAdd;
                timeToAdd = 0f;
            }
            else timeLeft -= Time.deltaTime;
            yield return null;
        }
        this.EndMovementAbility();
    }
}
