using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public bool canFlicker; // Control flickering
    public float flickerRate = 0.1f; // Time between flickers
    float brightness;
    float onThreshold;
    float holdTime;
    Light myLite;
    public AudioSource audioSource;
    List<Light> childLights; // Store child lights
    GameObject spotLight; // Reference to Spot Light

    // Start is called before the first frame update
    void Start()
    {
        holdTime = 0.0f;
        myLite = GetComponent<Light>();
        brightness = myLite.intensity;

        // Find all child lights
        childLights = new List<Light>(GetComponentsInChildren<Light>());
        childLights.Remove(myLite); // Remove self from list

        // Try to find Spot Light
        Transform spotLightTransform = transform.Find("Spot Light");
        if (spotLightTransform != null)
        {
            spotLight = spotLightTransform.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canFlicker) return; // Exit if flickering is disabled

        if (holdTime > flickerRate)
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

    // Set intensity for all lights and toggle Spot Light if it exists
    void SetLightIntensity(float intensity)
    {
        myLite.intensity = intensity;
        foreach (var light in childLights)
        {
            light.enabled = intensity > 0;
        }
        if (spotLight != null)
        {
            spotLight.SetActive(intensity > 0); // Toggle Spot Light and its children
        }
    }
}
