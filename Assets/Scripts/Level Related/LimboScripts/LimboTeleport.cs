using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimboTeleport : MonoBehaviour
{
    public Transform tpTo;
    public GameObject prevSegment;
    public GameObject nextSegment;

    public LimboHandler limboHandler;
    public LimboOverlays limboOverlays;

    private UnityEngine.Object tpParticles;

    // Start is called before the first frame update
    void Start()
    {
        tpParticles = Resources.Load("Prefabs/TeleportSmokeParticles");
    }

    void OnCollisionEnter(Collision col){
        if (col.gameObject.tag == "Player"){
            limboHandler.currentLimboObj++;
            limboHandler.objChanged = true;
            col.gameObject.transform.position = tpTo.position;
            Instantiate(tpParticles, col.gameObject.transform.position, Quaternion.Euler(-90f, 0f, 0f), col.gameObject.transform);
            limboOverlays.runCompleteMat = true;
            nextSegment.SetActive(true);
            prevSegment.SetActive(false);
        }
    }
}
