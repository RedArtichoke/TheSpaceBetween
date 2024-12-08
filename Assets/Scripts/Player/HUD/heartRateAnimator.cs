using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartRateAnimator : MonoBehaviour
{
    public float beatsPerMinute; //150.0f; // How fast is your heart racing?
    public float pulseMagnitude = 0.1f; // How much does your heart grow when it beats?
    public AudioClip heartBeatSound; // The sound of your heart going "thump thump"

    private Vector3 originalScale; // The heart's original size before it gets all excited
    private float pulseSpeed; // How quickly the heart beats
    private AudioSource heartAudioSource; // The source of the heart's sound
    private float lastHeartBeatTime = 0.0f; // When did the heart last go "thump"?
    private float noiseOffset; // A little randomness to keep things interesting

    void Start()
    {
        originalScale = transform.localScale; // Remember the heart's original size
        heartAudioSource = gameObject.AddComponent<AudioSource>(); // Give the heart a voice
        heartAudioSource.clip = heartBeatSound; // Assign the heartbeat sound
        noiseOffset = Random.Range(0f, 100f); // Start the randomness at a random place

        // Add a reverb filter to make the heart sound like it's in a cave (spooky!)
        var reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
        reverbFilter.reverbPreset = AudioReverbPreset.Cave; // Choose your echo chamber
    }

    void Update()
    {
        pulseSpeed = beatsPerMinute / 60.0f * Mathf.PI * 2; // Calculate how fast the heart should beat
        float time = Time.time * pulseSpeed; // Time to get the heart racing
        float scaleFactor = 1 + Mathf.Sin(time) * Mathf.Exp(-Mathf.Pow(time % (2 * Mathf.PI) - Mathf.PI, 2)) * pulseMagnitude;
        
        // Add a dash of randomness to the heart's size
        float noiseScale = Mathf.PerlinNoise(Time.time, noiseOffset) * 0.05f; // A sprinkle of noise
        transform.localScale = originalScale * (scaleFactor + noiseScale); // Make the heart grow and shrink

        // Is it time for the heart to go "thump thump"?
        if ((Mathf.Sin(time) > 0.95f || Mathf.Sin(time - Mathf.PI / 2) > 0.95f) && Time.time - lastHeartBeatTime > 60.0f / beatsPerMinute / 2.5f)
        {
            lastHeartBeatTime = Time.time; // Update the last heartbeat time
            PlayHeartBeatSound(); // Let the heart sing its song
        }
    }

    void PlayHeartBeatSound()
    {
        // Adjust the pitch based on how fast the heart is beating
        float basePitch = Mathf.Lerp(0.8f, 1.3f, (beatsPerMinute - 70) / (110 - 70));

        // Add some randomness to the pitch for extra fun
        float noisePitch = Mathf.PerlinNoise(Time.time, noiseOffset + 1) * 0.4f - 0.2f; // Range [-0.2, 0.2]
        heartAudioSource.pitch = basePitch + noisePitch;

        // Adjust the volume to match the heart's excitement level
        float noiseVolume = Mathf.PerlinNoise(Time.time, noiseOffset + 2) * 0.1f; // Range [0, 0.1]
        heartAudioSource.volume = Mathf.Lerp(0.3f, 1.0f, (beatsPerMinute - 70) / (110 - 70)) + noiseVolume;

        heartAudioSource.Play(); // Let the heart's sound echo through the land
    }

    ////arduino ts
    //void OnMessageArrived(string msg)
    //{
    //    Debug.Log("bpm: " + msg);
    //    beatsPerMinute = float.Parse(msg); //this takes the message from arduino and makes it equal to BPM value

    //}


}
