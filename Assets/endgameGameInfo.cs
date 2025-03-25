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

    [SerializeField] float startTime; //when the player is able to leave the ship
    [SerializeField] float playTime; //time in seconds?

    //heartrate animator is not in endscene
    [SerializeField] int highRate;
    [SerializeField] int lowRate; 
    
    [SerializeField] HeartRateAnimator heartinfo;

    [SerializeField] GameObject endgameCanvas;
    [SerializeField] TextMeshProUGUI endgameText;

    // Start is called before the first frame update
    void Start()
    {
        inGame = false;

        startTime = 0;
        playTime = 0;
        yogurtCollected = 0;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // ** TESTING FEATURE, TO BE REMOVED **
        if (Input.GetKeyDown(KeyCode.B))
        {
            stopTimer();
            SceneManager.LoadScene(2);
        }
    }

    public void displayData()
    {
        endgameCanvas = GameObject.Find("Stats");
        endgameText = endgameCanvas.GetComponent<TextMeshProUGUI>();

        endgameText.text =  "Time Played - " + playTime +
                            "\n\nHighest Heart Rate - " + highRate + " BPM" +
                            "\n\nLowest Heart Rate - " + lowRate + " BPM" +
                            "\n\nYogurt Cups Collected - " + yogurtCollected + "/10";
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
            playTime = Time.timeSinceLevelLoad - startTime;

            highRate = (int)heartinfo.highestHeartRate;
            lowRate = (int)heartinfo.lowestHeartRate;
        }
    }
}
