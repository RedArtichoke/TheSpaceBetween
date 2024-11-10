using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentAdd : MonoBehaviour
{
    public ShipComponent.ShipComponentIdentity requiredShipComponentIdentity;
    public GameObject objectView;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShipComponent"))
        {
            ShipComponent shipComponent = other.GetComponent<ShipComponent>();
            if (shipComponent != null && shipComponent.identity == requiredShipComponentIdentity)
            {
                Destroy(shipComponent.gameObject);
                objectView.SetActive(true);
                Debug.Log("Added component");
            }
            else
            {
                Debug.Log("Incorrect component");
            }
        }
    }
}

