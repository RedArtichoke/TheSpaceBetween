using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyBindManager : MonoBehaviour
{
    // Singleton instance to access from anywhere
    public static KeyBindManager Instance { get; private set; }

    // Dev indicator for always resetting the control on start
    [Header("DEV: Enabling this will always reset the controls on start")]
    public bool resetOnStart = false;
    
    // Public keybind variables with default values
    [Header("Interaction Controls")]
    public KeyCode flashlightKey = KeyCode.F;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode throwKey = KeyCode.Mouse0; // Left mouse button
    public KeyCode dimensionShiftKey = KeyCode.Q;

    public TextMeshProUGUI flashBangBinding;
    public TextMeshProUGUI flashLightBinding;
    public TextMeshProUGUI dimensionShiftBinding;
    
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes this object persist across scenes
            LoadKeybinds(); // Load saved keybinds when the game starts

            if (!resetOnStart) 
            {
                // Create a temporary KeyBindButton instance
                KeyBindButton tempButton = new KeyBindButton();

                // Use the instance to get display names for key codes
                flashBangBinding.text = "Hold " + tempButton.GetKeyCodeDisplayName(flashlightKey);
                flashLightBinding.text = tempButton.GetKeyCodeDisplayName(flashlightKey);
                dimensionShiftBinding.text = tempButton.GetKeyCodeDisplayName(dimensionShiftKey);

                // Destroy the temporary instance
                Destroy(tempButton);
            }
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }
    
    // Saves current keybind configuration to PlayerPrefs
    public void SaveKeybinds()
    {
        PlayerPrefs.SetInt("flashlightKey", (int)flashlightKey);
        PlayerPrefs.SetInt("interactKey", (int)interactKey);
        PlayerPrefs.SetInt("crouchKey", (int)crouchKey);
        PlayerPrefs.SetInt("throwKey", (int)throwKey);
        PlayerPrefs.SetInt("dimensionShiftKey", (int)dimensionShiftKey);
        
        PlayerPrefs.Save();
    }
    
    // Loads keybind configuration from PlayerPrefs
    private void LoadKeybinds()
    {
        // Only load if the key exists in PlayerPrefs, otherwise keep default
        if (PlayerPrefs.HasKey("flashlightKey"))
            flashlightKey = (KeyCode)PlayerPrefs.GetInt("flashlightKey");
            
        if (PlayerPrefs.HasKey("interactKey"))
            interactKey = (KeyCode)PlayerPrefs.GetInt("interactKey");
            
        if (PlayerPrefs.HasKey("crouchKey"))
            crouchKey = (KeyCode)PlayerPrefs.GetInt("crouchKey");
            
        if (PlayerPrefs.HasKey("throwKey"))
            throwKey = (KeyCode)PlayerPrefs.GetInt("throwKey");
            
        if (PlayerPrefs.HasKey("dimensionShiftKey"))
            dimensionShiftKey = (KeyCode)PlayerPrefs.GetInt("dimensionShiftKey");
    }
    
    // Method to change a keybind at runtime
    public void ChangeKeybind(string actionName, KeyCode newKey)
    {
        switch (actionName)
        {
            case "flashlight":
                flashlightKey = newKey;
                break;
            case "interact":
                interactKey = newKey;
                break;
            case "crouch":
                crouchKey = newKey;
                break;
            case "throw":
                throwKey = newKey;
                break;
            case "dimensionShift":
                dimensionShiftKey = newKey;
                break;
        }
        
        // Save changes immediately
        SaveKeybinds();
    }
    
    // Resets all keybinds to their default values
    public void ResetToDefaults()
    {
        // Reset to default values
        flashlightKey = KeyCode.F;
        interactKey = KeyCode.E;
        crouchKey = KeyCode.LeftControl;
        throwKey = KeyCode.Mouse0;
        dimensionShiftKey = KeyCode.Q;
        
        // Save the default values to PlayerPrefs
        SaveKeybinds();
    }

    private void Start()
    {
        if (resetOnStart)
        {
            ResetToDefaults();
        }
    }
}
