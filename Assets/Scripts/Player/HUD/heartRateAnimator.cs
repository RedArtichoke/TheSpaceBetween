using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heartRateAnimator : MonoBehaviour
{
    public float beatsPerMinute = 150.0f;
    public float pulseMagnitude = 0.1f;
    public AudioClip heartBeatSound;

    private Vector3 initialScale;
    private float pulseSpeed;
    private AudioSource audioSource;
    private float lastBeatTime = 0.0f;
    private float noiseOffset;

    void Start()
    {
        initialScale = transform.localScale;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = heartBeatSound;
        noiseOffset = Random.Range(0f, 100f); // Random starting point for Perlin noise
    }

    void Update()
    {
        pulseSpeed = beatsPerMinute / 60.0f * Mathf.PI * 2;
        float time = Time.time * pulseSpeed;
        float scaleFactor = 1 + Mathf.Sin(time) * Mathf.Exp(-Mathf.Pow(time % (2 * Mathf.PI) - Mathf.PI, 2)) * pulseMagnitude;
        
        // Apply Perlin noise to scale
        float noiseScale = Mathf.PerlinNoise(Time.time, noiseOffset) * 0.05f; // Adjust 0.05f for desired effect
        transform.localScale = initialScale * (scaleFactor + noiseScale);

        // Check if it's time to play the heartbeat sound
        // Adjust the condition to play a "badump badump" sound
        if ((Mathf.Sin(time) > 0.95f || Mathf.Sin(time - Mathf.PI / 2) > 0.95f) && Time.time - lastBeatTime > 60.0f / beatsPerMinute / 2.5f)
        {
            lastBeatTime = Time.time;
            PlayHeartBeatSound();
        }
    }

    void PlayHeartBeatSound()
    {
        // Base pitch adjustment based on beats per minute
        float basePitch = Mathf.Lerp(0.8f, 1.3f, (beatsPerMinute - 70) / (110 - 70));

        // Vary pitch more significantly with Perlin noise
        float noisePitch = Mathf.PerlinNoise(Time.time, noiseOffset + 1) * 0.4f - 0.2f; // Range [-0.2, 0.2]
        audioSource.pitch = basePitch + noisePitch;

        // Adjust volume to be more subtle at 70 BPM and more defined at 110 BPM
        float noiseVolume = Mathf.PerlinNoise(Time.time, noiseOffset + 2) * 0.1f; // Range [0, 0.1]
        audioSource.volume = Mathf.Lerp(0.3f, 1.0f, (beatsPerMinute - 70) / (110 - 70)) + noiseVolume;

        audioSource.Play();
    }
}
