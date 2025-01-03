using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    public LayerMask PlayerLayer;

    public Light[] warnLight;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerLayer == null)
        {
            PlayerLayer = 6;
        }

        //status of lights when nothing is detected
        warnLight[0].enabled = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.layer == PlayerLayer)
        {
            //metal passed through detector
            StartCoroutine(MetalFound());
        }
    }

    IEnumerator MetalFound()
    {
        //turning on or changing colour (metal detected)
        for (int i = 0; i < 3; i++)
        {
            warnLight[0].enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        //back to normal state
        for (int i = 0; i < 3; i++)
        {
            warnLight[0].enabled = false;
        }
    }
}
