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

            // Consume the Escape key event to prevent default behaviour
            Event e = Event.current;
            if (e != null && e.isKey && e.keyCode == KeyCode.Escape)
            {
                e.Use();
            }
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
        }
        else
        {
            Time.timeScale = 1f; // Resume time
            pauseMenuPrefab.SetActive(false); // Hide the pause menu
            Cursor.visible = false; // Hide the cursor
        }
    }
}
