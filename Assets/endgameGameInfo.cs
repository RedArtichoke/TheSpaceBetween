using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class endgameGameInfo : MonoBehaviour
{
    public int yogurtCollected;
    public AudioSource collectSound;

    bool alreadyCalled;
    [SerializeField] float startTime; //when the player is able to leave the ship
    [SerializeField] float playTime; //time in seconds?
    
    [SerializeField] HeartRateAnimator heartinfo;

    [SerializeField] GameObject endgameCanvas;
    [SerializeField] TextMeshProUGUI endgameScript;

    // Start is called before the first frame update
    void Start()
    {
        alreadyCalled = false;

        startTime = 0;
        playTime = 0;
        yogurtCollected = 0;
    }
    
    void displayData()
    {
        endgameScript.text =    "Time Played: " + playTime +
                                "\nHighest Heart Rate: " + heartinfo.highestHeartRate +
                                "\nLowest Heart Rate: " + heartinfo.lowestHeartRate +
                                "\nYogurt Cups Collected: " + yogurtCollected;
    }

    public void startTimer()
    {
        if(!alreadyCalled)
        {
            //The time from the intro to when the player can actually start playing the game
            startTime = Time.timeSinceLevelLoad;
            alreadyCalled = true;
        }
        else
        {
            Debug.Log("Timer has already been started");
        }
        
    }

    public void stopTimer()
    {
        //the time from starting to play to beating the game
        playTime = Time.timeSinceLevelLoad - startTime;
    }
}
