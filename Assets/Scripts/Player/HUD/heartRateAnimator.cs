using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HeartRateAnimator : MonoBehaviour
{
    public float beatsPerMinute; //150.0f; // How fast is your heart racing?
    public float BPMChange;
    public float pulseMagnitude = 0.1f; // How much does your heart grow when it beats?
    public AudioClip heartBeatSound; // The sound of your heart going "thump thump"

    private Vector3 originalScale; // The heart's original size before it gets all excited
    private float pulseSpeed; // How quickly the heart beats
    private AudioSource heartAudioSource; // The source of the heart's sound
    private float noiseOffset; // A little randomness to keep things interesting
    private Volume globalVolume; // Reference to the global volume
    private LensDistortion lensDistortion; // Lens distortion effect
    private ChromaticAberration chromaticAberration; // Chromatic aberration effect

    void Start()
    {
        originalScale = transform.localScale; // Remember the heart's original size
        heartAudioSource = gameObject.AddComponent<AudioSource>(); // Give the heart a voice
        heartAudioSource.clip = heartBeatSound; // Assign the heartbeat sound
        heartAudioSource.loop = true; // Enable looping
        noiseOffset = Random.Range(0f, 100f); // Start the randomness at a random place

        // Add a reverb filter to make the heart sound like it's in a cave (spooky!)
        var reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
        reverbFilter.reverbPreset = AudioReverbPreset.Cave; // Choose your echo chamber

        // Calculate initial delay based on beats per minute
        float initialDelay = (60.0f / beatsPerMinute) / 1.1f; // Adjust delay proportionally

        // Start playing the looped sound with a delay
        StartCoroutine(StartHeartBeatWithDelay(initialDelay));

        // Find the global volume in the scene
        globalVolume = FindObjectOfType<Volume>();
        if (globalVolume != null && globalVolume.profile.TryGet(out lensDistortion))
        {
            // Successfully found and accessed the lens distortion effect
        }

        if (globalVolume != null && globalVolume.profile.TryGet(out chromaticAberration))
        {
            // Successfully found and accessed the chromatic aberration effect
        }
    }

    IEnumerator StartHeartBeatWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!heartAudioSource.isPlaying)
        {
            heartAudioSource.Play();
        }
    }

    void Update()
    {
        pulseSpeed = beatsPerMinute / 60.0f * Mathf.PI * 2; // Calculate how fast the heart should beat
        float time = Time.time * pulseSpeed; // Time to get the heart racing
        float scaleFactor = 1 + Mathf.Sin(time) * Mathf.Exp(-Mathf.Pow(time % (2 * Mathf.PI) - Mathf.PI, 2)) * pulseMagnitude;
        
        // Add a dash of randomness to the heart's size
        float noiseScale = Mathf.PerlinNoise(Time.time, noiseOffset) * 0.05f; // A sprinkle of noise
        transform.localScale = originalScale * (scaleFactor + noiseScale); // Make the heart grow and shrink

        // Adjust pitch to match heart rate
        float basePitch = (beatsPerMinute / 60.0f) / 2.0f; // Halve the pitch to slow down the loop
        heartAudioSource.pitch = basePitch;

        // Adjust lens distortion based on heart rate
        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = Mathf.Lerp(-0.1f, 0.3f, Mathf.Clamp((beatsPerMinute - 80) / 30f, 0f, 1f));
        }

        // Adjust chromatic aberration based on heart rate
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(0.21f, 1.0f, Mathf.Clamp((beatsPerMinute - 80) / 30f, 0f, 1f));
        }
    }

    void PlayHeartBeatSound()
    {
        float basePitch = Mathf.Lerp(0.8f, 1.3f, (beatsPerMinute - 70) / (110 - 70));
        float noisePitch = Mathf.PerlinNoise(Time.time, noiseOffset + 1) * 0.4f - 0.2f;
        heartAudioSource.pitch = basePitch + noisePitch;

        float noiseVolume = Mathf.PerlinNoise(Time.time, noiseOffset + 2) * 0.1f;
        heartAudioSource.volume = (Mathf.Lerp(0.3f, 1.0f, (beatsPerMinute - 70) / (110 - 70)) + noiseVolume) * 1.5f; 

        // Use PlayOneShot to handle quick successive plays
        heartAudioSource.PlayOneShot(heartBeatSound, heartAudioSource.volume);
    }


}
