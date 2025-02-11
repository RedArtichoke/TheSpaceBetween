using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public bool canFlicker; // Control flickering
    float brightness;
    float onThreshold;
    float holdTime;
    Light myLite;
    public AudioSource audioSource;
    List<Light> childLights; // Store child lights

    // Start is called before the first frame update
    void Start()
    {
        holdTime = 0.0f;
        myLite = GetComponent<Light>();
        brightness = myLite.intensity;

        // Find all child lights
        childLights = new List<Light>(GetComponentsInChildren<Light>());
        childLights.Remove(myLite); // Remove self from list
    }

    // Update is called once per frame
    void Update()
    {
        if (!canFlicker) return; // Exit if flickering is disabled

        if (holdTime > 0.1f)
        {
            onThreshold = Random.Range(0.0f, 1f);

            if (onThreshold > 0.9f)
            {
                SetLightIntensity(brightness * 2);
            }
            else if (onThreshold > 0.7f)
            {
                SetLightIntensity(brightness);
                audioSource.Play();
            }
            else
            {
                SetLightIntensity(0.0f);
            }
            holdTime = 0.0f;
        }
        holdTime += Time.deltaTime;
    }

    // Set intensity for all lights
    void SetLightIntensity(float intensity)
    {
        myLite.intensity = intensity;
        foreach (var light in childLights)
        {
            light.enabled = intensity > 0;
        }
    }
}
