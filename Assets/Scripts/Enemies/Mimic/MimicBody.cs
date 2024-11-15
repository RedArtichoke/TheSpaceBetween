using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicBody : MonoBehaviour
{
    // Determines visibility of the object
    public bool isVisible = false;

    void Start()
    {
        UpdateVisibility();
    }

    // Updates the visibility of the object
    public void UpdateVisibility()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = isVisible;
        }
    }
}
