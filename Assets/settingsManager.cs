using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI components
using TMPro; // For TextMeshPro
using UnityEngine.Audio; // For audio mixing

public class settingsManager : MonoBehaviour
{
    private FpsCameraController cameraController;
    public Slider sensitivitySlider; // Reference to the UI slider
    public TextMeshProUGUI sensitivityText; // Reference to display the value

    // Audio slider references
    public Slider enemySlider;
    public Slider globalSlider;
    public Slider musicSlider;
    public Slider heartBeatSlider;
    public TextMeshProUGUI enemyVolumeText;
    public TextMeshProUGUI globalVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI heartBeatVolumeText;

    // Layer mask for enemy audio
    private LayerMask enemyLayer;

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

        // Set up enemy layer mask
        enemyLayer = LayerMask.GetMask("Enemy");

        // Set up audio slider event listeners and initialize values
        if (enemySlider != null)
        {
            enemySlider.onValueChanged.AddListener(OnEnemyVolumeChanged);
            // Find first enemy audio source to get current volume
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource source in allAudioSources)
            {
                if (enemyLayer == (enemyLayer | (1 << source.gameObject.layer)))
                {
                    enemySlider.value = source.volume;
                    UpdateVolumeDisplay(enemyVolumeText, source.volume);
                    break;
                }
            }
        }

        if (globalSlider != null)
        {
            globalSlider.onValueChanged.AddListener(OnGlobalVolumeChanged);
            // Find first non-special audio source to get current volume
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource source in allAudioSources)
            {
                if (source.gameObject.layer != LayerMask.NameToLayer("Enemy") &&
                    source.GetComponent<bgMusicPlayer>() == null &&
                    source.GetComponent<HeartRateAnimator>() == null)
                {
                    globalSlider.value = source.volume;
                    UpdateVolumeDisplay(globalVolumeText, source.volume);
                    break;
                }
            }
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            bgMusicPlayer musicPlayer = FindObjectOfType<bgMusicPlayer>();
            if (musicPlayer != null)
            {
                AudioSource musicSource = musicPlayer.GetComponent<AudioSource>();
                if (musicSource != null)
                {
                    musicSlider.value = musicSource.volume;
                    UpdateVolumeDisplay(musicVolumeText, musicSource.volume);
                }
            }
        }

        if (heartBeatSlider != null)
        {
            heartBeatSlider.onValueChanged.AddListener(OnHeartBeatVolumeChanged);
            HeartRateAnimator heartRateAnimator = FindObjectOfType<HeartRateAnimator>();
            if (heartRateAnimator != null)
            {
                AudioSource heartBeatSource = heartRateAnimator.GetComponent<AudioSource>();
                if (heartBeatSource != null)
                {
                    heartBeatSlider.value = heartBeatSource.volume;
                    UpdateVolumeDisplay(heartBeatVolumeText, heartBeatSource.volume);
                }
                else
                {
                    // Set a default volume if source isn't available yet
                    heartBeatSlider.value = 0.5f; // 50% volume default
                    UpdateVolumeDisplay(heartBeatVolumeText, 0.5f);
                }
            }
            else
            {
                // Set a default volume if animator isn't available yet
                heartBeatSlider.value = 0.5f; // 50% volume default
                UpdateVolumeDisplay(heartBeatVolumeText, 0.5f);
            }
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

    // Audio volume change handlers
    private void OnEnemyVolumeChanged(float value)
    {
        // Find all audio sources on objects in the Enemy layer
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (enemyLayer == (enemyLayer | (1 << source.gameObject.layer)))
            {
                source.volume = value;
            }
        }
        UpdateVolumeDisplay(enemyVolumeText, value);
    }

    private void OnGlobalVolumeChanged(float value)
    {
        // Find all audio sources in the scene and adjust their volume
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            // Skip sources that are controlled by other sliders
            if (source.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
                source.GetComponent<bgMusicPlayer>() != null ||
                source.GetComponent<HeartRateAnimator>() != null)
            {
                continue;
            }
            source.volume = value;
        }
        UpdateVolumeDisplay(globalVolumeText, value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        bgMusicPlayer musicPlayer = FindObjectOfType<bgMusicPlayer>();
        if (musicPlayer != null)
        {
            AudioSource musicSource = musicPlayer.GetComponent<AudioSource>();
            if (musicSource != null)
            {
                musicSource.volume = value;
            }
        }
        UpdateVolumeDisplay(musicVolumeText, value);
    }

    private void OnHeartBeatVolumeChanged(float value)
    {
        HeartRateAnimator heartRateAnimator = FindObjectOfType<HeartRateAnimator>();
        if (heartRateAnimator != null)
        {
            AudioSource heartBeatSource = heartRateAnimator.GetComponent<AudioSource>();
            if (heartBeatSource != null)
            {
                heartBeatSource.volume = value;
            }
        }
        UpdateVolumeDisplay(heartBeatVolumeText, value);
    }

    // Helper method to update volume display text
    private void UpdateVolumeDisplay(TextMeshProUGUI displayText, float value)
    {
        if (displayText != null)
        {
            // Convert to percentage and round to nearest integer
            int percentage = Mathf.RoundToInt(value * 100);
            displayText.text = percentage.ToString() + "%";
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
