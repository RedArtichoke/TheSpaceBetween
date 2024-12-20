using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color defaultColour; // default text colour
    public Color hoverColour;   // hover text colour

    private TextMeshProUGUI tmpText; // reference to TMP text component
    private static TextHover currentlySelected; // track currently selected button

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmpText != null && currentlySelected != this)
        {
            tmpText.color = hoverColour;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmpText != null && currentlySelected != this)
        {
            tmpText.color = defaultColour;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.tmpText.color = currentlySelected.defaultColour;
        }

        currentlySelected = this;
        tmpText.color = hoverColour;
    }
}
