using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color defaultColour; // default text colour
    public Color hoverColour;   // hover text colour

    private TextMeshProUGUI tmpText; // reference to TMP text component

    // Called when the script instance is being loaded
    void Awake()
    {
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.color = defaultColour;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }
    }

    // Called when the pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmpText != null)
        {
            tmpText.color = hoverColour;
        }
    }

    // Called when the pointer exits the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmpText != null)
        {
            tmpText.color = defaultColour;
        }
    }
}
