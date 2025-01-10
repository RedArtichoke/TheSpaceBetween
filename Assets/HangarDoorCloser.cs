using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarDoorCloser : MonoBehaviour
{
    public HangarDoorController hangarDoor;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Ship"))
        {
            hangarDoor.CloseHangarDoors();
        }
    }
}

