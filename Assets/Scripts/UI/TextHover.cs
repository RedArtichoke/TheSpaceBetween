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

    public bool isSelected; // Select by default
    public Image tabImage; // Image to show on tab selection

    private TextMeshProUGUI tmpText; // reference to TMP text component
    private Image buttonImage; // reference to Image component
    private static TextHover selectedOption; // Track selected option outside "OptionsTabs"
    private static TextHover selectedTab; // Track selected option inside "OptionsTabs"

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

        // Select by default if isSelected is true
        if (isSelected)
        {
            if (transform.parent.name == "OptionsTabs")
            {
                HandleTabSelection();
            }
            else
            {
                HandleOptionSelection();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmpText != null && !IsCurrentlySelected())
        {
            tmpText.color = hoverColour;
            buttonImage.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmpText != null && !IsCurrentlySelected())
        {
            tmpText.color = defaultColour;
            buttonImage.sprite = defaultSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.parent.name == "OptionsTabs")
        {
            HandleTabSelection();
        }
        else
        {
            HandleOptionSelection();
        }
    }

    // Handle selection for buttons inside "OptionsTabs"
    void HandleTabSelection()
    {
        if (selectedTab != null && selectedTab != this)
        {
            selectedTab.Deselect();

            // Hide the previous tab's image
            if (selectedTab.tabImage != null)
            {
                selectedTab.tabImage.gameObject.SetActive(false);
            }
        }
        selectedTab = this;
        Select();

        // Show the image if it exists
        if (tabImage != null)
        {
            tabImage.gameObject.SetActive(true);
        }
    }

    // Handle selection for buttons outside "OptionsTabs"
    void HandleOptionSelection()
    {
        if (selectedOption != null && selectedOption != this)
        {
            selectedOption.Deselect();
        }
        selectedOption = this;
        Select();

        // Check if the button has the label "Options"
        if (gameObject.name != "Options")
        {
            // Hide sibling with label "OptionsTabs" if it exists
            Transform sibling = transform.parent.Find("OptionsTabs");
            if (sibling != null)
            {
                sibling.gameObject.SetActive(false);
            }
        }
    }

    // Select the button
    void Select()
    {
        tmpText.color = hoverColour;
        buttonImage.sprite = hoverSprite;
    }

    // Deselect the button
    void Deselect()
    {
        tmpText.color = defaultColour;
        buttonImage.sprite = defaultSprite;
    }

    // Check if this button is currently selected
    bool IsCurrentlySelected()
    {
        return (selectedOption == this || selectedTab == this);
    }
}
