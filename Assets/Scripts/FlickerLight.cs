using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    float brightness;
    float onThreshold;

    float holdTime;

    Light myLite;

    //public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        holdTime = 0.0f;

        myLite = GetComponent<Light>();

        brightness = myLite.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (holdTime > 0.1f)
        {
            onThreshold = Random.Range(0.0f, 1f);

            if (onThreshold > 0.9f)
            {
                myLite.intensity = brightness * 2;
                //audioSource.Play();

            }
            else if (onThreshold > 0.7f)
            {
                myLite.intensity = brightness;
            }
            else
            {
                myLite.intensity = 0.0f;
            }
            holdTime = 0.0f;
        }
        holdTime += Time.deltaTime;
        //turning on and off after a random amount of time to seem like flickering
    }
}
