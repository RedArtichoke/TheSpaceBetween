using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartShipLanding : MonoBehaviour
{
    public float interactionRange = 5f;
    public LayerMask button;

    private Camera playerCamera;

    public ShipFlight shipFlight;
    

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange, button))
            {
                StartTravel();
            }
        }
    }

    public void StartTravel()
    {
        shipFlight.MoveShip();
    }
}
