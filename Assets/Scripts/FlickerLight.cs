using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public float brightness;
    public float onThreshold;

    public float holdTime;

    Light myLite;

    // Start is called before the first frame update
    void Start()
    {
        holdTime = 0.0f;
        brightness = 5f;

        myLite = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (holdTime > 0.1f)
        {
            onThreshold = Random.Range(0.0f, 1f);

            if (onThreshold > 0.9f)
            {
                myLite.intensity = brightness;

            }
            else if (onThreshold > 0.7f)
            {
                myLite.intensity = brightness / 2f;
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
