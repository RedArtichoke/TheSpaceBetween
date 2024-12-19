using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class QuitGameButton : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI tmpText; // reference to TMP text component

    // Called when the script instance is being loaded
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
        Debug.Log("Quitting Game");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
