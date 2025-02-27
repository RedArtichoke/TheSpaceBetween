using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DarkController : MonoBehaviour
{
    public bool hasDevice = false;
    public bool inDark = false; // Tracks if player is in dark
    private GameObject globalVolume; // Reference to Global Volume
    private Camera mainCamera; // Reference to Main Camera
    private GameObject[] mimics; // Store references to all mimics
    private GameObject[] footprints; // Store references to all footprints
    private Volume volume; // Reference to Volume component
    private ColorAdjustments colorAdjustments; // Reference to Color Adjustments
    private float targetExposure; // Target exposure value
    private float currentExposure; // Current exposure value
    private float exposureVelocity; // Velocity for SmoothDamp
    public ParticleSystem darkParticles;
    private List<GameObject> mimicMarkers = new List<GameObject>();
    public GameObject mimicMarkerPrefab; // Assign in inspector - should be a 2D sprite prefab
    public AudioClip[] darkEntrySounds; // Array to store sound clips
    private AudioSource audioSource; // AudioSource to play sounds
    public AudioClip continuousDarkSound; // Sound to play continuously in the dark
    private AudioSource continuousAudioSource; // Separate AudioSource for continuous sound
    public ParticleSystem eyeParticles; // Reference to Eye Particle System
    private Coroutine fadeInCoroutine; // Store reference to FadeIn coroutine
    private Coroutine exitDarkCoroutine; // Store reference to ExitDark coroutine
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    public AudioClip darkAmbientSound; // Sound to play continuously in the dark
    private AudioSource darkAmbientAudioSource; // Separate AudioSource for ambient sound
    public GameObject keyItemReplacementPrefab; // Assign in inspector
    public GameObject keyItemChildPrefab; // Assign in inspector

    private List<GameObject> keyItemsWithHighlights = new List<GameObject>();

    private HeartRateSimulator heartRateSimulator;

    void Start()
    {
        heartRateSimulator = GameObject.FindWithTag("HeartRateSimulator").GetComponent<HeartRateSimulator>();
        // Configure fog
        RenderSettings.fog = false; 
        RenderSettings.fogColor = Color.black; // Set fog color
        RenderSettings.fogMode = FogMode.Linear; // Set fog mode
        RenderSettings.fogStartDistance = 5f; // Start fog closer
        RenderSettings.fogEndDistance = 15f; // End fog at a closer distance

        globalVolume = GameObject.FindGameObjectWithTag("CameraVolume");
        mainCamera = Camera.main;

        if (globalVolume.TryGetComponent<Volume>(out volume))
        {
            volume.profile.TryGet(out colorAdjustments);
            volume.profile.TryGet(out vignette);
            volume.profile.TryGet(out chromaticAberration);
            volume.profile.TryGet(out lensDistortion);
            colorAdjustments.postExposure.value = -2f;
            currentExposure = colorAdjustments.postExposure.value;
        }

        GameObject particleEmitter = GameObject.FindGameObjectWithTag("DarkParticleEmitter");
        if (particleEmitter != null)
        {
            darkParticles = particleEmitter.GetComponent<ParticleSystem>();
            darkParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); 
            darkParticles.Clear();
        }

        audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource component
        continuousAudioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource component
        continuousAudioSource.clip = continuousDarkSound;
        continuousAudioSource.loop = true;
        continuousAudioSource.volume = 0f; // Start silent

        // Find and configure Eye Particle Emitter
        GameObject eyeParticleEmitter = GameObject.FindGameObjectWithTag("EyeParticleEmitter");
        if (eyeParticleEmitter != null)
        {
            eyeParticles = eyeParticleEmitter.GetComponent<ParticleSystem>();
            eyeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            eyeParticles.Clear();
        }

        darkAmbientAudioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource component
        darkAmbientAudioSource.clip = darkAmbientSound;
        darkAmbientAudioSource.loop = true;
        darkAmbientAudioSource.volume = 0.3f; // Set desired volume

        // Add and configure AudioReverbFilter for underwater effect
        AudioReverbFilter reverbFilter = darkAmbientAudioSource.gameObject.AddComponent<AudioReverbFilter>();
        reverbFilter.reverbPreset = AudioReverbPreset.Underwater; // Use underwater preset
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && hasDevice)
        {
            if (inDark)
            {
                ExitDark();
            }
            else
            {
                EnterDark();
            }
        }
    }

    IEnumerator AdjustExposure()
    {
        float duration = 0.15f; // Half of the total time for each transition
        float elapsedTime = 0f;

        // Transition from -2 to -7
        while (elapsedTime < duration)
        {
            currentExposure = Mathf.Lerp(-2f, -7f, elapsedTime / duration);
            colorAdjustments.postExposure.value = currentExposure;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it hits -7
        currentExposure = -7f;
        colorAdjustments.postExposure.value = currentExposure;
        ToggleMimics(inDark);
        ToggleFootprints(inDark);

        // Hue shift and fog
        if (inDark)
        {
            // Play a random sound with variations when entering the dark
            if (darkEntrySounds.Length > 0)
            {
                int randomIndex = Random.Range(0, darkEntrySounds.Length);
                
                // Randomise pitch between 0.8 and 1.2 for variation
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                
                // Randomise reverb zone mix between 0 and 1
                audioSource.reverbZoneMix = Random.Range(0f, 1f);
                
                // Play the sound
                audioSource.PlayOneShot(darkEntrySounds[randomIndex]);
            }

            // Apply a light red color filter
            colorAdjustments.colorFilter.value = new Color(1f, 0.5f, 0.5f); // Light red

            RenderSettings.fog = true; // Enable fog
            if (darkParticles != null) {
                darkParticles.Play(); // Start particles
            }
            if (continuousAudioSource != null)
            {
                if (fadeInCoroutine != null) StopCoroutine(fadeInCoroutine); // Stop if already running
                continuousAudioSource.volume = 0f; // Ensure volume starts at 0
                fadeInCoroutine = StartCoroutine(FadeInContinuousSound(60f)); // Start fade in
            }

            if (eyeParticles != null)
            {
                eyeParticles.Play(); // Start eye particles
            }

            if (darkAmbientAudioSource != null && !darkAmbientAudioSource.isPlaying)
            {
                darkAmbientAudioSource.Play(); // Start ambient sound
            }
        }
        else
        {
            // Reset color filter
            colorAdjustments.colorFilter.value = Color.white;

            RenderSettings.fog = false; // Disable fog
            if (darkParticles != null) {
                darkParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); 
                darkParticles.Clear();
            }
            if (fadeInCoroutine != null) StopCoroutine(fadeInCoroutine); // Stop fade in
            continuousAudioSource.volume = 0f; // Ensure volume is reset

            if (eyeParticles != null)
            {
                eyeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                eyeParticles.Clear();
            }

            if (darkAmbientAudioSource != null && darkAmbientAudioSource.isPlaying)
            {
                darkAmbientAudioSource.Stop(); // Stop ambient sound
            }
        }

        elapsedTime = 0f;

        // Transition from -7 to -2
        while (elapsedTime < duration)
        {
            currentExposure = Mathf.Lerp(-7f, -2f, elapsedTime / duration);
            colorAdjustments.postExposure.value = currentExposure;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at -2
        currentExposure = -2f;
        colorAdjustments.postExposure.value = currentExposure;
    }

    IEnumerator FadeInContinuousSound(float duration)
    {
        float elapsedTime = 0f;
        continuousAudioSource.Play(); // Ensure audio starts playing
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            if (inDark) {
                continuousAudioSource.volume = Mathf.Lerp(0f, 1f, t);
                if (vignette != null) vignette.intensity.value = Mathf.Lerp(0.1f, 1f, t);
                if (chromaticAberration != null) chromaticAberration.intensity.value = Mathf.Lerp(0.21f, 1f, t);
                if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(0f, 0.6f, t);
            } 
            else {
                continuousAudioSource.volume = 0;
                if (vignette != null) vignette.intensity.value = 0.1f;
                if (chromaticAberration != null) chromaticAberration.intensity.value = 0.21f;
                if (lensDistortion != null) lensDistortion.intensity.value = 0f;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (inDark) {
            continuousAudioSource.volume = 1f; // Ensure volume is maxed
            if (vignette != null) vignette.intensity.value = 1f;
            if (chromaticAberration != null) chromaticAberration.intensity.value = 1f;
            if (lensDistortion != null) lensDistortion.intensity.value = 0.6f;
        }
        else {
            continuousAudioSource.volume = 0f; // Ensure volume is reset
            if (vignette != null) vignette.intensity.value = 0.1f;
            if (chromaticAberration != null) chromaticAberration.intensity.value = 0.21f;
            if (lensDistortion != null) lensDistortion.intensity.value = 0f;
        }
    }

    // Boot player out of the dark if they are in it for more than a minute
    IEnumerator ExitDarkAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (inDark) // Check if still in dark
        {
            // Reset color filter
            colorAdjustments.colorFilter.value = Color.white;

            RenderSettings.fog = false; // Disable fog
            darkParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); 
            darkParticles.Clear();
            continuousAudioSource.Stop(); // Stop continuous sound

            inDark = false; // Exit dark
            StartCoroutine(AdjustExposure());
        }
    }

    // Toggles Mimic enemies based on inDark state
    void ToggleMimics(bool state)
    {
        if (state) // If entering dark
        {
            mimics = GameObject.FindGameObjectsWithTag("Mimic");
            foreach (GameObject mimic in mimics)
            {
                NavMeshAgent agent = mimic.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    Vector3 markerPos = mimic.transform.position;
                    markerPos.y = 0.01f; // Slightly above ground to prevent z-fighting
                    GameObject marker = Instantiate(mimicMarkerPrefab, markerPos, Quaternion.Euler(0, 0, 0));
                    mimicMarkers.Add(marker);

                    agent.isStopped = true; // Stop the agent
                    mimic.SetActive(false); // Disable mimics
                }
            }
        }
        else if (mimics != null) // If not in dark, enable stored mimics
        {
            // Clear all markers
            foreach (GameObject marker in mimicMarkers)
            {
                Destroy(marker);
            }
            mimicMarkers.Clear();
            foreach (GameObject mimic in mimics)
            {
                mimic.SetActive(true); // Enable mimics
                NavMeshAgent agent = mimic.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    if (NavMesh.SamplePosition(mimic.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position); // Ensure agent is on NavMesh
                        agent.isStopped = false; // Resume the agent
                    }
                }
            }
        }
    }

    // Toggles Footprint objects based on inDark state
    void ToggleFootprints(bool state)
    {
        if (state) // If in dark, store references and disable footprints
        {
            footprints = GameObject.FindGameObjectsWithTag("FootPrint");
            foreach (GameObject footprint in footprints)
            {
                footprint.SetActive(false); // Disable footprints
            }
        }
        else if (footprints != null) // If not in dark, enable stored footprints
        {
            foreach (GameObject footprint in footprints)
            {
                if (footprint != null) // Check if footprint exists
                {
                    footprint.SetActive(true); // Enable footprints
                }
            }
        }
    }

    public void EnterDark()
    {
        if (!inDark)
        {
            inDark = true; // Set inDark to true
            StartCoroutine(AdjustExposure()); // Adjust exposure for entering dark

            AddKeyItemHighlights(); // Add child prefab to key items

            if (exitDarkCoroutine != null)
            {
                StopCoroutine(exitDarkCoroutine); // Stop if already running
            }
            exitDarkCoroutine = StartCoroutine(ExitDarkAfterDelay(60f)); // Start new timer
        }
    }

    private void AddKeyItemHighlights()
    {
        // Find all objects on the "KeyItems" layer
        GameObject[] keyItems = GameObject.FindGameObjectsWithTag("KeyItem");
        foreach (GameObject keyItem in keyItems)
        {
            // Instantiate the child prefab and set it as a child of the key item
            GameObject child = Instantiate(keyItemChildPrefab, keyItem.transform);
            keyItemsWithHighlights.Add(child);
        }
    }

    public void ExitDark()
    {
        if (inDark)
        {
            inDark = false; // Set inDark to false
            StopCoroutine(AdjustExposure());
            StartCoroutine(AdjustExposure()); // Adjust exposure for exiting dark

            RemoveKeyItemHighlights(); // Remove child prefab from key items

            if (exitDarkCoroutine != null)
            {
                StopCoroutine(exitDarkCoroutine); // Stop exit timer
            }
        }
    }

    private void RemoveKeyItemHighlights()
    {
        foreach (GameObject child in keyItemsWithHighlights)
        {
            if (child != null && child.layer == LayerMask.NameToLayer("KeyItems"))
            {
                Destroy(child); // Destroy the child prefab
            }
        }
        keyItemsWithHighlights.Clear(); // Clear the list
    }
}
