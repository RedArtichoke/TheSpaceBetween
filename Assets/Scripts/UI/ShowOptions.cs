using UnityEngine;
using UnityEngine.UI;

public class ShowOptions : MonoBehaviour
{
    public GameObject prefabToShow; // Public reference to the prefab

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found!");
        }
    }

    // Called when the button is clicked
    void OnButtonClick()
    {
        if (prefabToShow != null)
        {
            prefabToShow.SetActive(true); // Show the prefab
        }
        else
        {
            Debug.LogError("Prefab reference not set!");
        }
    }
}
