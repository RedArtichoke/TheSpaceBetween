using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DarkController : MonoBehaviour
{
    public bool inDark = false; // Tracks if player is in dark
    private GameObject globalVolume; // Reference to Global Volume
    private Camera mainCamera; // Reference to Main Camera
    private GameObject[] mimics; // Store references to all mimics
    private GameObject[] footprints; // Store references to all footprints
    private Volume volume; // Reference to Volume component
    private ColorAdjustments colorAdjustments; // Reference to Color Adjustments
    private float targetExposure; // Target exposure value
    private float currentExposure; // Current exposure value
    private float exposureVelocity; // Velocity for SmoothDamp

    // Start is called before the first frame update
    void Start()
    {
        globalVolume = GameObject.FindGameObjectWithTag("CameraVolume");
        mainCamera = Camera.main;

        if (globalVolume.TryGetComponent<Volume>(out volume))
        {
            volume.profile.TryGet(out colorAdjustments);
            currentExposure = colorAdjustments.postExposure.value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inDark = !inDark; // Toggle inDark
            targetExposure = -7f; // Always transition to -7 first
        }

        if (colorAdjustments != null)
        {
            currentExposure = Mathf.SmoothDamp(currentExposure, targetExposure, ref exposureVelocity, 0.1f);
            colorAdjustments.postExposure.value = currentExposure;

            // Check if transition to -7 is complete, then set target to -4
            if (Mathf.Approximately(currentExposure, -7f))
            {
                targetExposure = -4f;
                ToggleMimics(inDark);
                ToggleFootprints(inDark);
            }
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
                NavMeshAgent agent = mimic.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.isStopped = true; // Stop the agent
                    mimic.SetActive(false); // Disable mimics
                }
            }
        }
        else if (mimics != null) // If not in dark, enable stored mimics
        {
            foreach (GameObject mimic in mimics)
            {
                mimic.SetActive(true); // Enable mimics
                NavMeshAgent agent = mimic.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    if (NavMesh.SamplePosition(mimic.transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position); // Ensure agent is on NavMesh
                        agent.isStopped = false; // Resume the agent
                    }
                }
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
                if (footprint != null) // Check if footprint exists
                {
                    footprint.SetActive(true); // Enable footprints
                }
            }
        }
    }
}
