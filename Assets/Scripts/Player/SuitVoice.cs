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

    private bool hasPlayed75 = false;
    private bool hasPlayed50 = false;
    private bool hasPlayed25 = false;
    private bool hasPlayed10 = false;

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
    
    public void playDamageAudio()
    {
        suitVoice.clip = criticalDamageTaken;
        suitVoice.Play();
    }

    public void playRestoreAudio()
    {
        suitVoice.clip = medicalRestore;
        suitVoice.Play();
    }

    public void PlayPowerRestoreAudio()
    {
        suitVoice.clip = powerRestored;
        suitVoice.Play();
    }

    public void PlayConditionStabilizedAudio()
    {
        suitVoice.clip = conStable;
        suitVoice.Play();
    }
}
