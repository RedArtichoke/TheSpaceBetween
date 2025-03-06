using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class endgameGameInfo : MonoBehaviour
{
    public int yogurtCollected;

    [SerializeField] HeartRateAnimator heartinfo;

    [SerializeField] float allBPM;      //used for "averageBPM" calculation
    [SerializeField] float averageBPM;  //Average heart rate throughout the playthrough
    [SerializeField] float peakBPM;     //highest heart rate throughout the playthrough

    public float between;

    // Start is called before the first frame update
    void Start()
    {
        allBPM = 0.0f;
        averageBPM = 0.0f;
        peakBPM = 0.0f;

        between = 0;

        yogurtCollected = 0;
    }
    /*
    // Update is called once per frame
    void Update()
    {
        if(between > 1)
        {
            between = 0;

            allBPM += heartinfo.beatsPerMinute;

            averageBPM = allBPM / Time.fixedTime;

            //if current heart rate is higher than the previous peak
            if (heartinfo.beatsPerMinute > peakBPM)
            {
                peakBPM = heartinfo.beatsPerMinute;
            }
        }
        between += Time.deltaTime;
    }
    */
}
