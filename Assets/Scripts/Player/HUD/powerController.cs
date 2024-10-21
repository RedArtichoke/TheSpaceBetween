using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class powerController : MonoBehaviour
{
    public Image powerRing; // Reference to the UI Image for the power ring
    public Image chargingRing; // Reference to the UI Image for the charging ring
    public Light flashlight; // Reference to the spotlight
    public Image heartRateUI; // Reference to the heart rate UI element
    public Sprite originalHeartRateSprite; // Original sprite for the heart rate UI
    public Sprite chargingHeartRateSprite; // Sprite to use when charging
    public float power = 100f; // Player's power variable
    public float drainRate = 1f; // Rate at which power drains
    private bool isDraining = false; // Toggle for power drain
    private float targetIntensity = 0f; // Target intensity for the spotlight
    private float lerpDuration = 0.5f; // Duration for the lerp
    private float lerpTime = 0f; // Time elapsed for the current lerp

    private float holdTime = 0f; // Time the F key is held
    private float chargeDuration = 1f; // Duration to fully charge
    private bool isCharging = false; // Whether the player is charging the flashbang
    private float toggleThreshold = 0.2f; // Threshold to differentiate between toggle and hold

    private float originalInnerSpotAngle; // Store original inner spot angle
    private float originalOuterSpotAngle; // Store original outer spot angle
    private float originalIntensity; // Store original intensity

    private bool isFlickering = false; // Track if the flashlight is currently flickering
    private float flickerChance = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        chargingRing.gameObject.SetActive(false); // Ensure the charging ring is initially invisible
        originalInnerSpotAngle = flashlight.innerSpotAngle; // Initialize original inner spot angle
        originalOuterSpotAngle = flashlight.spotAngle; // Initialize original outer spot angle
        originalIntensity = flashlight.intensity; // Initialize original intensity
    }

    // Update is called once per frame
    void Update()
    {
        // Handle charging logic
        if (Input.GetKey(KeyCode.F) && power > 10)
        {
            holdTime += Time.deltaTime;
            isCharging = holdTime >= toggleThreshold; // Start charging if hold time exceeds threshold
            if (isCharging)
            {
                chargingRing.gameObject.SetActive(true); // Show the charging ring
                chargingRing.fillAmount = Mathf.Clamp01((holdTime - toggleThreshold) / (chargeDuration - toggleThreshold)); // Update charging ring to show charging
                if (holdTime >= chargeDuration)
                {
                    heartRateUI.sprite = chargingHeartRateSprite; // Change to charging sprite
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            if (isCharging && holdTime >= chargeDuration)
            {
                // Execute flashbang effect
                power -= 20;
                UpdateHUD();
                flashlight.intensity = 10000f; // Set high intensity for flashbang
                flashlight.innerSpotAngle = 179f; // Set inner spot angle for flashbang
                flashlight.spotAngle = 179f; // Set outer spot angle for flashbang
                StartCoroutine(ResetFlashlight()); // Start coroutine to reset intensity and angles
            }
            else if (holdTime < toggleThreshold)
            {
                // Toggle flashlight if it was a short press
                isDraining = !isDraining;
                targetIntensity = isDraining ? 100f : 0f; // Set target intensity based on toggle
                lerpTime = 0f; // Reset lerp time
            }
            holdTime = 0f;
            isCharging = false;
            chargingRing.gameObject.SetActive(false); // Hide the charging ring
            heartRateUI.sprite = originalHeartRateSprite; // Revert to original sprite
        }

        // Lerp spotlight intensity
        if (lerpTime < lerpDuration)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / lerpDuration;
            flashlight.intensity = Mathf.Lerp(flashlight.intensity, targetIntensity, t);
        }

        // Drain power if toggled on
        if (isDraining && power > 0)
        {
            power -= drainRate * Time.deltaTime;
            power = Mathf.Max(power, 0); // Ensure power doesn't go below zero
            if (!isCharging) {
                UpdateHUD();
            }
        }

        // Turn off the flashlight if power reaches 0
        if (power <= 0 && isDraining)
        {
            isDraining = false;
            targetIntensity = 0f;
            lerpTime = 0f;
        }

        // Start flickering if the flashlight is on, not already flickering, and chance allows
        if (isDraining && !isFlickering && Random.value < flickerChance)
        {
            StartCoroutine(FlashlightFlicker());
        }
    }

    // Coroutine to reset the flashlight intensity and angles after the flashbang effect
    private IEnumerator ResetFlashlight()
    {
        float duration = 0.2f; // Duration for the ease-out effect
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            flashlight.intensity = Mathf.Lerp(10000f, originalIntensity, 1 - (1 - t) * (1 - t)); // Ease-in effect
            yield return null;
        }
        if (isDraining) {
            flashlight.intensity = originalIntensity; // Ensure final intensity is set
        }
        else {
            flashlight.intensity = 0.0f;
        }
        flashlight.innerSpotAngle = originalInnerSpotAngle; // Restore original inner spot angle
        flashlight.spotAngle = originalOuterSpotAngle; // Restore original outer spot angle
    }

    // Coroutine to handle flashlight flickering
    private IEnumerator FlashlightFlicker()
    {
        isFlickering = true;
        float flickerDuration = Random.Range(0.05f, 0.2f);
        float elapsedTime = 0f;

        // Smoothly decrease intensity
        while (elapsedTime < flickerDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flickerDuration;
            flashlight.intensity = Mathf.Lerp(originalIntensity, 1f, t);
            yield return null;
        }

        // Smoothly increase intensity
        elapsedTime = 0f;
        while (elapsedTime < flickerDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flickerDuration;
            flashlight.intensity = Mathf.Lerp(0f, originalIntensity, t);
            yield return null;
        }

        isFlickering = false;
    }

    // Update the HUD element based on the current power level
    void UpdateHUD()
    {
        // Update the fill amount of the power ring
        powerRing.fillAmount = power / 100f;
    }
}
