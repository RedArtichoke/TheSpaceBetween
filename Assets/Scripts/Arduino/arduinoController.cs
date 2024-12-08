using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoHandler : MonoBehaviour
{

    public HeartRateAnimator heartRateScript;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMessageArrived(string msg)
    {
        Debug.Log("bpm: " + msg);
        heartRateScript.beatsPerMinute = float.Parse(msg); //this takes the message from arduino and makes it equal to BPM value

    }


}
