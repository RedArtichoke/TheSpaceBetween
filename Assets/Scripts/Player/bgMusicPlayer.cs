using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgMusicPlayer : MonoBehaviour
{
    public AudioClip[] audioTracks; // Array to hold audio tracks
    private AudioSource audioSource; // Audio source for playing music
    private float originalVolume = 0.5f; // Store original volume
    private float delayTime; // Time to wait before playing next track

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Create audio source
        audioSource.volume = originalVolume; // Store original volume
        StartCoroutine(PlayRandomTrack()); // Start playing tracks
    }

    // Coroutine to play tracks with random delay
    private IEnumerator PlayRandomTrack()
    {
        while (true)
        {
            delayTime = Random.Range(50f, 80f); // Random delay between 1-2 minutes
            yield return new WaitForSeconds(delayTime); // Wait for delay
            PlayTrack(); // Play a random track
            yield return new WaitWhile(() => audioSource.isPlaying); // Wait for track to finish
        }
    }

    // Play a random track
    private void PlayTrack()
    {
        int trackIndex = Random.Range(0, audioTracks.Length); // Random track index
        audioSource.clip = audioTracks[trackIndex]; // Set audio clip
        audioSource.Play(); // Play audio
    }

    // Public method to cancel music and reset timer
    public void CancelMusic()
    {
        audioSource.Stop(); // Stop current track
        StopAllCoroutines(); // Stop coroutine
        StartCoroutine(PlayRandomTrack()); // Restart coroutine with reset timer
    }

    // Public method to halve the volume
    public void HalveVolume()
    {
        audioSource.volume = originalVolume / 2; // Halve volume
    }

    // Public method to reset volume to original
    public void ResetVolume()
    {
        audioSource.volume = originalVolume; // Reset volume
    }
}
