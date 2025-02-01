using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRepeater : MonoBehaviour
{
    private AudioSource audioSource;

    //STATES
    public enum AudioState
    {
        Spatial,
        Broadcast
    }

    public enum RepeatState
    {
        LoopAlways,
        LoopEveryXSeconds
    }

    [SerializeField]
    private AudioState audioState;

    [Header("Audio Settings")]
    [Range(0.0f, 1.0f)]
    public float Volume = 0.5f;

    [SerializeField]
    private RepeatState repeatState;

    [Header("Delay Length")]
    public float xSeconds = 1.0f;

    [Header("Randomize Seconds")]
    public bool randomize = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioState == AudioState.Broadcast)
        {
            audioSource.spatialBlend = 0.0f;
        }

        if (audioState == AudioState.Spatial)
        {
            audioSource.spatialBlend = 1.0f;
        }

        audioSource.volume = Volume;

        if (repeatState == RepeatState.LoopAlways)
        {
            audioSource.loop = true;
        }

        if (repeatState == RepeatState.LoopEveryXSeconds)
        {
            if (randomize)
            {
                float randomFloat = Random.Range(10.0f, 30.0f); // Random float between 1.0 and 10.0
                xSeconds = randomFloat;
            } 

            audioSource.loop = false;
            StartCoroutine(EveryXSeconds(xSeconds));
        }

    }

    IEnumerator EveryXSeconds(float seconds)
    {
        while (true)
        {
            audioSource.Play();
            Debug.Log("Audio played");

            yield return new WaitForSeconds(seconds);
        }
    }
}
