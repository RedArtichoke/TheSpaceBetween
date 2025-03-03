using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    public bool isDestroyed = false;
    public float rotationSpeed = 90f;

    public GameObject light1;
    public GameObject light2;

    public GameObject alarmAudio;

    void Start()
    {
        light1.SetActive(false);
        light2.SetActive(false);
    }

    void Update()
    {
        if(isDestroyed)
        {
            light1.SetActive(true);
            light2.SetActive(true);
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            alarmAudio.SetActive(true);
        }
        
    }
}
