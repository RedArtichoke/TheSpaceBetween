using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI components
using TMPro; // For TextMeshPro

public class settingsManager : MonoBehaviour
{
    private FpsCameraController cameraController;
    public Slider sensitivitySlider; // Reference to the UI slider
    public TextMeshProUGUI sensitivityText; // Reference to display the value

    // Toggle references
    public Toggle toggleCrouch;
    public Toggle toggleHighlight;
    public Toggle toggleHUD;
    public Toggle toggleCrossHair;

    // HUD component references
    public GameObject crosshair;
    public GameObject uiComponents;

    // Start is called before the first frame update
    void Start()
    {
        // Find the camera controller in the scene
        cameraController = FindObjectOfType<FpsCameraController>();
        
        // Set up the slider's value changed event
        if (sensitivitySlider != null && cameraController != null)
        {
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            
            // Calculate initial normalized value (reverse mapping from sensitivity to 0-1 range)
            float currentSensitivity = cameraController.mouseSensitivity;
            float normalizedValue = Mathf.InverseLerp(0.1f * cameraController.baseSensitivity, 
                                                    2.0f * cameraController.baseSensitivity, 
                                                    currentSensitivity);
            
            // Set slider value and update display
            sensitivitySlider.value = normalizedValue;
            UpdateSensitivityDisplay(currentSensitivity);
        }

        // Set up toggle event listeners
        if (toggleCrouch != null) toggleCrouch.onValueChanged.AddListener(OnToggleCrouchChanged);
        if (toggleHighlight != null) 
        {
            toggleHighlight.onValueChanged.AddListener(OnHighlightToggleChanged);
            toggleHighlight.isOn = true; // Set highlight toggle on by default
        }
        if (toggleHUD != null) 
        {
            toggleHUD.onValueChanged.AddListener(OnToggleHUDChange);
            toggleHUD.isOn = false; // Set HUD toggle off by default
        }
        if (toggleCrossHair != null)
        {
            toggleCrossHair.onValueChanged.AddListener(OnToggleCrossHairChanged);
            toggleCrossHair.isOn = false; // Set crosshair toggle off by default
        }
    }

    // Called when the slider value changes
    private void OnSensitivityChanged(float value)
    {
        if (cameraController != null)
        {
            float sensitivity = cameraController.SetSensitivity(value);
            UpdateSensitivityDisplay(sensitivity);
        }
    }

    // Updates the sensitivity display text
    private void UpdateSensitivityDisplay(float value)
    {
        if (sensitivityText != null)
        {
            sensitivityText.text = value.ToString("F1"); // Format to 1 decimal place
        }
    }

    // Toggle event handlers
    private void OnToggleCrouchChanged(bool isOn)
    {
        // Find the player movement controller and update its crouch mode
        PlayerMovementController playerMovement = FindObjectOfType<PlayerMovementController>();
        if (playerMovement != null)
        {
            playerMovement.SetCrouchToggleMode(isOn);
        }
    }

    private void OnHighlightToggleChanged(bool isOn)
    {
        // Find the player movement controller and update its highlight state
        PlayerMovementController playerMovement = FindObjectOfType<PlayerMovementController>();
        if (playerMovement != null)
        {
            playerMovement.SetHighlightEnabled(isOn);
        }
    }

    private void OnToggleHUDChange(bool isOn)
    {
        if (uiComponents != null)
        {
            Canvas[] uiCanvases = uiComponents.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas canvas in uiCanvases)
            {
                canvas.enabled = !isOn;
            }
        }
    }

    private void OnToggleCrossHairChanged(bool isOn)
    {
        // Toggle visibility of all canvas renderers in crosshair and uiComponents
        if (crosshair != null)
        {
            Image[] crosshairImages = crosshair.GetComponentsInChildren<Image>(true);
            foreach (Image image in crosshairImages)
            {
                image.enabled = !isOn;
            }
        }
    }   
}
