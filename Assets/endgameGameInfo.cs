using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class endgameGameInfo : MonoBehaviour
{
    public int yogurtCollected;
    public AudioSource collectSound;

    bool inGame;

    public int deaths;
    public int shifterUse;

    [SerializeField] float startTime; //when the player is able to leave the ship
    [SerializeField] int playSec; //time in seconds?
    [SerializeField] int playMin; //time in seconds?
    [SerializeField] string playTime; //time in seconds?
    

    //heartrate animator is not in endscene
    [SerializeField] int highRate;
    [SerializeField] int lowRate; 
    
    [SerializeField] HeartRateAnimator heartinfo;

    [SerializeField] GameObject endgameCanvas;
    [SerializeField] TextMeshProUGUI endgameText;

    [SerializeField] IntroCutscene introFunction;

    // Start is called before the first frame update
    void Start()
    {
        inGame = false;

        startTime = 0;
        playSec = 0;
        playMin = 0;
        playTime = "";
        yogurtCollected = 0;
        deaths = 0;
        shifterUse = 0;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(introFunction.gameObject);
    }

    public void displayData()
    {
        endgameCanvas = GameObject.Find("Stats");
        endgameText = endgameCanvas.GetComponent<TextMeshProUGUI>();

        /*endgameText.text =    "Time Played - " + playTime +
                                "\r\nDeath Count - " + deaths +
                                "\r\nDimension Shifter Used " + shifterUse + " Times" +
                                "\r\nHighest Heart Rate - " + highRate + " BPM" +
                                "\r\nLowest Heart Rate - " + lowRate + " BPM" +
                                "\r\nYogurt Cups Collected - " + yogurtCollected + "/10\n" */

        StartCoroutine(introFunction.TypeText(endgameText, 
                                                            "Time Played - " + playTime +
                                                            "\r\nDeath Count - " + deaths +
                                                            "\r\nDimension Shifter Used " + shifterUse + " Times" +
                                                            "\r\nHighest Heart Rate - " + highRate + " BPM" +
                                                            "\r\nLowest Heart Rate - " + lowRate + " BPM" +
                                                            "\r\nYogurt Cups Collected - " + yogurtCollected + "/10\n", 
            0.05f, 0.02f, true));
    }

    public void startTimer()
    {
        if(!inGame)
        {
            //The time from the intro to when the player can actually start playing the game
            startTime = Time.timeSinceLevelLoad;
            inGame = true;
        }
        else
        {
            Debug.Log("Timer has already been started");
        }
        
    }

    public void stopTimer()
    {
        if (inGame)
        {
            //the time from starting to play to beating the game
            playSec = (int)(Time.timeSinceLevelLoad - startTime); //example: 125 second playthrough = 125 playSec

            playMin = playSec / 60; // 125/60 seconds = 2 playMin and 5 playSec
            playTime = playMin + ":" + (playSec%60); // 125 seconds should read as 2:05

            highRate = (int)heartinfo.highestHeartRate;
            lowRate = (int)heartinfo.lowestHeartRate;
        }
    }
}
