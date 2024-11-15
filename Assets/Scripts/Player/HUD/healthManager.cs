using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthManager : MonoBehaviour
{
    // Player's health (0-100)
    public int health = 100;

    // Indicates if the player is damaged
    public bool isDamaged = false;

    // Reference to the global volume for vignette control
    public Volume globalVolume;

    // Reference to the player's camera
    private Camera playerCamera;

    // Reference to the vignette component
    private Vignette vignette;

    // Original vignette color
    private Color originalColor;

    // Original vignette intensity
    private float originalIntensity;

    // Reference to the color adjustments component
    private ColorAdjustments colorAdjustments;

    // Coroutine for strobe effect
    private Coroutine strobeCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        // Automatically assign the player's camera
        playerCamera = Camera.main;

        // Try to get the vignette component from the global volume
        if (globalVolume.profile.TryGet(out vignette))
        {
            // Store the original vignette color
            originalColor = vignette.color.value;

            // Store the original vignette intensity
            originalIntensity = vignette.intensity.value;
        }

        // Try to get the color adjustments component from the global volume
        if (globalVolume.profile.TryGet(out colorAdjustments))
        {
            // Set the initial color filter to white
            colorAdjustments.colorFilter.value = Color.white;
        }
    }

    // Damages the player by reducing health
    public void DamagePlayer()
    {
        if (!isDamaged)
        {
            health -= 70;
            if (health < 0)
            {
                health = 0;
            }
            isDamaged = true;
            StartCoroutine(DamageCooldown());
            StartCoroutine(ScreenShake());

            if (health < 50)
            {
                vignette.color.value = Color.red;
                if (strobeCoroutine != null)
                {
                    StopCoroutine(strobeCoroutine);
                }
                strobeCoroutine = StartCoroutine(StrobeVignette());
            }
        }
    }

    // Coroutine for damage cooldown
    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(2f);
        isDamaged = false;
        yield return new WaitForSeconds(15f);
        if (health >= 50)
        {
            vignette.color.value = originalColor;
            vignette.intensity.value = originalIntensity;
            colorAdjustments.colorFilter.value = Color.white;
        }
    }

    // Coroutine for screen shake effect
    private IEnumerator ScreenShake()
    {
        float duration = 0.4f; // Shake duration
        float initialMagnitude = 0.6f; // Initial shake magnitude
        float magnitude = initialMagnitude;
        Vector3 originalPosition = playerCamera.transform.position;

        float elapsed = 0.0f;
        float velocity = 0.0f;

        while (elapsed < duration)
        {
            magnitude = Mathf.SmoothDamp(magnitude, 0f, ref velocity, duration - elapsed);
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            playerCamera.transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        playerCamera.transform.position = originalPosition;
    }

    // Coroutine for strobe vignette effect
    private IEnumerator StrobeVignette()
    {
        while (health < 50)
        {
            float intensity = Mathf.Lerp(0.5f, 0.8f, (50f - health) / 50f);
            vignette.intensity.value = intensity;

            float tintAmount = Mathf.Lerp(0f, 0.7f, (50f - health) / 50f);
            colorAdjustments.colorFilter.value = Color.Lerp(Color.white, Color.red, tintAmount);

            yield return new WaitForSeconds(Mathf.Lerp(0.5f, 0.3f, (50f - health) / 50f));
            vignette.intensity.value = Mathf.Lerp(intensity, originalIntensity, 0.5f);
            yield return new WaitForSeconds(Mathf.Lerp(0.5f, 0.3f, (50f - health) / 50f));
        }
    }
}
