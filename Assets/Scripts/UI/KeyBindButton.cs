using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Button))]
public class KeyBindButton : MonoBehaviour
{
    // The action name that matches the case in KeyBindManager.ChangeKeybind
    public string actionName;
    
    // Reference to text component that displays the current key
    [SerializeField] private TextMeshProUGUI keyText;
    
    // Visual elements for when waiting for input
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color listeningColor = Color.yellow;
    
    // Private variables
    private Button button;
    private bool isListening = false;
    
    private KeyBindManager kbm;

    // Start is called before the first frame update
    void Start()
    {
        // Get required components
        button = GetComponent<Button>();
        kbm = FindObjectOfType<KeyBindManager>();
        if (keyText == null)
            keyText = GetComponentInChildren<TextMeshProUGUI>();
            
        // Set up button click listener
        button.onClick.AddListener(StartListeningForInput);
        
        // Update text to show current keybind
        UpdateKeyText();
    }
    
    // Updates the button text to show the current keybind
    public void UpdateKeyText()
    {
        KeyCode currentKey = GetCurrentKeyCode();
        keyText.text = GetKeyCodeDisplayName(currentKey);
    }
    
    // Gets the current KeyCode from KeyBindManager based on actionName
    private KeyCode GetCurrentKeyCode()
    {
        switch (actionName)
        {
            case "flashlight": return kbm.flashlightKey;
            case "interact": return kbm.interactKey;
            case "crouch": return kbm.crouchKey;
            case "throw": return kbm.throwKey;
            case "dimensionShift": return kbm.dimensionShiftKey;
            default: return KeyCode.None;
        }
    }
    
    // Converts KeyCode to a display-friendly string
    public string GetKeyCodeDisplayName(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Mouse0) return "LMB";
        if (keyCode == KeyCode.Mouse1) return "RMB";
        if (keyCode == KeyCode.Mouse2) return "MMB";
        if (keyCode == KeyCode.LeftControl) return "CTRL";
        if (keyCode == KeyCode.LeftShift) return "SHIFT";
        if (keyCode == KeyCode.LeftAlt) return "ALT";
        
        // For most keys, just return the string representation
        return keyCode.ToString();
    }
    
    // Called when button is clicked
    private void StartListeningForInput()
    {
        // Don't allow multiple buttons to listen at once
        StopAllOtherButtonsFromListening();
        
        // Start listening for input
        isListening = true;
        keyText.text = "Press Key";
        keyText.color = listeningColor;
        
        // Start coroutine to listen for input
        StartCoroutine(ListenForInput());
    }
    
    // Stops all other KeyBindButtons from listening
    private void StopAllOtherButtonsFromListening()
    {
        KeyBindButton[] allButtons = FindObjectsOfType<KeyBindButton>();
        foreach (KeyBindButton button in allButtons)
        {
            if (button != this)
                button.CancelListening();
        }
    }
    
    // Cancels listening mode
    public void CancelListening()
    {
        if (isListening)
        {
            isListening = false;
            keyText.color = normalColor;
            UpdateKeyText();
            StopAllCoroutines();
        }
    }
    
    // Coroutine that listens for key input
    private IEnumerator ListenForInput()
    {
        // Wait for a frame to prevent the click from being detected as the new key
        yield return null;
        
        while (isListening)
        {
            // Check for escape to cancel
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelListening();
                yield break;
            }
            
            // Check for any key press
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode) && IsKeyCodeValid(keyCode))
                {
                    // Apply the new keybind
                    kbm.ChangeKeybind(actionName, keyCode);
                    
                    // Update UI
                    isListening = false;
                    keyText.color = normalColor;
                    UpdateKeyText();

                    if (actionName == "flashlight")
                    {
                        kbm.flashBangBinding.text = "Hold " + GetKeyCodeDisplayName(GetCurrentKeyCode());
                        kbm.flashLightBinding.text = GetKeyCodeDisplayName(GetCurrentKeyCode());
                    }
                    else if (actionName == "dimensionShift")
                    {
                        kbm.dimensionShiftBinding.text = GetKeyCodeDisplayName(GetCurrentKeyCode());
                    }
                    
                    yield break;
                }
            }
            
            yield return null;
        }
    }
    
    // Checks if a keycode is valid and not already assigned
    private bool IsKeyCodeValid(KeyCode keyCode)
    {
        // Skip non-assignable keys
        if (keyCode == KeyCode.None || keyCode == KeyCode.Escape)
            return false;
            
        // Check if key is already assigned to another action
        if (kbm.flashlightKey == keyCode && actionName != "flashlight")
            return false;
        if (kbm.interactKey == keyCode && actionName != "interact")
            return false;
        if (kbm.crouchKey == keyCode && actionName != "crouch")
            return false;
        if (kbm.throwKey == keyCode && actionName != "throw")
            return false;
        if (kbm.dimensionShiftKey == keyCode && actionName != "dimensionShift")
            return false;
            
        return true;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Cancel listening if clicked elsewhere
        if (isListening && Input.GetMouseButtonDown(0))
        {
            CancelListening();
        }
    }
} 