using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class LimboRespawn : MonoBehaviour
{

    public Transform rsPoint;
    public LimboOverlays limboOverlays;
    public LimboHandler limboHandler;
    public bool isObj5;
    public string respawnSFX;

    private UnityEngine.Object tpParticles;

    // Start is called before the first frame update
    void Start()
    {
        tpParticles = Resources.Load("Prefabs/TeleportSmokeParticles");
    }

    void OnCollisionEnter(Collision col){
        if (col.gameObject.tag == "Player"){
            FMODUnity.RuntimeManager.PlayOneShot(respawnSFX);
            col.gameObject.transform.position = rsPoint.position;
            Instantiate(tpParticles, col.gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f), col.gameObject.transform);
            limboOverlays.runRestartMat = true;
            if (isObj5){
                limboHandler.restartObj5 = true;
            }
        }
    }
}
