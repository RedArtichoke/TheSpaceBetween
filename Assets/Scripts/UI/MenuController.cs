using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public GameObject pauseMenuPrefab; // Reference to the pause menu prefab
    public GameObject UIComponents;
    public GameObject screenQuad; // Reference to the quad
    public bool isPaused = false; // Track the pause state

    void Start()
    {
        pauseMenuPrefab.SetActive(false); // Ensure the pause menu is hidden at the start
        screenQuad.SetActive(false); // Ensure the quad is hidden at the start
    }

    void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused; // Toggle the pause state

        if (isPaused)
        {
            Time.timeScale = 0f; // Freeze time
            UIComponents.SetActive(false); // Hide the UI components
            pauseMenuPrefab.SetActive(true); // Show the pause menu
            screenQuad.SetActive(true); // Show the quad
            Cursor.visible = true; // Make the cursor visible
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f; // Resume time
            UIComponents.SetActive(true); // Show the UI components
            pauseMenuPrefab.SetActive(false); // Hide the pause menu
            screenQuad.SetActive(false); // Hide the quad
            Cursor.visible = false; // Hide the cursor
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
            AudioListener.pause = false;
        }
    }
}
