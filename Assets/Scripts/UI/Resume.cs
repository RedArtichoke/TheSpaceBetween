using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resume : MonoBehaviour
{
    public MenuController menuController; // Public reference to the MenuController

    void Start()
    {
        Button resumeButton = GetComponent<Button>();
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found!");
        }
    }

    // Called when the resume button is clicked
    void OnResumeButtonClick()
    {
        if (menuController != null)
        {
            menuController.TogglePause();
        }
        else
        {
            Debug.LogError("MenuController reference not set!");
        }
    }
}
