using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class WebsiteClickButton : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI tmpText; // reference to TMP text component
    
    void Awake()
    {
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }
    }

    // Called when the button is clicked
    public void OnPointerClick(PointerEventData eventData)
    {   
        Application.OpenURL("https://tsb.crd.co/");
    }
}
