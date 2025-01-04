using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    public string playerName;

    public Light[] warnLight;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            //metal passed through detector
            StartCoroutine(MetalFound());
        }
    }

    IEnumerator MetalFound()
    {
        //turning on or changing colour (metal detected)
        for (int i = 0; i < warnLight.Length; i++)
        {
            warnLight[i].enabled = true;
        }

        yield return new WaitForSeconds(1.0f);

        //back to normal state
        for (int j = 0; j < warnLight.Length; j++)
        {
            warnLight[j].enabled = false;
        }
    }
}
