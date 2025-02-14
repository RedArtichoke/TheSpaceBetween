using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyShipTrigger : MonoBehaviour
{
    public ShipRepair ship;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ship.DestroyShip();
            Destroy(gameObject);
        }
    }
}
