using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarDoorCloser : MonoBehaviour
{
    public HangarDoorController hangarDoor;

    public AudioSource audioSource;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Ship"))
        {
            audioSource.Play();
            hangarDoor.CloseHangarDoors();
        }
    }
}

