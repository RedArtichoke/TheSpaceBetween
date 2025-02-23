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

    [Header("Ship objects changing")]
    public GameObject drawer1;
    public GameObject drawer2;
    public GameObject brokenDoor1;
    public GameObject brokenDoor2;

    public GameObject trash1;
    public GameObject trash2;

    public GameObject screwdriver;
    public GameObject mug;

    public FlickerLight lightsource1;

    public GameObject DoorOpens;
    public GameObject DoorDestroyed;
    public Alarm alarm;

    public GameObject wireBox1;
    public GameObject wireBox2;
    public GameObject wireBox3;

    public GameObject pipeburst1;
    public GameObject pipeburst2;

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

        drawer1.SetActive(false);
        drawer2.SetActive(true);

        brokenDoor1.SetActive(true);
        brokenDoor1.SetActive(true);

        trash1.SetActive(false);
        trash2.SetActive(true);

        screwdriver.SetActive(false);
        mug.SetActive(false);

        lightsource1.canFlicker = true;

        DoorOpens.SetActive(false);
        DoorDestroyed.SetActive(true);

        alarm.isDestroyed = true;

        wireBox1.SetActive(true);
        wireBox2.SetActive(true);
        wireBox3.SetActive(true);

        pipeburst1.SetActive(true);
        pipeburst2.SetActive(true);
    }
}
