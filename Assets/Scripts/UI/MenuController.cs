using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public GameObject pauseMenuPrefab; // Reference to the pause menu prefab
    public GameObject UIComponents;
    public GameObject screenQuad; // Reference to the quad
    public GameObject crosshair; // Reference to the crosshair
    public IntroCutscene intro;
    public ObjectiveMarkerUI objectiveMarkers; // Reference to the objective markers UI
    public bool isPaused = false; // Track the pause state
    
    private PlayerMovementController playerMovement; // Reference to player movement controller

    [SerializeField] endgameGameInfo gameInfo;

    void Start()
    {
        pauseMenuPrefab.SetActive(false); // Ensure the pause menu is hidden at the start
        screenQuad.SetActive(false); // Ensure the quad is hidden at the start
        
        // Find objective markers if not assigned
        if (objectiveMarkers == null)
        {
            objectiveMarkers = FindObjectOfType<ObjectiveMarkerUI>();
        }
        
        // Find player movement controller
        playerMovement = FindObjectOfType<PlayerMovementController>();
    }

    void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape) && gameInfo.inGame)
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
            crosshair.SetActive(false); // Hide the crosshair
            pauseMenuPrefab.SetActive(true); // Show the pause menu
            screenQuad.SetActive(true); // Show the quad
            Cursor.visible = true; // Make the cursor visible
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            AudioListener.pause = true;
            
            // Hide objective markers
            if (objectiveMarkers != null)
            {
                objectiveMarkers.SetMarkersVisibility(false);
            }
            
            // Hide interaction prompts
            if (playerMovement != null)
            {
                playerMovement.SetInteractPromptsVisibility(false);
            }
        }
        else
        {
            Time.timeScale = 1f; // Resume time
            if(intro.introPlaying == false)
            {
                UIComponents.SetActive(true); // Show the UI components
                crosshair.SetActive(true); // Show the crosshair
                
                // Show objective markers
                if (objectiveMarkers != null)
                {
                    objectiveMarkers.SetMarkersVisibility(true);
                }
                
                // Show interaction prompts
                if (playerMovement != null)
                {
                    playerMovement.SetInteractPromptsVisibility(true);
                }
            }
            pauseMenuPrefab.SetActive(false); // Hide the pause menu
            screenQuad.SetActive(false); // Hide the quad
            Cursor.visible = false; // Hide the cursor
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
            AudioListener.pause = false;
        }
    }
}
