using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerController : MonoBehaviour
{
    public Image powerRing; // The circle of power
    public Image chargingRing; // The circle of charging
    public Light flashlight; // The mighty beam of light
    public Image heartRateUI; // The heartbeat of the game
    public Sprite originalHeartRateSprite; // The heart's true form
    public Sprite chargingHeartRateSprite; // The heart on caffeine
    public float power = 100f; // The juice level
    public float drainRate = 1f; // The juice sipping speed 
    
    private bool isDraining = false; // Is the juice being sipped?
    private float targetIntensity = 0f; // The light's ambition
    private float lerpDuration = 0.5f; // The time it takes to change its mind
    private float lerpTime = 0f; // The time spent changing its mind

    private float holdTime = 0f; // How long the F key is held
    private float chargeDuration = 1f; // Time to reach full charge
    private bool isCharging = false; // Is the flashbang charging?
    private float toggleThreshold = 0.2f; // The line between a tap and a hold

    private float originalInnerSpotAngle; // The flashlight's inner secret
    private float originalOuterSpotAngle; // The flashlight's outer secret
    private float originalIntensity; // The flashlight's true power

    private bool isFlickering = false; // Is the light having a disco moment?
    private float flickerChance = 0.001f; // The chance of a disco

    public ArduinoHandler arduinoScript;


    // Start is called before the first frame update
    void Start()
    {
        chargingRing.gameObject.SetActive(false); // Hide the charging circle
        originalInnerSpotAngle = flashlight.innerSpotAngle; // Remember the inner secret
        originalOuterSpotAngle = flashlight.spotAngle; // Remember the outer secret
        originalIntensity = flashlight.intensity; // Remember the true power
    }

    // Update is called once per frame
    void Update()
    {
        // Handle charging logic
        if (Input.GetKey(KeyCode.F) && power > 10)
        {
            holdTime += Time.deltaTime;
            isCharging = holdTime >= toggleThreshold; // Start charging if held long enough
            if (isCharging)
            {
                chargingRing.gameObject.SetActive(true); // Show the charging circle
                chargingRing.fillAmount = Mathf.Clamp01((holdTime - toggleThreshold) / (chargeDuration - toggleThreshold)); // Fill the circle
                if (holdTime >= chargeDuration)
                {
                    heartRateUI.sprite = chargingHeartRateSprite; // Heart on caffeine
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
                flashlight.intensity = 10000f; // Supernova mode
                flashlight.innerSpotAngle = 179f; // Wide-eyed mode
                flashlight.spotAngle = 179f; // Wide-eyed mode
                if (arduinoScript) {
                    arduinoScript.sendFlashbang();
                }
                StartCoroutine(ResetFlashlight()); // Cool down the supernova
            }
            else if (holdTime < toggleThreshold)
            {
                // Toggle flashlight if it was a short press
                isDraining = !isDraining;
                targetIntensity = isDraining ? 100f : 0f; // Light on or off
                lerpTime = 0f; // Reset the mind-changing timer
            }
            holdTime = 0f;
            isCharging = false;
            chargingRing.gameObject.SetActive(false); // Hide the charging circle
            heartRateUI.sprite = originalHeartRateSprite; // Back to normal heart
        }

        // Lerp spotlight intensity
        if (lerpTime < lerpDuration)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / lerpDuration;
            flashlight.intensity = Mathf.Lerp(flashlight.intensity, targetIntensity, t); // Smoothly change the light's mind
        }

        // Drain power if toggled on
        if (isDraining && power > 0)
        {
            power -= drainRate * Time.deltaTime;
            power = Mathf.Max(power, 0); // No negative juice
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
            StartCoroutine(FlashlightFlicker()); // Disco time
        }


    }

    // Coroutine to reset the flashlight intensity and angles after the flashbang effect
    private IEnumerator ResetFlashlight()
    {
        float duration = 0.2f; // Quick cool down
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            flashlight.intensity = Mathf.Lerp(10000f, originalIntensity, 1 - (1 - t) * (1 - t)); // Smoothly return to normal
            yield return null;
        }
        if (isDraining) {
            flashlight.intensity = originalIntensity; // Back to normal power
        }
        else {
            flashlight.intensity = 0.0f; // Lights out
        }
        flashlight.innerSpotAngle = originalInnerSpotAngle; // Restore inner secret
        flashlight.spotAngle = originalOuterSpotAngle; // Restore outer secret
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
            flashlight.intensity = Mathf.Lerp(originalIntensity, 1f, t); // Dim the light
            yield return null;
        }

        // Smoothly increase intensity
        elapsedTime = 0f;
        while (elapsedTime < flickerDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flickerDuration;
            flashlight.intensity = Mathf.Lerp(0f, originalIntensity, t); // Brighten the light
            yield return null;
        }

        isFlickering = false;
    }

    // Update the HUD element based on the current power level
    void UpdateHUD()
    {
        // Update the fill amount of the power ring
        powerRing.fillAmount = power / 100f; // Show the juice level
    }
}
