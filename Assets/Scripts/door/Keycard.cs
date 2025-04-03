using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    public enum KeycardIdentity
    {
        ResearchRoom,
        MaintenanceRoom,
        MainDoor
    }

    private int doorsUnlocked = 0;
    private bool isDisabled = false;

    public KeycardIdentity identity;  

    public void RegisterDoorUnlock()
    {
        if (isDisabled) return;

        doorsUnlocked++;

        if (doorsUnlocked >= 2)
        {
            DisableKeycard();
        }
    }

    private void DisableKeycard()
    {
        Debug.Log("Keycard has unlocked 2 doors. Disabling...");
        isDisabled = true;
        gameObject.SetActive(false); 
    }
}
