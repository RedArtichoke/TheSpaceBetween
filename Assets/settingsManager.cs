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
}
