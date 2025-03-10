using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DetectMaintenancePuzzle : MonoBehaviour
{
    public GameObject evacuateAudio1;
    public GameObject evacuateAudio2;
    public GameObject powerUpaudio;
    public GameObject light1;
    public GameObject light2;
    public GameObject light3;
    public GameObject light4;

    public FlickerLight flickerLight;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            evacuateAudio1.SetActive(true);
            evacuateAudio2.SetActive(true);
            powerUpaudio.SetActive(true);

            light1.SetActive(true);
            light2.SetActive(true);
            light3.SetActive(true);
            light4.SetActive(true);
            
            flickerLight.canFlicker = false;
        }
            
    }
}
