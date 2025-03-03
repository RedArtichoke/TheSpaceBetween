using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public bool isBeta;

    public GameObject detectionZone;
    //public GameObject detectionZone2;

   // public doorOpen researchDoor;

    void Update()
    {
        if (isBeta)
        {
            detectionZone.SetActive(true);
           // detectionZone2.SetActive(true);
        }
        else if (!isBeta)
        {
            detectionZone.SetActive(false);
           // detectionZone2.SetActive(false);

        }
    }
}
