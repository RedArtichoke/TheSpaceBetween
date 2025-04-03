using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MarkerSequencing : MonoBehaviour
{
    public GameObject player;
    public GameObject hangarMarker;
    public GameObject securityMarker;
    public GameObject maintenance1Marker;
    public GameObject maintenance2Marker;
    public GameObject plugMarker;
    public GameObject keycard1Marker;
    public GameObject shipMarker;
    public GameObject keycard2Marker;
    public GameObject diensionMarker;
    private bool markerSystemEnabled = false;

    public float detectorRange = 5f;
    
    // Enum to identify which sequence is active
    public enum SequenceType
    {
        None,
        Plug,
        Ship,
        Device
    }
    
    // Track which sequence is currently active
    private SequenceType activeSequence = SequenceType.None;
    
    // Public property to access the active sequence
    public SequenceType ActiveSequence => activeSequence;
    
    // Variables to track the current active marker in each sequence
    private int currentMarkerIndex = 0;
    
    // Define arrays for each sequence
    private GameObject[] plugSequenceMarkers;
    private GameObject[] shipSequenceMarkers;
    private GameObject[] deviceSequenceMarkers;
    
    // Current active marker array (points to one of the above arrays)
    private GameObject[] currentSequenceMarkers;

    // Add text component references
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI instructionsText;

    // Start is called before the first frame update
    void Start()
    {
        // Check for null markers and log warnings
        if (hangarMarker == null) Debug.LogWarning("hangarMarker is null!");
        if (securityMarker == null) Debug.LogWarning("securityMarker is null!");
        if (maintenance1Marker == null) Debug.LogWarning("maintenance1Marker is null!");
        if (maintenance2Marker == null) Debug.LogWarning("maintenance2Marker is null!");
        if (plugMarker == null) Debug.LogWarning("plugMarker is null!");
        if (keycard1Marker == null) Debug.LogWarning("keycard1Marker is null!");
        if (keycard2Marker == null) Debug.LogWarning("keycard2Marker is null!");
        if (shipMarker == null) Debug.LogWarning("shipMarker is null!");
        if (diensionMarker == null) Debug.LogWarning("diensionMarker is null!");
        
        hideAllMarkers();
        
        // Initialize the plug sequence markers array
        plugSequenceMarkers = new GameObject[] 
        {
            hangarMarker,
            securityMarker,
            maintenance1Marker, 
            maintenance2Marker,
            plugMarker
        };
        
        // Initialize the ship sequence markers array
        shipSequenceMarkers = new GameObject[] 
        {
            keycard2Marker,
            maintenance2Marker,
            maintenance1Marker,
            securityMarker,
            hangarMarker,
            shipMarker
        };
        
        // Initialize the device sequence markers array
        deviceSequenceMarkers = new GameObject[] 
        {
            hangarMarker,
            securityMarker,
            diensionMarker
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (markerSystemEnabled && activeSequence != SequenceType.None)
        {
            // Process the active sequence
            ProcessMarkerSequence();
        }

        /* Plug Sequence:
        1. Show hangarMarker
        2. Show securityMarker
        3. Show maintenance1Marker
        4. Show maintenance2Marker
        5. Show plugMarker
        */

        /* Ship Sequence:
        1. Show keycard2Marker
        2. Show maintenance2Marker
        3. Show maintenance3Marker
        4. Show securityMarker
        5. Show hangarMarker
        6. Show shipMarker
        */

        /* Device Sequence:
        1. Show hangarMarker
        2. Show securityMarker
        3. Show deviceMarker
        */
    }

    // Public methods to switch between sequences
    public void EnablePlugSequence()
    {
        // Switch to the plug sequence
        activeSequence = SequenceType.Plug;
        currentSequenceMarkers = plugSequenceMarkers;
        StartSequence();
    }
    
    public void EnableShipSequence()
    {
        // Switch to the ship sequence
        activeSequence = SequenceType.Ship;
        currentSequenceMarkers = shipSequenceMarkers;
        StartSequence();
    }
    
    public void EnableDeviceSequence()
    {
        // Switch to the device sequence
        activeSequence = SequenceType.Device;
        currentSequenceMarkers = deviceSequenceMarkers;
        
        // Update the text components
        if (objectiveText != null)
            objectiveText.text = "Repair your Ship";
        if (instructionsText != null)
            instructionsText.text = "Ship Parts found 0/4";
            
        StartSequence();
    }
    
    private void StartSequence()
    {
        markerSystemEnabled = true;
        currentMarkerIndex = 0;
        hideAllMarkers();
        
        // Show the first marker in the sequence with null checks
        if (currentSequenceMarkers != null && currentSequenceMarkers.Length > 0)
        {
            GameObject firstMarker = currentSequenceMarkers[0];
            if (firstMarker != null)
                firstMarker.SetActive(true);
            else
                Debug.LogWarning("First marker in sequence is null! Check your marker references.");
        }
    }

    public void enableMarkerSystem()
    {
        // For backward compatibility - starts the plug sequence
        EnablePlugSequence();
    }

    public void hideAllMarkers()
    {
        // Add null checks before accessing each marker
        if (hangarMarker != null) hangarMarker.SetActive(false);
        if (securityMarker != null) securityMarker.SetActive(false);
        if (maintenance1Marker != null) maintenance1Marker.SetActive(false);
        if (maintenance2Marker != null) maintenance2Marker.SetActive(false);
        if (plugMarker != null) plugMarker.SetActive(false);
        if (keycard1Marker != null) keycard1Marker.SetActive(false);
        if (keycard2Marker != null) keycard2Marker.SetActive(false);
        if (shipMarker != null) shipMarker.SetActive(false);
        if (diensionMarker != null) diensionMarker.SetActive(false);
    }

    public void showShipMarker(bool show)
    {
        shipMarker.SetActive(show);
    }

    private void ProcessMarkerSequence()
    {
        // Early safety check for null current sequence array
        if (currentSequenceMarkers == null || currentMarkerIndex >= currentSequenceMarkers.Length)
            return;
        
        // Get the current active marker with a null check
        GameObject currentMarker = currentSequenceMarkers[currentMarkerIndex];
        if (currentMarker == null)
        {
            // Skip this destroyed marker and move to the next one if possible
            Debug.LogWarning("Marker at index " + currentMarkerIndex + " is null/destroyed. Skipping.");
            currentMarkerIndex++;
            if (currentMarkerIndex < currentSequenceMarkers.Length)
            {
                hideAllMarkers();
                GameObject nextMarker = currentSequenceMarkers[currentMarkerIndex];
                if (nextMarker != null)
                    nextMarker.SetActive(true);
            }
            return;
        }
        
        // Make sure the current marker is visible
        if (!currentMarker.activeSelf)
        {
            hideAllMarkers();
            currentMarker.SetActive(true);
        }
        
        // Check if the player is within range of the current marker
        float distanceToMarker = Vector3.Distance(player.transform.position, currentMarker.transform.position);
        if (distanceToMarker <= detectorRange)
        {
            // Player reached the marker, advance to the next one
            currentMarkerIndex++;
            
            // If there's a next marker, show it
            if (currentMarkerIndex < currentSequenceMarkers.Length)
            {
                hideAllMarkers();
                currentSequenceMarkers[currentMarkerIndex].SetActive(true);
            }
            else
            {
                // We completed the sequence
                hideAllMarkers();
                Debug.Log(activeSequence.ToString() + " sequence completed!");
                
                // If we just completed the Ship sequence, start the Device sequence
                if (activeSequence == SequenceType.Ship)
                {
                    Debug.Log("Ship sequence completed! Starting Device sequence...");
                    EnableDeviceSequence();
                }
            }
        }
    }
    
    // Method to disable the marker system completely
    public void DisableMarkerSystem()
    {
        markerSystemEnabled = false;
        activeSequence = SequenceType.None;
        hideAllMarkers();
    }
}
