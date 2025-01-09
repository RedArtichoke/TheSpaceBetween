using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipDoorOpen : MonoBehaviour
{
    public float interactionRange = 5f;  
    public LayerMask shipDoorLayer;  

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange, shipDoorLayer))
            {
                rotatingDoorOpen door = hit.collider.GetComponent<rotatingDoorOpen>();
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }
    }
}
