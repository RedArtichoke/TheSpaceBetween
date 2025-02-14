using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepair : MonoBehaviour
{
    //public bool isDestroyed;
    public int repairCount;

    public GameObject door1;
    public GameObject door2;
    public GameObject cap1;
    public GameObject powerCell;
    public GameObject steeringWheel;
    void Update()
    {
        if (repairCount == 4)
        {
            Debug.Log("SHIP REPAIRED");
        }
    }

    public void DestroyShip()
    {
        door1.SetActive(false);
        door2.SetActive(false);
        cap1.SetActive(false);
        powerCell.SetActive(false);
        steeringWheel.SetActive(false);
    }
}
