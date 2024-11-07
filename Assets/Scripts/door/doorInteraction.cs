using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public float interactionRange = 5f;  
    public LayerMask doorLayer;  

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
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange, doorLayer))
            {
                doorOpen door = hit.collider.GetComponent<doorOpen>();
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }
    }
}
