using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public GameObject pauseMenuPrefab; // Reference to the pause menu prefab
    private bool isPaused = false; // Track the pause state

    void Start()
    {
        pauseMenuPrefab.SetActive(false); // Ensure the pause menu is hidden at the start
    }

    void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused; // Toggle the pause state

        if (isPaused)
        {
            Time.timeScale = 0f; // Freeze time
            pauseMenuPrefab.SetActive(true); // Show the pause menu
            Cursor.visible = true; // Make the cursor visible
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f; // Resume time
            pauseMenuPrefab.SetActive(false); // Hide the pause menu
            Cursor.visible = false; // Hide the cursor
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
            AudioListener.pause = false;
        }
    }
}
