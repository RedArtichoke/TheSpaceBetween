using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI components
using TMPro; // For TextMeshPro
using UnityEngine.Audio; // For audio mixing
using UnityEngine.Rendering; // For global volume
using UnityEngine.Rendering.Universal; // For URP post-processing effects

public class settingsManager : MonoBehaviour
{
    // Static property to access reduced flashbang setting from anywhere
    public static bool ReducedFlashEnabled { get; private set; }
    public static bool SubtitlesEnabled { get; private set; }
    public static bool RedLightEnabled { get; private set; }

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
    public Toggle toggleVSync; // Controls vertical synchronisation

    // Accessibility settings
    public Slider crosshairSizeSlider;
    public Toggle subtitlesToggle;
    public Toggle reducedFlashToggle;
    public Toggle redLightToggle;
    public TextMeshProUGUI crosshairSizeText;
    public RectTransform crosshairCanvasRect; // Reference to crosshair's RectTransform
    public Slider colorblindnessSlider;
    public TextMeshProUGUI colorblindnessText;

    // Store default color values
    private float defaultSaturation;
    private float defaultHueShift;
    private float defaultContrast;

    // Quality settings controls
    public Slider antiAliasingSlider;
    public Slider anisotropicSlider;
    public Slider shadowQualitySlider;
    public Slider textureQualitySlider;
    public Slider lodBiasSlider;
    public TextMeshProUGUI antiAliasingText;
    public TextMeshProUGUI anisotropicText;
    public TextMeshProUGUI shadowQualityText;
    public TextMeshProUGUI textureQualityText;
    public TextMeshProUGUI lodBiasText;

    // HUD component references
    public GameObject crosshair;
    public GameObject uiComponents;

    // Graphical effects
    private Volume globalVolume;
    public Slider brightnessSlider;
    public Slider contrastSlider;
    public Slider bloomSlider;
    public TextMeshProUGUI brightnessText;
    public TextMeshProUGUI contrastText;
    public TextMeshProUGUI bloomText;

    // Post-processing effect components
    private ColorAdjustments colorAdjustments;
    private Bloom bloom;

    // Start is called before the first frame update
    void Start()
    {
        // Find the camera controller in the scene
        cameraController = FindObjectOfType<FpsCameraController>();

        // Get the global volume and its effects
        globalVolume = FindObjectOfType<Volume>();
        if (globalVolume != null)
        {
            Volume volume = globalVolume.GetComponent<Volume>();
            if (volume != null)
            {
                volume.profile.TryGet(out colorAdjustments);
                volume.profile.TryGet(out bloom);

                // Store default color values
                if (colorAdjustments != null)
                {
                    defaultSaturation = colorAdjustments.saturation.value;
                    defaultHueShift = colorAdjustments.hueShift.value;
                    defaultContrast = colorAdjustments.contrast.value;

                    // Enable additional overrides for colorblindness
                    colorAdjustments.saturation.overrideState = true;
                    colorAdjustments.hueShift.overrideState = true;
                }
                
                if (colorAdjustments != null)
                {
                    colorAdjustments.active = true;
                    colorAdjustments.postExposure.overrideState = true;
                    colorAdjustments.contrast.overrideState = true;
                }
                
                if (bloom != null)
                {
                    bloom.active = true;
                    bloom.intensity.overrideState = true;
                }

                // Set up brightness slider
                if (brightnessSlider != null && colorAdjustments != null)
                {
                    // Set initial post exposure to 0
                    colorAdjustments.postExposure.Override(0f);
                    
                    brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
                    // Set slider to middle position (0 post exposure)
                    brightnessSlider.value = 0.4f; // 0.4 maps to approximately 0 in our -2 to 3 range
                    UpdateBrightnessDisplay(0f); // Show initial value of 0
                }

                // Set up contrast slider
                if (contrastSlider != null && colorAdjustments != null)
                {
                    contrastSlider.onValueChanged.AddListener(OnContrastChanged);
                    contrastSlider.value = colorAdjustments.contrast.value / 100; // Map 0-100 range to 0-1
                    UpdateContrastDisplay(colorAdjustments.contrast.value);
                }

                // Set up bloom slider
                if (bloomSlider != null && bloom != null)
                {
                    bloomSlider.onValueChanged.AddListener(OnBloomChanged);
                    bloomSlider.value = bloom.intensity.value / 3; // Map 0-3 range to 0-1
                    UpdateBloomDisplay(bloom.intensity.value);
                }
            }
        }

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

        if (toggleVSync != null)
        {
            toggleVSync.onValueChanged.AddListener(OnToggleVSyncChanged);
            toggleVSync.isOn = QualitySettings.vSyncCount > 0; // Set initial state based on current VSync setting
        }

        // Set up accessibility settings
        if (crosshairSizeSlider != null)
        {
            crosshairSizeSlider.onValueChanged.AddListener(OnCrosshairSizeChanged);
            // Set default to 1.0 (middle of 0.5-2.0 range)
            crosshairSizeSlider.value = 0.5f; // 0.5 maps to 1.0 in our 0.5-2.0 range
            OnCrosshairSizeChanged(0.5f); // Apply default size
        }

        if (subtitlesToggle != null)
        {
            subtitlesToggle.onValueChanged.AddListener(OnSubtitlesToggleChanged);
            subtitlesToggle.isOn = true; // Enable subtitles by default
            SubtitlesEnabled = true; // Set initial static property value
        }

        if (reducedFlashToggle != null)
        {
            reducedFlashToggle.onValueChanged.AddListener(OnReducedFlashToggleChanged);
            reducedFlashToggle.isOn = false; // Flash effects enabled by default
        }

        if (redLightToggle != null)
        {
            redLightToggle.onValueChanged.AddListener(OnRedLightToggleChanged);
            redLightToggle.isOn = false; // Normal light by default
        }

        // Set up colorblindness slider
        if (colorblindnessSlider != null)
        {
            colorblindnessSlider.onValueChanged.AddListener(OnColorblindnessChanged);
            colorblindnessSlider.value = 0f; // Default to no colorblindness correction
            UpdateColorblindnessDisplay(0);
        }

        // Set up quality settings sliders
        if (antiAliasingSlider != null)
        {
            antiAliasingSlider.onValueChanged.AddListener(OnAntiAliasingChanged);
            float normalizedValue = QualitySettings.antiAliasing / 8f;
            antiAliasingSlider.value = normalizedValue;
            UpdateAntiAliasingDisplay(QualitySettings.antiAliasing);
        }

        if (anisotropicSlider != null)
        {
            anisotropicSlider.onValueChanged.AddListener(OnAnisotropicChanged);
            float normalizedValue = (int)QualitySettings.anisotropicFiltering / 16f;
            anisotropicSlider.value = normalizedValue;
            UpdateAnisotropicDisplay((int)QualitySettings.anisotropicFiltering);
        }

        if (shadowQualitySlider != null)
        {
            shadowQualitySlider.onValueChanged.AddListener(OnShadowQualityChanged);
            float normalizedValue = (int)QualitySettings.shadowResolution / 3f;
            shadowQualitySlider.value = normalizedValue;
            UpdateShadowQualityDisplay((int)QualitySettings.shadowResolution);
        }

        if (textureQualitySlider != null)
        {
            textureQualitySlider.onValueChanged.AddListener(OnTextureQualityChanged);
            float normalizedValue = (3 - QualitySettings.globalTextureMipmapLimit) / 3f;
            textureQualitySlider.value = normalizedValue;
            UpdateTextureQualityDisplay(3 - QualitySettings.globalTextureMipmapLimit);
        }

        if (lodBiasSlider != null)
        {
            lodBiasSlider.onValueChanged.AddListener(OnLODBiasChanged);
            float normalizedValue = Mathf.InverseLerp(0.5f, 2.0f, QualitySettings.lodBias);
            lodBiasSlider.value = normalizedValue;
            UpdateLODBiasDisplay(QualitySettings.lodBias);
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

    private void OnToggleVSyncChanged(bool isOn)
    {
        // Enable or disable VSync based on toggle state
        QualitySettings.vSyncCount = isOn ? 1 : 0;
    }

    // Accessibility settings handlers
    private void OnCrosshairSizeChanged(float value)
    {
        if (crosshairCanvasRect != null)
        {
            // Map 0-1 slider value to 0.5-2.0 scale range
            float scale = Mathf.Lerp(0.5f, 2.0f, value);
            crosshairCanvasRect.localScale = new Vector3(scale, scale, 1f);
            UpdateCrosshairSizeDisplay(scale);
        }
    }

    private void OnSubtitlesToggleChanged(bool isOn)
    {
        SubtitlesEnabled = isOn;
        
        // Find and clear any active subtitles when disabled
        if (!isOn)
        {
            SubtitleText[] activeSubtitles = FindObjectsOfType<SubtitleText>();
            foreach (SubtitleText subtitle in activeSubtitles)
            {
                subtitle.ClearSubtitles();
            }
        }
    }

    private void OnReducedFlashToggleChanged(bool isOn)
    {
        ReducedFlashEnabled = isOn;
        // Will implement flash reduction system later
        // This toggle will control intensity of flash effects
    }

    private void OnRedLightToggleChanged(bool isOn)
    {
        RedLightEnabled = isOn;
    }

    private void UpdateCrosshairSizeDisplay(float value)
    {
        if (crosshairSizeText != null)
        {
            crosshairSizeText.text = value.ToString("F1") + "x";
        }
    }

    // Post-processing effect handlers
    private void OnBrightnessChanged(float value)
    {
        if (colorAdjustments != null)
        {
            float postExposure = (value * 5) - 2; // Map 0-1 to -2 to 3 range
            colorAdjustments.postExposure.Override(postExposure);
            UpdateBrightnessDisplay(postExposure);
        }
    }

    private void OnContrastChanged(float value)
    {
        if (colorAdjustments != null)
        {
            float contrast = value * 100; // Map 0-1 to 0-100 range
            colorAdjustments.contrast.Override(contrast);
            UpdateContrastDisplay(contrast);
        }
    }

    private void OnBloomChanged(float value)
    {
        if (bloom != null)
        {
            float intensity = value * 3; // Map 0-1 to 0-3 range for more intense bloom
            bloom.intensity.Override(intensity);
            UpdateBloomDisplay(intensity);
        }
    }

    private void UpdateBrightnessDisplay(float value)
    {
        if (brightnessText != null)
        {
            brightnessText.text = value.ToString("F1");
        }
    }

    private void UpdateContrastDisplay(float value)
    {
        if (contrastText != null)
        {
            contrastText.text = value.ToString("F0");
        }
    }

    private void UpdateBloomDisplay(float value)
    {
        if (bloomText != null)
        {
            bloomText.text = value.ToString("F1");
        }
    }

    // Quality settings handlers
    private void OnAntiAliasingChanged(float value)
    {
        int msaaLevel = Mathf.RoundToInt(value * 8); // Convert 0-1 to 0, 2, 4, 8
        QualitySettings.antiAliasing = msaaLevel;
        UpdateAntiAliasingDisplay(msaaLevel);
    }

    private void OnAnisotropicChanged(float value)
    {
        int anisoLevel = Mathf.RoundToInt(value * 16); // Convert 0-1 to 0, 2, 4, 8, 16
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)anisoLevel;
        UpdateAnisotropicDisplay(anisoLevel);
    }

    private void OnShadowQualityChanged(float value)
    {
        int qualityLevel = Mathf.RoundToInt(value * 3); // Convert 0-1 to 0-3
        QualitySettings.shadowResolution = (UnityEngine.ShadowResolution)qualityLevel;
        UpdateShadowQualityDisplay(qualityLevel);
    }

    private void OnTextureQualityChanged(float value)
    {
        int qualityLevel = Mathf.RoundToInt(value * 3); // Convert 0-1 to 0-3
        QualitySettings.globalTextureMipmapLimit = 3 - qualityLevel; // Invert because lower limit = higher quality
        UpdateTextureQualityDisplay(qualityLevel);
    }

    private void OnLODBiasChanged(float value)
    {
        float bias = Mathf.Lerp(0.5f, 2.0f, value); // Convert 0-1 to 0.5-2.0
        QualitySettings.lodBias = bias;
        UpdateLODBiasDisplay(bias);
    }

    // Display update methods
    private void UpdateAntiAliasingDisplay(int value)
    {
        if (antiAliasingText != null)
        {
            antiAliasingText.text = value == 0 ? "Off" : value + "x";
        }
    }

    private void UpdateAnisotropicDisplay(int value)
    {
        if (anisotropicText != null)
        {
            anisotropicText.text = value == 0 ? "Off" : value + "x";
        }
    }

    private void UpdateShadowQualityDisplay(int value)
    {
        if (shadowQualityText != null)
        {
            string[] qualities = { "Low", "Medium", "High", "Ultra" };
            shadowQualityText.text = qualities[value];
        }
    }

    private void UpdateTextureQualityDisplay(int value)
    {
        if (textureQualityText != null)
        {
            string[] qualities = { "Low", "Medium", "High", "Ultra" };
            textureQualityText.text = qualities[value];
        }
    }

    private void UpdateLODBiasDisplay(float value)
    {
        if (lodBiasText != null)
        {
            lodBiasText.text = value.ToString("F1");
        }
    }

    private void OnColorblindnessChanged(float value)
    {
        if (colorAdjustments != null)
        {
            int mode = Mathf.RoundToInt(value * 3); // 0-3: None, Protanopia, Deuteranopia, Tritanopia

            switch (mode)
            {
                case 0: // None - restore defaults
                    colorAdjustments.saturation.Override(defaultSaturation);
                    colorAdjustments.hueShift.Override(defaultHueShift);
                    colorAdjustments.contrast.Override(defaultContrast);
                    break;

                case 1: // Protanopia (red-blind)
                    colorAdjustments.saturation.Override(-10f);
                    colorAdjustments.hueShift.Override(5f);
                    colorAdjustments.contrast.Override(defaultContrast + 5f);
                    break;

                case 2: // Deuteranopia (green-blind)
                    colorAdjustments.saturation.Override(-15f);
                    colorAdjustments.hueShift.Override(-5f);
                    colorAdjustments.contrast.Override(defaultContrast + 10f);
                    break;

                case 3: // Tritanopia (blue-blind)
                    colorAdjustments.saturation.Override(-20f);
                    colorAdjustments.hueShift.Override(15f);
                    colorAdjustments.contrast.Override(defaultContrast + 15f);
                    break;
            }

            UpdateColorblindnessDisplay(mode);
        }
    }

    private void UpdateColorblindnessDisplay(int mode)
    {
        if (colorblindnessText != null)
        {
            string[] modes = { "Off", "Protanopia", "Deuteranopia", "Tritanopia" };
            colorblindnessText.text = modes[mode];
        }
    }
}
