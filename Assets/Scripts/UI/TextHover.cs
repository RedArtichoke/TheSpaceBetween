using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color defaultColour; // default text colour
    public Color hoverColour;   // hover text colour

    public Sprite defaultSprite; // default sprite
    public Sprite hoverSprite;   // hover sprite

    private TextMeshProUGUI tmpText; // reference to TMP text component
    private Image buttonImage; // reference to Image component
    private static TextHover currentlySelected; // track currently selected button

    void Awake()
    {
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();

        if (tmpText != null)
        {
            tmpText.color = defaultColour;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found!");
        }

        if (buttonImage != null)
        {
            buttonImage.sprite = defaultSprite;
        }
        else
        {
            Debug.LogError("Image component not found!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmpText != null && currentlySelected != this)
        {
            tmpText.color = hoverColour;
            buttonImage.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmpText != null && currentlySelected != this)
        {
            tmpText.color = defaultColour;
            buttonImage.sprite = defaultSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentlySelected != this)
        {
            if (currentlySelected != null)
            {
                currentlySelected.tmpText.color = currentlySelected.defaultColour;
                currentlySelected.buttonImage.sprite = currentlySelected.defaultSprite;
            }

            currentlySelected = this;
            tmpText.color = hoverColour;
            buttonImage.sprite = hoverSprite;
        }
    }
}
