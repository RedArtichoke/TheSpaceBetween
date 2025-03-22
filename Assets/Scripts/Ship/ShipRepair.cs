using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public GameObject alarmAudio;
    public GameObject sparksAudio;

    [Header("Transparent Ship Objects")]
    public GameObject transWheel;
    public GameObject transCell;
    public GameObject deadButton;
    public GameObject buttonTakeOff;
    public TextMeshProUGUI collectedText;

    void Update()
    {
        if(repairCount == 1)
        {
            collectedText.text = "Ship Parts repaired: 1/4";
        }
        if(repairCount == 2)
        {
            collectedText.text = "Ship Parts repaired: 2/4";
        }
        if(repairCount == 3)
        {
            collectedText.text = "Ship Parts repaired: 3/4";
        }
        if (repairCount == 4)
        {
            collectedText.text = "Ship Parts repaired: 4/4";
            Debug.Log("SHIP REPAIRED");

            // START END CUTSCENE
             EndGame();
        }
        
    }

    public void DestroyShip()
    {
        door1.SetActive(false);
        door2.SetActive(false);
        cap1.SetActive(false);

        alarmAudio.SetActive(true);

        sparksAudio.SetActive(true);

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
        
        transWheel.SetActive(true);
        transCell.SetActive(true);
    }

    public void EndGame()
    {
        deadButton.SetActive(false);
        buttonTakeOff.SetActive(true);
    }

}
