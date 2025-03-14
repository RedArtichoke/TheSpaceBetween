using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PowerController : MonoBehaviour
{
    public Image powerRing; // The circle of power
    public Image chargingRing; // The circle of charging
    public Image powerBar; // The vertical power bar
    public TextMeshProUGUI powerText; // The power percentage text
    private Color lowPowerColor = new Color(0.94f, 0.27f, 0.22f); // #F04438 in RGB
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

    public float holdTime = 0f; // How long the F key is held
    public float chargeDuration = 1f; // Time to reach full charge
    public bool isCharging = false; // Is the flashbang charging?
    private float toggleThreshold = 0.2f; // The line between a tap and a hold

    private float originalInnerSpotAngle; // The flashlight's inner secret
    private float originalOuterSpotAngle; // The flashlight's outer secret
    private float originalIntensity; // The flashlight's true power

    private bool isFlickering = false; // Is the light having a disco moment?
    private float flickerChance = 0.001f; // The chance of a disco

    public ArduinoHandler arduinoScript;
    public AudioSource lightClick; // Add this line to declare the audio source
    public AudioClip[] toggleSounds; // Array of audio clips
    private AudioSource audioSource; // Declare the audio source

    private float originalDrainRate; // Store the original drain rate

    public AudioClip stunSound; // Add this line to declare the stun sound
    public AudioClip preStunSound; // Add this line to declare the pre-stun sound
    public AudioClip chargeSound; // Add this line to declare the pre-stun sound
    private bool chargeAudioPlaying = false;

    public IntroCutscene introCutscene;

    // Start is called before the first frame update
    private KeyBindManager keyBindManager;
    public TextMeshProUGUI flashBangBinding;

    void Start()
    {
        keyBindManager = FindObjectOfType<KeyBindManager>();
        if (chargingRing != null) { 
            chargingRing.gameObject.SetActive(false); // Hide the charging circle
        }
        if (flashlight != null) {
            originalInnerSpotAngle = flashlight.innerSpotAngle; // Remember the inner secret
            originalOuterSpotAngle = flashlight.spotAngle; // Remember the outer secret
            originalIntensity = flashlight.intensity; // Remember the true power
        }
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource component if not found
        }
        audioSource.volume = 0.5f; // Set volume to half
        originalDrainRate = drainRate; // Save the original drain rate
    }

    // Update is called once per frame
    void Update()
    {
        // Handle charging logic
        if (Input.GetKey(keyBindManager.flashlightKey) && power > 10)
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
                if (!chargeAudioPlaying){
                    audioSource.PlayOneShot(chargeSound);
                    chargeAudioPlaying = true;
                }
                if (chargeAudioPlaying && holdTime >= chargeDuration){
                    audioSource.Stop(); 
                    flashBangBinding.text = "Release";
                }
            }
        }

        if (Input.GetKeyUp(keyBindManager.flashlightKey))
        {
            flashBangBinding.text = "Hold " + keyBindManager.flashlightKey;
            if (isCharging && holdTime >= chargeDuration)
            {
                // Execute flashbang effect
                power -= 20;
                UpdateHUD();
                flashlight.intensity = 10000f; // Supernova mode
                flashlight.innerSpotAngle = 179f; // Wide-eyed mode
                flashlight.spotAngle = 179f; // Wide-eyed mode
                audioSource.priority = 0;       
                if (arduinoScript) {
                    arduinoScript.sendFlashbang();
                }

                if (introCutscene) {
                    introCutscene.flashbang = false;
                }

                // Play pre-stun sound
                if (audioSource != null && preStunSound != null) {
                    audioSource.PlayOneShot(preStunSound); // Play the pre-stun sound
                }

                // Play stun sound
                if (audioSource != null && stunSound != null) {
                    audioSource.PlayOneShot(stunSound); // Play the stun sound
                }
                
                // Logic to stun mimics
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 10f, Vector3.forward, 0f);
                foreach (RaycastHit hit in hits)
                {
                    Debug.Log(hit.collider.name);
                    if (hit.collider.CompareTag("Mimic"))
                    {
                        MimicBehaviour mimic = hit.collider.GetComponent<MimicBehaviour>();
                        if (mimic != null)
                        {
                            StartCoroutine(mimic.StunMimic());
                        }
                    }
                    if (hit.collider.CompareTag("Mite"))
                    {
                        DarkmiteBehaviour darkmite = hit.collider.GetComponent<DarkmiteBehaviour>();
                        if (darkmite != null)
                        {
                            darkmite.Disintegrate();
                        }
                    }
                }
                
                StartCoroutine(ResetFlashlight()); // Cool down the supernova
            }
            else if (holdTime < toggleThreshold)
            {
                // Toggle flashlight if it was a short press
                isDraining = !isDraining;
                targetIntensity = isDraining ? 100f : 0f; // Light on or off
                lerpTime = 0f; // Reset the mind-changing timer

                if (audioSource != null && toggleSounds.Length > 0) {
                    audioSource.priority = 0; // Set to max priority, so it plays over the background music
                    int randomIndex = Random.Range(0, toggleSounds.Length); // Pick a random clip
                    audioSource.pitch = Random.Range(0.6f, 1.2f); // Random pitch variation
                    audioSource.PlayOneShot(toggleSounds[randomIndex]); // Play the random clip
                }
            }
            else if (holdTime < chargeDuration && holdTime > toggleThreshold)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop(); 
                    chargeAudioPlaying = false;
                }
            }
            holdTime = 0f;
            isCharging = false;
            chargingRing.gameObject.SetActive(false); // Hide the charging circle
            heartRateUI.sprite = originalHeartRateSprite; // Back to normal heart

            chargeAudioPlaying = false; // Reset the flag when charging is completed
        }

        // Set power to 100 with the 0 key
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            power = 100f;
            UpdateHUD();
            Debug.Log("Power reset to 100.");
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

        // Lerp spotlight intensity
        if (lerpTime < lerpDuration)
        {
            lerpTime += Time.deltaTime;
            float t = lerpTime / lerpDuration;
            flashlight.intensity = Mathf.Lerp(flashlight.intensity, targetIntensity, t); // Smoothly change the light's mind
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
        
        // Update the vertical power bar
        if (powerBar != null)
        {
            powerBar.fillAmount = power / 100f; // Set fill amount based on power
            
            // Tint the bar colour based on power level
            // Lower power = more red tint, higher power = more white
            float tintAmount = 1 - (power / 100f);
            powerBar.color = Color.Lerp(Color.white, lowPowerColor, tintAmount);
        }
        
        // Update the power percentage text
        if (powerText != null)
        {
            int powerPercentage = Mathf.RoundToInt(power);
            powerText.text = powerPercentage + "%";
        }
    }

    public void AddPower(float amount) {
        power = Mathf.Min(power + amount, 100f); // Cap power at 100
        UpdateHUD();
    }
}
