using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBobbing : MonoBehaviour
{

    public float bobIntensityX;
    public float bobIntensityY;
    public float bobSpeed;
    public Movement movementScript;

    float defaultY = 0;
    float defaultX = 0;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        defaultY = transform.localPosition.y;
        defaultX = transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(movementScript.GetInputs() && movementScript.isGrounded){
            timer += Time.deltaTime * bobSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x + Mathf.Sin(timer) * bobIntensityX, transform.localPosition.y + Mathf.Sin(timer) * bobIntensityY, transform.localPosition.z);
        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, defaultX, Time.deltaTime * bobSpeed), Mathf.Lerp(transform.localPosition.y, defaultY, Time.deltaTime * bobSpeed), transform.localPosition.z);
        }
    }
}
