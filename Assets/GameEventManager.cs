using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public bool isBeta;

    public GameObject detectionZone;

    void Update()
    {
        if (detectionZone != null) 
        {
            detectionZone.SetActive(isBeta);
        }
    }
}
