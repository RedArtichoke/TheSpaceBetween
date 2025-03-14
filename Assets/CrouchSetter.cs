using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchSetter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Try to get the PlayerMovementController from the colliding object
        PlayerMovementController playerMovement = GetPlayerMovementController(other.gameObject);
        
        // If the component exists, set canStand to false
        if (playerMovement != null)
        {
            playerMovement.canStand = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Try to get the PlayerMovementController from the colliding object
        PlayerMovementController playerMovement = GetPlayerMovementController(other.gameObject);
        
        // If the component exists, set canStand to true
        if (playerMovement != null)
        {
            playerMovement.canStand = true;
        }
    }
    
    // Helper method to find PlayerMovementController on object, its parent, or its "Player" child
    private PlayerMovementController GetPlayerMovementController(GameObject obj)
    {
        // First check if the component is on the object itself
        PlayerMovementController controller = obj.GetComponent<PlayerMovementController>();
        
        if (controller != null)
        {
            return controller;
        }
        
        // If the object is named "Hitbox", check its parent
        if (obj.name == "hitbox" && obj.transform.parent != null)
        {
            controller = obj.transform.parent.GetComponent<PlayerMovementController>();
            if (controller != null)
            {
                return controller;
            }
        }
        
        // If not found, look for a child named "Player"
        Transform playerChild = obj.transform.Find("Player");
        
        if (playerChild != null)
        {
            controller = playerChild.GetComponent<PlayerMovementController>();
            if (controller != null)
            {
                return controller;
            }
        }
        
        return null;
    }
}
