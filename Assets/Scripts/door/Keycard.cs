using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    public enum KeycardIdentity
    {
        ResearchRoom,
        MaintenanceRoom,
        MainDoor,
        NotUnlockable
    }

    public PlayerMovementController player;

    public KeycardIdentity identity;  

    private HashSet<string> unlockedDoorIDs = new HashSet<string>();
    private bool isDisabled = false;

    private static Dictionary<KeycardIdentity, int> maxUnlocksPerIdentity = new Dictionary<KeycardIdentity, int>
    {
        { KeycardIdentity.ResearchRoom, 2 },
        { KeycardIdentity.MaintenanceRoom, 1 },
        { KeycardIdentity.MainDoor, 1 }
    };

    public void RegisterDoorUnlock(string doorID)
    {
        if (isDisabled || unlockedDoorIDs.Contains(doorID)) return;

        unlockedDoorIDs.Add(doorID);

        int requiredUnlocks = maxUnlocksPerIdentity.ContainsKey(identity) ? maxUnlocksPerIdentity[identity] : 1;

        if (unlockedDoorIDs.Count >= requiredUnlocks)
        {
            DisableKeycard();
        }
    }

    private void DisableKeycard()
    {
        Debug.Log("Disabling card");
        isDisabled = true;

        player.DropObject(false);
        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.tag = "Untagged"; 

        StartCoroutine(countdownCardDestroy());
    }

    public IEnumerator countdownCardDestroy()
    {
        yield return new WaitForSeconds(8f);

        Destroy(gameObject);
    }
}
