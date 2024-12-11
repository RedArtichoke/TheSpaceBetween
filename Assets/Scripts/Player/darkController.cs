using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkController : MonoBehaviour
{
    public bool inDark = false; // Tracks if player is in dark
    private GameObject globalVolume; // Reference to Global Volume
    private Camera mainCamera; // Reference to Main Camera
    private GameObject[] mimics; // Store references to all mimics
    private GameObject[] footprints; // Store references to all footprints

    // Start is called before the first frame update
    void Start()
    {
        globalVolume = GameObject.Find("Global Volume");
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inDark = !inDark; // Toggle inDark
            ToggleMimics(inDark);
            ToggleFootprints(inDark);
        }
    }

    // Toggles Mimic enemies based on inDark state
    void ToggleMimics(bool state)
    {
        if (state) // If in dark, store references and disable mimics
        {
            mimics = GameObject.FindGameObjectsWithTag("Mimic");
            foreach (GameObject mimic in mimics)
            {
                mimic.SetActive(false); // Disable mimics
            }
        }
        else if (mimics != null) // If not in dark, enable stored mimics
        {
            foreach (GameObject mimic in mimics)
            {
                mimic.SetActive(true); // Enable mimics
            }
        }
    }

    // Toggles Footprint objects based on inDark state
    void ToggleFootprints(bool state)
    {
        if (state) // If in dark, store references and disable footprints
        {
            footprints = GameObject.FindGameObjectsWithTag("FootPrint");
            foreach (GameObject footprint in footprints)
            {
                footprint.SetActive(false); // Disable footprints
            }
        }
        else if (footprints != null) // If not in dark, enable stored footprints
        {
            foreach (GameObject footprint in footprints)
            {
                footprint.SetActive(true); // Enable footprints
            }
        }
    }
}
