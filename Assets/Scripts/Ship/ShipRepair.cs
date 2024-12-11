using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepair : MonoBehaviour
{
    public int repairCount;

    public doorOpen Door;

    void Update()
    {
        if (repairCount == 3)
        {
            Debug.Log("SHIP REPAIRED");
            Door.SetLockState(false);
        }
    }
}
