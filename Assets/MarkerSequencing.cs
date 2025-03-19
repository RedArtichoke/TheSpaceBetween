using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    // Variables to track the current active marker in each sequence
    private int currentMarkerIndex = 0;
    
    // Define arrays for each sequence
    private GameObject[] plugSequenceMarkers;
    private GameObject[] shipSequenceMarkers;
    private GameObject[] deviceSequenceMarkers;
    
    // Current active marker array (points to one of the above arrays)
    private GameObject[] currentSequenceMarkers;

    // Add these variables for marker switching logic
    private float markerSwitchCooldown = 2.0f;
    private float lastMarkerSwitchTime = 0f;
    private float backtrackingThreshold = -0.5f;
    private float backtrackingRangeMultiplier = 1.5f;

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
            maintenance1Marker, // Note: Comment says maintenance3Marker but using maintenance1Marker as placeholder
            securityMarker,
            hangarMarker,
            shipMarker
        };
        
        // Initialize the device sequence markers array
        deviceSequenceMarkers = new GameObject[] 
        {
            hangarMarker,
            securityMarker,
            diensionMarker // Using this as deviceMarker
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
        
        // Only process marker changes if we're not in the cooldown period
        if (Time.time - lastMarkerSwitchTime >= markerSwitchCooldown)
        {
            // Check if the player is within range of the current marker
            float distanceToMarker = Vector3.Distance(player.transform.position, currentMarker.transform.position);
            if (distanceToMarker <= detectorRange)
            {
                // Player reached the marker, advance to the next one
                currentMarkerIndex++;
                lastMarkerSwitchTime = Time.time; // Record switch time
                
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
                }
            }
            // Handle backtracking - but only if player has gone past a previous marker
            else if (currentMarkerIndex > 0)
            {
                bool shouldSwitch = false;
                int newIndex = currentMarkerIndex;
                
                // First check if we need to go back multiple steps
                for (int i = 0; i < currentMarkerIndex - 1; i++)
                {
                    GameObject prevMarker = currentSequenceMarkers[i];
                    GameObject nextMarker = currentSequenceMarkers[i+1];
                    
                    // Skip if either marker is null
                    if (prevMarker == null || nextMarker == null)
                        continue;
                    
                    // Calculate vector from this marker to the next marker (the intended path direction)
                    Vector3 pathDirection = (nextMarker.transform.position - prevMarker.transform.position).normalized;
                    
                    // Calculate vector from this marker to the player
                    Vector3 playerDirection = (player.transform.position - prevMarker.transform.position).normalized;
                    
                    // Use a more decisive threshold for backtracking detection
                    float dotProduct = Vector3.Dot(pathDirection, playerDirection);
                    float distance = Vector3.Distance(player.transform.position, prevMarker.transform.position);
                    
                    // Only consider it a backtrack if significantly behind the marker and relatively close
                    if (dotProduct < backtrackingThreshold && distance <= detectorRange * backtrackingRangeMultiplier)
                    {
                        newIndex = i;
                        shouldSwitch = true;
                        // Don't break - continue to find the earliest marker the player has backtracked past
                    }
                }
                
                // If we didn't find a multi-step backtrack, check the immediate previous marker
                if (!shouldSwitch && currentMarkerIndex > 0)
                {
                    int i = currentMarkerIndex - 1;
                    GameObject prevMarker = currentSequenceMarkers[i];
                    GameObject nextMarker = currentSequenceMarkers[currentMarkerIndex];
                    
                    Vector3 pathDirection = (nextMarker.transform.position - prevMarker.transform.position).normalized;
                    Vector3 playerDirection = (player.transform.position - prevMarker.transform.position).normalized;
                    
                    float dotProduct = Vector3.Dot(pathDirection, playerDirection);
                    float distance = Vector3.Distance(player.transform.position, prevMarker.transform.position);
                    
                    if (dotProduct < backtrackingThreshold && distance <= detectorRange * backtrackingRangeMultiplier)
                    {
                        newIndex = i;
                        shouldSwitch = true;
                    }
                }
                
                // Only switch if needed and record the switch time
                if (shouldSwitch && newIndex != currentMarkerIndex)
                {
                    currentMarkerIndex = newIndex;
                    lastMarkerSwitchTime = Time.time;
                    hideAllMarkers();
                    currentSequenceMarkers[currentMarkerIndex].SetActive(true);
                    Debug.Log("Player backtracked to marker " + currentMarkerIndex + " in " + activeSequence + " sequence");
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
