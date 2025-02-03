using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityField : MonoBehaviour
{
    // Tags to exclude from zero gravity
    private HashSet<string> excludedTags = new HashSet<string> { "Player", "Mimic", "Thing" };

    void OnTriggerStay(Collider other)
    {
        // Check if the object is not excluded and has a Rigidbody
        if (!excludedTags.Contains(other.tag))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; 
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the object is not excluded and has a Rigidbody
        if (!excludedTags.Contains(other.tag))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true; 
            }
        }
    }
}
