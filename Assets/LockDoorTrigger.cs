using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDoorTrigger : MonoBehaviour
{
    public doorOpen door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.SetLockState(true);
            Destroy(gameObject);
        }
    }
}
