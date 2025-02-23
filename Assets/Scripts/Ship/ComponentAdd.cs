using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentAdd : MonoBehaviour
{
    public ShipComponent.ShipComponentIdentity requiredShipComponentIdentity;
    public GameObject objectView;
    public GameObject transView;

    public ShipRepair shipRepair;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KeyItem"))
        {
            ShipComponent shipComponent = other.GetComponent<ShipComponent>();
            if (shipComponent != null && shipComponent.identity == requiredShipComponentIdentity)
            {
                transView.SetActive(false);
                Destroy(shipComponent.gameObject);
                objectView.SetActive(true);
                shipRepair.repairCount++;
                Debug.Log("Added component");
            }
            else
            {
                Debug.Log("Incorrect component");
            }
        }
    }
}

