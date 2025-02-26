using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SuitVoice : MonoBehaviour
{
    public PowerController powerController;
    public AudioSource suitVoice;
    [Header("Power")]
    public AudioClip power75;
    public AudioClip power50;
    public AudioClip power25;
    public AudioClip power10;

    public AudioClip powerRestored;

    [Header("Health")]
    public AudioClip criticalDamageTaken;
    public AudioClip medicalRestore;
    public AudioClip conStable;
    public AudioClip suitEquip;

    public AudioClip suitInstall;

    public AudioClip hostile;

    private bool hasPlayed75 = false;
    private bool hasPlayed50 = false;
    private bool hasPlayed25 = false;
    private bool hasPlayed10 = false;

    [Header("Effects")]
    public GameObject steam1;
    public GameObject steam2;

    private bgMusicPlayer musicPlayer;

    public IntroCutscene intro;

    void Start()
    {
        steam1.SetActive(false);
        steam2.SetActive(false);

        // Find the GameObject with the bgMusicPlayer component
        GameObject musicPlayerObject = GameObject.Find("Main Camera");
        if (musicPlayerObject != null)
        {
            musicPlayer = musicPlayerObject.GetComponent<bgMusicPlayer>();
        }
    }

    void Update()
    {
        //75% WARNING
        if (Mathf.FloorToInt(powerController.power) == 75 && !hasPlayed75)
        {
            suitVoice.clip = power75;
            suitVoice.Play();
            hasPlayed75 = true;
        }   
        // Reset the flag when the power is no longer 75 so it can be played again later if needed
        else if (Mathf.FloorToInt(powerController.power) != 75)
        {
            hasPlayed75 = false;
        }

        //50% WARNING
        if (Mathf.FloorToInt(powerController.power) == 50 && !hasPlayed75)
        {
            suitVoice.clip = power50;
            suitVoice.Play();
            hasPlayed50 = true;
        }   
        else if (Mathf.FloorToInt(powerController.power) != 50)
        {
            hasPlayed50 = false;
        }

        //25% WARNING
        if (Mathf.FloorToInt(powerController.power) == 25 && !hasPlayed75)
        {
            suitVoice.clip = power25;
            suitVoice.Play();
            hasPlayed25 = true;
        }   
        else if (Mathf.FloorToInt(powerController.power) != 25)
        {
            hasPlayed25 = false;
        }

        //10% WARNING
        if (Mathf.FloorToInt(powerController.power) == 10 && !hasPlayed75)
        {
            suitVoice.clip = power10;
            suitVoice.Play();
            hasPlayed10 = true;
        }   
        else if (Mathf.FloorToInt(powerController.power) != 10)
        {
            hasPlayed10 = false;
        }
    }
    
    private void PlayAudioWithVolumeControl(AudioClip clip)
    {
        if (musicPlayer != null)
        {
            musicPlayer.HalveVolume(); // Halve the volume before playing
        }

        suitVoice.clip = clip;
        suitVoice.Play();

        StartCoroutine(ResetVolumeAfterPlayback());
    }

    private IEnumerator ResetVolumeAfterPlayback()
    {
        yield return new WaitWhile(() => suitVoice.isPlaying); // Wait for audio to finish

        if (musicPlayer != null)
        {
            musicPlayer.ResetVolume(); // Reset volume after playback
        }
    }

    public void playDamageAudio()
    {
        PlayAudioWithVolumeControl(criticalDamageTaken);
    }

    public void playRestoreAudio()
    {
        PlayAudioWithVolumeControl(medicalRestore);
        EnableSuitSteamEffect();
    }

    public void PlayPowerRestoreAudio()
    {
        PlayAudioWithVolumeControl(powerRestored);
    }

    public void PlayConditionStabilizedAudio()
    {
        PlayAudioWithVolumeControl(conStable);
    }

    public void PlaySuitEquipAudio()
    {
        PlayAudioWithVolumeControl(suitEquip);

        StartCoroutine(CheckIfAudioFinished());
    }

    public IEnumerator CheckIfAudioFinished()
    {
        while (suitVoice.isPlaying)
        {
            yield return null;
        }

        intro.PlayFlashlightAudio();

    }

    public void PlaySuitInstallAudio()
    {
        PlayAudioWithVolumeControl(suitInstall);
    }

    public void PlayHostileDetected()
    {
        PlayAudioWithVolumeControl(hostile);
    }

    public void EnableSuitSteamEffect()
    {
        StartCoroutine(SteamEffect());
    }

    public IEnumerator SteamEffect()
    {
        yield return new WaitForSeconds(3f);

        steam1.SetActive(true);
        steam2.SetActive(true);

        yield return new WaitForSeconds(4f);

        steam1.SetActive(false);
        steam2.SetActive(false);
    }
}
