using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyShipTrigger : MonoBehaviour
{
    public ShipRepair ship;

    public GameObject triggerZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ship.DestroyShip();
            triggerZone.SetActive(true);
            Destroy(gameObject);
        }
    }
}
